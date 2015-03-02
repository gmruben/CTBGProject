using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour
{
	private const float moveSpeed = 5.0f;

	public event Action onMoveEnded;
	public event Action onTackleEnded;
	public event Action onPassEnded;
	public event Action onShootEnded;

	public GameObject token;
	public Transform ballPosition;
	
	private SquareIndex _index;
	private Transform cachedTransform;

	public Team team { get; set; }

	private PlayerData _playerData;
	private Board board;
	private Ball ball;

	private int modifier;							//Stores all the modifier applied by cards to the player
	private List<SquareIndex> toMoveSquareList;

	private bool _hasTheBall = false;
	private bool _isInactive = false;

	private Animator animator;

	//The action for when a move finishes
	private Action endAction;

	public void init(Team team, PlayerData playerData, Board board, Ball ball)
	{
		this.team = team;
		this.board = board;
		this.ball = ball;

		_playerData = playerData;
		cachedTransform = transform;

		//Set token texture
		Texture texture = Resources.Load<Texture>("Textures/Tokens/" + team.teamData.id + "/Token_" + playerData.id);
		token.renderer.material.SetTexture("_MainTex", texture);

		animator = GetComponent<Animator>();
	}

	public void setPosition(SquareIndex index)
	{
		_index = index;
		cachedTransform.position = board.squareIndexToWorld(index);

		board.updatePlayerPosition(index, this);
	}

	public void move(List<SquareIndex> toMoveSquareList)
	{
		this.toMoveSquareList = toMoveSquareList;
		board.removePlayerFromSquare(index);

		endAction = endMove;
		StartCoroutine(updateMove());
	}

	public void pass(List<SquareIndex> toMoveSquareList)
	{
		loseBall();

		ball.onPassEnded += passEnded;
		ball.pass(this, index, toMoveSquareList);
	}

	private void passEnded()
	{
		ball.onPassEnded -= passEnded;
		if (onPassEnded != null) onPassEnded();
	}

	public void shoot(List<SquareIndex> toMoveSquareList)
	{
		loseBall();
		
		ball.onShootEnded += shootEnded;
		ball.shoot(this, index, toMoveSquareList);
	}
	
	private void shootEnded()
	{
		ball.onPassEnded -= shootEnded;
		if (onShootEnded != null) onShootEnded();
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
				cachedTransform.position += directionV3 * moveSpeed * Time.deltaTime;
				currentMagnitude = (cachedTransform.position - position).sqrMagnitude;
				
				yield return null;
			}
			cachedTransform.position = board.squareIndexToWorld(nextSquareIndex);
			
			_index = nextSquareIndex;
			board.updatePlayerPosition(_index, this);

			toMoveSquareList.RemoveAt(0);

			//If the player has the ball, check for tackles
			if (hasTheBall)
			{
				List<Player> playerList = board.retrieveAdjacentPlayerList(_index, team.opponentTeam.teamData.id);
				if (playerList.Count > 0)
				{
					checkForTackle(playerList[0]);
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

	public void giveBall()
	{
		_hasTheBall = true;

		ball.cachedTransform.parent = cachedTransform;
		ball.cachedTransform.position = ballPosition.position;
	}

	public void loseBall()
	{
		_hasTheBall = false;
		
		ball.cachedTransform.parent = null;
	}

	public void intercept(SquareIndex index, bool success)
	{
		if (success)
		{
			toMoveSquareList = new List<SquareIndex>();
			toMoveSquareList.Add(index);

			board.removePlayerFromSquare(index);
			
			endAction = endIntercept;
			StartCoroutine(updateMove());
		}
		else
		{
			setInactive(true);
		}
	}

	private void endMove()
	{
		//Check if the ball is in the end square
		if (board.isBallOnSquare(index))
		{
			board.removeBallFromSquare(index);
			giveBall();
		}

		if (onMoveEnded != null) onMoveEnded();
	}

	private void endIntercept()
	{
		giveBall();
		board.updatePlayerPosition(index, this);
	}

	public void tackle(Player opponent)
	{
		//If player's level is greater or equal to ball's power, the player intercepts the ball
		if (opponent.playerData.level < playerData.level)
		{
			giveBall();

			opponent.loseBall();
			opponent.setInactive(true);
		}
		else if (opponent.playerData.level > playerData.level)
		{
			setInactive(true);
		}
		else
		{
			setInactive(true);

			opponent.loseBall();
			opponent.setInactive(true);

			ball.clear (this, opponent.index);
		}

		if (onTackleEnded != null) onTackleEnded();
	}

	private void checkForTackle(Player opponent)
	{
		//If player's level is greater or equal to ball's power, the player intercepts the ball
		if (opponent.playerData.level > playerData.level)
		{
			loseBall();
			setInactive(true);

			opponent.giveBall();

			MessageBus.dispatchUserTurnEnded();
			if (onMoveEnded != null) onMoveEnded();
		}
		else if (opponent.playerData.level < playerData.level)
		{
			opponent.setInactive(true);

			StartCoroutine(updateMove());
		}
		else
		{
			loseBall();
			setInactive(true);

			opponent.setInactive(true);
			
			ball.clear (this, index);
		}
	}

	public void save(SquareIndex targetIndex)
	{
		if (index != targetIndex)
		{
			toMoveSquareList = PathFinder.findPath(index, targetIndex, board, false, true, 10);
		}

		endAction = endSave;
		StartCoroutine(updateMove());
	}

	private void endSave()
	{
		if (ball.power < _playerData.level)
		{
			giveBall();
		}
		else
		{
			MessageBus.dispatchGoalScored();
		}
	}

	public void setInactive(bool isInactive)
	{
		_isInactive = isInactive;

		if (isInactive) animator.Play("Flip");
		else animator.Play("FlipBack");
	}

	public PlayerData playerData
	{
		get { return _playerData; }
	}

	public SquareIndex index
	{
		get { return _index; }
	}

	public bool hasTheBall
	{
		get { return _hasTheBall; }
	}

	public bool isInactive
	{
		get { return _isInactive; }
	}
}