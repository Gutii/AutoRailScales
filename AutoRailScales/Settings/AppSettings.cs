using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASHK.AutoRailScales.Settings
{
    public class AppSettings
    {
        public AplicationSettings AplicationSettings { get; set; } = new AplicationSettings();
        public void LoadFromFile(string fileName)
        {
            try
            {
                var _iniFile = new ASHK.Common.INI.IniFile(fileName);
                #region "AplicationSettings"
                try
                {
                    this.AplicationSettings.ComPortNameAutomobile = _iniFile.GetValue("AplicationSettings", "ComPortNameAutomobile");
                }
                catch (Exception ex)
                {
                    ServicePanel.WriteLog(ex.Message, true);
                }
                try
                {
                    this.AplicationSettings.ComPortNameRailway = _iniFile.GetValue("AplicationSettings", "ComPortNameRailway");
                }
                catch (Exception ex)
                {
                    ServicePanel.WriteLog(ex.Message, true);
                }
                try
                {
                    this.AplicationSettings.SocketPortAuto = int.Parse(_iniFile.GetValue("AplicationSettings", "SocketPortAuto"));
                }
                catch (Exception ex)
                {
                    ServicePanel.WriteLog(ex.Message, true);
                }
                try
                {
                    this.AplicationSettings.SocketPortRailway = int.Parse(_iniFile.GetValue("AplicationSettings", "SocketPortRailway"));
                }
                catch (Exception ex)
                {
                    ServicePanel.WriteLog(ex.Message, true);
                }
                
                #endregion
              
            }
            catch (Exception ex)
            {
                ServicePanel.WriteLog(ex.Message, true);
            }
        }
    }
}
