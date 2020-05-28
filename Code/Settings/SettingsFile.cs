using System.Xml.Serialization;


namespace RoadSignReplacer
{
    /// <summary>
    /// Configuration file class.
    /// </summary>
    [ConfigurationPath("RoadSignReplacer.xml")]
    [XmlType("RoadSignReplacer")]
    public class SettingsFile
    {
        public string signPackName;
        public string speedPackName;
    }
}