using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour
{
	private const float moveSpeed = 5.0f;

	public event Action onMoveEnded;
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
			toMoveSquareList.RemoveAt(0);

			StartCoroutine(updateMove());
		}
		else
		{
			//Check if the ball is in the end square
			if (board.isBallOnSquare(index))
			{
				board.removeBallFromSquare(index);
				giveBall();
			}

			board.updatePlayerPosition(index, this);
			if (onMoveEnded != null) onMoveEnded();
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