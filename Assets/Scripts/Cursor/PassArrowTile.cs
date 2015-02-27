using UnityEngine;
using System.Collections;

public class PassArrowTile : MonoBehaviour
{
	public GameObject front;
	public GameObject back;

	public GameObject shadowPrefab;
	private GameObject shadow;

	public void init()
	{
		ColorPalette.setMeshToColor(front, ColorPalette.red);
		ColorPalette.setMeshToColor(back, ColorPalette.red * 0.5f);

		shadow = GameObject.Instantiate(shadowPrefab) as GameObject;
		ColorPalette.setMeshToColor(shadow.GetComponentInChildren<MeshFilter>().gameObject, Color.black);

		shadow.transform.parent = transform.parent;
	}

	public void setPosition2(Vector3 position, float magnitude)
	{
		shadow.transform.localPosition = new Vector3(transform.localPosition.x, 0.01f, transform.localPosition.z);
		shadow.transform.rotation = Quaternion.identity;
		shadow.transform.localScale = new Vector3(magnitude, 1, 1);
	}
}