using System.Data.SQLite;

namespace Verona.Model
{
  public static class Database
  {
    public static SQLiteConnection Connection = new SQLiteConnection($"Data Source=VeronaDB.db;Version=3");
    
    static Database()
    {
      Connection.Open();
    }
  }
}
