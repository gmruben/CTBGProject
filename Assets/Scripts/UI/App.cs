using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// This class controls all the flow in the menus.
/// </summary>
public class App : MonoBehaviour
{
	private static App _instance;
	private Stack<UIMenu> menus = new Stack<UIMenu>();

	public static App instance
	{ 
		get
		{
			return _instance;
		}
	} 
	
	void Start()
	{	
		if (_instance == null)
		{
			_instance = this;
			_instance.init();
		}
	}

	public void init()
	{
		//Create the current menu and add it to the stack
		MainMenu mainMenu = MenuManager.instantiateMainMenu ();
		mainMenu.init ();
		
		menus.Push (mainMenu);
	}

	public void showOptionsMenu()
	{
		//Set current menu inactive
		UIMenu currentMenu = menus.Peek();
		currentMenu.setActive(false);

		OptionsMenu gameMenu = MenuManager.instantiateOptionsMenu();
		gameMenu.init();
		
		menus.Push(gameMenu);
	}

	/// <summary>
	/// Goes back to the previous menu.
	/// </summary>
	public void back()
	{
		//Get last menu and set it active
		UIMenu menu = menus.Pop();
		
		UIMenu currentMenu = menus.Peek();
		currentMenu.setActive(true);

		//Destroy the current menu
		GameObject.Destroy(menu.gameObject);
	}
}