using System.Windows;
using Verona.Model;

namespace Verona.View
{
  public partial class AddGroupWindow : Window
  {
    private int _id = -1;

    public GroupData Data
    {
      get { return new GroupData() {Id = _id, Name = _nameGroup.Text}; }
    }

    public bool Adding { get; } = true;

    public AddGroupWindow(int parId)
    {
      InitializeComponent();
      _id = parId;
      Adding = false;
      Title = "Редактирование группы";
      AddButton.Content = "Изменить";
    }

    public AddGroupWindow()
    {
      InitializeComponent();
      Adding = true;
      Title = "Добавление группы";
      AddButton.Content = "Добавить";
    }

    private void AddButton_Click(object sender, RoutedEventArgs e)
    {
      if (Adding)
      {
        var data = Data;
        if (GroupUtils.AddGroupData(ref data))
        {
          DialogResult = true;
          Close();
        }
      }
      else
      {
        if (GroupUtils.EditGroupData(Data))
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
