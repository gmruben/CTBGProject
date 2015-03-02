using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Board : MonoBehaviour
{
	public const float SIZE_SQUARE = 1;

	public const int SIZE_X = 18;
	public const int SIZE_Y = 15;

	public GameObject boardSquarePrefab;
	public GameObject activeSquarePrefab;

	private BoardSquareData[][] _boardData;
	private Transform cachedTransform;

	private Vector2 origin;

	private Dictionary<string, GameObject> activeTileList;

	public CardDeck p1Deck;
	public CardDeck p2Deck;

	public BoardPositionList p1BoardGoals;
	public BoardPositionList p2BoardGoals;

	public BoardPositionList boardTurn;
	public BoardPositionList boardTime;

	//HACK: Put this in a config class?
	private List<SquareIndex> goalIndexList;

	public void init()
	{	
		origin = new Vector2((float) (-SIZE_X / 2) + (SIZE_SQUARE / 2), (float) (-SIZE_Y / 2));
		cachedTransform = transform;

		_boardData = new BoardSquareData[SIZE_X][];
		for (int i = 0; i < SIZE_X; i++)
		{
			_boardData[i] = new BoardSquareData[SIZE_Y];
			for (int j = 0; j < SIZE_Y; j++)
			{
				Transform square = (GameObject.Instantiate(boardSquarePrefab) as GameObject).transform;
				
				square.parent = transform;
				square.localPosition = new Vector3(origin.x + (i * SIZE_SQUARE), 0, origin.y + (j * SIZE_SQUARE));
				
				BoardSquareData data = new BoardSquareData();
				
				data.index = new SquareIndex(i, j);
				data.info = BoardSquareData.Info.Empty;
				data.boardSquare = square;
				
				//Set index
				square.GetComponent<BoardSquare>().index = data.index;
				
				_boardData[i][j] = data;
			}
		}

		activeTileList = new Dictionary<string, GameObject>();

		//Instantiate and initialize all tokens
		GameObject tokenTurn = EntityManager.instantiateTokenTurn();
		boardTurn.init(tokenTurn.transform, 11);

		GameObject tokenTime = EntityManager.instantiateTokenTime();
		boardTime.init(tokenTime.transform, 0);

		Token p1TeamToken = EntityManager.instantiateTokenTeam();
		Token p2TeamToken = EntityManager.instantiateTokenTeam();

		Texture p1TeamTokenTexture = Resources.Load<Texture>("Textures/Tokens/Nankatsu/Token_Nankatsu");
		Texture p2TeamTokenTexture = Resources.Load<Texture>("Textures/Tokens/Touhou/Token_Touhou");

		p1TeamToken.setTexture(p1TeamTokenTexture);
		p2TeamToken.setTexture(p2TeamTokenTexture);

		p1BoardGoals.init(p1TeamToken.transform, 0);
		p2BoardGoals.init(p2TeamToken.transform, 0);

		//Create goal index list
		goalIndexList = new List<SquareIndex>();

		goalIndexList.Add(new SquareIndex(0, 5));
		goalIndexList.Add(new SquareIndex(0, 6));
		goalIndexList.Add(new SquareIndex(0, 7));
		goalIndexList.Add(new SquareIndex(0, 8));
		goalIndexList.Add(new SquareIndex(0, 9));
	}
	
	public void showRadius(SquareIndex index, int radius, bool squareWithPlayerIn)
	{
		List<SquareIndex> indexList = getMoveToSquareList(index, radius, squareWithPlayerIn);
		for (int i = 0; i < indexList.Count; i++)
		{
			instantiateSquare(indexList[i].x, indexList[i].y);
		}
	}

	public void hideRadius()
	{
		foreach(KeyValuePair<string, GameObject> tile in activeTileList)
		{
			GameObject.Destroy(tile.Value);
		}
		activeTileList.Clear();
	}

	private void instantiateSquare(int x, int y)
	{
		string key = x + "_" + y;
		
		GameObject go = GameObject.Instantiate(activeSquarePrefab) as GameObject;
		go.transform.position = squareIndexToWorld(new SquareIndex(x, y));
		go.transform.parent = cachedTransform;

		go.GetComponent<SquareColorFull>().init(Color.white);
		
		activeTileList.Add(key, go);
	}

	//Returns a list with all the possible squares a player can move to
	public List<SquareIndex> getMoveToSquareList(SquareIndex index, int radius, bool squareWithPlayerIn)
	{
		List<SquareIndex> indexList = new List<SquareIndex>();
		List<SquareIndex> indexListInArea = getSquareListInArea(index, radius);
		
		for (int i = 0; i < indexListInArea.Count; i++)
		{
			SquareIndex currentIndex = indexListInArea[i];
			
			bool sameIndex = index.Equals(currentIndex);
			bool onBounds = isOnBounds(currentIndex);
			bool isPlayer = isPlayerOnSquare(currentIndex);
			
			if (!sameIndex && onBounds && (!isPlayer || squareWithPlayerIn))
			{
				indexList.Add(currentIndex);
			}
		}
		
		return indexList;
	}

	//Returns a list with all the squares in an area
	public List<SquareIndex> getSquareListInArea(SquareIndex index, int radius)
	{
		List<SquareIndex> indexList = new List<SquareIndex>();
		for (int i = index.x - radius; i < index.x + radius + 1; i++)
		{
			for (int j = index.y - radius; j < index.y + radius + 1; j++)
			{
				if (Mathf.Abs(index.x - i) <= radius && Mathf.Abs(index.y - j) <= radius && Mathf.Abs(index.x - i) + Mathf.Abs(index.y - j) <= radius)
				{
					SquareIndex currentIndex = new SquareIndex(i, j);
					bool onBounds = isOnBounds(currentIndex);
					
					indexList.Add(currentIndex);
				}
			}
		}
		return indexList;
	}

	public List<SquareIndex> getAdjacentTileList(SquareIndex index)
	{
		List<SquareIndex> tileList = new List<SquareIndex>();
		for (int i = index.x - 1; i <= index.x + 1; i++)
		{
			for (int j = index.y - 1; j <= index.y + 1; j++)
			{
				SquareIndex newindex = new SquareIndex(i, j);
				if (isOnBounds(newindex) && index != newindex)
				{
					tileList.Add(newindex);
				}
			}
		}

		return tileList;
	}

	public bool isOnBounds(SquareIndex index)
	{
		return index.x >= 0 && index.x < SIZE_X && index.y >= 0 && index.y < SIZE_Y;
	}

	public Player getPlayerOnSquare(SquareIndex index)
	{
		if (isOnBounds(index)) return _boardData[index.x][index.y].player;
		else return null;
	}

	/// <summary>
	/// Retrieves all the players adjacent to an index
	/// </summary>
	/// <returns>The player list.</returns>
	/// <param name="index">The index whose adjacent players we want to retrieve.</param>
	/// <param name="teamId">The id of the team whose players we want to retrieve.</param>
	public List<Player> retrieveAdjacentPlayerList(SquareIndex index, string teamId)
	{
		List<Player> playerList = new List<Player>();
		List<SquareIndex> indexList = getAdjacentTileList(index);

		for (int i = 0; i < indexList.Count; i++)
		{
			Player player = getPlayerOnSquare(indexList[i]);
			if (player != null && !player.isInactive && player.team.teamData.id == teamId)
			{
				playerList.Add(player);
			}
		}

		return playerList;
	}

	/// <summary>
	/// Checks wether an index is on a goal or not
	/// </summary>
	/// <returns><c>true</c>, if the index is in a goal, <c>false</c> otherwise.</returns>
	/// <param name="index">The index we want to check</param>
	/// <param name="onLeft">If the goal is on the left side of the field or not.</param>
	public bool isIndexInGoal(SquareIndex index, bool onLeftSide)
	{
		SquareIndex correctIndex = onLeftSide ? index : inverseIndex(index);
		return goalIndexList.Contains(correctIndex);
	}

	public bool isPlayerOnSquare(SquareIndex index)
	{
		return getPlayerOnSquare (index) != null;
	}

	public void updatePlayerPosition(SquareIndex index, Player player)
	{
		_boardData[index.x][index.y].player = player;
	}

	public void removePlayerFromSquare(SquareIndex index)
	{
		_boardData[index.x][index.y].player = null;
	}

	public void setBallPosition(SquareIndex index)
	{
		_boardData[index.x][index.y].info = BoardSquareData.Info.Ball;
	}

	public void removeBallFromSquare(SquareIndex index)
	{
		_boardData[index.x][index.y].info = BoardSquareData.Info.Empty;
	}

	public bool isBallOnSquare(SquareIndex index)
	{
		return _boardData[index.x][index.y].info == BoardSquareData.Info.Ball;
	}

	//Returns whether the current list of active tiles contains the index or not
	public bool isIndexInActiveTileList(SquareIndex index)
	{
		string key = index.x + "_" + index.y;
		return activeTileList.ContainsKey(key);
	}

	//Converts a square index to world coordinates
	public Vector3 squareIndexToWorld(SquareIndex index)
	{
		float posx = origin.x + transform.position.x + (index.x * SIZE_SQUARE);
		float posy = origin.y + transform.position.z + (index.y * SIZE_SQUARE);
		
		return new Vector3(posx, 0, posy);
	}

	//Converts a world position to a square index
	public static SquareIndex worldToSquareIndex(Vector3 worldPosition)
	{
		return new SquareIndex((int)(worldPosition.x / SIZE_SQUARE), (int)(worldPosition.z / SIZE_SQUARE));
	}

	public SquareIndex inverseIndex(SquareIndex index)
	{
		int x = (SIZE_X - 1) - index.x;
		int y = (SIZE_Y - 1) - index.y;
		
		return new SquareIndex(x, y);
	}

	public BoardSquareData[][] boardData
	{
		get { return _boardData; }
	}
}

public class BoardSquareData
{
	public enum Info { Empty, Player, Ball };
	
	public SquareIndex index;
	public Info info;
	public Transform boardSquare;
	
	public Player player;
}