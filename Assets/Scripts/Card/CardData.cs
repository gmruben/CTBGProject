using UnityEngine;
using System.Collections;

public class CardData
{
	public string id;

	public string teamId;
	public string title;
	public string comment;
	public string player1Id;
	public string player2Id;
	public int value;
	public string action;
	public string inAction;
	public string outAction;
}

public class ActionIds
{
	public string Motivation;
	public string Move;
}

public class ConditionIds
{
	public string Goal;							//Play the card after a player's goal
	public string Action;						//Play the card after a player's action
	public string OpponentGKBox;				//Play the card on the opponent GK's box
	public string ShotOnOpponentGKBox;			//Play the card before a shot on the opponent GK's box
}

public class InActionIds
{
	public string KickOff;
	public string SecondHalf;
	public string AnyTime;
	public string RoundStart;
	public string TurnStart;
	public string InsteadDiceRoll;
}

public class OutActionIds
{
	public string RoundEnd;
	public string TurnEnd;
	public string MatchEnd;
	public string NextTurn;
}