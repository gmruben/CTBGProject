using UnityEngine;
using System.Collections.Generic;

public class CardStore
{
    public static List<CardData> retrieveCardDeck(string teamId)
    {
		string query = "SELECT id, teamId, player1Id, player2Id, value, inAction, outAction FROM cards WHERE teamId = '" + teamId + "'";
		
		DataBaseHandler.open(Application.persistentDataPath + "/Data/PlayerData.db");
		DataTable data = DataBaseHandler.executeQuery (query);

		List<CardData> cardDataList = new List<CardData>();

		for (int i = 0; i < data.Rows.Count; i++)
		{
			CardData cardData = new CardData();

			cardData.id = data.Rows[i]["id"].ToString();
			cardData.teamId = data.Rows[i]["teamId"].ToString();

			cardData.title = StringStore.retrieve(cardData.id + "Title");
			cardData.comment = StringStore.retrieve(cardData.id + "Comment");
			cardData.action = StringStore.retrieve(cardData.id + "Action");

			cardData.player1Id = data.Rows[i]["player1Id"].ToString();
			cardData.player2Id = data.Rows[i]["player2Id"].ToString();
			cardData.value = (int) data.Rows[i]["value"];
			cardData.inAction = data.Rows[i]["inAction"].ToString();
			cardData.outAction = data.Rows[i]["outAction"].ToString();

			cardDataList.Add(cardData);
		}
	
		DataBaseHandler.close();

        return cardDataList;
    }
}