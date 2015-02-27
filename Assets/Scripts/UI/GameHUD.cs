using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameHUD : UIMenu
{
	//public Scorecard scorecard;

	public Text p1NumMoves;
	public Text p2NumMoves;

	public override void setEnabled (bool isEnabled)
	{
		
	}

	public void updateNumMoves(Team p1team, Team p2Team)
	{
		p1NumMoves.text = "MOVIMIENTOS: " + p1team.numMoves;
		p2NumMoves.text = "MOVIMIENTOS: " + p2Team.numMoves;
	}
}