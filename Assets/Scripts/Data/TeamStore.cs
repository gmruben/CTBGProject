using UnityEngine;
using System.Collections.Generic;

public class TeamStore
{
    public static TeamData retrieveTeam(string teamId)
    {
		string query = "SELECT id, name FROM teams WHERE id = '" + teamId + "'";
		
		DataBaseHandler.open(Application.persistentDataPath + "/Data/PlayerData.db");
		DataTable data = DataBaseHandler.executeQuery (query);

		TeamData team = new TeamData();

    	team.id = data.Rows[0]["id"].ToString();
		team.name = data.Rows[0]["name"].ToString();

		team.playerIdList = retrieveTeamPlayerIds(teamId);

		DataBaseHandler.close();

        return team;
    }

    private static string[] retrieveTeamPlayerIds(string teamId)
    {
		string query = "SELECT players.playerId " +
						"FROM teams INNER JOIN teamPlayers AS players ON teams.id = players.teamId " +
						"WHERE teams.id = '" + teamId + "'";

		DataBaseHandler.open(Application.persistentDataPath + "/Data/PlayerData.db");
		DataTable data = DataBaseHandler.executeQuery (query);

        List<string> players = new List<string>();
		for (int i = 0; i < data.Rows.Count;i++)
        {
			players.Add(data.Rows[i]["playerId"].ToString());
        }

		DataBaseHandler.close();

        return players.ToArray();
    }
}