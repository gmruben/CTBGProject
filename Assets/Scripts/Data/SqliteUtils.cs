using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class SqliteUtils : MonoBehaviour
{
    public Action copyDatabaseComplete;

    public static SqliteUtils _instance;

    private int numDatabases;

    public static SqliteUtils instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new GameObject("SqliteUtils").AddComponent<SqliteUtils>();
            }

            return _instance;
        }
    }

    public void initialiseDatabases()
    {
        numDatabases = 2;

        //StartCoroutine(copyDatabaseFromBundle("English"));
		StartCoroutine(copyDatabaseFromBundle("Spanish"));
		//StartCoroutine(copyDatabaseFromBundle("Japanese"));

        StartCoroutine(copyDatabaseFromBundle("PlayerData"));
    }

    private IEnumerator copyDatabaseFromBundle(string databaseName)
    {
        string dataBasePath = Application.persistentDataPath + "/Data/" + databaseName + ".db";

        if (!File.Exists(dataBasePath))
        {
			Debug.Log("FILE EXISTS");
            WWW wwwFile = new WWW(Util.streamingAssetsPath + "/Data/" + databaseName + ".db");
            yield return wwwFile;

            //Create Directory if it doesn't exist
            if (!Directory.Exists(Application.persistentDataPath + "/Data")) Directory.CreateDirectory(Application.persistentDataPath + "/Data");

            //Save to persistent data path
            File.WriteAllBytes(dataBasePath, wwwFile.bytes);
        }
		else
		{
			Debug.Log("FILE DOESN'T EXIST");
		}

        numDatabases--;
        if (numDatabases == 0)
        {
			createProgressTable();
        }
    }

	private void createProgressTable()
	{
		createSettingsTable();

		if (copyDatabaseComplete != null) copyDatabaseComplete();
	}
	
	private void createSettingsTable()
	{
		string create = "CREATE TABLE IF NOT EXISTS settings (" +
						"id TEXT PRIMARY KEY, " +
						"value TEXT " +
						")";

		DataBaseHandler.open(Application.persistentDataPath + "/Data/Progress.db");
		DataBaseHandler.executeNonQuery(create);

		foreach( KeyValuePair<string, string> entry in SettingsIds.defaults )
		{
			try
			{
				string insert = "INSERT OR IGNORE INTO settings ( id, value ) VALUES ( '" + entry.Key + "', '" + entry.Value + "' )";
				DataBaseHandler.executeNonQuery(insert);
			}
			catch (System.Exception error) 
			{
				Debug.Log(error.ToString());
				DataBaseHandler.close();
				
				return;
			}
		}
		
		DataBaseHandler.close();
	}
}