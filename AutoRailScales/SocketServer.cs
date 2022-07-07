using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ASHK.AutoRailScales
{
    public class SocketServer : IDisposable
    {
        Thread Thread1 = null;
        public string IP { get; set; }
        public int Port { get; set; }
        private ComPort.ScalePort ScalePort { get; set; }
        private Socket listenSocket { get; set; }
        public SocketServer()
        { 
        
        }
        public SocketServer(string IP, int Port, ComPort.ScalePort scalePort)
        {
            this.IP = IP;
            this.Port = Port;
            ScalePort = scalePort;
        }

        public void Start()
        {
            try
            {
                Thread1 = new Thread(CreateServerSocket);
                Thread1.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
                Thread1.Start();
            }
            catch(Exception ex)
            {
                ServicePanel.WriteLog(ex.HResult + " " + ex.Message , true);
            }
        }

        public void Stop()
        {            
            try
            {
                if (Thread1 != null)
                {
                    Thread1.Suspend();
                }
                listenSocket.Close();
                listenSocket.Dispose();

                ScalePort.Close();
                ScalePort.Dispose();
            }
            catch(Exception ex)
            {
                ServicePanel.WriteLog(ex.HResult + " " + ex.Message, true);
            }
        }

        private void CreateServerSocket()
        {
            IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse(IP), Port);

            listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                // связываем сокет с локальной точкой, по которой будем принимать данные
                listenSocket.Bind(ipPoint);

                // начинаем прослушивание
                listenSocket.Listen(10);

                while (true)
                {
                    Socket handler = listenSocket.Accept();
                    
                    // получаем сообщение
                    byte[] data = new byte[256]; // буфер для получаемых данных
                    StringBuilder builder = new StringBuilder();
                    int bytes = 0; // количество полученных байтов

                    //do
                    //{
                    //    bytes = handler.Receive(data);
                    //    builder.Append(Encoding.ASCII.GetString(data, 0, bytes));
                    //}
                    //while (handler.Available > 0);
                    //читаем строку из сокета
                    //ScalePort.Weight = 1000;

                    try
                    {
                        ScalePort.Open();
                        do
                        {
                            ScalePort.Weighting();
                            System.Threading.Thread.Sleep(250);
                        }
                        while (!ScalePort.Stabilization);
                    }
                    catch (Exception ex)
                    {
                        ServicePanel.WriteLog(ex.HResult + " " + ex.Message, true);
                    }
                    // отправляем ответ
                    string message = ScalePort.Weight + ";" + ScalePort.Stabilization + ";ERROR:" + ServicePanel.ErrorLog;
                    Encoding encoding = Encoding.GetEncoding("windows-1251");
                    data = encoding.GetBytes(message);
                    handler.Send(data);
                    // закрываем сокет
                    handler.Shutdown(SocketShutdown.Both);
                    handler.Close();
                }
            }
            catch (Exception ex)
            {
                ServicePanel.WriteLog(ex.HResult + " " + ex.Message, true);
            }

        }

        public void Dispose()
        {
            Stop();            
        }
    }
}
