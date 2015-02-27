using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class PassArrow : MonoBehaviour
{
	public Action<SquareIndex> onClick;
	public Action onCancel;

	public GameObject arrowPart01Prefab;
	public GameObject arrowPart02Prefab;

	private Board board;
	private Player player;

	private SquareIndex startIndex;
	private SquareIndex currentIndex;

	private GameObject arrow;

	private bool isActive;

	void Update()
	{
		if (isActive)
		{
			updateMouse();
		}
	}

	public void init(Board board, Player player)
	{
		this.board = board;
		this.player = player;

		startIndex = player.index;
		currentIndex = getCorrectIndex();
		
		isActive = true;
		transform.position = board.squareIndexToWorld(startIndex);

		board.showRadius(startIndex, player.playerData.level, true);
		
		rotateArrow();
		create(startIndex, currentIndex);
	}

	private SquareIndex getCorrectIndex()
	{
		SquareIndex index = new SquareIndex(player.team.isP1Team ? (player.index.x - 2) : (player.index.x + 2), player.index.y);
		if (!board.isOnBounds(index)) index = new SquareIndex(player.team.isP1Team ? (player.index.x + 2) : (player.index.x - 2), index.y);
		
		return index;
	}
	
	private void updateMouse()
	{
		RaycastHit rayHit = new RaycastHit();
		Ray ray = GameCamera.camera.ScreenPointToRay(Input.mousePosition);
		
		if (Input.GetMouseButtonDown(0))
		{
			if (Physics.Raycast(ray, out rayHit, GameConfig.RAY_DISTANCE, GameConfig.boardLayerMask))
			{
				SquareIndex index = rayHit.collider.GetComponent<BoardSquare>().index;
				
				if (startIndex == index) cancel();
				else if (currentIndex == index) accept();
				else moveTo(index);
			}
		}
	}

	private void accept()
	{
		isActive = false;

		board.hideRadius();
		if (onClick != null) onClick(currentIndex);

		GameObject.Destroy(gameObject);
	}
	
	private void cancel()
	{
		board.hideRadius();
		if (onCancel != null) onCancel();

		GameObject.Destroy(gameObject);
	}

	private void moveTo(SquareIndex index)
	{
		if (board.isIndexInActiveTileList(index))
		{
			currentIndex = index;

			rotateArrow();
			create(startIndex, currentIndex);
		}
	}

	private void rotateArrow()
	{
		Vector2 direction = currentIndex.V2 - startIndex.V2;
		float scale = direction.magnitude * 0.5f;
		Quaternion rotation = Quaternion.FromToRotation(Vector3.right, new Vector3(direction.x, 0, direction.y));
		
		transform.rotation = rotation;
	}
	
	private void create(SquareIndex startIndex, SquareIndex endIndex)
	{
		if (arrow != null) GameObject.Destroy(arrow);
		arrow = new GameObject("Arrow");
		
		float magnitude = (endIndex - startIndex).magnitude;
		float radius = magnitude * 0.5f;
		Vector2 center = new Vector2(radius, 0.0f);
		
		float full = Mathf.PI;	//Only half a circunference
		int sections = 10;
		int index = 1;
		
		Vector2[] points = new Vector2[sections];
		
		for(int i = index; i < sections; ++i )
		{
			float angle = -Mathf.PI * 0.5f + full * ((float)i / sections);
			
			float posx = center.x + Mathf.Sin( angle ) * radius;
			float posy = center.y + Mathf.Cos( angle ) * radius;
			
			points[i] = new Vector2(posx, posy);
		}
		
		float startx = -points[index].x;
		float starty = -points[index].y;
		
		for(int i = index; i < sections - 1; ++i )
		{
			Vector2 p1 = points[i];
			Vector2 p2 = points[i + 1];
			
			Vector2 v = p2 - p1;
			Vector2 point = p1 + v * 0.5f;
			
			float angle = MathUtil.signedAngle(v, Vector2.right);
			
			GameObject part;
			
			PassArrowTile pass_01 = null;
			PassArrowTile pass_02 = null;
			
			if (i == sections - 2)
			{
				part = GameObject.Instantiate(arrowPart02Prefab) as GameObject;
			}
			else
			{
				part = GameObject.Instantiate(arrowPart01Prefab) as GameObject;
				pass_01 = part.GetComponent<PassArrowTile>();
			}
			
			part.transform.Rotate(0, 0, angle);
			
			part.transform.localScale = new Vector3(v.magnitude, 1, 1);
			part.transform.parent = arrow.transform;
			part.transform.localPosition = new Vector3(startx + point.x, starty + point.y, 0);
			
			float vmagnitude = Mathf.Abs(p2.x - p1.x);
			if (pass_01 != null)
			{
				pass_01.init();
				pass_01.setPosition2(new Vector3(startx + point.x, starty + point.y, 0), vmagnitude);
			}
		}
		
		arrow.transform.parent = transform;
		
		arrow.transform.localPosition = Vector3.zero;
		arrow.transform.localRotation = Quaternion.identity;
	}
}