﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Utils
{
	public static class Helpers
	{
		public static bool IsWindowOpen<T>(string name = "") where T : Window
		{
			return string.IsNullOrEmpty(name)
			   ? Application.Current.Windows.OfType<T>().Any()
			   : Application.Current.Windows.OfType<T>().Any(w => w.Name.Equals(name));
		}
	}
}
