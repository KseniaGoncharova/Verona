using System;
using System.Collections.Generic;
using System.Windows;
using Verona.Model;

namespace Verona.View
{
  public partial class GroupListWindow : Window
  {
    public GroupListWindow()
    {
      InitializeComponent();

      GroupUtils.GroupAdded += GroupUtils_GroupAdded;
      GroupUtils.GroupEdited += GroupUtils_GroupEdited;
      GroupUtils.GroupRemoved += GroupUtils_GroupRemoved;

      AquariumUtils.AquariumRemoved += AquariumUtils_AquariumRemoved;
      AquariumUtils.AquariumAdded += AquariumUtils_AquariumAdded;
      AquariumUtils.AquariumEdited += AquariumUtilsOnAquariumEdited;

      UpdateGroups();
      UpdateButtons();
    }

    private void GroupListWindow_OnClosed(object parSender, EventArgs parE)
    {
      GroupUtils.GroupAdded -= GroupUtils_GroupAdded;
      GroupUtils.GroupEdited -= GroupUtils_GroupEdited;
      GroupUtils.GroupRemoved -= GroupUtils_GroupRemoved;

      AquariumUtils.AquariumRemoved -= AquariumUtils_AquariumRemoved;
      AquariumUtils.AquariumAdded -= AquariumUtils_AquariumAdded;
      AquariumUtils.AquariumEdited -= AquariumUtilsOnAquariumEdited;
    }

    private void UpdateButtons()
    {
      NoWorkingButton.Content = $"Не требуют ухода ({AquariumUtils.GetCountAquariumDataNoWorking()})";
      BrokenFilterButton.Content = $"Поломка фильтра ({AquariumUtils.GetCountAquariumDataBrokenFilter()})";
    }

    public void UpdateGroups()
    {
      GroupsStackPanel.Children.Clear();
      List<GroupData> datas = GroupUtils.GetListGroupData();
      for (int i = 0; i < datas.Count; i++)
      {
        GroupButton groupButton = new GroupButton()
        {
          Data = datas[i],
          Margin = new Thickness(20, 10, 20, 10)
        };

        groupButton.Click += GroupButton_OnClick;
        groupButton.RemoveClick += GroupButton_OnRemoveClick;

        GroupsStackPanel.Children.Add(groupButton);
      }
    }

    private void GroupButton_OnClick(object parSender, RoutedEventArgs parE)
    {
      //открытие окна группы
      GroupData data = ((GroupButton) parSender).Data;
      var taskTableWindow = new TaskTableWindow(data);
      taskTableWindow.Show();
      Close();
    }

    private void GroupButton_OnRemoveClick(object parSender, RoutedEventArgs parE)
    {
      //открытие окна группы
      GroupData data = ((GroupButton) parSender).Data;

      var res = MessageBox.Show($"Вы уверены, что хотите удалить группу \"{data.Name}\" и все аквариумы, находящиеся в ней?", "Удаление группы",
        MessageBoxButton.YesNo);
      switch (res)
      {
        case MessageBoxResult.Yes:
          GroupUtils.RemoveGroupData(data.Id);
          break;
      }
    }

    private void AddGroupButton_OnClick(object parSender, RoutedEventArgs parE)
    {
      //открытие окна добавления группы
      AddGroupWindow window = new AddGroupWindow();
      window.Owner = this;
      window.ShowDialog();
    }

    private void GeneralScheduleButton_OnClick(object parSender, RoutedEventArgs parE)
    {
      //открытие окна с общим графиком
      var generalScheduleWindow = new GeneralScheduleWindow();
      generalScheduleWindow.Show();
      Close();
    }

    private void BrokenFilterButton_OnClick(object parSender, RoutedEventArgs parE)
    {
      //открытие окна со сломанными фильтрами
      var aquriumListBrokenFilterWindow = new AquriumListBrokenFilterWindow();
      aquriumListBrokenFilterWindow.Show();
      Close();
    }

    private void NoWorkingButton_OnClick(object parSender, RoutedEventArgs parE)
    {
      //открытие окна с неиспользуемыми аквариумами
      var aquriumListNoWorkingWindow = new AquriumListNoWorkingWindow();
      aquriumListNoWorkingWindow.Show();
      Close();
    }

    private void AquariumUtilsOnAquariumEdited(AquariumData parData)
    {
      UpdateButtons();
    }

    private void AquariumUtils_AquariumAdded(AquariumData parData)
    {
      UpdateButtons();
    }

    private void AquariumUtils_AquariumRemoved(int parId)
    {
      UpdateButtons();
    }


    private void GroupUtils_GroupAdded(GroupData parData)
    {
      UpdateGroups();
      UpdateButtons();
    }

    private void GroupUtils_GroupEdited(GroupData parData)
    {
      UpdateGroups();
      UpdateButtons();
    }

    private void GroupUtils_GroupRemoved(int parId)
    {
      UpdateGroups();
      UpdateButtons();
    }
  }
}
