﻿using UnityEngine;
using System.Collections;

public class Card : MonoBehaviour
{
	public TextMesh title;
	public TextMesh comment;
	public TextMesh player;
	public TextMesh level;
	public TextMesh action;
	public TextMesh inAction;
	public TextMesh outAction;

	public Renderer cardRenderer;
	public Renderer imageRenderer;

	private CardAction cardAction;

	public void init(CardData cardData)
	{
		string playerName = "";

		if (cardData.player1Id == "-")
		{
			if (cardData.player2Id == "-") playerName = StringsStore.retrieve(cardData.teamId);
			else StringsStore.retrieve(cardData.player2Id);
		}
		else 
		{
			if (cardData.player2Id == "-") playerName = StringsStore.retrieve(cardData.player1Id);
			else playerName = StringsStore.retrieve(cardData.player2Id) + "\n" + StringsStore.retrieve(cardData.player2Id);
		}

		title.text = cardData.title;
		comment.text = cardData.comment;
		player.text = playerName;
		level.text = cardData.value.ToString();
		action.text = cardData.action;
		inAction.text = StringsStore.retrieve(cardData.inAction);
		outAction.text = StringsStore.retrieve(cardData.outAction);

		//HACK: Haz esto sin hardcodearlo
		if (cardData.teamId == "Nankatsu")
		{
			comment.color = ColorPalette.HexToColor("00AEFF");
			player.color = ColorPalette.HexToColor("FFF200");
			level.color = ColorPalette.HexToColor("FFFFFF");
			action.color = ColorPalette.HexToColor("283891");
		}
		else
		{
			comment.color = ColorPalette.HexToColor("262262");
			player.color = ColorPalette.HexToColor("FFFFFF");
			level.color = ColorPalette.HexToColor("FFDE16");
			action.color = ColorPalette.HexToColor("FFDE16");
		}

		Texture cardTexture = Resources.Load<Texture>("Textures/Cards/" + cardData.teamId + "Card_01_Texture");
		Texture imageTexture = Resources.Load<Texture>("Textures/Cards/" + cardData.teamId + "/CardImage_" + cardData.id);

		cardRenderer.material.SetTexture("_MainTex", cardTexture);
		imageRenderer.material.SetTexture("_MainTex", imageTexture);

		cardAction = new CardAction_N04A();
	}

	public void use()
	{
		cardAction.use();
	}
}