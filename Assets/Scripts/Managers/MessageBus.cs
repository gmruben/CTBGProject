using UnityEngine;
using System;
using System.Collections;

public class MessageBus
{
    public static event Action UserMoveEnded;
	public static event Action GoalScored;

    public static void dispatchUserMoveEnded()
    {
		if (UserMoveEnded != null) UserMoveEnded();
    }

	public static void dispatchGoalScored()
	{
		if (GoalScored != null) GoalScored();
	}
}