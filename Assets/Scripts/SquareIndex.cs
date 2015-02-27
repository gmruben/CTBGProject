using UnityEngine;
using System.Collections;

public class SquareIndex
{
	public int x;
	public int y;
	
	public SquareIndex(int x, int y)
	{
		this.x = x;
		this.y = y;
	}
	
	public override string ToString()
	{
		return x + ", " + y;
	}
	
	public override bool Equals(object obj)
	{
		SquareIndex index = obj as SquareIndex;
		return index.x == x && index.y == y;
	}
	
	public static bool operator ==(SquareIndex a, SquareIndex b)
	{
		// If both are null, or both are same instance, return true.
		if (System.Object.ReferenceEquals(a, b))
		{
			return true;
		}
		
		// If one is null, but not both, return false.
		if (((object)a == null) || ((object)b == null))
		{
			return false;
		}
		
		// Return true if the fields match:
		return a.x == b.x && a.y == b.y;
	}
	
	public static bool operator !=(SquareIndex a, SquareIndex b)
	{
		return !(a == b);
	}
	
	//HACK: UFFFF, mira todo esto...
	public static SquareIndex operator -(SquareIndex index1, SquareIndex index2)
	{
		return new SquareIndex(index1.x - index2.x, index1.y - index2.y);
	}
	
	public float magnitude
	{
		get { return Mathf.Sqrt(x * x + y * y); }
	}
	
	public Vector2 V2
	{
		get { return new Vector2(x, y); }
	}
}