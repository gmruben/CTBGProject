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

    private Board board;
	private Player player;
    private SquareIndex startIndex;
    protected bool isTracking = false;
    private bool isActive = false;

	private SquareIndex lastIndex;
    protected SquareIndex currentIndex;

	private int numMoves;
	private bool canUseEmptySquares;

	private bool hasMoved = false;

    void Update()
    {
        if (isActive)
        {
			updateMouse();
        }
    }

    public void init(Board board, Player player, int numMoves, bool canUseEmptySquares)
    {
        this.board = board;
		this.player = player;

		this.numMoves = numMoves;
		this.canUseEmptySquares = canUseEmptySquares;

        isActive = true;

        nodeList = new List<MoveNode>();

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
						hasMoved = true;
						moveTo(index);
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
						List<SquareIndex> path = PathFinder.findPath(startIndex, index, board, canUseEmptySquares);
						for (int i = 0; i < path.Count; i++)
						{
							moveTo(path[i]);
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
		board.hideRadius();
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

	private MoveNode instantiateMoveNode(GameObject prefab, SquareIndex index, int numMoves)
	{
		MoveNode moveNode = (GameObject.Instantiate(prefab) as GameObject).GetComponent<MoveNode>();
		moveNode.init(numMoves, index);
		
		moveNode.transform.parent = transform;
		moveNode.transform.localPosition = board.squareIndexToWorld(index);

		return moveNode;
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
}