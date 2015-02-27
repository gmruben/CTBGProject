using UnityEngine;
using System.Collections.Generic;

public class SettingsStore
{
	private static Dictionary<string, string> settings = new Dictionary<string, string>();

    public static void storeSetting(string id, string value)
    {
        //string encValue = EncryptionHelper.EncryptString(value);
		string insert = "REPLACE INTO settings ( id, value ) VALUES ( '" + id + "', '" + value + "' )";

		DataBaseHandler.open(Application.persistentDataPath + "/Data/Progress.db");
		DataBaseHandler.executeNonQuery (insert);
		DataBaseHandler.close ();

		if (settings.ContainsKey(id)) settings[id] = value;
		else settings.Add(id, value);
    }

    public static void storeSetting(string id, int value)
    {
        storeSetting(id, value.ToString());
    }

    public static void storeSetting(string id, float value)
    {
        storeSetting(id, value.ToString());
    }

    public static void storeSetting(string id, bool value)
    {
        storeSetting(id, value ? "1" : "0");
    }

    public static void deleteSetting(string id)
    {
        string query = "DELETE FROM settings WHERE id = '" + id + "'";

		DataBaseHandler.open(Application.persistentDataPath + "/Data/Progress.db");
		DataBaseHandler.executeNonQuery (query);
		DataBaseHandler.close ();
    }

    public static T retrieveSetting<T>(string id)
    {
		object value = null;
		if (settings.ContainsKey(id))
		{
			string valueString = settings[id];
			value = retrieveValueByType<T>(valueString);
		}
		else
		{
	        string query = "SELECT value FROM settings WHERE id = '" + id + "'";

			DataBaseHandler.open(Application.persistentDataPath + "/Data/Progress.db");
			DataTable data = DataBaseHandler.executeQuery (query);

		    //string decValue = EncryptionHelper.DecryptString(valueString);

			if (data.Rows.Count > 0)
			{
				string valueString = data.Rows[0]["value"].ToString();
				value = retrieveValueByType<T>(valueString);
			}
			else
			{
				if( typeof(T) == typeof(uint) )
				{
					value = 0;
					storeSetting(id, (uint) value);
				}
				else if( typeof(T) == typeof(int) )
				{
					value = 0;
					storeSetting(id, (int) value);
				}
				else if( typeof(T) == typeof(float) )
				{
					value = 0.0f;
					storeSetting(id, (float) value);
				}
				else if( typeof(T) == typeof(bool) )
				{
					value = false;
					storeSetting(id, (bool) value);
				}
			}

			DataBaseHandler.close ();
		}
		return (T)value;
    }

	private static T retrieveValueByType<T>(string value)
	{
		object newValue = null;
		if (typeof(T) == typeof(string))
		{
			newValue = value;
		}
		else if (typeof(T) == typeof(int))
		{
			newValue = int.Parse(value);
		}
		else if (typeof(T) == typeof(float))
		{
			newValue = float.Parse(value);
		}
		else if (typeof(T) == typeof(bool))
		{
			newValue = (value == "1");
		}
		return (T)newValue;
	}

    public static void createtable()
    {
		//No se que ha pasado aqui pero lo que habia antes no estaba bien (creaba la tabla cada vez que pedia la conexion)
		string create = "CREATE TABLE IF NOT EXISTS settings (" +
			"id TEXT PRIMARY KEY, " +
				"value TEXT " +
				")";
    }
}