using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class StringStore : MonoBehaviour
{
	private static Dictionary<string, string> idList;

	public static event Action onInit;
	private static StringStore _instance;

	public static bool isInit = false;

	public static StringStore instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new GameObject("StringStore").AddComponent<StringStore>();
			}
			
			return _instance;
		}
	}

	public void init ()
	{
		string language = "Spanish"; //SettingsStore.retrieveSetting<string> (SettingsIds.language);
		idList = new Dictionary<string, string> ();

		StartCoroutine(readLanguage(language));
	}

	private IEnumerator readLanguage(string language)
	{
		WWW wwwFile = new WWW(Util.streamingAssetsPath + "/Languages/" + language + ".tsv");
		yield return wwwFile;

		string[] lines = wwwFile.text.Split(new Char[] { '\n' });

		/*StreamReader sr = new StreamReader(Util.streamingAssetsPath + "/Languages/" + languague + ".tsv");
		string fileContents = sr.ReadToEnd();
		sr.Close();
		string[] lines = fileContents.Split(new Char[] { '\n' });*/
		
		int length = lines.Length;
		for (int i = 1; i < length; ++i)
		{
			string line = lines[i];
			string[] values = line.Split(new Char[] { '\t' });
			
			string id = values[1].Trim();
			string value = values[2].Trim();
		
			if (id != "") idList.Add(id, value);
		}

		isInit = true;
		if (onInit != null) onInit();
	}

    public static string retrieve(string id)
    {
		if (idList == null) Debug.LogError("YOU NEED TO INIT STRING STORE BEFORE RETRIEVING A STRING");

        if (idList.ContainsKey(id)) return idList[id];
		else Debug.Log("THE DICTIONARY DOESN'T CONTAIN THE ID: " + id);

		return "";
    }
}