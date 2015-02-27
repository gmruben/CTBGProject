using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class TurnOverlay : MonoBehaviour
{
	public Text label;

	private Action onEndAction;

	public void init(string message, Action onEndAction)
	{
		label.text = message;
		this.onEndAction = onEndAction;

		animation.Play("TurnOverlay_Anim");
	}

	private void onAnimationEnded()
	{
		if (onEndAction != null) onEndAction();
		GameObject.Destroy(gameObject);
	}
}