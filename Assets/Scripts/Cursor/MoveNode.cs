using UnityEngine;
using System.Collections;

public class MoveNode : MonoBehaviour
{
	public TextMesh valueLabel;
	
	public SquareIndex index { get; set; }

	public void init(int value, SquareIndex index)
	{
		valueLabel.text = value.ToString();
		this.index = index;
	}

	public void setValue(int value)
	{
		valueLabel.text = value.ToString();
	}
}