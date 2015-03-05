using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LogMenu : UIMenu
{
	private static LogMenu _instance;

	public Text log;

	public static LogMenu instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = MenuManager.instantiateLogMenu();
			}
			return _instance;
		}
	}

	public override void setEnabled (bool isEnabled)
	{
		
	}

	public void logMessage(string message)
	{
		log.text = "\n" + message;
	}
}