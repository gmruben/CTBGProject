using UnityEngine;
using System.Collections;

public class EntityManager
{
	public static Ball instantiateBall()
	{
		GameObject playerPrefab = Resources.Load<GameObject>("Prefabs/Game/Ball");
		return (GameObject.Instantiate(playerPrefab) as GameObject).GetComponent<Ball>();
	}

    public static Player instantiatePlayer()
    {
		GameObject playerPrefab = Resources.Load<GameObject>("Prefabs/Game/TokenPlayer");
        return (GameObject.Instantiate(playerPrefab) as GameObject).GetComponent<Player>();
    }

	public static GameObject instantiateTokenTime()
	{
		GameObject playerPrefab = Resources.Load<GameObject>("Prefabs/Game/TokenTime");
		return (GameObject.Instantiate(playerPrefab) as GameObject);
	}

	public static GameObject instantiateTokenTurn()
	{
		GameObject playerPrefab = Resources.Load<GameObject>("Prefabs/Game/TokenTurn");
		return (GameObject.Instantiate(playerPrefab) as GameObject);
	}

	public static Token instantiateTokenTeam()
	{
		GameObject playerPrefab = Resources.Load<GameObject>("Prefabs/Game/TokenTeam");
		return (GameObject.Instantiate(playerPrefab) as GameObject).GetComponent<Token>();
	}

	public static Card instantiateCard()
	{
		GameObject playerPrefab = Resources.Load<GameObject>("Prefabs/Game/Card");
		return (GameObject.Instantiate(playerPrefab) as GameObject).GetComponent<Card>();
	}

	public static MoveArrow instantiateMoveArrow()
	{
		GameObject playerPrefab = Resources.Load<GameObject>("Prefabs/Game/MoveArrow");
		return (GameObject.Instantiate(playerPrefab) as GameObject).GetComponent<MoveArrow>();
	}

	public static TackleArrow instantiateTackleArrow()
	{
		GameObject playerPrefab = Resources.Load<GameObject>("Prefabs/Game/TackleArrow");
		return (GameObject.Instantiate(playerPrefab) as GameObject).GetComponent<TackleArrow>();
	}
}