using UnityEngine;
using System.Collections;

public class MainMenu : UIMenu
{
	public UIButton playButton;
	public UIButton optionsButton;

	public UIButton quitButton;

	public void init()
	{
		playButton.onClick += onPlayButtonClick;
		optionsButton.onClick += onOptionsButtonClick;

		quitButton.onClick += onQuitButtonClick;
	}

	public override void setEnabled (bool isEnabled)
	{
		
	}

	private void onPlayButtonClick()
	{
		Application.LoadLevel("Game");
	}

	private void onOptionsButtonClick()
	{
		App.instance.showOptionsMenu();
	}

	private void onQuitButtonClick()
	{
		Application.Quit ();
	}
}