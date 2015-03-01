using UnityEngine;
using System.Collections;

public class PlayerData
{
	public string id;
	public string name;
	public string position;
	public int number;
	public int level;
	public SquareIndex startIndex;
}

public class PlayerPositionIds
{
	public static string GK = "P";
	public static string DF = "DF";
	public static string MF = "MD";
	public static string FW = "DL";
}