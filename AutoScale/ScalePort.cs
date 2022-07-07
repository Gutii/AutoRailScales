using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;

namespace ComPort
{
    public class ScalePort : IDisposable
    {
        public System.IO.Ports.SerialPort SerialPort { get; set; } = new System.IO.Ports.SerialPort();
        public bool Connect 
        { 
            get
            {
                return SerialPort.IsOpen;
            }
            private set
            {
                value = SerialPort.IsOpen;
            }
        }
        public int Weight { get; set; }
        public bool Stabilization { get; set; }

        private Scale.IScale Scale { get; set; }
        private System.Threading.Thread ThreadWeight { get; set; }
        private System.Threading.Thread ThreadConnect { get; set; }
        private bool _weighingConnect = false;
        private int _countStab = 0;
        private int _lastWeight = 0;
        private bool _autoResetThread = false;
        public ScalePort(string ComPortName, Scale.IScale scale)
        {
            try
            {
                SerialPort.PortName = ComPortName;
                Scale = scale;              

            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public void Weighting()
        {
            try
            {
                if (!_weighingConnect && !_autoResetThread)
                {
                    Open();
                    _weighingConnect = true;
                    ThreadConnect.Start();

                    _autoResetThread = true;
                    ThreadWeight.Start();
                }
                _weighingConnect = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        private void TimeConnecting()
        {
            while(_weighingConnect)
            {
                _weighingConnect = false;
                System.Threading.Thread.Sleep(1500);
                if (!_weighingConnect)
                {
                    _autoResetThread = false;                    
                    _countStab = 0;
                    Close();
                }
            }
            
        }
        private void ScaleWeight()
        {
            try
            { 
                while(_autoResetThread)
                {
                    if (!SerialPort.IsOpen)
                    {
                        Weight = 0;
                        Stabilization = false;
                        return;
                    }

                    if(SerialPort.BytesToRead>0)
                    {
                        byte[] readByte = new byte[SerialPort.BytesToRead];
                        SerialPort.Read(readByte, 0, SerialPort.BytesToRead);
                        string str = Encoding.Default.GetString(readByte);
                        Weight = Scale.SetWeight(str); 

                        Stabiliz();
                    }
                    
                    System.Threading.Thread.Sleep(200);
                }                
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void Stabiliz()
        {
            if (Weight == _lastWeight)
            {
                if (_countStab == 8)
                {
                    Stabilization = true;
                }
                else
                {
                    _countStab++;
                }
            }
            else
            {
                Stabilization = false;
                _countStab = 0;
                _lastWeight = Weight;
            }
        }

        public void Open()
        {
            try
            {
                if (!SerialPort.IsOpen)
                {
                    SerialPort.Open();
                    ThreadWeight = new System.Threading.Thread(ScaleWeight);
                    ThreadWeight.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
                    ThreadConnect = new System.Threading.Thread(TimeConnecting);
                    ThreadConnect.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Close()
        {
            try
            {
                if (SerialPort.IsOpen)
                {
                    SerialPort.Close();
                }
                _weighingConnect = false;
                Weight = 0;
                Stabilization = false;
                _countStab = 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Dispose()
        {
            SerialPort.Close();
            SerialPort.Dispose();

            _autoResetThread = false;
            System.Threading.Thread.Sleep(20);

            if(ThreadWeight.ThreadState == System.Threading.ThreadState.Running)
            {
                ThreadWeight.Suspend();
            }
            if (ThreadConnect.ThreadState == System.Threading.ThreadState.Running)
            {
                ThreadConnect.Suspend();
            }
        }
    }
}
