using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace Verona.Model
{
  public static class GroupUtils
  {
    public static event Action<GroupData> GroupAdded;
    public static event Action<GroupData> GroupEdited;
    public static event Action<int> GroupRemoved;

    public static bool AddGroupData(ref GroupData parData)
    {
      bool res = false;

      string commandText = "INSERT INTO Groups (Name) values(@name)";

      SQLiteCommand command = new SQLiteCommand(commandText, Database.Connection);
      command.Parameters.AddWithValue("@name", parData.Name);

      try
      {
        res = command.ExecuteNonQuery() == 1;
        //TODO: получить id
        parData.Id = (int) Database.Connection.LastInsertRowId;
      }
      catch
      {
      }

      if (res)
      {
        GroupAdded?.Invoke(parData);
        return true;
      }

      return false;
    }

    public static bool EditGroupData(GroupData parData)
    {
      bool res = false;

      string commandText = "UPDATE Groups SET Name=@name WHERE Id=@id";

      SQLiteCommand command = new SQLiteCommand(commandText, Database.Connection);
      command.Parameters.AddWithValue("@id", parData.Id);
      command.Parameters.AddWithValue("@name", parData.Name);

      try
      {
        res = command.ExecuteNonQuery() == 1;
      }
      catch
      {
      }


      if (res)
      {
        GroupEdited?.Invoke(parData);
        return true;
      }

      return false;
    }


    public static bool RemoveGroupData(int parId)
    {
      bool res = false;

      string commandText = $"DELETE FROM Groups WHERE Id=@id";

      SQLiteCommand command = new SQLiteCommand(commandText, Database.Connection);

      command.CommandText = "PRAGMA foreign_keys=ON";
      command.ExecuteNonQuery();

      command.CommandText = commandText;
      command.Parameters.AddWithValue("@id", parId);
      try
      {
        res = command.ExecuteNonQuery() == 1;
      }
      catch
      {
      }


      if (res)
      {
        GroupRemoved?.Invoke(parId);
        return true;
      }

      return false;
    }

    public static List<GroupData> GetListGroupData()
    {
      List<GroupData> datas = new List<GroupData>();

      string commandText = $"SELECT * FROM Groups";

      SQLiteCommand command = new SQLiteCommand(commandText, Database.Connection);

      SQLiteDataReader dataReader = command.ExecuteReader();
      if (dataReader.HasRows)
      {
        while (dataReader.Read())
        {
          datas.Add(new GroupData()
          {
            Id = Convert.ToInt32(dataReader["Id"]),
            Name = Convert.ToString(dataReader["Name"])
          });
        }
      }


      return datas;
    }

    public static bool GetGroupData(int parId, out GroupData outData)
    {
      outData = new GroupData();
      bool res = false;

      string commandText = $"SELECT * FROM Groups WHERE Id=@id";

      SQLiteCommand command = new SQLiteCommand(commandText, Database.Connection);
      command.Parameters.AddWithValue("@id", parId);

      SQLiteDataReader dataReader = command.ExecuteReader();
      if (dataReader.HasRows)
      {
        if (dataReader.Read())
        {
          outData = new GroupData()
          {
            Id = Convert.ToInt32(dataReader["Id"]),
            Name = Convert.ToString(dataReader["Name"])
          };
          res = true;
        }
      }


      return res;
    }
  }
}
