using UnityEngine;
using System.Collections;

public class BoardPositionList : MonoBehaviour
{
	public Transform[] positionList;
	private Transform turnToken;

	public void init(Transform turnToken, int index)
	{
		this.turnToken = turnToken;
		updateTurn(index);
	}

	public void updateTurn(int index)
	{
		turnToken.transform.position = positionList[index].position;
	}
}