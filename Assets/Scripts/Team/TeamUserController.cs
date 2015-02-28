﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TeamUserController : TeamController
{
	private Player currentPlayer;

	private MoveArrow moveArrow;
	private ActionMenu actionMenu;

	private bool isMenuActive;

	public TeamUserController(Board board): base(board)
	{

	}
	
	public override void update()
	{
		if (!isMenuActive)
		{
			RaycastHit rayHit = new RaycastHit();
			Ray ray = GameCamera.camera.ScreenPointToRay(Input.mousePosition);
			
			if (Physics.Raycast(ray, out rayHit, GameConfig.RAY_DISTANCE, GameConfig.boardLayerMask))
			{
				SquareIndex index = rayHit.collider.GetComponent<BoardSquare>().index;

				if (Input.GetMouseButtonDown(0) && board.isPlayerOnSquare(index))
				{
					currentPlayer = board.getPlayerOnSquare(index);

					if (currentPlayer.team.teamData.id == team.teamData.id && !currentPlayer.isInactive && team.numMoves > 0)
					{
						showActionMenu();
					}
				}
			}
		}
	}

	private void showActionMenu()
	{
		actionMenu = MenuManager.instantiateActionMenu();

		addActionMenuListeners();
		actionMenu.init(currentPlayer);

		isMenuActive = true;
	}

	private void addActionMenuListeners()
	{
		actionMenu.moveButton.onClick += onMoveButtonClick;
		actionMenu.passButton.onClick += onPassButtonClick;
		actionMenu.shootButton.onClick += onShootButtonClick;
		actionMenu.menuCancelButton.onClick += onCancelButtonClick;
	}

	private void removeActionMenuListeners()
	{
		actionMenu.moveButton.onClick -= onMoveButtonClick;
		actionMenu.passButton.onClick -= onPassButtonClick;
		actionMenu.shootButton.onClick -= onShootButtonClick;
		actionMenu.menuCancelButton.onClick -= onCancelButtonClick;
	}

	private void onMoveButtonClick()
	{
		moveArrow = EntityManager.instantiateMoveArrow();
		moveArrow.init(board, currentPlayer, team.numMoves, false);
			
		moveArrow.onClick += moveTo;
		//moveArrow.onCancel += cancelMove;
	}

	private void moveTo(List<SquareIndex> indexList)
	{
		currentPlayer.move(indexList);
		currentPlayer.onMoveEnded += onMoveEnded;

		team.updateNumMoves(indexList.Count);

		isMenuActive = false;
		GameObject.Destroy(moveArrow.gameObject);
	}

	private void onMoveEnded()
	{
		currentPlayer.onMoveEnded -= onMoveEnded;
		isMenuActive = false;
	}

	private void onPassButtonClick()
	{
		moveArrow = EntityManager.instantiateMoveArrow();
		moveArrow.init(board, currentPlayer, team.numMoves, true);
		
		moveArrow.onClick += passTo;
	}

	private void passTo(List<SquareIndex> indexList)
	{
		currentPlayer.pass(indexList);
		currentPlayer.onPassEnded += onPassEnded;

		GameObject.Destroy(moveArrow.gameObject);
	}

	private void onPassEnded()
	{
		currentPlayer.onPassEnded -= onPassEnded;
		isMenuActive = false;
	}

	private void onShootButtonClick()
	{
		moveArrow = EntityManager.instantiateMoveArrow();
		moveArrow.init(board, currentPlayer, team.numMoves, true);
		
		moveArrow.onClick += shootTo;
	}

	private void shootTo(List<SquareIndex> indexList)
	{
		//Check that the target index is correct
		if (indexList[indexList.Count - 1].x == 17)
		{
			currentPlayer.shoot(indexList);
			isMenuActive = false;

			GameObject.Destroy(moveArrow.gameObject);
		}
	}

	private void onCancelButtonClick()
	{
		removeActionMenuListeners();
		GameObject.Destroy(actionMenu.gameObject);

		isMenuActive = false;
	}
}