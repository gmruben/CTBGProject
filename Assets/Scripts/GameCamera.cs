using UnityEngine;
using System.Collections;

public class GameCamera : MonoBehaviour
{
	private const float cameraSpeed = 10;

	private const int minPosY = 7;
	private const int maxPosY = 14;

	private static Camera _camera;
	private Transform cachedTransform;
	
	private float lastMouseX;
	private float lastMouseY;

	private const float mouseSpeed = 0.50f;
	private bool isMouseDown = false;

	void Start()
	{
		GameCamera._camera = GetComponentInChildren<Camera>();
		cachedTransform = transform;
	}

	void Update()
	{
		float deltax = Input.GetAxis("Horizontal") * cameraSpeed * Time.deltaTime;
		float deltaz = Input.GetAxis("Vertical") * cameraSpeed * Time.deltaTime;

		transform.position += new Vector3(deltax, 0, deltaz);

		if (Input.GetAxis("Mouse ScrollWheel") < 0)
		{
			if (_camera.transform.position.y < maxPosY) _camera.transform.position += Vector3.up;
		}
		if (Input.GetAxis("Mouse ScrollWheel") > 0)
		{
			if (_camera.transform.position.y > minPosY) _camera.transform.position -= Vector3.up;
		}

		//updateMouse();
	}

	private void updateMouse()
	{
		if (Input.GetMouseButtonDown(0))
		{
			lastMouseX = Input.mousePosition.x;
			lastMouseY = Input.mousePosition.y;

			isMouseDown = true;
		}
		
		if (Input.GetMouseButton(0) && isMouseDown)
		{
			float mouseX = Input.mousePosition.x;
			float deltaX = mouseX - lastMouseX;
			float mouseY = Input.mousePosition.y;
			float deltaY = mouseY - lastMouseY;
			
			float posx = cachedTransform.position.x - Time.deltaTime * deltaX * mouseSpeed;
			//posx = Mathf.Clamp(posx, -(_sizex + 1), _sizex + 1);
			float posz = cachedTransform.position.z - Time.deltaTime * deltaY * mouseSpeed;
			//posz = Mathf.Clamp(posz, -(_sizey + 1), _sizey + 1);
			
			cachedTransform.position = new Vector3(posx, cachedTransform.position.y, posz);
			
			lastMouseX = mouseX;
			lastMouseY = mouseY;
		}
		
		if (Input.GetMouseButtonUp(0))
		{
			isMouseDown = false;
		}
	}

	public static Camera camera
	{
		get { return _camera; }
	}
}