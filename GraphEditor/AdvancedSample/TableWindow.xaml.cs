using System.Data;
using System.Windows;
using NetworkModel;
using TableModel;

namespace SampleCode
{
	/// <summary>
	/// Interaction logic for TableWindow.xaml
	/// </summary>
	public partial class TableWindow : Window
	{
		public TableWindow(MainWindowViewModel vm)
		{
			InitializeComponent();
			if (vm != null)
			{
				LoadTable(vm.Table);
				vm.TableChanged += new TableChangedEventHandler(mainWindowViewModel_TableChanged);
			}
		}

		private void mainWindowViewModel_TableChanged(object sender, TableChangedEventArgs selectionChangedEventArgs)
		{
			LoadTable(selectionChangedEventArgs.Table);
		}

		private void LoadTable(TableViewModel tablevm)
		{
			if (tablevm == null)
			{
				ResetUi();
				return;
			}
			
			switch (tablevm.Type)
			{
				case NetworkType.Mealy:
					UpdateUi("Вариант: " + tablevm.TaskId, "Тип автомата: Мили", LoadMealyTable(tablevm));
					break;
				case NetworkType.Moore:
					UpdateUi("Вариант: " + tablevm.TaskId, "Тип автомата: Мура", LoadMooreTable(tablevm));
					break;
				default:
					ResetUi();
					break;
			}
		}

		private void UpdateUi(string task, string type, DataView table)
		{
			TaskIdTextBlock.Text = task;
			TypeTextBlock.Text = type;
			TableDataGrid.ItemsSource = table;
		}

		private void ResetUi()
		{
			UpdateUi("Автомат не задан", "", null);
		}

		private DataView LoadMealyTable(TableViewModel tablevm)
		{
			int rowCount = tablevm.InputsCount + 1,
				colCount = tablevm.StatesCount + 1;
			var table = new DataTable();

			for (var i = 0; i < colCount; i++)
			{
				table.Columns.Add(new DataColumn("Column " + i));
			}

			var headerRow = table.NewRow();
			headerRow["Column 0"] = "x\\a";
			for (var j = 1; j < colCount; j++)
			{
				headerRow["Column " + j] = "a" + j;
			}
			table.Rows.Add(headerRow);

			for (var i = 1; i < rowCount; i++)
			{
				var row = table.NewRow();
				row["Column 0"] = "x" + i;
				for (var j = 1; j < colCount; j++)
				{
					row["Column " + j] = tablevm.ContentFirst[i-1][j] + "/" + tablevm.ContentSecond[i-1][j];
				}
				table.Rows.Add(row);
			}

			return table.DefaultView;
		}

		private DataView LoadMooreTable(TableViewModel tablevm)
		{
			int rowCount = tablevm.InputsCount + 2,
				colCount = tablevm.StatesCount + 1;
			var table = new DataTable();

			for (var i = 0; i < colCount; i++)
			{
				table.Columns.Add(new DataColumn("Column " + i));
			}

			var outputRow = table.NewRow();
			outputRow["Column 0"] = "Вых. сигнал";
			for (var j = 1; j < colCount; j++)
			{
				outputRow["Column " + j] = tablevm.ContentFirst[0][j-1];
			}
			table.Rows.Add(outputRow);

			var headerRow = table.NewRow();
			headerRow["Column 0"] = "x\\a";
			for (var j = 1; j < colCount; j++)
			{
				headerRow["Column " + j] = "a" + j;
			}
			table.Rows.Add(headerRow);

			for (var i = 2; i < rowCount; i++)
			{
				var row = table.NewRow();
				row["Column 0"] = "x" + (i - 1);
				for (var j = 1; j < colCount; j++)
				{
					row["Column " + j] = tablevm.ContentSecond[i - 2][j];
				}
				table.Rows.Add(row);
			}
			
			return table.DefaultView;
		}
	}
}
