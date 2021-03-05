using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Verona.Model;

namespace Verona.View
{
  public class AquriumListBrokenFilterWindow : AquriumListWindow
  {
    public override void UpdateAquariums()
    {
      AquariumsStackPanel.Children.Clear();
      List<AquariumData> preDatas = AquariumUtils.GetListAquariumDataBrokenFilter();

      Dictionary<int, List<AquariumData>> datas = new Dictionary<int, List<AquariumData>>();
      for (int i = 0; i < preDatas.Count; i++)
      {
        if (datas.TryGetValue(preDatas[i].GroupId, out List<AquariumData> list))
        {
          list.Add(preDatas[i]);
        }
        else
        {
          datas[preDatas[i].GroupId] = new List<AquariumData>() { preDatas[i] };
        }
      }

      foreach (var data in datas)
      {
        TextBlock groupName = new TextBlock()
        {
          Margin = new Thickness(20, 5, 20, 5)
        };
        if (GroupUtils.GetGroupData(data.Key, out GroupData groupData))
        {
          groupName.Text = groupData.Name;
        }
        else
        {
          groupName.Text = "Группа не найдена";
        }
        AquariumsStackPanel.Children.Add(groupName);

        for (int i = 0; i < data.Value.Count; i++)
        {
          AquariumButton aquariumButton = new AquariumButton()
          {
            Data = data.Value[i],
            Margin = new Thickness(20, 10, 20, 10)
          };
          
          aquariumButton.RemoveClick += RemoveClick;

          AquariumsStackPanel.Children.Add(aquariumButton);
        }
      }
    }

    private void RemoveClick(object parSender, RoutedEventArgs parE)
    {
      AquariumData data = ((AquariumButton)parSender).Data;
      
      var res = MessageBox.Show($"Вы уверены, что хотите удалить аквариум \"{data.Name}\" из списка?", "Удаление аквариума из списка",
        MessageBoxButton.YesNo);
      switch (res)
      {
        case MessageBoxResult.Yes:
          data.BrokenFilter = false;
          AquariumUtils.EditAquariumData(data);
          break;
      }
    }
  }

  public class AquriumListNoWorkingWindow : AquriumListWindow
  {
    public override void UpdateAquariums()
    {
      AquariumsStackPanel.Children.Clear();
      List<AquariumData> preDatas = AquariumUtils.GetListAquariumDataNoWorking();

      Dictionary<int, List<AquariumData>> datas = new Dictionary<int, List<AquariumData>>();
      for (int i = 0; i < preDatas.Count; i++)
      {
        if (datas.TryGetValue(preDatas[i].GroupId, out List<AquariumData> list))
        {
          list.Add(preDatas[i]);
        }
        else
        {
          datas[preDatas[i].GroupId] = new List<AquariumData>(){ preDatas[i] };
        }
      }

      foreach (var data in datas)
      {
        TextBlock groupName = new TextBlock()
        {
          Margin = new Thickness(20, 5, 20, 5)
        };
        if (GroupUtils.GetGroupData(data.Key, out GroupData groupData))
        {
          groupName.Text = groupData.Name;
        }
        else
        {
          groupName.Text = "Группа не найдена";
        }
        AquariumsStackPanel.Children.Add(groupName);

        for (int i = 0; i < data.Value.Count; i++)
        {
          AquariumButton aquariumButton = new AquariumButton()
          {
            Data = data.Value[i],
            Margin = new Thickness(20, 10, 20, 10)
          };
          aquariumButton.RemoveClick += RemoveClick;

          AquariumsStackPanel.Children.Add(aquariumButton);
        }
      }
    }

    private void RemoveClick(object parSender, RoutedEventArgs parE)
    {
      AquariumData data = ((AquariumButton)parSender).Data;

      var res = MessageBox.Show($"Вы уверены, что хотите удалить аквариум \"{data.Name}\" из списка?", "Удаление аквариума",
        MessageBoxButton.YesNo);
      switch (res)
      {
        case MessageBoxResult.Yes:
          data.NoWorking = false;
          AquariumUtils.EditAquariumData(data);
          break;
      }
    }
  }

  public partial class AquriumListWindow : Window
  {
    public AquriumListWindow()
    {
      InitializeComponent();

      GroupUtils.GroupAdded += GroupUtils_GroupAdded;
      GroupUtils.GroupEdited += GroupUtils_GroupEdited;
      GroupUtils.GroupRemoved += GroupUtils_GroupRemoved;

      AquariumUtils.AquariumRemoved += AquariumUtils_AquariumRemoved;
      AquariumUtils.AquariumAdded += AquariumUtils_AquariumAdded;
      AquariumUtils.AquariumEdited += AquariumUtilsOnAquariumEdited;

      UpdateAquariums();
    }

    private void AquriumListWindow_OnClosed(object parSender, EventArgs parE)
    {
      GroupUtils.GroupAdded -= GroupUtils_GroupAdded;
      GroupUtils.GroupEdited -= GroupUtils_GroupEdited;
      GroupUtils.GroupRemoved -= GroupUtils_GroupRemoved;

      AquariumUtils.AquariumRemoved -= AquariumUtils_AquariumRemoved;
      AquariumUtils.AquariumAdded -= AquariumUtils_AquariumAdded;
      AquariumUtils.AquariumEdited -= AquariumUtilsOnAquariumEdited;
    }

    public virtual void UpdateAquariums()
    {
    }

    private void BackButton_OnClick(object parSender, RoutedEventArgs parE)
    {
      //открытие окна со списком групп
      var groupListWindow = new GroupListWindow();
      groupListWindow.Show();
      Close();
    }

    private void AquariumUtilsOnAquariumEdited(AquariumData parData)
    {
      UpdateAquariums();
    }

    private void AquariumUtils_AquariumAdded(AquariumData parData)
    {
      UpdateAquariums();
    }

    private void AquariumUtils_AquariumRemoved(int parId)
    {
      UpdateAquariums();
    }


    private void GroupUtils_GroupAdded(GroupData parData)
    {
      UpdateAquariums();
    }

    private void GroupUtils_GroupEdited(GroupData parData)
    {
      UpdateAquariums();
    }

    private void GroupUtils_GroupRemoved(int parId)
    {
      UpdateAquariums();
    }
  }
}
