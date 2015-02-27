using UnityEngine;
using System.Collections;

public class Scorecard : MonoBehaviour
{
	public TextMesh p1TeamNameLabel;
	public TextMesh p2TeamNameLabel;

	public TextMesh p1MovesLabel;
	public TextMesh p2MovesLabel;

	public void init(string p1TeamName, string p2TeamName)
	{
		p1TeamNameLabel.text = p1TeamName;
		p2TeamNameLabel.text = p2TeamName;
	}

	public void updateNumMoves(int p1Moves, int p2Moves)
	{
		p1MovesLabel.text = "MOVES: " + p1Moves.ToString();
		p2MovesLabel.text = "MOVES: " + p2Moves.ToString();
	}
}