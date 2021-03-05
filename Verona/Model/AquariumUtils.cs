using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace Verona.Model
{
  public static class AquariumUtils
  {
    public static event Action<AquariumData> AquariumAdded;
    public static event Action<AquariumData> AquariumEdited;
    public static event Action<int> AquariumRemoved;

    public static bool AddAquariumData(ref AquariumData parData)
    {
      bool res = false;

      string commandText =
        "INSERT INTO Aquariums (Name, GroupId, NoWorking, BrokenFilter) values(@name, @groupId, @noWorking, @brokenFilter)";

      SQLiteCommand command = new SQLiteCommand(commandText, Database.Connection);
      command.Parameters.AddWithValue("@name", parData.Name);
      command.Parameters.AddWithValue("@groupId", parData.GroupId);
      command.Parameters.AddWithValue("@noWorking", parData.NoWorking);
      command.Parameters.AddWithValue("@brokenFilter", parData.BrokenFilter);

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
        AquariumAdded?.Invoke(parData);
        return true;
      }

      return false;
    }

    public static bool EditAquariumData(AquariumData parData)
    {
      bool res = false;

      string commandText =
        "UPDATE Aquariums SET Name=@name, GroupId=@groupId, NoWorking=@noWorking, BrokenFilter=@brokenFilter WHERE Id=@id";

      SQLiteCommand command = new SQLiteCommand(commandText, Database.Connection);
      command.Parameters.AddWithValue("@id", parData.Id);
      command.Parameters.AddWithValue("@name", parData.Name);
      command.Parameters.AddWithValue("@groupId", parData.GroupId);
      command.Parameters.AddWithValue("@noWorking", parData.NoWorking);
      command.Parameters.AddWithValue("@brokenFilter", parData.BrokenFilter);

      try
      {
        res = command.ExecuteNonQuery() == 1;
      }
      catch
      {
      }

      if (res)
      {
        AquariumEdited?.Invoke(parData);
        return true;
      }

      return false;
    }

    public static bool RemoveAquariumData(int parId)
    {
      bool res = false;

      string commandText = $"DELETE FROM Aquariums WHERE Id=@id";

      SQLiteCommand command = new SQLiteCommand(Database.Connection);

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
        AquariumRemoved?.Invoke(parId);
        return true;
      }

      return false;
    }

    public static List<AquariumData> GetListAquariumData()
    {
      List<AquariumData> datas = new List<AquariumData>();

      string commandText = $"SELECT * FROM Aquariums ORDER BY NoWorking";

      SQLiteCommand command = new SQLiteCommand(commandText, Database.Connection);

      SQLiteDataReader dataReader = command.ExecuteReader();
      if (dataReader.HasRows)
      {
        while (dataReader.Read())
        {
          datas.Add(new AquariumData()
          {
            Id = Convert.ToInt32(dataReader["Id"]),
            Name = Convert.ToString(dataReader["Name"]),
            GroupId = Convert.ToInt32(dataReader["GroupId"]),
            NoWorking = Convert.ToBoolean(dataReader["NoWorking"]),
            BrokenFilter = Convert.ToBoolean(dataReader["BrokenFilter"]),
          });
        }
      }

      return datas;
    }

    public static int GetCountAquariumDataBrokenFilter()
    {
      int retCount = 0;
      string commandText = $"SELECT COUNT(*) FROM Aquariums WHERE BrokenFilter=@brokenFilter and NoWorking=@noWorking";

      SQLiteCommand command = new SQLiteCommand(commandText, Database.Connection);
      command.Parameters.AddWithValue("@brokenFilter", true);
      command.Parameters.AddWithValue("@noWorking", false);

      retCount = Convert.ToInt32(command.ExecuteScalar());


      return retCount;
    }

    public static List<AquariumData> GetListAquariumDataBrokenFilter()
    {
      List<AquariumData> datas = new List<AquariumData>();

      string commandText =
        $"SELECT * FROM Aquariums WHERE BrokenFilter=@brokenFilter and NoWorking=@noWorking ORDER BY GroupId";

      SQLiteCommand command = new SQLiteCommand(commandText, Database.Connection);
      command.Parameters.AddWithValue("@brokenFilter", true);
      command.Parameters.AddWithValue("@noWorking", false);

      SQLiteDataReader dataReader = command.ExecuteReader();
      if (dataReader.HasRows)
      {
        while (dataReader.Read())
        {
          datas.Add(new AquariumData()
          {
            Id = Convert.ToInt32(dataReader["Id"]),
            Name = Convert.ToString(dataReader["Name"]),
            GroupId = Convert.ToInt32(dataReader["GroupId"]),
            NoWorking = Convert.ToBoolean(dataReader["NoWorking"]),
            BrokenFilter = Convert.ToBoolean(dataReader["BrokenFilter"]),
          });
        }
      }

      return datas;
    }

    public static int GetCountAquariumDataNoWorking()
    {
      int retCount = 0;
      string commandText = $"SELECT COUNT(*) FROM Aquariums WHERE NoWorking=@noWorking";

      SQLiteCommand command = new SQLiteCommand(commandText, Database.Connection);
      command.Parameters.AddWithValue("@noWorking", true);

      retCount = Convert.ToInt32(command.ExecuteScalar());


      return retCount;
    }

    public static List<AquariumData> GetListAquariumDataNoWorking()
    {
      List<AquariumData> datas = new List<AquariumData>();

      string commandText = $"SELECT * FROM Aquariums WHERE NoWorking=@noWorking ORDER BY GroupId";

      SQLiteCommand command = new SQLiteCommand(commandText, Database.Connection);
      command.Parameters.AddWithValue("@noWorking", true);

      SQLiteDataReader dataReader = command.ExecuteReader();
      if (dataReader.HasRows)
      {
        while (dataReader.Read())
        {
          datas.Add(new AquariumData()
          {
            Id = Convert.ToInt32(dataReader["Id"]),
            Name = Convert.ToString(dataReader["Name"]),
            GroupId = Convert.ToInt32(dataReader["GroupId"]),
            NoWorking = Convert.ToBoolean(dataReader["NoWorking"]),
            BrokenFilter = Convert.ToBoolean(dataReader["BrokenFilter"]),
          });
        }
      }


      return datas;
    }

    public static List<AquariumData> GetListAquariumData(int parGroupId)
    {
      List<AquariumData> datas = new List<AquariumData>();

      string commandText = $"SELECT * FROM Aquariums WHERE GroupId=@groupId ORDER BY NoWorking";

      SQLiteCommand command = new SQLiteCommand(commandText, Database.Connection);
      command.Parameters.AddWithValue("@groupId", parGroupId);

      SQLiteDataReader dataReader = command.ExecuteReader();
      if (dataReader.HasRows)
      {
        while (dataReader.Read())
        {
          datas.Add(new AquariumData()
          {
            Id = Convert.ToInt32(dataReader["Id"]),
            Name = Convert.ToString(dataReader["Name"]),
            GroupId = Convert.ToInt32(dataReader["GroupId"]),
            NoWorking = Convert.ToBoolean(dataReader["NoWorking"]),
            BrokenFilter = Convert.ToBoolean(dataReader["BrokenFilter"]),
          });
        }
      }


      return datas;
    }

    public static bool GetAquariumData(int parId, out AquariumData outData)
    {
      outData = new AquariumData();
      bool res = false;

      string commandText = $"SELECT * FROM Aquariums WHERE Id=@id";

      SQLiteCommand command = new SQLiteCommand(commandText, Database.Connection);
      command.Parameters.AddWithValue("@id", parId);

      SQLiteDataReader dataReader = command.ExecuteReader();
      if (dataReader.HasRows)
      {
        if (dataReader.Read())
        {
          outData = new AquariumData()
          {
            Id = Convert.ToInt32(dataReader["Id"]),
            Name = Convert.ToString(dataReader["Name"]),
            GroupId = Convert.ToInt32(dataReader["GroupId"]),
            NoWorking = Convert.ToBoolean(dataReader["NoWorking"]),
            BrokenFilter = Convert.ToBoolean(dataReader["BrokenFilter"]),
          };
          res = true;
        }
      }


      return res;
    }
  }
}
