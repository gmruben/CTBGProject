using UnityEngine;
using System.Collections;

/// <summary>
/// This class stores a reference to the UI Camera and have some other useful functions
/// </summary>
public class UICamera : MonoBehaviour
{
    private static UICamera _instance;

	[HideInInspector]
    public Camera cachedCamera;
    
    public static UICamera instance
    {
        get
        {
            if (_instance == null)
			{
				_instance = GameObject.Find("UICamera").GetComponent<UICamera>();
				_instance.init();
			}

            return _instance;
        }
    }

    private void init()
    {
        cachedCamera = GetComponent<Camera>();
    }

	/// <summary>
	/// Checks it the mouse is over a GUI element
	/// </summary>
	/// <returns><c>true</c>, if mouse is over GUI, <c>false</c> otherwise.</returns>
	public bool isMouseOnGUI()
	{
		return UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject();
	}
}