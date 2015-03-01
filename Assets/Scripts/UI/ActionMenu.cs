using UnityEngine;
using System;
using System.Collections;

public class ActionMenu : UIMenu
{
	public event Action onMove;

	public GameObject menuButtons;
	public GameObject actionButtons;

	public UIButton moveButton;
	public UIButton tackleButton;
	public UIButton passButton;
	public UIButton shootButton;
	public UIButton menuCancelButton;

	public UIButton acceptButton;
	public UIButton actionCancelButton;

	public void init(Player player)
	{
		menuButtons.SetActive(true);
		actionButtons.SetActive(false);

		passButton.setActive(player.hasTheBall);
		shootButton.setActive(player.hasTheBall);

		moveButton.onClick += onAction;
		tackleButton.onClick += onAction;
		passButton.onClick += onAction;
		shootButton.onClick += onAction;
	}

	public override void setEnabled (bool isEnabled)
	{
		
	}

	private void onAction()
	{
		menuButtons.SetActive(false);
		//if (onMove != null) onMove();
	}
}