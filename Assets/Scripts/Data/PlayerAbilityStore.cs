using UnityEngine;
using System.Collections;

public class PlayerAbilityStore : MonoBehaviour
{
	public static PlayerAbility retrieve(string id)
    {
        string query = "SELECT id, name, description, sp, ap, power, type, category FROM abilities where id = '" + id + "'";

		DataBaseHandler.open(Application.persistentDataPath + "/Data/Players.db");
		DataTable data = DataBaseHandler.executeQuery (query);

		PlayerAbility ability = new PlayerAbility();

		if (data.Rows.Count > 0)
		{
			ability.id = data.Rows [0]["id"].ToString ();
			ability.name = data.Rows [0] ["name"].ToString ();
			ability.description = data.Rows [0] ["description"].ToString ();
			ability.sp = (int)data.Rows [0] ["sp"];
			ability.ap = (int)data.Rows [0] ["ap"];
			ability.power = (int)data.Rows [0] ["power"] * PlayersStore.statTestMod;
			ability.type = data.Rows [0] ["type"].ToString ();
			ability.category = data.Rows [0] ["category"].ToString ();
		}
		else
		{
			Debug.Log("COULDN'T FIND ABILITY WITH ID " + id);
		}

		DataBaseHandler.close ();

        return ability;
    }
}

public struct PlayerAbility
{
	public string id;
    public string name;
    public string description;
    public int sp;
    public int ap;
	public int power;
    public string type;
	public string category;

	public static bool wins(string c1, string c2)
	{
		//HACK: No creo que vaya a utilizar esto...
		if (c1 == "power" && c2 == "skill") return true;
		else if (c1 == "skill" && c2 == "agility") return true;
		else if (c1 == "agility" && c2 == "power") return true;
		else return false;
	}
}