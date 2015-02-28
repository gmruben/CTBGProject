using UnityEngine;
using System.IO;
using System.Collections;

public class Game : MonoBehaviour
{
	public GameHUD gameHUD;
	public Board board;

	private Match match;
	private Ball ball;

	private bool isActive = false;

	void Awake()
	{
		if (!StringStore.isInit)
		{
			StringStore.onInit += onStringStoreInit;
			StringStore.instance.init();
		}
		else
		{
			onStringStoreInit();
		}
	}

	private void onStringStoreInit()
	{	
		if (!File.Exists(Application.persistentDataPath + "/Data/PlayerData.db"))
		{
			SqliteUtils.instance.copyDatabaseComplete += copyDatabaseComplete;
			SqliteUtils.instance.initialiseDatabases();
		}
		else
		{
			init();
		}
	}

	private void copyDatabaseComplete()
	{
		SqliteUtils.instance.copyDatabaseComplete -= copyDatabaseComplete;
		init();
	}

	public void init()
	{
		board.init();
		gameHUD.init();

		match = new Match();

		ball = EntityManager.instantiateBall();
		ball.init(board);

		TeamData p1TeamData = new TeamData();
		TeamData p2TeamData = new TeamData();

		p1TeamData.id = "Nankatsu";
		p1TeamData.name =  "NANKATSU";
		p2TeamData.id = "Touhou";
		p2TeamData.name =  "TOUHOU";

		TeamController p1TeamController = new TeamUserController(board);
		TeamController p2TeamController = new TeamUserController(board);

		Team p1Team = new Team(p1TeamData, p1TeamController, board.p1Deck);
		Team p2Team = new Team(p2TeamData, p2TeamController, board.p2Deck);

		//Create players
		string[] playerIdList = TeamStore.retrieveTeam(p1TeamData.id).playerIdList;
		for (int i = 0; i < playerIdList.Length; i++)
		{
			Player player = EntityManager.instantiatePlayer();
			PlayerData playerData = PlayersStore.retrievePlayerData(p1TeamData.id, playerIdList[i]);

			SquareIndex index = playerData.startIndex;

			player.init(p1Team, playerData, board, ball);
			player.setPosition(index);
			
			p1Team.addPlayer(player);
			board.updatePlayerPosition(index, player);
		}
		
		//Create opponent players
		string[] opponentPlayerIdList = TeamStore.retrieveTeam(p2TeamData.id).playerIdList;
		for (int i = 0; i < opponentPlayerIdList.Length; i++)
		{
			Player player = EntityManager.instantiatePlayer();
			PlayerData playerData = PlayersStore.retrievePlayerData(p2TeamData.id, opponentPlayerIdList[i]);

			SquareIndex index = board.inverseIndex(playerData.startIndex);

			player.init(p2Team, playerData, board, ball);
			player.setPosition(index);
			
			p2Team.addPlayer(player);
			board.updatePlayerPosition(index, player);
		}

		match.currentTeam = p1Team;

		match.p1Team = p1Team;
		match.p2Team = p2Team;

		p1Team.opponentTeam = p2Team;
		p2Team.opponentTeam = p1Team;

		match.p1Team.onUpdateNumMoves += onTeamUpdateNumMoves;
		match.p2Team.onUpdateNumMoves += onTeamUpdateNumMoves;

		p1Team.playerList[p1Team.playerList.Count - 1].giveBall();

		startRound();

		isActive = true;
		MessageBus.GoalScored += goalScored;
	}

	private void onTeamUpdateNumMoves()
	{
		gameHUD.updateNumMoves(match.p1Team, match.p2Team);
	}

	private void startRound()
	{
		match.p1Team.startRound();
		match.p2Team.startRound();

		startTurn();
	}

	private void startTurn()
	{	
		TurnOverlay turnOverlay = GUICreator.instantiateTurnOverlay();
		turnOverlay.init(StringStore.retrieve("Turn") + " " + match.currentTeam.teamData.name, onTurnOverlayEnded);
	}
	
	private void onTurnOverlayEnded()
	{
		//MessageBus.UserTurnEnded += userTurnEnded;

		match.currentTeam.startTurn();
		gameHUD.updateNumMoves(match.p1Team, match.p2Team);
	}
	
	private void userTurnEnded()
	{
		//MessageBus.UserTurnEnded -= userTurnEnded;
		endTurn();
	}
	
	private void endTurn()
	{
		match.currentTeam.endTurn();
		match.currentTeam = (match.currentTeam.teamData.id == match.p1Team.teamData.id) ? match.p2Team : match.p1Team;

		startTurn();
	}

	void Update()
	{
		if (isActive)
		{
			match.currentTeam.update();
		}
	}

	private void goalScored()
	{	
		TurnOverlay turnOverlay = GUICreator.instantiateTurnOverlay();
		turnOverlay.init("GOAL", onGoalOverlayEnded);
	}
	
	private void onGoalOverlayEnded()
	{
		match.currentTeam.startTurn();
		gameHUD.updateNumMoves(match.p1Team, match.p2Team);

		resetPlayerPositions();

		board.removeBallFromSquare(ball.index);
		match.p1Team.playerList[match.p1Team.playerList.Count - 1].giveBall();
	}

	private void resetPlayerPositions()
	{
		//P1 TEAM
		for (int i = 0; i < match.p1Team.playerList.Count; i++)
		{
			Player player = match.p1Team.playerList[i];

			player.setPosition(player.playerData.startIndex);
			if (player.isInactive) player.setInactive(false);
		}

		//P2 TEAM
		for (int i = 0; i < match.p2Team.playerList.Count; i++)
		{
			Player player = match.p2Team.playerList[i];

			player.setPosition(board.inverseIndex(player.playerData.startIndex));
			if (player.isInactive) player.setInactive(false);
		}
	}
}