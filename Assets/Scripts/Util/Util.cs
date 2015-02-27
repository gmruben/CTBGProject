using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Util
{
	public static string streamingAssetsPath
	{
		get
		{
			if (Application.platform == RuntimePlatform.Android)
			{
				return "jar:file://" + Application.dataPath + "!/assets";
			}
			else
			{
				return "file://" + Application.streamingAssetsPath;
			}
		}
	}

	public static string formatNumber00(int number)
	{
		if (number < 10) return "0" + number.ToString();
		else return number.ToString();
	}

	//Returns true or false randomly
	public static bool randomBoolean()
	{
		return Random.Range(0.0f, 1.0f) < 0.5f;
	}

	public static bool isIPad()
	{
#if UNITY_IPHONE && UNITY_EDITOR
		return true;
#elif UNITY_IPHONE
		return (iPhone.generation == iPhoneGeneration.iPad1Gen 		||
		        iPhone.generation == iPhoneGeneration.iPad2Gen 		||
		        iPhone.generation == iPhoneGeneration.iPad3Gen 		||
		        iPhone.generation == iPhoneGeneration.iPad4Gen 		||
		        iPhone.generation == iPhoneGeneration.iPad5Gen 		||
		        iPhone.generation == iPhoneGeneration.iPadMini1Gen  ||
		        iPhone.generation == iPhoneGeneration.iPadMini2Gen);
#else
		return false;
#endif
	}

	//Converts a Vector3 into a Vector2 using x and z coordinates
	public static Vector2 v3ToV2(Vector3 vector)
	{
		return new Vector2(vector.x, vector.z);
	}

	//Converts a Vector2 into a Vector3 using x and z coordinates
	public static Vector3 v2ToV3(Vector2 vector)
	{
		return new Vector3(vector.x, 0, vector.y);
	}
}