using System;

namespace Verona.Model
{
  public struct TaskData
  {
    public DateTime Date;
    public int AquariumId;
    public int TasksMask;

    public bool GetTask(TaskType parId)
    {
      return (TasksMask & (1 << (int)parId)) != 0;
    }

    public void SetTask(TaskType parId, bool parValue)
    {
      if (parValue)
      {
        TasksMask |= 1 << (int)parId;
      }
      else
      {
        TasksMask &= (~(1 << (int)parId));
      }
    }
  }
}
