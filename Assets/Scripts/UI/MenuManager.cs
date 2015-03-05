using UnityEngine;
using System.Collections;

/// <summary>
/// Class for instantiating all the different menus in the game. We could have a class with all the references to the prefabs,
/// but that would mean loading all the prefabs when loading the scene, which would increase the loading time (and it is very
/// likely that we don't need them all at onche. The bad thing about using Resoucer.Load is that we are hardcoding the path.
/// </summary>
public class MenuManager
{
	public static MainMenu instantiateMainMenu()
	{
		GameObject resource = Resources.Load<GameObject>("Prefabs/UI/Menus/MainMenu");
		return (GameObject.Instantiate(resource) as GameObject).GetComponent<MainMenu>();
	}

	public static OptionsMenu instantiateOptionsMenu()
	{
		GameObject resource = Resources.Load<GameObject>("Prefabs/UI/Menus/OptionsMenu");
		return (GameObject.Instantiate(resource) as GameObject).GetComponent<OptionsMenu>();
	}

	public static ActionMenu instantiateActionMenu()
	{
		GameObject resource = Resources.Load<GameObject>("Prefabs/UI/Game/ActionMenu");
		return (GameObject.Instantiate(resource) as GameObject).GetComponent<ActionMenu>();
	}

	public static CheckMenu instantiateCheckMenu()
	{
		GameObject resource = Resources.Load<GameObject>("Prefabs/UI/CheckMenu");
		return (GameObject.Instantiate(resource) as GameObject).GetComponent<CheckMenu>();
	}

	public static LogMenu instantiateLogMenu()
	{
		GameObject resource = Resources.Load<GameObject>("Prefabs/UI/LogMenu");
		return (GameObject.Instantiate(resource) as GameObject).GetComponent<LogMenu>();
	}
}