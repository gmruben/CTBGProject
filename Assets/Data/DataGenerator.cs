#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

using System;
using System.IO;
using System.Text.RegularExpressions;

using System.Text;

public class DataGenerator
{
    [MenuItem("Data/Generate Text Strings")]
	static void GenerateTextStrings()
	{
		GenerateTextDataBase("English");
		GenerateTextDataBase("Spanish");
	}

    static void GenerateTextDataBase(string language)
    {
        FileUtil.DeleteFileOrDirectory("Assets/StreamingAssets/Data/" + language + ".db");

        string create = "CREATE TABLE IF NOT EXISTS strings (" +
                "id TEXT PRIMARY KEY, " +
                "value TEXT " +
                ")";
        
		DataBaseHandler.open("Assets/StreamingAssets/Data/" + language + ".db");
		DataBaseHandler.executeNonQuery (create);

        StreamReader sr = new StreamReader("Assets/Data/StringTexts/" + language + ".html");
        string fileContents = sr.ReadToEnd();
        sr.Close();

        int start = fileContents.IndexOf("<tr");
        int end = fileContents.LastIndexOf("</tr>");
        fileContents = fileContents.Substring(start, end - start);

		string insert;

        string[] data = fileContents.Split(new string[] { "</tr>" }, StringSplitOptions.None);
        int length = data.Length;
        for (int i = 2; i < length; ++i)
        {
            string dataString = data[i];
            string[] items = dataString.Split(new string[] { "</td>" }, StringSplitOptions.None);
            for (int j = 0; j < items.Length; ++j)
            {
                string item = items[j];
                items[j] = Regex.Replace(item.Replace("<br>", "\n").Replace("&nbsp;", " ").Replace("&amp;", "&"), @"<td.*?>", "").Trim();
            }

			if (items[1] != String.Empty)
            {
				try
				{
					insert = "INSERT INTO strings ( id, value ) VALUES ( '" + items[1] + "', '" + items[2] + "' )";

					//Encode string so we don't get an exception because of the accents
					Encoding utf8 = Encoding.UTF8;
					Encoding ascii = Encoding.ASCII;
					
					byte[] utf8Bytes = utf8.GetBytes(insert);
					
					byte[] unicodeBytes = Encoding.Convert(utf8, ascii, utf8Bytes);
					
					char[] uniChars = new char[ascii.GetCharCount(unicodeBytes, 0, unicodeBytes.Length)];
					ascii.GetChars(unicodeBytes, 0, unicodeBytes.Length, uniChars, 0);
					
					string asciiString = new string(uniChars);

					DataBaseHandler.executeNonQuery(asciiString);
				}
				catch (System.Exception error) 
				{
					Debug.Log(error.ToString());
					DataBaseHandler.close();
					
					return;
				}
            }
        }

		DataBaseHandler.close();

        Debug.Log(language + ".db generated.");
    }

    [MenuItem("Data/Generate Player Data")]
    private static void generatePlayerData()
    {
        FileUtil.DeleteFileOrDirectory("Assets/StreamingAssets/Data/PlayerData.db");

		DataBaseHandler.open("Assets/StreamingAssets/Data/PlayerData.db");

		createTeamsDataBase();
		createPlayersDataBase();
		createCardDataBase();
		createTeamPlayerDataBase();

		DataBaseHandler.close ();

        Debug.Log("Players.db generated.");
    }

	private static void createTeamsDataBase()
	{
		string create;
		string insert;

		create = 	"CREATE TABLE IF NOT EXISTS teams (" +
				"id TEXT PRIMARY KEY, " +
				"name TEXT " +
				")";
		
		DataBaseHandler.executeNonQuery (create);
		
		StreamReader sr = new StreamReader("Assets/Data/GameData/TeamData.tsv");
		string fileContents = sr.ReadToEnd();
		sr.Close();
		string[] lines = fileContents.Split(new Char[] { '\n' });
		
		int length = lines.Length;
		for (int i = 1; i < length; ++i)
		{
			string line = lines[i];
			string[] values = line.Split(new Char[] { '\t' });
			
			insert = "INSERT INTO teams (id, name) VALUES ( " +
					"'" + values[0].Trim() + "', " +
					"'" + values[1].Trim() + "' )";
			try
			{
				Debug.Log(insert);
				DataBaseHandler.executeNonQuery(insert);
			}
			catch (System.Exception error) 
			{
				Debug.Log(error.ToString());
				DataBaseHandler.close();
				
				break;
			}
		}
	}

	private static void createPlayersDataBase()
    {
		string create;
		string insert;
        
		create = 	"CREATE TABLE IF NOT EXISTS players (" +
            		"id TEXT PRIMARY KEY, " +
                	"name TEXT, " +
                	"position TEXT, " +
					"shirtNumber INTEGER, " +
					"level INTEGER, " +
                	"squareIndex TEXT " +
                	")";
        
		Debug.Log(create);
		DataBaseHandler.executeNonQuery (create);

        StreamReader sr = new StreamReader("Assets/Data/GameData/PlayerData.tsv");
        string fileContents = sr.ReadToEnd();
        sr.Close();
        string[] lines = fileContents.Split(new Char[] { '\n' });

        int length = lines.Length;
        for (int i = 1; i < length; ++i)
        {
            string line = lines[i];
            string[] values = line.Split(new Char[] { '\t' });

			insert = "INSERT INTO players (id, name, position, shirtNumber, level, squareIndex) VALUES ( " +
					"'" + values[0].Trim() + "', " +
					"'" + values[1].Trim() + "', " +
					"'" + values[2].Trim() + "', " +
					"'" + values[3].Trim() + "', " +
					"'" + values[4].Trim() + "', " +
					"'" + values[5].Trim() + "')";

            try
            {
                DataBaseHandler.executeNonQuery(insert);
            }
            catch (System.Exception error) 
            {
                Debug.Log(error.ToString());
				DataBaseHandler.close();

                break;
            }
        }
    }

	private static void createCardDataBase()
	{
		string create;
		string insert;
		
		create = 	"CREATE TABLE IF NOT EXISTS cards (" +
			"id TEXT PRIMARY KEY, " +
				"teamId TEXT, " +
				"player1Id TEXT, " + 
				"player2Id TEXT, " +
				"value INTEGER, " +
				"inAction TEXT, " +
				"outAction TEXT " +
				")";
		
		Debug.Log(create);
		DataBaseHandler.executeNonQuery (create);
		
		StreamReader sr = new StreamReader("Assets/Data/GameData/CardData.tsv");
		string fileContents = sr.ReadToEnd();
		sr.Close();
		string[] lines = fileContents.Split(new Char[] { '\n' });
		
		int length = lines.Length;
		for (int i = 1; i < length; ++i)
		{
			string line = lines[i];
			string[] values = line.Split(new Char[] { '\t' });

			if (values[0].Trim() != String.Empty)
			{
				insert = "INSERT INTO cards (id, teamId, player1Id, player2Id, value, inAction, outAction) VALUES ( " +
					"'" + values[0].Trim() + "', " +
					"'" + values[1].Trim() + "', " +
					"'" + values[2].Trim() + "', " +
					"'" + values[3].Trim() + "', " +
					"'" + values[4].Trim() + "', " +
					"'" + values[5].Trim() + "', " +
					"'" + values[6].Trim() + "')";
				Debug.Log(insert);
				try
				{
					DataBaseHandler.executeNonQuery(insert);
				}
				catch (System.Exception error) 
				{
					Debug.Log(error.ToString());
					DataBaseHandler.close();
					
					break;
				}
			}
		}
	}

	private static void createTeamPlayerDataBase()
	{
		string create;
		string insert;
		
		create = "CREATE TABLE IF NOT EXISTS teamPlayers (" +
				"teamId TEXT, " +
				"playerId TEXT, " +
				"FOREIGN KEY (teamId) REFERENCES teams(id), " +
				"FOREIGN KEY (playerId) REFERENCES players(id), " +
				"PRIMARY KEY (teamId, playerId) " +
				")";
		
		DataBaseHandler.executeNonQuery (create);
		
		StreamReader sr = new StreamReader("Assets/Data/GameData/TeamPlayerData.tsv");
		string fileContents = sr.ReadToEnd();
		sr.Close();
		string[] lines = fileContents.Split(new Char[] { '\n' });
		
		int length = lines.Length;
		for (int i = 1; i < length; ++i)
		{
			string line = lines[i];
			string[] values = line.Split(new Char[] { '\t' });
			
			insert = "INSERT INTO teamPlayers (teamId, playerId) VALUES (" +
				"'" + values[0].Trim() + "', " +
					" '" + values[1].Trim() + "')";
			
			try
			{
				DataBaseHandler.executeNonQuery(insert);
			}
			catch (System.Exception error) 
			{
				Debug.Log(error.ToString());
				DataBaseHandler.close();
				
				break;
			}
		}
	}
}

#endif