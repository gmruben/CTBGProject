using UnityEngine;
using System.Collections;

public class PlayerInfoMenu : MonoBehaviour
{
	public TextMesh playerNumber;
	public TextMesh playerPosition;
	public TextMesh playerLevel;
	public TextMesh playerName;

	private PlayerData playerData;

	public void init(PlayerData playerData)
	{
		this.playerData = playerData;

		playerNumber.text = playerData.number.ToString();
		playerPosition.text = playerData.position;
		playerLevel.text = playerData.level.ToString();
		playerName.text = playerData.name;
	}
}