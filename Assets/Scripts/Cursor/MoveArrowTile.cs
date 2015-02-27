using UnityEngine;
using System.Collections;

public class MoveArrowTile : MonoBehaviour
{
	private const float offset = 0.025f;

	public GameObject tile;
	public GameObject tileShadow;

	public void rotate(int angle)
	{
		transform.rotation = Quaternion.AngleAxis(angle, Vector3.up);
		tileShadow.transform.position = new Vector3(tile.transform.position.x + offset, tileShadow.transform.position.y, tile.transform.position.z - offset);
	}
}