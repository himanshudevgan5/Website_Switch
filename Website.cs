using System.Windows.Forms;
using System.Xml.Serialization;

namespace BrowserSwitch
{
    public class Website
    {
        [XmlElement("ID")]
        public int ID { get; set; }

        [XmlElement("PageTitle")]
        public string PageTitle { get; set; }

        [XmlElement("URL")]
        public string URL { get; set; }

        [XmlElement("URLtype")]
        public string URLType { get; set; }

        [XmlElement("Zoomlevel")]
        public int ZoomLevel { get; set; }

        [XmlElement("ErrorURL")]
        public string ErrorURL { get; set; }

        [XmlElement("ErrorURLType")]
        public string ErrorURLType { get; set; }

        [XmlElement("ZoomLevelErrorURL")]
        public int ZoomLevelErrorURL { get; set; }

    }
}