using System.Windows;
using System.Windows.Controls;
using Verona.Model;

namespace Verona.View
{
  public partial class GroupButton : UserControl
  {
    private GroupData _data;

    public GroupData Data
    {
      get { return _data; }
      set { SetData(value); }
    }

    public RoutedEventHandler Click;
    public RoutedEventHandler RemoveClick;

    public GroupButton()
    {
      InitializeComponent();
    }

    public void SetData(GroupData parData)
    {
      _data = parData;
      EnterButton.Content = _data.Name;
    }

    private void EnterButton_Click(object sender, System.Windows.RoutedEventArgs e)
    {
      Click.Invoke(this, e);
    }

    private void RemoveButton_Click(object sender, System.Windows.RoutedEventArgs e)
    {
      RemoveClick.Invoke(this, e);
    }
  }
}
