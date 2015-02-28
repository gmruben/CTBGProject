using UnityEngine;
using UnityEngine.UI;

public class CheckMenu : UIMenu
{
	public Text label;

	public UIButton yesButton;
	public UIButton noButton;

	public void init(string message)
	{
		label.text = message;

		yesButton.setText (StringStore.retrieve("yes"));
		noButton.setText (StringStore.retrieve("no"));
	}

	public override void setEnabled(bool isEnabled)
	{

	}
}