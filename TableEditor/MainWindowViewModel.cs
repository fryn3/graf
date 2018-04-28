using System;
using System.Collections.Generic;
using System.Windows;
using FsmUtils;
using NetworkModel;
using Newtonsoft.Json;
using TableModel;
using Utils;

namespace TableEditor
{
	public class MainWindowViewModel : AbstractModelBase
	{

		private TableViewModel _table = new TableViewModel(false);

		private int _columnCount;

		private int _rowCount;

		private readonly FileHandler _fileHandler = new FileHandler();

		private string _tableFirstLabel = "�������� �������";

		private string _tableSecondLabel = "�������� �������";

		public ImpObservableCollection<string> PossibleStates { get; } = new ImpObservableCollection<string>();

		public ImpObservableCollection<string> PossibleInputs { get; } = new ImpObservableCollection<string>();

		public ImpObservableCollection<string> PossibleOutputs { get; } = new ImpObservableCollection<string>();

		private int ColumnCount
		{
			get { return _columnCount; }
			set
			{
				if (_columnCount == value) return;
				_columnCount = value;
				Table.PopulateDataGrid(_rowCount, _columnCount);
				OnPropertyChanged("ColumnCount");
			}
		}

		private int RowCount
		{
			get { return _rowCount; }
			set
			{
				if (_rowCount == value) return;
				_rowCount = value;
				Table.PopulateDataGrid(_rowCount, _columnCount);
				OnPropertyChanged("RowCount");
			}
		}

		public bool IsTaskSelected
		{
			get { return Table.TaskId != 0; }
		}

		public TableViewModel Table
		{
			get { return _table; }
			set
			{
				_table = value;
				OnPropertyChanged("Table");
			}
		}

		public String TableFirstLabel
		{
			get { return _tableFirstLabel; }
			set
			{
				_tableFirstLabel = value;
				OnPropertyChanged("TableFirstLabel");
			}
		}

		public String TableSecondLabel
		{
			get { return _tableSecondLabel; }
			set
			{
				_tableSecondLabel = value;
				OnPropertyChanged("TableSecondLabel");
			}
		}

		public void Save()
		{
			if (!IsValid())
			{
				MessageBox.Show("���������� ������� ���������� ���� �����. ����������, ���������, ������������ ���������� �����.",
					"������ ��� ����������", MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
			}

			try
			{
				var json = JsonConvert.SerializeObject(Table, Formatting.Indented,
					new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
				_fileHandler.SaveTable(json, Table.TaskId);
			}
			catch (Exception ex)
			{
				MessageBox.Show("�������� ������ ��� ���������� �����:" + ex.Message,
					"������ ��� ����������", MessageBoxButton.OK, MessageBoxImage.Warning);
			}
		}

		/// <summary>
		/// ����� ��������� ��������.
		/// </summary>
		public void Reset()
		{
			int taskId = 0;
			if (Table != null) taskId = Table.TaskId;
			Table = new TableViewModel(false);
			Table.TaskId = taskId;
		}

		public void OnTaskIdChanged()
		{
			if (!TryLoad())
			{
				Reset();
			}
			OnPropertyChanged("IsTaskSelected");
		}

		public void OnTypeChanged()
		{
			Table.PopulateDataGrid(RowCount, ColumnCount);
			if (Table.Type == NetworkType.Mealy)
			{
				TableFirstLabel = "������� ���������";
				TableSecondLabel = "������� �������� ��������";
			} else if (Table.Type == NetworkType.Moore)
			{
				TableFirstLabel = "������� �������� ��������";
				TableSecondLabel = "������� ���������";
			}
		}

		public void OnStatesCountChanged()
		{
			ColumnCount = Table.StatesCount;
			SetPossibleStates(Table.StatesCount);
		}

		public void OnInputsCountChanged()
		{
			RowCount = Table.InputsCount;
			SetPossibleInputs(Table.InputsCount);
		}

		public void OnOutputsCountChanged()
		{
			SetPossibleOutputs(Table.OutputsCount);
		}

		/// <summary>
		/// ��������� ������ � ������� TaskId, �� ������� ��������� �� ������ � ������ �������� � ���������.
		/// </summary>
		private bool TryLoad()
		{
			try
			{
				var json = _fileHandler.OpenTable(Table.TaskId);
				var table = JsonConvert.DeserializeObject<TableViewModel>(json);
				if (table == null)
					return false;
				this.Table = table;
				// �����, ���� WPF ������� �������� ������ � ������� ��� ������, ����������� �������.
				// �� ��� ���, ���� ���� �� ����������� �������� false, ������� ����� ��������� �� ��������,
				// ������� ���� ����������� ��� ������������� ����� JsonConvert.
				this.Table.SuppressPopulateRequests = false;
				return true;
			}
			catch
			{
				// ignored
			}

			return false;
		}

		private void SetPossibleStates(int maxState)
		{
			var possibleStates = new List<string>();

			possibleStates.Add("-");
			for (var i = 1; i <= maxState; i++)
			{
				possibleStates.Add('a' + i.ToString());
			}

			PossibleStates.Clear();
			PossibleStates.AddRange(possibleStates);
		}


		private void SetPossibleInputs(int maxInput)
		{
			var possibleInputs = new List<string>();

			possibleInputs.Add("-");
			for (var i = 1; i <= maxInput; i++)
			{
				possibleInputs.Add('a' + i.ToString());
			}

			PossibleInputs.Clear();
			PossibleInputs.AddRange(possibleInputs);
		}


		private void SetPossibleOutputs(int maxOutput)
		{
			var possibleOutputs = new List<string>();

			possibleOutputs.Add("-");
			for (var i = 1; i <= maxOutput; i++)
			{
				possibleOutputs.Add('a' + i.ToString());
			}

			PossibleOutputs.Clear();
			PossibleOutputs.AddRange(possibleOutputs);
		}

		/// <summary>
		/// �������� ���������� ��������.
		/// </summary>
		/// <returns></returns>
		private bool IsValid()
		{
			return Table != null &&
				(Table.TaskId > 0 && Table.TaskId <= Constants.TasksCount) &&
				(Table.Type == NetworkType.Mealy || Table.Type == NetworkType.Moore) &&
				(Table.StatesCount >= Constants.InnerStatesMin && Table.StatesCount <= Constants.InnerStatesMax) &&
				(Table.InputsCount >= Constants.InputsMin && Table.InputsCount <= Constants.InputsMax) &&
				(Table.OutputsCount >= Constants.OutputsMin && Table.OutputsCount <= Constants.OutputsMax);
		}

		/// <summary>
		/// �������� �������� �� ���������� ��������.
		/// </summary>
		/// <returns></returns>
		private bool IsEmpty()
		{
			return Table != null &&
				(Table.TaskId > 0 && Table.TaskId <= Constants.TasksCount) &&
				(Table.Type == 0) && (Table.StatesCount == 0) && (Table.InputsCount == 0) && (Table.OutputsCount == 0);
		}
	}
}