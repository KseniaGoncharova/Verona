using System.Windows;
using Verona.Model;

namespace Verona.View
{
  public partial class AddAquariumWindow : Window
  {
    private int _id = -1;
    private int _groupId = -1;

    public AquariumData Data
    {
      get { return new AquariumData() {Id = _id, Name = _nameGroup.Text, GroupId = _groupId, NoWorking = NoWorkingCheckBox.IsChecked == true}; }
    }

    public bool Adding { get; } = true;

    public AddAquariumWindow(int parId, int parGroupId)
    {
      InitializeComponent();
      _id = parId;
      _groupId = parGroupId;
      Adding = false;
      Title = "Редактирование аквариума";
      AddButton.Content = "Изменить";
    }

    public AddAquariumWindow(int parGroupId)
    {
      InitializeComponent();
      _groupId = parGroupId;
      Adding = true;
      Title = "Добавление аквариума";
      AddButton.Content = "Добавить";
    }

    private void AddButton_Click(object sender, RoutedEventArgs e)
    {
      if (Adding)
      {
        var data = Data;
        if (AquariumUtils.AddAquariumData(ref data))
        {
          DialogResult = true;
          Close();
        }
      }
      else
      {
        if (AquariumUtils.EditAquariumData(Data))
        {
          DialogResult = true;
          Close();
        }
      }
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
      DialogResult = false;
      Close();
    }
  }
}
