using UnityEngine;
using System.Collections;

public class Token : MonoBehaviour
{
	private Renderer renderer;

	public void setTexture(Texture texture)
	{
		renderer = GetComponentInChildren<MeshRenderer>();
		renderer.material.SetTexture("_MainTex", texture);
	}
}