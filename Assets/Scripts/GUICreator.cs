using UnityEngine;
using System.Collections;

public class GUICreator
{
    public static TurnOverlay instantiateTurnOverlay()
    {
		GameObject resource = Resources.Load<GameObject>("Prefabs/UI/Game/TurnOverlay");
		return (GameObject.Instantiate(resource) as GameObject).GetComponent<TurnOverlay>();
    }
}