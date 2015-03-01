using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class TackleArrow : MonoBehaviour
{
    public Action<Player> onClick;
    public Action onCancel;

    private Board board;
	private Player player;

    private SquareIndex startIndex;
    private bool isActive = false;

    void Update()
    {
        if (isActive)
        {
			updateMouse();
        }
    }

	public void init(Board board, Player player)
    {
        this.board = board;
		this.player = player;

        isActive = true;

		startIndex = player.index;
    }
	
    private void updateMouse()
    {           
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit rayHit = new RaycastHit();
            Ray ray = GameCamera.camera.ScreenPointToRay(Input.mousePosition);

			if (Physics.Raycast(ray, out rayHit, GameConfig.RAY_DISTANCE, GameConfig.boardLayerMask))
			{
				SquareIndex index = rayHit.collider.GetComponent<BoardSquare>().index;
				Player opponent = board.getPlayerOnSquare(index);

				if (opponent != null)
				{
					if (onClick != null) onClick(opponent);
				}
			}
        }
    }
}