using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Enterwell.VS.Links.Options;
using Microsoft.VisualStudio.Shell;
using Button = Options.Button;
using Process = System.Diagnostics.Process;

namespace Enterwell.VS.Links
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class CommandBuilder
    {
        private readonly OleMenuCommandService commandService;

        private readonly int PlaceholderId = PackageIds.PlaceholderCommandId;

        private readonly Dictionary<string, int> additionalButtonNamesAndIds = new Dictionary<string, int>();

        /// <summary>
        /// Method used to create a singleton instance of the <see cref="CommandBuilder"/>.
        /// </summary>
        /// <param name="package"></param>
        /// <returns></returns>
        public static async Task<CommandBuilder> CreateAsync(AsyncPackage package)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

            OleMenuCommandService commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;

            Instance = new CommandBuilder(commandService, package);

            return Instance;
        }

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = PackageGuids.guidLinksPackageCmdSet;

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly AsyncPackage package;

        private CommandBuilder(OleMenuCommandService commandService, AsyncPackage package)
        {
            this.commandService = commandService;
            this.package = package;
        }

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static CommandBuilder Instance
        {
            get;
            private set;
        }

        /// <summary>
        /// Method to register new command to the <see cref="OleMenuCommandService"/> command service.
        /// </summary>
        /// <param name="commandId">Integer representing the ID of the command.</param>
        public void Register(int commandId)
        {
            var cmdId = new CommandID(CommandSet, commandId);
            var menuItem = new OleMenuCommand((s, e) => Execute(s), cmdId);

            this.commandService.AddCommand(menuItem);
        }

        /// <summary>
        /// Method to register new user added command to the <see cref="OleMenuCommandService"/> command service.
        /// </summary>
        /// <param name="commandId">Integer representing the ID of the command.</param>
        /// <param name="commandName">Name to assign to the command.</param>
        public void Register(int commandId, string commandName)
        {
            var cmdId = new CommandID(CommandSet, commandId);
            var menuItem = new OleMenuCommand((s, e) => Execute(s), cmdId);
            menuItem.Text = commandName;

            var existingCommandWithId = this.commandService.FindCommand(cmdId);
            if (existingCommandWithId != null)
            {
                this.commandService.RemoveCommand(existingCommandWithId);
            }

            this.commandService.AddCommand(menuItem);
        }

        /// <summary>
        /// Callback for when the button (command) is clicked. 
        /// </summary>
        /// <param name="s">Clicked object that invoked the callback.</param>
        private void Execute(object s)
        {
            if (s is OleMenuCommand menuCommand)
            {
                // Checking if one of the buttons created through the Options page was clicked.
                if (additionalButtonNamesAndIds.ContainsKey(menuCommand.Text))
                {
                    var buttonsFromTheOptions = GeneralOptions.Instance.Buttons;

                    var selectedBtnUrl = buttonsFromTheOptions.First(b => b.ButtonName == menuCommand.Text).URL;

                    try
                    {
                        Process.Start(selectedBtnUrl);
                    }
                    catch (Exception ex)
                    {
                        var textToDisplay = $"{ex.Message}" + Environment.NewLine +
                                            $"Action bound to the button: '{selectedBtnUrl}'" + Environment.NewLine +
                                            "Check if it is valid.";

                        MessageBox.Show(textToDisplay, "Exception Happened", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    // If we processed the button action return at this point. No need to continue.
                    return;
                }

                // If no user created button was clicked, it was one of the static ones.
                switch (menuCommand.CommandID.ID)
                {
                    case PackageIds.WikiCommandId:
                        Process.Start(GeneralOptions.Instance.WikiUrl);
                        break;
                    case PackageIds.AzureCommandId:
                        Process.Start(GeneralOptions.Instance.DevOpsUrl);
                        break;
                    case PackageIds.ClockifyCommandId:
                        Process.Start(GeneralOptions.Instance.ClockifyUrl);
                        break;
                }
            }
        }

        /// <summary>
        /// Used to register newly added buttons from the Options page. Called automatically after saving Options in the <see cref="GeneralOptions"/>.
        /// </summary>
        public void RegisterOptionButtons()
        {
            RemoveExistingAdditionalButtons();

            var additionalButtons = GeneralOptions.Instance.Buttons;
            var latestId = 0;

            foreach (Button button in additionalButtons)
            {
                var newButtonId = latestId == 0 ? this.PlaceholderId : latestId;
                Register(newButtonId, button.ButtonName);

                if (!additionalButtonNamesAndIds.ContainsKey(button.ButtonName))
                {
                    this.additionalButtonNamesAndIds.Add(button.ButtonName, newButtonId);
                }

                latestId = newButtonId + 1;
            }

            latestId = 0;
        }

        /// <summary>
        /// Used to remove all existing additional buttons added by the user so that the old versions don't stick around in the Menu Controller
        /// if user changed them somehow in the Options page.
        /// </summary>
        private void RemoveExistingAdditionalButtons()
        {
            foreach (var button in additionalButtonNamesAndIds)
            {
                var buttonId = button.Value;
                var cmdId = new CommandID(CommandSet, buttonId);

                var buttonCommand = this.commandService.FindCommand(cmdId);
                if (buttonCommand != null)
                {
                    this.commandService.RemoveCommand(buttonCommand);
                }
            }

            additionalButtonNamesAndIds.Clear();
        }
    }
}
