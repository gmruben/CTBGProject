﻿using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Ball : MonoBehaviour
{
	private const float passSpeed = 5.0f;

	public event Action onPassEnded;
	public event Action onShootEnded;

	public Transform cachedTransform { get; private set; }

	private Board board;
	private Player owner;

	public SquareIndex index { get; private set; }
	private List<SquareIndex> toMoveSquareList;

	private Action endAction;

	public void init(Board board)
	{
		cachedTransform = transform;

		this.board = board;
	}

	public void pass(Player owner, SquareIndex index, List<SquareIndex> toMoveSquareList)
	{
		this.owner = owner;
		this.index = index;
		this.toMoveSquareList = toMoveSquareList;

		endAction = endPass;
		StartCoroutine(updateMove());
	}

	private void endPass()
	{
		//Check if there is a player in the end index
		Player player = board.getPlayerOnSquare(index);
		if (player != null)
		{
			player.giveBall();
		}
		else
		{
			board.setBallPosition(index);
		}
		
		if (onPassEnded != null) onPassEnded();
	}

	public IEnumerator updateMove()
	{
		if (toMoveSquareList.Count > 0)
		{
			SquareIndex nextSquareIndex = toMoveSquareList[0];
			Vector2 direction = nextSquareIndex.V2 - index.V2;
			
			Vector3 position = board.squareIndexToWorld(index);
			Vector3 targetPosition = board.squareIndexToWorld(nextSquareIndex);
			
			float magnitude = (targetPosition - position).sqrMagnitude;
			float currentMagnitude = (cachedTransform.position - position).sqrMagnitude;
			
			Vector3 directionV3 = Util.v2ToV3(direction);
			
			while (currentMagnitude < magnitude)
			{
				cachedTransform.position += directionV3 * passSpeed * Time.deltaTime;
				currentMagnitude = (cachedTransform.position - position).sqrMagnitude;
				
				yield return null;
			}
			cachedTransform.position = board.squareIndexToWorld(nextSquareIndex);
			
			index = nextSquareIndex;
			toMoveSquareList.RemoveAt(0);

			//Check for opponents in the new index
			List<Player> playerList = board.retrieveAdjacentPlayerList(index, owner.team.opponentTeam.teamData.id);
			if (playerList.Count > 0)
			{
				checkForInterception(playerList[0]);
			}
			else
			{
				StartCoroutine(updateMove());
			}
		}
		else
		{
			endAction();
		}
	}

	public void shoot(Player owner, SquareIndex index, List<SquareIndex> toMoveSquareList)
	{
		this.owner = owner;
		this.index = index;
		this.toMoveSquareList = toMoveSquareList;

		endAction = endShoot;
		StartCoroutine(updateMove());
	}

	private void endShoot()
	{
		MessageBus.dispatchGoalScored();
		if (onShootEnded != null) onShootEnded();
	}

	private void checkForInterception(Player player)
	{
		player.setInactive(true);
		StartCoroutine(updateMove());
	}
}