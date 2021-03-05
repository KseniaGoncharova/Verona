using System.Windows;
using System.Windows.Controls;
using Verona.Model;

namespace Verona.View
{
  public partial class AquariumButton : UserControl
  {
    private AquariumData _data;

    public AquariumData Data
    {
      get { return _data; }
      set { SetData(value); }
    }

    public RoutedEventHandler RemoveClick;

    public AquariumButton()
    {
      InitializeComponent();
    }

    public void SetData(AquariumData parData)
    {
      _data = parData;
      AquariumName.Text = _data.Name;
    }

    private void RemoveButton_Click(object sender, System.Windows.RoutedEventArgs e)
    {
      RemoveClick.Invoke(this, e);
    }
  }
}
