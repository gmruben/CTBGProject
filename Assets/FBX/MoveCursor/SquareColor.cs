using UnityEngine;
using System.Collections;

public class SquareColor : MonoBehaviour
{
	public GameObject square01;
	public GameObject shadow;

	public void init(Color color)
	{
		ColorPalette.setMeshToColor(square01, color);
		ColorPalette.setMeshToColor(shadow, Color.black);
	}
}