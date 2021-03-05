using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Verona.Model;

namespace Verona.View
{
  public class DataGridAquariumCellRoutedEventArgs : RoutedEventArgs
  {
    private DataGridAquariumCell _dataGridAquariumCell = null;

    public DataGridAquariumCell DataGridAquariumCell
    {
      get { return _dataGridAquariumCell; }
    }

    public DataGridAquariumCellRoutedEventArgs(DataGridAquariumCell parDataGridAquariumCell)
    {
      this._dataGridAquariumCell = parDataGridAquariumCell;
    }
  }

  public delegate void DataGridAquariumCellRoutedEventHandler(object sender,
    DataGridAquariumCellRoutedEventArgs parArgs);

  public partial class DataGridAquariumCell : UserControl
  {
    public static readonly DependencyProperty DataProperty =
      DependencyProperty.Register("Data", typeof(AquariumData), typeof(DataGridAquariumCell), new
        PropertyMetadata(new AquariumData(), new PropertyChangedCallback(OnDataChanged)));

    public static readonly DependencyProperty ReadOnlyProperty =
      DependencyProperty.Register("ReadOnly", typeof(bool), typeof(DataGridAquariumCell), new
        PropertyMetadata(false, new PropertyChangedCallback(OnReadOnlyChanged)));

    public static readonly RoutedEvent CleaningClickEvent = EventManager.RegisterRoutedEvent(
      "CleaningClick", RoutingStrategy.Bubble, typeof(DataGridAquariumCellRoutedEventHandler),
      typeof(DataGridAquariumCell));

    public static readonly RoutedEvent WaterChangeClickEvent = EventManager.RegisterRoutedEvent(
      "WaterChangeClick", RoutingStrategy.Bubble, typeof(DataGridAquariumCellRoutedEventHandler),
      typeof(DataGridAquariumCell));

    public static readonly RoutedEvent AddingMedicineClickEvent = EventManager.RegisterRoutedEvent(
      "AddingMedicineClick", RoutingStrategy.Bubble, typeof(DataGridAquariumCellRoutedEventHandler),
      typeof(DataGridAquariumCell));

    public static readonly RoutedEvent BrokenFilterClickEvent = EventManager.RegisterRoutedEvent(
      "BrokenFilterClick", RoutingStrategy.Bubble, typeof(DataGridAquariumCellRoutedEventHandler),
      typeof(DataGridAquariumCell));

    public static readonly RoutedEvent WorkingClickEvent = EventManager.RegisterRoutedEvent(
      "WorkingClick", RoutingStrategy.Bubble, typeof(DataGridAquariumCellRoutedEventHandler),
      typeof(DataGridAquariumCell));

    public static readonly RoutedEvent RemoveClickEvent = EventManager.RegisterRoutedEvent(
      "RemoveClick", RoutingStrategy.Bubble, typeof(DataGridAquariumCellRoutedEventHandler),
      typeof(DataGridAquariumCell));

    private static readonly SolidColorBrush _brokenFilterColor = new SolidColorBrush(Colors.Red);
    private static readonly SolidColorBrush _noBrokenFilterColor = new SolidColorBrush(Colors.Gray);

    private AquariumData _data;

    public AquariumData Data
    {
      get { return (AquariumData) GetValue(DataProperty); }
      set { SetValue(DataProperty, value); }
    }

    private bool _readOnly;

    public bool ReadOnly
    {
      get { return (bool) GetValue(ReadOnlyProperty); }
      set { SetValue(ReadOnlyProperty, value); }
    }

    public event DataGridAquariumCellRoutedEventHandler CleaningClick
    {
      add { AddHandler(CleaningClickEvent, value); }
      remove { RemoveHandler(CleaningClickEvent, value); }
    }

    public event DataGridAquariumCellRoutedEventHandler WaterChangeClick
    {
      add { AddHandler(WaterChangeClickEvent, value); }
      remove { RemoveHandler(WaterChangeClickEvent, value); }
    }

    public event DataGridAquariumCellRoutedEventHandler AddingMedicineClick
    {
      add { AddHandler(AddingMedicineClickEvent, value); }
      remove { RemoveHandler(AddingMedicineClickEvent, value); }
    }

    public event DataGridAquariumCellRoutedEventHandler WorkingClick
    {
      add { AddHandler(WorkingClickEvent, value); }
      remove { RemoveHandler(WorkingClickEvent, value); }
    }

    public event DataGridAquariumCellRoutedEventHandler BrokenFilterClick
    {
      add { AddHandler(BrokenFilterClickEvent, value); }
      remove { RemoveHandler(BrokenFilterClickEvent, value); }
    }

    public event DataGridAquariumCellRoutedEventHandler RemoveClick
    {
      add { AddHandler(RemoveClickEvent, value); }
      remove { RemoveHandler(RemoveClickEvent, value); }
    }


    public DataGridAquariumCell()
    {
      InitializeComponent();
      _readOnly = ReadOnly;
    }

    public DataGridAquariumCell(AquariumData parData)
    {
      InitializeComponent();
      Data = parData;
      UpdateView();
    }

    private static void OnReadOnlyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      (d as DataGridAquariumCell).OnReadOnlyChanged(e);
    }

    private void OnReadOnlyChanged(DependencyPropertyChangedEventArgs e)
    {
      _readOnly = (bool) e.NewValue;
      UpdateView();
    }

    private static void OnDataChanged(DependencyObject d,
      DependencyPropertyChangedEventArgs e)
    {
      (d as DataGridAquariumCell).OnDataChanged(e);
    }

    private void OnDataChanged(DependencyPropertyChangedEventArgs e)
    {
      _data = (AquariumData) e.NewValue;
      UpdateView();
    }

    private void UpdateView()
    {
      DateTime lastCleaningDate = TaskUtils.GetLastCleaningDate(_data.Id);
      this.ToolTip = (lastCleaningDate != DateTime.MinValue) ?$"Последняя дата чистки: {TaskUtils.GetLastCleaningDate(_data.Id).ToShortDateString()}" : null;
      _name.Text = _data.Name;
      if (!_readOnly)
      {
        CleaningButton.Visibility = Visibility.Visible;
        WaterChangeButton.Visibility = Visibility.Visible;
        AddingMedicineButton.Visibility = Visibility.Visible;

        WorkingButton.Visibility = Visibility.Visible;
        BrokenFilterButton.Visibility = Visibility.Visible;

        RemoveButton.Visibility = Visibility.Visible;

        CleaningButton.IsEnabled =
          WaterChangeButton.IsEnabled =
            AddingMedicineButton.IsEnabled =
              BrokenFilterButton.IsEnabled = !_data.NoWorking;
        
        BrokenFilterButton.BorderBrush = _data.BrokenFilter && !_data.NoWorking ? _brokenFilterColor : _noBrokenFilterColor;
        WorkingButton.BorderBrush = _data.NoWorking ? _brokenFilterColor : _noBrokenFilterColor;
      }
      else
      {
        CleaningButton.Visibility = Visibility.Collapsed;
        WaterChangeButton.Visibility = Visibility.Collapsed;
        AddingMedicineButton.Visibility = Visibility.Collapsed;

        WorkingButton.Visibility = Visibility.Collapsed;
        BrokenFilterButton.Visibility = Visibility.Collapsed;

        RemoveButton.Visibility = Visibility.Collapsed;
      }
    }

    private void CleaningButton_OnClick(object parSender, RoutedEventArgs parE)
    {
      DataGridAquariumCellRoutedEventArgs newEventArgs = new DataGridAquariumCellRoutedEventArgs(this)
      {
        RoutedEvent = DataGridAquariumCell.CleaningClickEvent
      };
      RaiseEvent(newEventArgs);
    }

    private void WaterChangeButton_OnClick(object parSender, RoutedEventArgs parE)
    {
      DataGridAquariumCellRoutedEventArgs newEventArgs = new DataGridAquariumCellRoutedEventArgs(this)
      {
        RoutedEvent = DataGridAquariumCell.WaterChangeClickEvent
      };
      RaiseEvent(newEventArgs);
    }

    private void AddingMedicineButton_OnClick(object parSender, RoutedEventArgs parE)
    {
      DataGridAquariumCellRoutedEventArgs newEventArgs = new DataGridAquariumCellRoutedEventArgs(this)
      {
        RoutedEvent = DataGridAquariumCell.AddingMedicineClickEvent
      };
      RaiseEvent(newEventArgs);
    }

    private void WorkingButton_OnClick(object parSender, RoutedEventArgs parE)
    {
      _data.NoWorking = !_data.NoWorking;
      AquariumUtils.EditAquariumData(_data);

      DataGridAquariumCellRoutedEventArgs newEventArgs = new DataGridAquariumCellRoutedEventArgs(this)
      {
        RoutedEvent = DataGridAquariumCell.WorkingClickEvent
      };
      RaiseEvent(newEventArgs);
    }

    private void BrokenFilterButton_OnClick(object parSender, RoutedEventArgs parE)
    {
      _data.BrokenFilter = !_data.BrokenFilter;
      AquariumUtils.EditAquariumData(_data);

      DataGridAquariumCellRoutedEventArgs newEventArgs = new DataGridAquariumCellRoutedEventArgs(this)
      {
        RoutedEvent = DataGridAquariumCell.BrokenFilterClickEvent
      };
      RaiseEvent(newEventArgs);
    }

    private void RemoveButton_OnClick(object parSender, RoutedEventArgs parE)
    {
      if (MessageBox.Show($"Вы уверены, что хотите удалить аквариум \"{_data.Name}\"?", "Удаление аквариума из списка",
            MessageBoxButton.YesNo) == MessageBoxResult.Yes)
      {
        AquariumUtils.RemoveAquariumData(_data.Id);
      }

      DataGridAquariumCellRoutedEventArgs newEventArgs = new DataGridAquariumCellRoutedEventArgs(this)
      {
        RoutedEvent = DataGridAquariumCell.RemoveClickEvent
      };
      RaiseEvent(newEventArgs);
    }
  }
}
