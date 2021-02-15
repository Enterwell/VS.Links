using Microsoft.VisualStudio.Shell;

namespace Enterwell.VS.Links.Options
{
    /// <summary>
    /// A base class for a DialogPage to show in Tools -> Options.
    /// </summary>
    internal class BaseOptionPage<T> : DialogPage where T : BaseOptionModel<T>, new()
    {
        private readonly BaseOptionModel<T> model;

        public BaseOptionPage()
        {
#pragma warning disable VSTHRD104 // Offer async methods
            model = ThreadHelper.JoinableTaskFactory.Run(BaseOptionModel<T>.CreateAsync);
#pragma warning restore VSTHRD104 // Offer async methods
        }

        public override object AutomationObject => model;

        public override void LoadSettingsFromStorage()
        {
            model.Load();
        }

        public override void SaveSettingsToStorage()
        {
            model.Save();
        }
    }
}
