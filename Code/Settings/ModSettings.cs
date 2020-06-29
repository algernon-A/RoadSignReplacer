using System.Xml.Serialization;


namespace RoadSignReplacer
{
    internal static class Settings
    {
        internal static string signPackName;
        internal static string speedPackName;
    }


    /// <summary>
    /// Defines the XML settings file.
    /// </summary>
    [XmlRoot("RoadSignReplacer")]
    public class XMLSettingsFile
    {
        // Language.
        [XmlElement("Language")]
        public string language
        {
            get
            {
                return Translations.Language;
            }
            set
            {
                Translations.Language = value;
            }
        }

        [XmlElement("signPackName")]
        public string SignPackName { get => Settings.signPackName; set => Settings.signPackName = value; }

        [XmlElement("speedPackName")]
        public string SpeedPackName { get => Settings.speedPackName; set => Settings.speedPackName = value; }
    }
}