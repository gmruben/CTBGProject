using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Team
{
	public event Action onUpdateNumMoves;

	public bool isP1Team { get; set; }
	private TeamData _teamData;

	private int modifier;			//Stores all the modifier applied by cards to the team
	
	private int _numMoves;

	private TeamController teamController;
	private CardDeck cardDeck;
	private List<Player> _playerList;

	public Team opponentTeam { get; set; }

	public Player gk { get; private set; }

	public Team(TeamData teamData, TeamController teamController, CardDeck cardDeck)
	{
		_teamData = teamData;
		_playerList = new List<Player>();

		this.teamController = teamController;
		this.cardDeck = cardDeck;

		teamController.init(this);
		cardDeck.init(this);
	}

	public void addPlayer(Player player)
	{
		_playerList.Add(player);

		//Check if it is the GK
		if (player.playerData.position == PlayerPositionIds.GK)
		{
			gk = player;
		}
	}

	private void createDeck()
	{

	}

	public void startRound()
	{
		cardDeck.drawCard();
		cardDeck.drawCard();
		cardDeck.drawCard();
		cardDeck.drawCard();
	}

	public void startTurn()
	{
		_numMoves = GameConfig.NUM_MOVES_IN_TURN;

	}

	public void endTurn()
	{

	}

	public void update()
	{
		teamController.update();
	}

	public void updateNumMoves(int moves)
	{
		_numMoves -= moves;
		if (onUpdateNumMoves != null) onUpdateNumMoves();
	}

	#region PROPERTIES

	public TeamData teamData
	{
		get { return _teamData; }
	}

	public int numMoves
	{
		get { return _numMoves; }
	}

	public List<Player> playerList
	{
		get { return _playerList; }
	}

	#endregion
}