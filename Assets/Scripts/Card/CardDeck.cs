using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class CardDeck : MonoBehaviour
{
	private const float speed = 2.5f;

	public Transform deckPosition;
	public Transform showPosition;
	public Transform handPosition;

	private List<Card> cardList;		//The list of cards the player has in his hand
	private List<CardData> cardDeck;	//The player's deck, that contains all the cards left

	private Team team;

	private Action endAction;
	private Card currentCard;

	private Vector3 startPosition;
	private Vector3 targetPosition;

	public void init(Team team)
	{
		this.team = team;

		cardList = new List<Card>();
		cardDeck = CardStore.retrieveCardDeck(team.teamData.id);
	}

	public void drawCard()
	{
		currentCard = instantiateCard();
		currentCard.flip();

		startPosition = deckPosition.position;
		targetPosition = showPosition.position;

		endAction = onShowPosition;
		StartCoroutine(updatePosition());
	}

	private Card instantiateCard()
	{
		int index = UnityEngine.Random.Range(0, cardDeck.Count);
		Card card = EntityManager.instantiateCard();
		
		card.init(cardDeck[index]);
		
		card.transform.parent = transform;
		card.transform.position = deckPosition.position;
		
		cardDeck.RemoveAt(index);
		cardList.Add(card);

		return card;
	}

	private IEnumerator updatePosition()
	{
		float t = 0;

		while(t < 1)
		{
			t += speed * Time.deltaTime;
			currentCard.transform.position = Vector3.Lerp(startPosition, targetPosition, t);

			yield return new WaitForSeconds(Time.deltaTime);
		}

		yield return new WaitForSeconds(1.5f);
		endAction();
	}

	private void onShowPosition()
	{
		startPosition = showPosition.position;
		targetPosition = handPosition.position;

		endAction = onHandPosition;
		StartCoroutine(updatePosition());
	}

	private void onHandPosition()
	{

	}
}