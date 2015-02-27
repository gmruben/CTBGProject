using UnityEngine;
using System.Collections;

public abstract class TeamController
{
	protected Board board;
	protected Team team;

	public TeamController(Board board)
	{
		this.board = board;
	}

	public void init(Team team)
	{
		this.team = team;
	}

	public abstract void update();
}