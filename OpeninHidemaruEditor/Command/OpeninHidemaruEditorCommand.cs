using System;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using OpeninHidemaruEditor.Helpers;
using Task = System.Threading.Tasks.Task;

namespace OpeninHidemaruEditor
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class OpeninHidemaruEditorCommand
    {
        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly AsyncPackage   _package;
        private readonly Settings       _settings;
        private DTE2 _dte;

        /// <summary>
        /// Initializes a new instance of the <see cref="OpeninHidemaruEditorCommand"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        /// <param name="commandService">Command service to add command to, not null.</param>
        private OpeninHidemaruEditorCommand(AsyncPackage package, OleMenuCommandService commandService)
        {
            _settings= (Settings)package.GetDialogPage(typeof(Settings));
            this._package = package ?? throw new ArgumentNullException(nameof(package));
            commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));

            {
                var menuCommandID = new CommandID(PackageGuids.guidCommandPackageCmdSet, PackageIds.CommandId);
                var menuItem = new MenuCommand(this.Execute, menuCommandID);
                commandService.AddCommand(menuItem);
            }
            {
                var menuCommandID = new CommandID(PackageGuids.guidCommandAPackageCmdSet, PackageIds.CommandAId);
                var menuItem = new MenuCommand(this.Execute2, menuCommandID);
                commandService.AddCommand(menuItem);
            }
        }

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static OpeninHidemaruEditorCommand Instance
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the service provider from the owner package.
        /// </summary>
        private Microsoft.VisualStudio.Shell.IAsyncServiceProvider ServiceProvider
        {
            get
            {
                return this._package;
            }
        }

        /// <summary>
        /// Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        public static async Task InitializeAsync(AsyncPackage package)
        {
            // Switch to the main thread - the call to AddCommand in Command1's constructor requires
            // the UI thread.
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

            OleMenuCommandService commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
            Instance = new OpeninHidemaruEditorCommand(package, commandService);
            Instance._dte = await package.GetServiceAsync(typeof(DTE)) as DTE2;
        }

        private void Execute2(object sender, EventArgs e)
        {
            MessageBox.Show("Excute2");
        }

        /// <summary>
        /// This function is the callback used to execute the command when the menu item is clicked.
        /// See the constructor to see how the menu item is associated with this function using
        /// OleMenuCommandService service and MenuCommand class.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event args.</param>
        private  void Execute(object sender, EventArgs e)
        {
            var service = _dte;
            try
            {
                ThreadHelper.ThrowIfNotOnUIThread();
                var selectedFilePath = ProjectHelpers.GetSelectedPath(service);
                var executablePath = _settings.FolderPath;
                if (!string.IsNullOrEmpty(selectedFilePath) && !string.IsNullOrEmpty(executablePath))
                {
                    OpenHidemaruEditor(executablePath, selectedFilePath);
                }
                else
                {
                    MessageBox.Show("Couldn't resolve the folder");
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }


        private static void OpenHidemaruEditor(string executablePath, string selectedFilePath)
        {
            ///j行番号,桁番号　または/j行番号 
            Debug.WriteLine("executablePath={0} / selectedFilePath={1}", executablePath,selectedFilePath);            

            var startInfo = new ProcessStartInfo
            {
                //WorkingDirectory = selectedFilePath,
                FileName = $"\"{executablePath}\"",
                Arguments = $"\"{selectedFilePath}\"",
                //CreateNoWindow = false,
                //WindowStyle = ProcessWindowStyle.Normal;
            };
            using (System.Diagnostics.Process.Start(startInfo))
            {
                //TODO : Should this be empty?
            }
        }
    }
}
