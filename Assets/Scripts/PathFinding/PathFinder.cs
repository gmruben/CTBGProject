using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//HACK: Hay que pegarle un repaso a toda la clase
public class PathFinder
{
	private static bool m_canUseDiagonals = true;

	private static int mapWidth = 10;
	private static int mapHeight = 10;

	private static BinaryHeap<Node> openList;
	private static List<SquareIndex> closedList;

	private static Board board;
	private static bool canUseNonEmptySquare;

	private static bool[][] isOnOpenList;
	private static SquareIndex[][] parent;
	private static int[][] Gcost;
    
	public static bool isTherePath;

	private static int numDiagonals;

	public static List<SquareIndex> findPath(SquareIndex start, SquareIndex target, Board board, bool canUseNonEmptySquare, bool canChangeDirection, int numDiagonals)
    {
        mapWidth = Board.SIZE_X;
		mapHeight = Board.SIZE_Y;

		PathFinder.board = board;
		PathFinder.canUseNonEmptySquare = canUseNonEmptySquare;
		PathFinder.numDiagonals = numDiagonals;

        isTherePath = false;

        //Initialize lists
        openList = new BinaryHeap<Node>(mapWidth * mapHeight + 2);
        closedList = new List<SquareIndex>();

        //Initialize arrays
        isOnOpenList = new bool[mapWidth + 1][];
        parent = new SquareIndex[mapWidth + 1][];
        Gcost = new int[mapWidth + 1][];

        for (int i = 0; i < mapWidth + 1; i++)
        {
            isOnOpenList[i] = new bool[mapHeight + 1];
            parent[i] = new SquareIndex[mapWidth + 1];
            Gcost[i] = new int[mapWidth + 1];
        }

        int parentXval = 0, parentYval = 0, a = 0, b = 0, addedGCost = 0, tempGcost = 0;

        //If the target position is the same as the start position
        if (start == target)
        {
            return new List<SquareIndex>();
        }

        //Reset starting square's G value to 0
        Gcost[start.x][start.y] = 0;

        //Add the starting location to the open list of squares to be checked.
        isOnOpenList[start.x][start.y] = true;
        openList.add(new Node(start, 0, 0));

        //5.Do the following until a path is found or deemed nonexistent.
        do
        {
            //If the open list is not empty, take the first cell off of the list. This is the lowest F cost cell on the open list.
            if (openList.count != 0)
            {
                //Pop the first item off the open list.
                Node node = openList.remove();
                closedList.Add(node.index);

                parentXval = (int)node.index.x;
                parentYval = (int)node.index.y;

                //7.Check the adjacent squares. (Its "children" -- these path children are similar, conceptually, to the binary heap children mentioned
                // above, but don't confuse them. They are different. Path children are portrayed in Demo 1 with grey pointers pointing toward their
				// parents.) Add these adjacent child squares to the open list for later consideration if appropriate (see various if statements below).
                for (b = parentYval - 1; b <= parentYval + 1; b++)
                {
                    for (a = parentXval - 1; a <= parentXval + 1; a++)
                    {
                        //	If not off the map (do this first to avoid array out-of-bounds errors)
                        if (a != -1 && b != -1 && a != mapWidth && b != mapHeight)
                        {
                            //If not already on the closed list (items on the closed list have already been considered and can now be ignored)
							SquareIndex index = new SquareIndex(a, b);
                            if (!closedList.Contains(index))
                            {
                                //If it can move to the square
                                if (canMoveTo(a, b))
                                {
                                    bool isDiagonal = (Mathf.Abs(a - parentXval) == 1 && Mathf.Abs(b - parentYval) == 1);

                                    //If it is not diagonal, or it is and we can use diagonals
									if (!isDiagonal || (isDiagonal && m_canUseDiagonals && calculateNumDiagonalsInPath(start, parentXval, parentYval) < numDiagonals))
                                    {
                                        //If not already on the open list, add it to the open list.			
                                        if (!isOnOpenList[a][b])
                                        {
                                            //Figure out and set its G Cost
                                            addedGCost = isDiagonal ? 14 : 10;
                                            Gcost[a][b] = Gcost[parentXval][parentYval] + addedGCost;

                                            //Change whichList to show that the new item is on the open list.
                                            isOnOpenList[a][b] = true;

                                            //Figure out its H and F costs and parent
                                            parent[a][b] = new SquareIndex(parentXval, parentYval);
                                            int hcost = 10 * (Mathf.Abs(a - (int)target.x) + Mathf.Abs(b - target.y));
                                            int fcost = Gcost[a][b] + hcost;

                                            //Add the node to the open list
                                            openList.add(new Node(new SquareIndex(a, b), fcost, hcost));
                                        }
                                        //8.If adjacent cell is already on the open list, check to see if this  path to that cell from the starting location is
										// a better one. If so, change the parent of the cell and its G and F costs.	
                                        else
                                        {
                                            //Figure out and set its G Cost
                                            addedGCost = isDiagonal ? 14 : 10;
                                            tempGcost = Gcost[parentXval][parentYval] + addedGCost;

                                            //If this path is shorter (G cost is lower) then change the parent cell, G cost and F cost. 		
											if (tempGcost < Gcost[a][b])
											{
                                                parent[a][b] = new SquareIndex(parentXval, parentYval); //change the square's parent
                                                Gcost[a][b] = tempGcost;//change the G cost

                                                //Because changing the G cost also changes the F cost, if the item is on the open list we need to change the
												//item's recorded F cost and its position on the open list to make sure that we maintain a properly ordered
												//open list.
                                                for (int x = 1; x <= openList.count; x++)
                                                {
													//HACK: There is a bug in the binary heap code that doesn't count the number of items correctly
													//if (m_openList.items[x] != null)
													//{
                                                        //If it is the current node
                                                        if (openList.items[x].index.x == a && openList.items[x].index.y == b)
                                                        {
                                                            //Change the F cost
                                                            openList.items[x].FCost = Gcost[a][b] + openList.items[x].HCost;
                                                            openList.updateItemAtIndex(x);
                                                        }
													//}
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            //If open list is empty then there is no path.	
            else
            {
                isTherePath = false;
                break;
            }

            //If target is added to open list then path has been found.
            if (isOnOpenList[(int)target.x][(int)target.y])
            {
                isTherePath = true;
                break;
            }

        }
        while (true);

        //If there is a path, save it
        if (isTherePath)
        {
            SquareIndex node = target;
            List<SquareIndex> nodeList = new List<SquareIndex>();

            //Working backwards from the target to the starting location by checking each cell's parent, figure out the length of the path.
            do
            {
                nodeList.Add(node);
                node = parent[node.x][node.y];
            }
            while (node != start);

            //Create the array
            int index = 1;
            List<SquareIndex> path = new List<SquareIndex>();

            for (int i = nodeList.Count - 1; i >= 0; i--)
            {
                path.Add(nodeList[i]);
                index++;
            }

            return path;
        }
        else
        {
			return new List<SquareIndex>();
        }
    }

	private static bool canMoveTo(int x, int y)
	{
		bool isPlayer = canUseNonEmptySquare || board.boardData[x][y].player == null;

		return 	(board.boardData[x][y].info == BoardSquareData.Info.Empty || board.boardData[x][y].info == BoardSquareData.Info.Ball) && isPlayer;
	}

	private static int calculateNumDiagonalsInPath(SquareIndex startIndex, int a, int b)
	{
		bool isDiagonal;
		int diagonals = 0;

		SquareIndex currentIndex = new SquareIndex(a, b);
		SquareIndex parentIndex = parent[currentIndex.x][currentIndex.y];

		if (parentIndex == null) return 0;

		while (parentIndex != startIndex)
		{
			isDiagonal = (Mathf.Abs(currentIndex.x - parentIndex.x) == 1 && Mathf.Abs(currentIndex.y - parentIndex.y) == 1);
			if (isDiagonal) diagonals++;

			currentIndex = new SquareIndex(parentIndex.x, parentIndex.y);
			parentIndex = parent[currentIndex.x][currentIndex.y];
		}

		//Check between the start index and the first index
		isDiagonal = (Mathf.Abs(currentIndex.x - startIndex.x) == 1 && Mathf.Abs(currentIndex.y - startIndex.y) == 1);
		if (isDiagonal) diagonals++;

		return diagonals;
	}
}