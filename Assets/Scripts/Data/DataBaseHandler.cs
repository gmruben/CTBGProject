using UnityEngine;
using System.Collections;

//using System.Collections.Generic;
//using Mono.Data.Sqlite;

public class DataBaseHandler : MonoBehaviour
{
	private static SqliteDatabase database;
	//private static SqliteConnection connection;

	public static void open(string url)
	{
		openSqliteDatabase (url);
		//openSqliteConnection(url);
	}

	public static void close()
	{
		closeSqliteDatabase ();
		//closeSqliteConnection ();
	}

	public static void executeNonQuery(string query)
	{
		executeNonQuerySqliteDatabase (query);
		//executeQuerySqliteConnection (query);
	}

	public static DataTable executeQuery(string query)
	{
		return executeQuerySqliteDatabase(query);
		//return executeQuerySqliteConnection (query);
	}

	#region Sqlite Database

	private static void openSqliteDatabase(string url)
	{
		database = new SqliteDatabase ();
		database.Open(url);
	}
	
	private static void closeSqliteDatabase()
	{
		if (database != null)
		{
			database.Close ();
			database = null;
		}
	}
	
	private static void executeNonQuerySqliteDatabase(string query)
	{
		database.ExecuteNonQuery(query);
	}
	
	private static DataTable executeQuerySqliteDatabase(string query)
	{
		return database.ExecuteQuery(query);
	}

	#endregion

	#region Sqlite Connection

	/*private static void openSqliteConnection(string url)
	{
		connection = new SqliteConnection("URI=file:" + url);
		connection.Open();
	}
	
	private static void closeSqliteConnection()
	{
		connection.Close();
		connection.Dispose();
	}
	
	private static void executeNonQuerySqliteConnection(string query)
	{
		SqliteCommand create = connection.CreateCommand();
		create.CommandText = query;
		create.ExecuteNonQuery();
	}
	
	private static DataTable executeQuerySqliteConnection(string query)
	{
		SqliteCommand create = connection.CreateCommand();
		create.CommandText = query;
		SqliteDataReader reader = create.ExecuteReader();

		return parseDataTable (reader);
	}

	private static DataTable parseDataTable(SqliteDataReader reader)
	{
		int count = reader.FieldCount;
		DataTable dataTable = new DataTable ();

		for (int i = 0; i < count; i++)
		{
			string columnName = reader.GetName(i);
			dataTable.Columns.Add(columnName);
		}

		if (reader.Read())
		{
			object[] objects = new object[count];
			for (int i = 0; i < count; i++)
			{
				objects[i] = reader.GetValue(i);
			}
			dataTable.AddRow(objects);
		}

		return dataTable;
	}*/

	#endregion
}