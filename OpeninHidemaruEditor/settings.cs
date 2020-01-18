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

			this.FolderPath = FindHidemaruEditor();
		}

		private static string FindHidemaruEditor()
		{
//			return @"C:\Program Files\Hidemaru\Hidemaru.exe";

			string[] rootPaths;
			if (Environment.Is64BitOperatingSystem)
			{
				//秀丸エディタ64bit版を優先して探す。
				rootPaths = new string[] {
					@"C:\Program Files",
					@"C:\Program Files (x86)"
				};
			}
			else
			{
				rootPaths=new string[] { 
					Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) 
				};
			}

			foreach(var root in rootPaths)
			{
				var absPath=Path.Combine(root, @"Hidemaru\Hidemaru.exe");
				if (File.Exists(absPath))
				{
					return absPath;
				}
			}
			return null;
		}
	}
}
