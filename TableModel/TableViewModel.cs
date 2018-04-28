using System.Data;
using NetworkModel;
using Newtonsoft.Json;
using Utils;

namespace TableModel
{
	public class TableViewModel : AbstractModelBase
	{

		private int _taskId;

		private NetworkType _type;

		private int _statesCount;

		private int _inputsCount;

		private int _outputsCount;

		private DataView _contentFirst;

		private DataView _contentSecond;

		public TableViewModel()
		{
			SuppressPopulateRequests = true;
		}

		public TableViewModel(bool suppressPopulateRequests)
		{
			SuppressPopulateRequests = suppressPopulateRequests;
		}

		/// <summary>
		/// Не давать обнулять данные таблиц, пока не установится значение false.
		/// </summary>
		[JsonIgnore]
		public bool SuppressPopulateRequests { get; set; }

		public int TaskId
		{
			get { return _taskId; }
			set
			{
				if (_taskId == value) return;
				_taskId = value;
				OnPropertyChanged("TaskId");
			}
		}

		public NetworkType Type
		{
			get { return _type; }
			set
			{
				if (_type == value) return;
				_type = value;
				OnPropertyChanged("Type");
			}
		}

		public int StatesCount
		{
			get { return _statesCount; }
			set
			{
				if (_statesCount == value) return;
				_statesCount = value;
				OnPropertyChanged("StatesCount");
			}
		}

		public int InputsCount
		{
			get { return _inputsCount; }
			set
			{
				if (_inputsCount == value) return;
				_inputsCount = value;
				OnPropertyChanged("InputsCount");
			}
		}

		public int OutputsCount
		{
			get { return _outputsCount; }
			set
			{
				if (_outputsCount == value) return;
				_outputsCount = value;
				OnPropertyChanged("OutputsCount");
			}
		}

		[JsonIgnore]
		public DataView ContentFirst
		{
			get { return _contentFirst; }
			set
			{
				if (_contentFirst == value) return;
				_contentFirst = value;
				OnPropertyChanged("ContentFirst");
			}
		}

		[JsonIgnore]
		public DataView ContentSecond
		{
			get { return _contentSecond; }
			set
			{
				if (_contentSecond == value) return;
				_contentSecond = value;
				OnPropertyChanged("ContentSecond");
			}
		}

		public DataTable ContentFirstTable
		{
			get { return ContentFirst?.Table; }
			set { ContentFirst = value.DefaultView; }
		}

		public DataTable ContentSecondTable
		{
			get { return ContentSecond?.Table; }
			set { ContentSecond = value.DefaultView; }
		}

		public void PopulateDataGrid(int rowCount, int columnCount)
		{
			if (SuppressPopulateRequests
				|| (rowCount == 0 || columnCount == 0)
				|| (Type != NetworkType.Moore && Type != NetworkType.Mealy)) return;

			DataTable dataTable = new DataTable();
			DataTable dataTableSecond = new DataTable();

			if (Type == NetworkType.Mealy)
			{
				PopulateMealyDataGrid(rowCount, columnCount, dataTable, dataTableSecond);
			}
			else if (Type == NetworkType.Moore)
			{
				PopulateMooreDataGrid(rowCount, columnCount, dataTable, dataTableSecond);
			}

			ContentFirst = dataTable.DefaultView;
			ContentSecond = dataTableSecond.DefaultView;
		}

		private void PopulateMealyDataGrid(int rowCount, int columnCount, DataTable table1, DataTable table2)
		{
			table1.Columns.Add(new DataColumn("x\\a"));
			for (int j = 1; j <= columnCount; j++)
			{
				table1.Columns.Add(new DataColumn("a" + j));
			}

			for (int i = 0; i < rowCount; i++)
			{
				var newRow = table1.NewRow();
				newRow["x\\a"] = "x" + (i + 1);
				for (int j = 1; j <= columnCount; j++)
					newRow["a" + j] = "-";
				table1.Rows.Add(newRow);
			}


			table2.Columns.Add(new DataColumn("x\\a"));
			for (int j = 1; j <= columnCount; j++)
			{
				table2.Columns.Add(new DataColumn("a" + j));
			}

			for (int i = 0; i < rowCount; i++)
			{
				var newRow = table2.NewRow();
				newRow["x\\a"] = "x" + (i + 1);
				for (int j = 1; j <= columnCount; j++)
					newRow["a" + j] = "-";
				table2.Rows.Add(newRow);
			}
		}

		private void PopulateMooreDataGrid(int rowCount, int columnCount, DataTable table1, DataTable table2)
		{
			for (int j = 1; j <= columnCount; j++)
			{
				table1.Columns.Add(new DataColumn("a" + j));
			}

			var outputRow = table1.NewRow();
			for (int j = 1; j <= columnCount; j++)
			{
				outputRow["a" + j] = "-";
			}
			table1.Rows.Add(outputRow);
			
			table2.Columns.Add(new DataColumn("x\\a"));
			for (int j = 1; j <= columnCount; j++)
			{
				table2.Columns.Add(new DataColumn("a" + j));
			}

			for (int i = 0; i < rowCount; i++)
			{
				var newRow = table2.NewRow();
				newRow["x\\a"] = "x" + (i + 1);
				for (int j = 1; j <= columnCount; j++)
					newRow["a" + j] = "-";
				table2.Rows.Add(newRow);
			}
		}
	}
}
