using UnityEngine;
using System.Collections;

public class SquareColorFull : MonoBehaviour
{
	public GameObject square01;
	public GameObject square02;

	public GameObject shadow;

	public void init(Color color)
	{
		setColor(color);
	}

	public void setColor(Color color)
	{
		ColorPalette.setMeshToColor(square01, color);
		ColorPalette.setMeshToColor(square02, color);
		
		ColorPalette.setMeshToColor(shadow, Color.black);
	}
}