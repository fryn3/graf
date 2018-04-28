using System;
using System.IO;
using Microsoft.Win32;

namespace FsmUtils
{
	public class FileHandler
	{
		// private const string SolutionPath = @"data\solution_{TaskId}.bin";

		private const string TablePath = @"data\table_{TaskId}.bin";

		private const string FileDialogFilter = @"Файл программы .dat|*.dat|Все файлы|*.*";

		public string Open(out string filePath)
		{
			var dialog = new OpenFileDialog();
			dialog.Filter = FileDialogFilter;
			if (dialog.ShowDialog() != true)
			{
				filePath = null;
				return null;
			}

			filePath = dialog.FileName;
			return File.ReadAllText(dialog.FileName);
		}

		public void Save(string data, string filePath)
		{
			if (string.IsNullOrEmpty(filePath))
			{
				filePath = GetSaveFileName();
				if (string.IsNullOrEmpty(filePath)) return;
			}

			var file = new FileInfo(filePath);
			file.Directory?.Create();
			File.WriteAllText(filePath, data);
		}

		public void SaveAs(string data, out string filePath)
		{
			filePath = GetSaveFileName();
			if (string.IsNullOrEmpty(filePath)) return;
			Save(data, filePath);
		}

		private string GetSaveFileName()
		{
			var dialog = new SaveFileDialog();
			dialog.Filter = FileDialogFilter;
			return dialog.ShowDialog() != true ? null : dialog.FileName;
		}

		/* Solution */

		/*
		public string OpenSolution(int taskId)
		{
			var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, SolutionPath.Replace("{TaskId}", taskId.ToString()));
			var data = File.ReadAllText(path);
			return Encription.Decrypt(data);
		}

		public void SaveSolution(string json, int taskId)
		{
			var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, SolutionPath.Replace("{TaskId}", taskId.ToString()));
			var data = Encription.Encrypt(json);
			Save(data, path);
		}
		*/

		/* Table */

		public string OpenTable(int taskId)
		{
			var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, TablePath.Replace("{TaskId}", taskId.ToString()));
			var data = File.ReadAllText(path);
			//return Encription.Decrypt(data);
			return data;
		}

		public void SaveTable(string json, int taskId)
		{
			var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, TablePath.Replace("{TaskId}", taskId.ToString()));
			var data = Encription.Encrypt(json);
			//Save(data, path);
			Save(json, path);
		}

	}
}
