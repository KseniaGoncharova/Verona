using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Verona.Model;

namespace Verona.View
{
  [ValueConversion(typeof(Boolean), typeof(String))]
  public class NoWorkingConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      bool noWorking = (bool) value;
      if (noWorking)
        return "Не требуют ухода";
      else
        return "Рабочие аквариумы";
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      return (string) value == "Не требуют ухода";
    }
  }

  public partial class TaskTableWindow : Window
  {
    private bool _block = false;
    private DateTime _selectedDate;
    private GroupData _groupData;

    private bool _currentSelected = false;

    public DateTime SelectedDate
    {
      get { return _selectedDate; }
      set { SetSelectedDate(value); }
    }

    public GroupData GroupData
    {
      get { return _groupData; }
      set { SetGroupData(value); }
    }

    public TaskTableWindow()
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

    private void TaskTableWindow_OnClosed(object parSender, EventArgs parE)
    {
      AquariumUtils.AquariumAdded -= AquariumUtils_AquariumAdded;
      AquariumUtils.AquariumEdited -= AquariumUtils_AquariumEdited;
      AquariumUtils.AquariumRemoved -= AquariumUtils_AquariumRemoved;

      TaskUtils.TaskAdded -= TaskUtils_TaskAdded;
      TaskUtils.TaskEdited -= TaskUtils_TaskEdited;
    }

    public TaskTableWindow(GroupData parGroupData) : this()
    {
      SetGroupData(parGroupData);
    }
    
    public bool SetSelectedDate(DateTime parDate)
    {
      parDate = new DateTime(parDate.Year, parDate.Month, parDate.Day);

      if (parDate > DateTime.Now)
      {
        return false;
      }

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

      UpdateSelectedCells();
      
      _block = false;

      return true;
    }

    public void UpdateSelectedCells()
    {
      _currentSelected = true;
      TaskTable.SelectedCells.Clear();

      var column = TaskTable.Columns[_selectedDate.Day];
      if (column != null)
      {
        foreach (var item in TaskTable.Items)
        {
          TaskTable.SelectedCells.Add(new DataGridCellInfo(item, column));
        }
      }

      _currentSelected = false;
    }

    public void SetGroupData(GroupData parGroupData)
    {
      _groupData = parGroupData;
      GroupName.Text = _groupData.Name;

      UpdateTaskTable();
    }

    public void UpdateDataGridColumns()
    {
      TaskTable.Columns.Clear();

      var aquariumFactory = new FrameworkElementFactory(typeof(DataGridAquariumCell));
      aquariumFactory.SetValue(DataGridAquariumCell.DataProperty, new Binding("Key"));
      aquariumFactory.AddHandler(DataGridAquariumCell.CleaningClickEvent,
        new DataGridAquariumCellRoutedEventHandler(CleaningClick));
      aquariumFactory.AddHandler(DataGridAquariumCell.WaterChangeClickEvent,
        new DataGridAquariumCellRoutedEventHandler(WaterChangeClick));
      aquariumFactory.AddHandler(DataGridAquariumCell.AddingMedicineClickEvent,
        new DataGridAquariumCellRoutedEventHandler(AddingMedicineClick));
      
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
      Dictionary<AquariumData, Dictionary<int, TaskData>> data =
        new Dictionary<AquariumData, Dictionary<int, TaskData>>();

      List<AquariumData> aquariumDatas = AquariumUtils.GetListAquariumData(_groupData.Id);
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

        data.Add(aquariumDatas[i], tasks);
      }

      CollectionView view = (CollectionView) CollectionViewSource.GetDefaultView(data);
      view.GroupDescriptions.Add(new PropertyGroupDescription("Key.NoWorking", new NoWorkingConverter()));

      TaskTable.ItemsSource = view;
      UpdateSelectedCells();
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

    private void TaskTable_OnSelectedCellsChanged(object parSender, SelectedCellsChangedEventArgs parE)
    {
      if (_currentSelected)
      {
        return;
      }

      if (TaskTable.SelectedCells.Count > 0 && TaskTable.SelectedCells[0].Column != null)
      {
        int day = TaskTable.SelectedCells[0].Column.DisplayIndex;

        if (day > 0 && day != _selectedDate.Day)
        {
          if (!SetSelectedDate(new DateTime(_selectedDate.Year, _selectedDate.Month, day)))
          {
            UpdateSelectedCells();
          }
        }
        else
        {
          UpdateSelectedCells();
        }
      }
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

    #region Handlers View

    private void CleaningClick(object parSender, DataGridAquariumCellRoutedEventArgs parAquariumCell)
    {
      Task_SetTaskType(parAquariumCell.DataGridAquariumCell.Data, TaskType.Cleaning);
    }

    private void WaterChangeClick(object parSender, DataGridAquariumCellRoutedEventArgs parAquariumCell)
    {
      Task_SetTaskType(parAquariumCell.DataGridAquariumCell.Data, TaskType.WaterChange);
    }

    private void AddingMedicineClick(object parSender, DataGridAquariumCellRoutedEventArgs parAquariumCell)
    {
      Task_SetTaskType(parAquariumCell.DataGridAquariumCell.Data, TaskType.AddingMedicine);
    }

    private void Task_SetTaskType(AquariumData parData, TaskType parType)
    {
      CollectionView view = (CollectionView) TaskTable.ItemsSource;

      var table = (Dictionary<AquariumData, Dictionary<int, TaskData>>) view.SourceCollection; // TaskTable.ItemsSource;
      if (table.TryGetValue(parData, out var tableTasks))
      {
        if (tableTasks.TryGetValue(_selectedDate.Day, out TaskData data))
        {
          data.SetTask(parType, !data.GetTask(parType));
          TaskUtils.EditTaskData(data);
        }
        else
        {
          data = new TaskData
          {
            AquariumId = parData.Id,
            Date = _selectedDate
          };
          data.SetTask(parType, true);
          TaskUtils.AddTaskData(ref data);
        }
      }
    }

    #endregion

    private void AddAquariumButton_OnClick(object parSender, RoutedEventArgs parE)
    {
      //открытие окна добавления аквариума
      AddAquariumWindow window = new AddAquariumWindow(_groupData.Id);
      window.Owner = this;
      window.ShowDialog();
    }

    private void BackButton_OnClick(object parSender, RoutedEventArgs parE)
    {
      //открытие окна списка групп
      var groupListWindow = new GroupListWindow();
      groupListWindow.Show();
      Close();
    }
  }
}
