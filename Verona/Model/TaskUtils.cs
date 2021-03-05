using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace Verona.Model
{
  public static class TaskUtils
  {
    public static event Action<TaskData> TaskAdded;
    public static event Action<TaskData> TaskEdited;

    public static bool AddTaskData(ref TaskData parData)
    {
      bool res = false;

      string commandText = "INSERT INTO Tasks (Date, AquariumId, TasksMask) values(@date, @aquariumId, @tasksMask)";

      SQLiteCommand command = new SQLiteCommand(commandText, Database.Connection);
      command.Parameters.AddWithValue("@date", parData.Date);
      command.Parameters.AddWithValue("@aquariumId", parData.AquariumId);
      command.Parameters.AddWithValue("@tasksMask", parData.TasksMask);

      try
      {
        res = command.ExecuteNonQuery() == 1;
      }
      catch
      {
      }


      if (res)
      {
        TaskAdded?.Invoke(parData);
        return true;
      }

      return false;
    }

    public static bool EditTaskData(TaskData parData)
    {
      bool res = false;

      string commandText = "UPDATE Tasks SET TasksMask=@tasksMask WHERE Date=@date and AquariumId=@aquariumId";

      SQLiteCommand command = new SQLiteCommand(commandText, Database.Connection);
      command.Parameters.AddWithValue("@date", parData.Date);
      command.Parameters.AddWithValue("@aquariumId", parData.AquariumId);
      command.Parameters.AddWithValue("@tasksMask", parData.TasksMask);

      try
      {
        res = command.ExecuteNonQuery() == 1;
      }
      catch
      {
      }


      if (res)
      {
        TaskEdited?.Invoke(parData);
        return true;
      }

      return false;
    }

    public static DateTime GetLastCleaningDate(int parAquariumId)
    {
      DateTime retDate = DateTime.MinValue;

      string commandText =
        $"SELECT max(Date) FROM Tasks WHERE AquariumId=@aquariumId and (TasksMask & @taskMask)<>0 and Date<=@now";

      SQLiteCommand command = new SQLiteCommand(commandText, Database.Connection);
      command.Parameters.AddWithValue("@aquariumId", parAquariumId);
      command.Parameters.AddWithValue("@taskMask", 1 << (int) TaskType.Cleaning);
      command.Parameters.AddWithValue("@now", DateTime.Now);

      var preDate = command.ExecuteScalar();

      if (preDate.GetType() != typeof(DBNull))
      {
        retDate = Convert.ToDateTime(preDate);
      }



      return retDate;
    }

    public static List<TaskData> GetListTaskData()
    {
      List<TaskData> datas = new List<TaskData>();

      string commandText = $"SELECT * FROM Tasks";

      SQLiteCommand command = new SQLiteCommand(commandText, Database.Connection);

      SQLiteDataReader dataReader = command.ExecuteReader();
      if (dataReader.HasRows)
      {
        while (dataReader.Read())
        {
          datas.Add(new TaskData()
          {
            Date = Convert.ToDateTime(dataReader["Date"]),
            AquariumId = Convert.ToInt32(dataReader["AquariumId"]),
            TasksMask = Convert.ToInt32(dataReader["TasksMask"]),
          });
        }
      }

      return datas;
    }

    public static List<TaskData> GetListTaskData(int parAquariumId, DateTime parStartDate, DateTime parEndDate)
    {
      List<TaskData> datas = new List<TaskData>();

      string commandText =
        $"SELECT * FROM Tasks WHERE AquariumId=@aquariumId and Date >= @startDate and Date <= @endDate";

      SQLiteCommand command = new SQLiteCommand(commandText, Database.Connection);
      command.Parameters.AddWithValue("@aquariumId", parAquariumId);
      command.Parameters.AddWithValue("@startDate", parStartDate);
      command.Parameters.AddWithValue("@endDate", parEndDate);

      SQLiteDataReader dataReader = command.ExecuteReader();
      if (dataReader.HasRows)
      {
        while (dataReader.Read())
        {
          datas.Add(new TaskData()
          {
            Date = Convert.ToDateTime(dataReader["Date"]),
            AquariumId = Convert.ToInt32(dataReader["AquariumId"]),
            TasksMask = Convert.ToInt32(dataReader["TasksMask"]),
          });
        }
      }

      return datas;
    }
  }
}
