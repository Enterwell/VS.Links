using System;

namespace Enterwell.VS.Links.Options
{
    /// <summary>
    /// Class used only to represent a single button in the options page. Needs to be serializable.
    /// </summary>
    [Serializable]
    public class Button
    {
        public string ButtonName { get; set; }
        public string URL { get; set; }
    }
}