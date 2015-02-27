using UnityEngine;
using System.Collections.Generic;

public class PlayersStore
{
	public static int statTestMod = 1;

	public static PlayerData retrievePlayerData(string teamId, string playerId)
	{
		string query = "SELECT pl.id, pl.name, pl.position, pl.level, pl.squareIndex " +
				"FROM players AS pl INNER JOIN teamPlayers AS tp ON pl.id = tp.playerId " +
				"WHERE pl.id = '" + playerId + "' AND tp.teamId = '" + teamId + "'";
		
		DataBaseHandler.open(Application.persistentDataPath + "/Data/PlayerData.db");
		DataTable data = DataBaseHandler.executeQuery (query);

		PlayerData playerData = new PlayerData();

		playerData.id = data.Rows[0]["id"].ToString();
		playerData.name = data.Rows[0]["name"].ToString();
		playerData.position = data.Rows[0]["position"].ToString();
		playerData.level = int.Parse(data.Rows[0]["level"].ToString());

		string[] index = data.Rows[0]["squareIndex"].ToString().Split(',');
		playerData.startIndex = new SquareIndex(int.Parse(index[0]), int.Parse(index[1]));

		DataBaseHandler.close();
		
		return playerData;
	}
}