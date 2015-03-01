using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Ball : MonoBehaviour
{
	private const float passSpeed = 5.0f;

	public event Action onClearEnded;
	public event Action onPassEnded;
	public event Action onShootEnded;

	public Transform cachedTransform { get; private set; }

	private Board board;
	private Player owner;

	public int power { get; private set; }
	private bool canBeIntercepted;

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

		//Add the player level as power
		power = owner.playerData.level;
		canBeIntercepted = true;

		endAction = endPass;
		StartCoroutine(updateMove());
	}

	public void clear(SquareIndex index)
	{
		this.index = index;
		toMoveSquareList = new List<SquareIndex>();

		int directionX = UnityEngine.Random.Range(-1, 2);
		int directionY = UnityEngine.Random.Range(-1, 2);

		//Check that the direction is not (0, 0)
		if (directionX == 0 && directionY == 0) directionX = 1;

		int distance = UnityEngine.Random.Range(1, 7);
		for (int i = 0; i < distance; i++)
		{
			toMoveSquareList.Add(index + new SquareIndex(directionX, directionY) * i);
		}

		canBeIntercepted = false;

		endAction = endClear;
		StartCoroutine(updateMove());
	}

	private void endPass()
	{
		checkForPlayer();
		if (onPassEnded != null) onPassEnded();
	}

	private void endClear()
	{
		checkForPlayer();
		if (onClearEnded != null) onClearEnded();
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

			//Decrease power
			power--;

			//Only check for interceptions if the ball can be intercepted
			if (canBeIntercepted)
			{
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

		//Add the player level as power
		power = owner.playerData.level;
		canBeIntercepted = true;

		endAction = endShoot;
		StartCoroutine(updateMove());
	}

	private void endShoot()
	{
		owner.team.opponentTeam.gk.save(index);
		if (onShootEnded != null) onShootEnded();
	}

	private void checkForPlayer()
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
	}

	private void checkForInterception(Player player)
	{
		//If player's level is greater or equal to ball's power, the player intercepts the ball
		if (player.playerData.level >= power)
		{
			player.intercept(index, true);
			if (onPassEnded != null) onPassEnded();
		}
		else
		{
			player.intercept(index, false);
			StartCoroutine(updateMove());
		}
	}
}