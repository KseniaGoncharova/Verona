using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Verona.Model;

namespace Verona.View
{
  public partial class GeneralScheduleWindow : Window
  {
    private bool _block = false;
    private DateTime _selectedDate;

    public DateTime SelectedDate
    {
      get { return _selectedDate; }
      set { SetSelectedDate(value); }
    }

    public GeneralScheduleWindow()
    {
      InitializeComponent();
      SetSelectedDate(DateTime.Now);
      UpdateDataGridColumns();

      AquariumUtils.AquariumAdded += AquariumUtils_AquariumAdded;
      AquariumUtils.AquariumEdited += AquariumUtils_AquariumEdited;
      AquariumUtils.AquariumRemoved += AquariumUtils_AquariumRemoved;

      TaskUtils.TaskAdded += TaskUtils_TaskAdded;
      TaskUtils.TaskEdited += TaskUtils_TaskEdited;
    }

    private void GeneralScheduleWindow_OnClosed(object parSender, EventArgs parE)
    {
      AquariumUtils.AquariumAdded -= AquariumUtils_AquariumAdded;
      AquariumUtils.AquariumEdited -= AquariumUtils_AquariumEdited;
      AquariumUtils.AquariumRemoved -= AquariumUtils_AquariumRemoved;

      TaskUtils.TaskAdded -= TaskUtils_TaskAdded;
      TaskUtils.TaskEdited -= TaskUtils_TaskEdited;
    }


    public bool SetSelectedDate(DateTime parDate)
    {
      parDate = new DateTime(parDate.Year, parDate.Month, parDate.Day);

      if (parDate > DateTime.Now) { return false; }

      _block = true;
      var oldDate = _selectedDate;
      _selectedDate = parDate;


      NowDate.Text = DateTime.Now.ToLongDateString();
      CurrentMounth.Text = _selectedDate.ToLongDateString();

      if (oldDate.Year != parDate.Year || oldDate.Month != parDate.Month)
      {
        UpdateDataGridColumns();
        UpdateTaskTable();
      }

      _block = false;

      return true;
    }

    public void UpdateDataGridColumns()
    {
      TaskTable.Columns.Clear();

      var aquariumFactory = new FrameworkElementFactory(typeof(DataGridAquariumCell));
      aquariumFactory.SetValue(DataGridAquariumCell.DataProperty, new Binding("Key.Value"));
      aquariumFactory.SetValue(DataGridAquariumCell.ReadOnlyProperty, true);

      //Создание столбца с аквариумами
      TaskTable.Columns.Add(new DataGridTemplateColumn()
      {
        Header = "Аквариумы",
        CellTemplate = new DataTemplate {VisualTree = aquariumFactory}
      });

      //Создание столбцов дней
      int daysCount = DateTime.DaysInMonth(_selectedDate.Year, _selectedDate.Month);
      for (int day = 1; day <= daysCount; day++)
      {
        var taskFactory = new FrameworkElementFactory(typeof(DataGridTaskCell));
        taskFactory.SetValue(DataGridTaskCell.DataProperty, new Binding($"Value[{day}]"));

        TaskTable.Columns.Add(new DataGridTemplateColumn()
        {
          Header = day.ToString(),
          CellTemplate = new DataTemplate {VisualTree = taskFactory}
        });
      }
    }

    public void UpdateTaskTable()
    {
      Dictionary<KeyValuePair<string, AquariumData>, Dictionary<int, TaskData>> data =
        new Dictionary<KeyValuePair<string, AquariumData>, Dictionary<int, TaskData>>();

      List<GroupData> groupDatas = GroupUtils.GetListGroupData();
      for (int j = 0; j < groupDatas.Count; j++)
      {
        List<AquariumData> aquariumDatas = AquariumUtils.GetListAquariumData(groupDatas[j].Id);
        if (aquariumDatas.Count == 0)
        {
          continue;
        }

        for (int i = 0; i < aquariumDatas.Count; i++)
        {
          List<TaskData> preDataTasks = TaskUtils.GetListTaskData(aquariumDatas[i].Id,
            new DateTime(_selectedDate.Year, _selectedDate.Month, 1),
            new DateTime(_selectedDate.Year, _selectedDate.Month,
              DateTime.DaysInMonth(_selectedDate.Year, _selectedDate.Month)));

          Dictionary<int, TaskData> tasks = new Dictionary<int, TaskData>();
          foreach (var task in preDataTasks)
          {
            tasks.Add(task.Date.Day, task);
          }

          data.Add(new KeyValuePair<string, AquariumData>(groupDatas[j].Name, aquariumDatas[i]), tasks);
        }
      }


      CollectionView view = (CollectionView) CollectionViewSource.GetDefaultView(data);
      view.GroupDescriptions.Add(new PropertyGroupDescription("Key.Key"));

      TaskTable.ItemsSource = view;
    }

    private void NextMounthButton_Click(object sender, RoutedEventArgs e)
    {
      if (_block) return;

      var nextDate = _selectedDate.AddMonths(1);
      SetSelectedDate(new DateTime(nextDate.Year, nextDate.Month, 1));
    }

    private void PrevMounthButton_Click(object sender, RoutedEventArgs e)
    {
      if (_block) return;

      var nextDate = _selectedDate.AddMonths(-1);
      SetSelectedDate(new DateTime(nextDate.Year, nextDate.Month, 1));
    }

    #region Handlears DB

    private void AquariumUtils_AquariumAdded(AquariumData parData)
    {
      UpdateTaskTable();
    }

    private void AquariumUtils_AquariumEdited(AquariumData parData)
    {
      UpdateTaskTable();
    }

    private void AquariumUtils_AquariumRemoved(int parIndex)
    {
      UpdateTaskTable();
    }


    private void TaskUtils_TaskAdded(TaskData parData)
    {
      UpdateTaskTable();
    }

    private void TaskUtils_TaskEdited(TaskData parData)
    {
      UpdateTaskTable();
    }

    #endregion

    private void BackButton_OnClick(object parSender, RoutedEventArgs parE)
    {
      //открытие окна списка групп
      var groupListWindow = new GroupListWindow();
      groupListWindow.Show();
      Close();
    }
  }
}
