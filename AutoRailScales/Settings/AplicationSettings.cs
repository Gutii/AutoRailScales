using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASHK.AutoRailScales.Settings
{
    public class AplicationSettings
    {
        [System.Xml.Serialization.XmlAttribute()]
        public string ComPortNameAutomobile { get; set; } = "COM2";
        [System.Xml.Serialization.XmlAttribute()]
        public string ComPortNameRailway { get; set; } = "COM4";
        [System.Xml.Serialization.XmlAttribute()]
        public int SocketPortAuto { get; set; } = 6981;
        [System.Xml.Serialization.XmlAttribute()]
        public int SocketPortRailway { get; set; } = 6982;
    }
}

