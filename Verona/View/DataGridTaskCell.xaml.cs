using System.Windows;
using System.Windows.Controls;
using Verona.Model;

namespace Verona.View
{
  /// <summary>
  /// Логика взаимодействия для DataGridTaskCell.xaml
  /// </summary>
  public partial class DataGridTaskCell : UserControl
  {
    public static readonly DependencyProperty DataProperty =
      DependencyProperty.Register("Data", typeof(TaskData), typeof(DataGridTaskCell), new
        PropertyMetadata(new TaskData(), new PropertyChangedCallback(OnDataChanged)));

    private TaskData _data;

    public TaskData Data
    {
      get { return (TaskData) GetValue(DataProperty); }
      set { SetValue(DataProperty, value); }
    }

    public DataGridTaskCell()
    {
      InitializeComponent();
      UpdateView();
    }

    public DataGridTaskCell(TaskData parData)
    {
      InitializeComponent();
      Data = parData;
    }
    
    private static void OnDataChanged(DependencyObject d,
      DependencyPropertyChangedEventArgs e)
    {
      (d as DataGridTaskCell).OnDataChanged(e);
    }

    private void OnDataChanged(DependencyPropertyChangedEventArgs e)
    {
      _data = (TaskData) e.NewValue;
      UpdateView();
    }

    private void UpdateView()
    {
      Cleaning.Visibility = _data.GetTask(TaskType.Cleaning) ? Visibility.Visible : Visibility.Collapsed;
      WaterChange.Visibility = _data.GetTask(TaskType.WaterChange) ? Visibility.Visible : Visibility.Collapsed;
      AddingMedicine.Visibility = _data.GetTask(TaskType.AddingMedicine) ? Visibility.Visible : Visibility.Collapsed;
    }
  }
}
