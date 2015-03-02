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
		p1TeamData.name =  "NEWTEAM S.C.";
		p2TeamData.id = "Touhou";
		p2TeamData.name =  "MUPPET F.C.";

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

		match.p1Team.isP1Team = true;
		match.p2Team.isP1Team = false;

		p1Team.opponentTeam = p2Team;
		p2Team.opponentTeam = p1Team;

		match.p1Team.onUpdateNumMoves += onTeamUpdateNumMoves;
		match.p2Team.onUpdateNumMoves += onTeamUpdateNumMoves;

		p1Team.playerList[p1Team.playerList.Count - 1].giveBall();

		startRound();

		isActive = true;

		MessageBus.UserTurnEnded += userTurnEnded;
		MessageBus.GoalScored += goalScored;
		MessageBus.ThrowIn += throwIn;
	}

	private void onTeamUpdateNumMoves()
	{
		gameHUD.updateNumMoves(match.currentTeam.isP1Team, match.p1Team, match.p2Team);
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
		match.currentTeam.startTurn();
		gameHUD.updateNumMoves(match.currentTeam.isP1Team, match.p1Team, match.p2Team);
	}
	
	private void userTurnEnded()
	{
		match.currentTeam.endTurn();
		match.currentTeam = match.currentTeam.opponentTeam;

		TurnOverlay turnOverlay = GUICreator.instantiateTurnOverlay();
		turnOverlay.init(StringStore.retrieve("Turn") + " " + match.currentTeam.teamData.name, onTurnOverlayEnded);
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
		turnOverlay.init("GOL", onGoalOverlayEnded);
	}
	
	private void onGoalOverlayEnded()
	{
		match.currentTeam.startTurn();
		gameHUD.updateNumMoves(match.currentTeam.isP1Team, match.p1Team, match.p2Team);

		resetPlayerPositions();

		board.removeBallFromSquare(ball.index);
		match.p1Team.playerList[match.p1Team.playerList.Count - 1].giveBall();
	}

	private void throwIn(Team throwInTeam)
	{
		match.currentTeam.endTurn();
		match.currentTeam = throwInTeam;

		TurnOverlay turnOverlay = GUICreator.instantiateTurnOverlay();
		turnOverlay.init("SAQUE DE BANDA", onthrowInOverlayEnded);
	}
	
	private void onthrowInOverlayEnded()
	{
		match.currentTeam.startTurn();
		gameHUD.updateNumMoves(match.currentTeam.isP1Team, match.p1Team, match.p2Team);

		//Move a player to the ball index
		Player player = match.currentTeam.playerList[10];

		player.setPosition(ball.index);
		player.giveBall();
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