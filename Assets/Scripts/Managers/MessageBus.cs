using UnityEngine;
using System;
using System.Collections;

public class MessageBus
{
    public static event Action UserTurnEnded;
	public static event Action GoalScored;

	public static event Action<Team> ThrowIn;
	public static event Action<bool> Corner;
	public static event Action<bool> GoalKick;

    public static void dispatchUserTurnEnded()
    {
		if (UserTurnEnded != null) UserTurnEnded();
    }

	public static void dispatchGoalScored()
	{
		if (GoalScored != null) GoalScored();
	}

	public static void dispatchThrowIn(Team throwInTeam)
	{
		if (ThrowIn != null) ThrowIn(throwInTeam);
	}

	public static void dispatchCorner(bool isP1Team)
	{
		if (Corner != null) Corner(isP1Team);
	}

	public static void dispatchGoalKick(bool isP1Team)
	{
		if (GoalKick != null) GoalKick(isP1Team);
	}
}