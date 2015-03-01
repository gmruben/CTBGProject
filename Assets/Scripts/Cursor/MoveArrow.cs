using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class MoveArrow : MonoBehaviour
{
    public Action<List<SquareIndex>> onClick;
    public Action onCancel;

	public GameObject moveNodePrefab;
	public GameObject moveNodeFinalPrefab;

	public GameObject moveLinkPrefab;
	public GameObject moveLinkDiagonalPrefab;

    private List<MoveNode> nodeList;
	private List<GameObject> linkList;

    private Board board;
	private Player player;
    private SquareIndex startIndex;
    protected bool isTracking = false;
    private bool isActive = false;

	private SquareIndex lastIndex;
    protected SquareIndex currentIndex;

	private int numMoves;
	private int numDiagonals;
	private bool canUseEmptySquares;
	private bool canChangeDirection;

	private bool hasMoved = false;

    void Update()
    {
        if (isActive)
        {
			updateMouse();
        }
    }

	public void init(Board board, Player player, int numMoves, int numDiagonals, bool canUseEmptySquares, bool canChangeDirection)
    {
        this.board = board;
		this.player = player;

		this.numMoves = numMoves;
		this.numDiagonals = numDiagonals;
		this.canUseEmptySquares = canUseEmptySquares;
		this.canChangeDirection = canChangeDirection;

        isActive = true;

        nodeList = new List<MoveNode>();
		linkList = new List<GameObject>();

		startIndex = player.index;
		lastIndex = startIndex;
        currentIndex = getCorrectIndex();

		moveTo(currentIndex);
    }
	
    private SquareIndex getCorrectIndex()
    {
		SquareIndex index = new SquareIndex(player.team.isP1Team ? (player.index.x - 1) : (player.index.x + 1), player.index.y);

		if (!isIndexCorrect(index)) index = new SquareIndex(player.index.x, index.y - 1);
		if (!isIndexCorrect(index)) index = new SquareIndex(player.index.x, index.y + 1);
		if (!isIndexCorrect(index)) index = new SquareIndex(player.team.isP1Team ? (player.index.x + 1) : (player.index.x - 1), index.y);
        
		return index;
	}

    private bool isIndexCorrect(SquareIndex index)
    {
		return board.isOnBounds(index) && !board.isPlayerOnSquare(index);
    }

    private void addIndex(SquareIndex index)
    {
        MoveNode endNode = nodeList[0];

		nodeList.Add(instantiateMoveNode(moveNodePrefab, endNode.index, 10));

        endNode.index = index;
		endNode.gameObject.transform.localPosition = board.squareIndexToWorld(index);
    }

    private void removeIndex(int listIndex, SquareIndex index)
    {
        for (int i = listIndex; i < nodeList.Count; i++)
        {
            GameObject.Destroy(nodeList[i].gameObject);
        }
        for (int i = listIndex; i < nodeList.Count; i++)
        {
            nodeList.RemoveRange(listIndex, nodeList.Count - listIndex);
        }

        MoveNode head = nodeList[0];

        head.index = index;
		head.gameObject.transform.position = board.squareIndexToWorld(head.index);

		SquareIndex prevIndex = nodeList.Count == 1 ? startIndex : nodeList[nodeList.Count - 1].index;
    }

	private void removeList()
	{
		for (int i = 0; i < nodeList.Count; i++)
		{
			GameObject.Destroy(nodeList[i].gameObject);
		}
		nodeList.Clear();
	}

    private int calculateListIndex(SquareIndex index)
    {
        for (int i = 0; i < nodeList.Count; i++)
        {
            if (nodeList[i].index.Equals(index))
            {
                return i;
            }
        }
        return -1;
    }
	
    private void updateMouse()
    {
        if (isTracking)
        {
            if (Input.GetMouseButton(0))
            {
                RaycastHit rayHit = new RaycastHit();
                Ray ray = GameCamera.camera.ScreenPointToRay(Input.mousePosition);

				if (Physics.Raycast(ray, out rayHit, GameConfig.RAY_DISTANCE, GameConfig.boardLayerMask))
                {
                    SquareIndex index = rayHit.collider.GetComponent<BoardSquare>().index;
                    SquareIndex headIndex = nodeList[0].index;

					//HACK: Have to think about this at some point
                    if (!index.Equals(headIndex) && !index.Equals(startIndex))
                    {
						//Check if we have enough moves left
						if (nodeList.Count < numMoves)
						{
							hasMoved = true;

							//Check if we have changed direction
							if (canChangeDirection || (!canChangeDirection && !isDirectionChange(index)))
							{
								moveTo(index);
							}
						}
                    }
                }
            }
			else if (Input.GetMouseButtonUp(0))
			{
				//If the user didn't move the arrow, perform a click
				if (!hasMoved)
				{
					click();
				}
				
				isTracking = false;
				hasMoved = false;
			}
        }
        else
        {
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit rayHit = new RaycastHit();
                Ray ray = GameCamera.camera.ScreenPointToRay(Input.mousePosition);

				if (Physics.Raycast(ray, out rayHit, GameConfig.RAY_DISTANCE, GameConfig.boardLayerMask))
				{
					SquareIndex headIndex = nodeList[0].index;
					SquareIndex index = rayHit.collider.GetComponent<BoardSquare>().index;

					if (index == headIndex)
					{
						isTracking = true;
					}
					//If we click the index the player is on, we cancel the move
					else if (player.index == index)
					{
						cancel();
					}
					else
					{
						removeList();
						List<SquareIndex> path = PathFinder.findPath(startIndex, index, board, canUseEmptySquares, canChangeDirection, numDiagonals);
						for (int i = 0; i < path.Count; i++)
						{
							//Check if we have enough moves left
							if (nodeList.Count < numMoves)
							{
								moveTo(path[i]);
							}
						}
					}
				}
            }
        }
    }
	
	private void click()
	{
		isTracking = false;
		if (onClick != null) onClick(retrieveIndexList());
	}

	private void cancel()
	{
		if (onCancel != null) onCancel();
	}

    private void moveTo(SquareIndex index)
    {
		if (nodeList.Count == 0)
		{
			lastIndex = startIndex;
			currentIndex = index;

			nodeList.Add(instantiateMoveNode(moveNodeFinalPrefab, currentIndex, 10));
		}
		else if (!index.Equals(nodeList[0].index))
    	{
	        int listIndex = calculateListIndex(index);

	        if (listIndex > 0)
				{
	            removeIndex(listIndex, index);
				}
	        else
				{
	            addIndex(index);
				}

				lastIndex = currentIndex;
	        currentIndex = index;
	    }

		createNodeLinks();
		calculateNumMoves();
    }

	private void calculateNumMoves()
	{
		int index = nodeList.Count - 1;
		for (int i = 1; i < nodeList.Count; i++)
		{
			nodeList[i].setValue(numMoves - i);
			index--;
		}

		nodeList[0].setValue(numMoves - nodeList.Count);
	}

	/// <summary>
	/// Instantiates a new move node
	/// </summary>
	/// <returns>The move node.</returns>
	/// <param name="prefab">The prefab for the node (there could be different nodes).</param>
	/// <param name="index">The index for the node.</param>
	/// <param name="numMoves">Number of moves left to show on the node.</param>
	private MoveNode instantiateMoveNode(GameObject prefab, SquareIndex index, int numMoves)
	{
		MoveNode moveNode = (GameObject.Instantiate(prefab) as GameObject).GetComponent<MoveNode>();
		moveNode.init(numMoves, index);
		
		moveNode.transform.parent = transform;
		moveNode.transform.localPosition = board.squareIndexToWorld(index);

		return moveNode;
	}

	private GameObject instantiateNodeLink(GameObject prefab, Vector3 position)
	{
		GameObject nodeLink = GameObject.Instantiate(prefab) as GameObject;
		
		nodeLink.transform.parent = transform;
		nodeLink.transform.position = position;
		
		return nodeLink;
	}

	private List<SquareIndex> retrieveIndexList()
	{
		List<SquareIndex> indexList = new List<SquareIndex>();
		for (int i = 1; i < nodeList.Count; i++)
		{
			indexList.Add(nodeList[i].index);
		}
		indexList.Add(nodeList[0].index);

		return indexList;
	}

	private void createNodeLinks()
	{
		clearNodeLinks();

		List<SquareIndex> indexList = retrieveIndexList();
		for (int i = 1; i < indexList.Count; i++)
		{
			SquareIndex index1 = indexList[i - 1];
			SquareIndex index2 = indexList[i];

			//Don't do diagonals for now
			if (!isDiagonal(index1, index2))
			{
				Vector3 pos1 = board.squareIndexToWorld(index1);
				Vector3 pos2 = board.squareIndexToWorld(index2);

				Vector3 position = pos1 + ((pos2 - pos1) * 0.5f);
				linkList.Add(instantiateNodeLink(moveLinkPrefab, position));
			}
		}	
	}

	private void clearNodeLinks()
	{
		for (int i = 0; i < linkList.Count; i++)
		{
			GameObject.Destroy(linkList[i]);
		}
		linkList.Clear();
	}

	private bool isHorizontal(SquareIndex index1, SquareIndex index2)
	{
		return index1.x != index2.x && index1.y == index2.y;
	}

	private bool isDiagonal(SquareIndex index1, SquareIndex index2)
	{
		return index1.x != index2.x && index1.y != index2.y;
	}

	private bool isDirectionChange(SquareIndex nextIndex)
	{
		List<SquareIndex> indexList = retrieveIndexList();
		SquareIndex index = indexList[0];

		if (isDiagonal(startIndex, index))
		{
			if (nodeList.Count == 1)
			{
				return false;
			}
			else 
			{
				if (isHorizontal(indexList[0], indexList[1])) return !isHorizontal(indexList[1], nextIndex);
				else return isHorizontal(indexList[1], nextIndex);
			}
		}
		else
		{
			if (isHorizontal(startIndex, indexList[0])) return !isHorizontal(indexList[0], nextIndex);
			else return isHorizontal(indexList[0], nextIndex);
		}
	}
}