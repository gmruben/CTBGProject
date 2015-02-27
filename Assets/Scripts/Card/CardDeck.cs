using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CardDeck : MonoBehaviour
{
	private const float CARD_OFFSET_X = 0.15f;
	private const float CARD_OFFSET_Y = 0.01f;

	private List<Card> cardList;		//The list of cards the player has in his hand
	private List<CardData> cardDeck;	//The player's deck, that contains all the cards left

	private Team team;

	public void init(Team team)
	{
		this.team = team;

		cardList = new List<Card>();
		cardDeck = CardStore.retrieveCardDeck(team.teamData.id);
	}

	public void drawCard()
	{
		int index = Random.Range(0, cardDeck.Count);
		Card card = EntityManager.instantiateCard();

		card.init(cardDeck[index]);

		card.transform.parent = transform;
		card.transform.localPosition = new Vector3(cardList.Count * CARD_OFFSET_X , cardList.Count * CARD_OFFSET_Y, 0);

		cardDeck.RemoveAt(index);
		cardList.Add(card);
	}
}