using System.Collections.Generic;
using System.Xml.Serialization;

namespace BrowserSwitch
{

    [XmlRoot("Websites")]
    public class WebsiteList
    {
        [XmlElement("Website")]
        public List<Website> Websites { get; set; }

    }
}