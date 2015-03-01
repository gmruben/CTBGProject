using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameHUD : UIMenu
{
	public Text p1NumMoves;
	public Text p2NumMoves;

	public UIButton p1EndTurnButton;
	public UIButton p2EndTurnButton;

	private CheckMenu checkMenu;

	public void init()
	{
		p1EndTurnButton.onClick += onEndTurnButtonClick;
		p2EndTurnButton.onClick += onEndTurnButtonClick;
	}

	public override void setEnabled (bool isEnabled)
	{
		
	}

	public void updateNumMoves(bool isP1TeamTurn, Team p1team, Team p2Team)
	{
		p1EndTurnButton.setActive(isP1TeamTurn);
		p2EndTurnButton.setActive(!isP1TeamTurn);

		p1NumMoves.text = "MOVIMIENTOS: " + p1team.numMoves;
		p2NumMoves.text = "MOVIMIENTOS: " + p2Team.numMoves;
	}

	private void onEndTurnButtonClick()
	{
		checkMenu = MenuManager.instantiateCheckMenu();
		checkMenu.init("¿QUIERES TERMINAR EL TURNO?");

		checkMenu.yesButton.onClick += onCheckMenuYesButtonClick;
		checkMenu.noButton.onClick += onCheckMenuNoButtonClick;
	}

	private void onCheckMenuYesButtonClick()
	{
		checkMenu.yesButton.onClick -= onCheckMenuYesButtonClick;
		checkMenu.noButton.onClick -= onCheckMenuNoButtonClick;

		GameObject.Destroy(checkMenu.gameObject);
		MessageBus.dispatchUserTurnEnded();
	}

	private void onCheckMenuNoButtonClick()
	{
		checkMenu.yesButton.onClick -= onCheckMenuYesButtonClick;
		checkMenu.noButton.onClick -= onCheckMenuNoButtonClick;

		GameObject.Destroy(checkMenu.gameObject);
	}
}