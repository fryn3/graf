using System.Collections.Generic;
using System.Drawing;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace SampleCode
{
	public partial class TaskSelectionDialog : Window
	{
		public int TaskId { get; set; } = 1;

		public TaskSelectionDialog(int tasksCount)
		{
			var tasksList = new List<int>();
			for (var i = 1; i <= tasksCount; i++)
			{
				tasksList.Add(i);
			}
			
			var questionMarkIcon = Imaging.CreateBitmapSourceFromHIcon(
				SystemIcons.Question.Handle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());

			InitializeComponent();

			TaskCombobox.ItemsSource = tasksList;
			QuestionMarkImage.Source = questionMarkIcon;
		}

		private void btnDialogOk_Click(object sender, RoutedEventArgs e)
		{
			this.DialogResult = true;
		}
	}
}
