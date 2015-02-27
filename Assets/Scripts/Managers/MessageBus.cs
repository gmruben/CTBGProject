using UnityEngine;
using System;
using System.Collections;

public class MessageBus
{
    public static event Action UserMoveEnded;

    public static void dispatchUserMoveEnded()
    {
		if (UserMoveEnded != null) UserMoveEnded();
    }
}