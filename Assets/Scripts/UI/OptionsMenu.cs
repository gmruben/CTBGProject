using UnityEngine;
using System.Collections;

public class OptionsMenu : UIMenu
{
	public UIButton applyButton;
	public UIButton backButton;

	public void init()
	{
		applyButton.onClick += onApplyButtonClick;
		backButton.onClick += onBackButtonClick;
	}

	public override void setEnabled (bool isEnabled)
	{
		
	}

	private void onApplyButtonClick()
	{

	}

	private void onBackButtonClick()
	{
		App.instance.back();
	}
}