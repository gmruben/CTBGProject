using UnityEngine;

public class StringsStore
{
    public static string retrieve(string id)
    {
        string query = "SELECT value FROM strings WHERE id = '" + id + "'";

		DataBaseHandler.open(Application.persistentDataPath + "/Data/" + SettingsStore.retrieveSetting<string>(SettingsIds.language) + ".db");
		DataTable data = DataBaseHandler.executeQuery (query);

		if (data.Rows.Count == 0) Debug.LogError("Couldn't find the string with ID: " + id);
		string value = data.Rows[0]["value"].ToString ();
        
		DataBaseHandler.close ();

        return value;
    }
}