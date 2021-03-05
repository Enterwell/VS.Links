using System.Collections.Generic;
using System.ComponentModel;

namespace Enterwell.VS.Links.Options
{
    /// <summary>
    /// Class that represents General Options in the Tools -> Options page.
    /// </summary>
    internal class GeneralOptions : BaseOptionModel<GeneralOptions>
    {
        /// <summary>
        /// Property that holds WikiUrl entered through the Options page.
        /// </summary>
        [Category("Enterwell Links")]
        [DisplayName("Wiki URL")]
        [Description("Specifies the url for the Wiki page.")]
        public string WikiUrl { get; set; } = "https://example.com?page=wikiPage";

        /// <summary>
        /// Property that holds DevOpsUrl entered through the Options page.
        /// </summary>
        [Category("Enterwell Links")]
        [DisplayName("DevOps URL")]
        [Description("Specifies the url for the DevOps page.")]
        public string DevOpsUrl { get; set; } = "https://dev.azure.com/";

        /// <summary>
        /// Property that holds ClockifyUrl entered through the Options page.
        /// </summary>
        [Category("Enterwell Links")]
        [DisplayName("Clockify URL")]
        [Description("Specifies the url for the Clockify page.")]
        public string ClockifyUrl { get; set; } = "https://clockify.me/tracker";

        /// <summary>
        /// Property that holds user defined buttons entered through the Options page.
        /// </summary>
        [Category("Enterwell Links")]
        [DisplayName("Additional Buttons")]
        [Description("Specifies the additional buttons to be display in the extension.")]
        public List<Button> Buttons { get; set; } = new List<Button>();

        /// <summary>
        /// Using the <see cref="CommandBuilder"/> to register the newly defined buttons from the Options page.
        /// </summary>
        public override void Save()
        {
            base.Save();

            CommandBuilder.Instance.RegisterOptionButtons();
        }
    }
}
