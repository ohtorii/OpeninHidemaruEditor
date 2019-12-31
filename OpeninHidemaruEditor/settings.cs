using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using Microsoft.VisualStudio.Shell;


namespace OpeninHidemaruEditor
{
	public class Settings : DialogPage
	{
		[Category("General")]
		[DisplayName("Install path")]
		[Description("The absolute path to the \"hidemaru.exe\" file.")]
		public string FolderPath { get; set; }

		public override void LoadSettingsFromStorage()
		{
			base.LoadSettingsFromStorage();

			if (!string.IsNullOrEmpty(this.FolderPath))
			{
				return;
			}

			this.FolderPath = FindNotepadPlusPlus();
		}

		private static string FindNotepadPlusPlus()
		{
			return "ダミー";

			var directoryInfo = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles));
			if (directoryInfo.Parent == null)
			{
				return null;
			}

			foreach (var directory in directoryInfo.Parent.GetDirectories(directoryInfo.Name.Replace(" (x86)", string.Empty) + "*"))
			{
				foreach (var fileSystemInfo in directory.GetDirectories("Notepad++").Reverse())
				{
					var path = Path.Combine(fileSystemInfo.FullName, "notepad++.exe");
					if (File.Exists(path))
					{
						return path;
					}
				}
			}

			return null;
		}
	}
}
