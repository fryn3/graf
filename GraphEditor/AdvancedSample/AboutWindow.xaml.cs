using System.Windows;

namespace SampleCode
{
	/// <summary>
	/// Interaction logic for AboutWindow.xaml
	/// </summary>
	public partial class AboutWindow : Window
	{
		public AboutWindow()
		{
			InitializeComponent();
		}
		
		private void CloseButton_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}
	}
}
