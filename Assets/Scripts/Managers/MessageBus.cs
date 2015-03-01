using UnityEngine;
using System;
using System.Collections;

public class MessageBus
{
    public static event Action UserTurnEnded;
	public static event Action GoalScored;

    public static void dispatchUserTurnEnded()
    {
		if (UserTurnEnded != null) UserTurnEnded();
    }

	public static void dispatchGoalScored()
	{
		if (GoalScored != null) GoalScored();
	}
}