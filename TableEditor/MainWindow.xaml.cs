using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using FsmUtils;

namespace TableEditor
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindowViewModel ViewModel
		{
			get
			{
				return (MainWindowViewModel)DataContext;
			}
		}

		public MainWindow()
		{

			var taskList = new List<int>();
			for (var i = 1; i <= Constants.TasksCount; i++)
			{
				taskList.Add(i);
			}

			var innerList = new List<int>();
			for (var i = Constants.InnerStatesMin; i <= Constants.InnerStatesMax; i++)
			{
				innerList.Add(i);
			}

			var inputList = new List<int>();
			for (var i = Constants.InputsMin; i <= Constants.InputsMax; i++)
			{
				inputList.Add(i);
			}

			var outputList = new List<int>();
			for (var i = Constants.OutputsMin; i <= Constants.OutputsMax; i++)
			{
				outputList.Add(i);
			}

			InitializeComponent();

			TaskComboBox.ItemsSource = taskList;
			InnerComboBox.ItemsSource = innerList;
			InputComboBox.ItemsSource = inputList;
			OutputComboBox.ItemsSource = outputList;
		}

		private void Save_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			ViewModel.Save();
		}

		private void Reset_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			ViewModel.Reset();
		}

		private void Exit_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			Application.Current.Shutdown();
		}

		private void TaskComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (e.AddedItems.Count == 0) return;

			var newVal = (e.AddedItems?[0] as ComboBoxItem)?.Content as int?;
			if (newVal == 0 || newVal == TaskComboBox.SelectedValue as int?) return;
			ViewModel.OnTaskIdChanged();
		}

		private void TypeComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			ViewModel.OnTypeChanged();
		}

		private void InnerComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			ViewModel.OnStatesCountChanged();
		}

		private void InputComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			ViewModel.OnInputsCountChanged();
		}

		private void OutputComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			ViewModel.OnOutputsCountChanged();
		}

		private void ValidateComboBoxInput<T>(object sender)
		{
			var cmb = sender as ComboBox;
			if (!(cmb?.SelectedItem is T)) return;

			var value = (T)cmb.SelectedValue;
			var source = cmb.ItemsSource.Cast<T>();
			if (!source.Contains(value))
			{
				cmb.SelectedIndex = -1;
			}
		}

		private void ComboBox_OnTargetUpdated(object sender, DataTransferEventArgs e)
		{
			ValidateComboBoxInput<int>(sender);
		}
	}
}
