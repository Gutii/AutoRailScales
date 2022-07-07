using System;
using System.IO;
using System.Configuration.Install;
using System.ServiceProcess;
using System.Threading;
using ASHK.AutoRailScales;
using System.Diagnostics;

public class ServicePanel : ServiceBase
{
    private readonly string FileUdl = System.Reflection.Assembly.GetExecutingAssembly().Location.Replace(".exe", ".udl");
    private static readonly string FileLog = System.Reflection.Assembly.GetExecutingAssembly().Location.Replace(".exe", ".log");
    
    private static ASHK.AutoRailScales.Settings.AppSettings Settings = new ASHK.AutoRailScales.Settings.AppSettings();
    private static SocketServer _socketServerRailway = null;
    private static SocketServer _socketServerAutomobile = null;
    public static string ErrorLog = string.Empty;
    private static System.Timers.Timer TimerErrorLog = new System.Timers.Timer(3000);
    // Главная точка входа процесса
    [MTAThread()]
    public static void Main(string[] args)
    {
        if (args.Length > 0)
        {
            switch (args[0].ToUpper())
            {
                case "-I":
                case "-i":
                case "-install":
                case "-Install":
                    {
                        Install();
                        break;
                    }

                case "-U":
                case "-u":
                case "-uninstall":
                case "-Uninstall":
                    {
                        UnInstall();
                        break;
                    }

                default:
                    {
                        WriteHelp();
                        break;
                    }
            }
        }
        else if (Environment.UserInteractive)
        {
            ServicePanel service = new ServicePanel();
            service.StartDebug();
            Thread.Sleep(Timeout.Infinite);
        }
        else
        {
            System.ServiceProcess.ServiceBase servicesToRun;
            servicesToRun = new ServicePanel();
            ServiceBase.Run(servicesToRun);
        }
    }
    public ServicePanel()
    {
    }
    
    private void Load()
    {
        InitializeApp();
        ComPort.ScalePort Auto = new ComPort.ScalePort(Settings.AplicationSettings.ComPortNameAutomobile, new ComPort.Scale.Automobile());
        ComPort.ScalePort Railway = new ComPort.ScalePort(Settings.AplicationSettings.ComPortNameRailway, new ComPort.Scale.Railway());
        _socketServerAutomobile = new SocketServer("0.0.0.0", Settings.AplicationSettings.SocketPortAuto, Auto);
        _socketServerRailway = new SocketServer("0.0.0.0", Settings.AplicationSettings.SocketPortRailway, Railway);
    }

    public static void InitializeApp()
    {
        try
        {
            //Create AppDomain ProcessExit and UnhandledException event handlers
            AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
        }
        catch (Exception ex)
        {
            WriteLog(ex.Message, true);
            System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
        }
        Settings.LoadFromFile(ASHK.Common.IO.Folders.SettingsFile());
    }
    protected override void OnStart(string[] args)
    {
        Load();
        _socketServerAutomobile.Start();
        _socketServerRailway.Start();
        TimerErrorLog.Elapsed += TimerErrorLogClear;
        TimerErrorLog.AutoReset = true;
        TimerErrorLog.Start();
        WriteLog("Сервис запущен.");
    }
    protected override void OnStop()
    {
        // Добавьте здесь код для завершающих операций перед остановкой службы.
        _socketServerAutomobile.Stop();
        _socketServerRailway.Stop();
        TimerErrorLog.Stop();
        TimerErrorLog.Dispose();
        WriteLog("Сервис остановлен.");
    }
    public void StartDebug()
    {
        this.OnStart(null);
    }   

   
    // Private Sub ErrorPanelHandler(sender As Object, exc As Exception)
    // Try
    // WriteLog(String.Format("{0} - {1}", CType(sender, Panel).ToString, exc.ToString))
    // Catch
    // End Try
    // End Sub
    public static void WriteLog(string message, bool Error = false)
    {
        string str = string.Format("{0} : {1}", DateTime.Now.ToString(), message);
        using (StreamWriter stream = new StreamWriter(FileLog, true))
        {
                stream.WriteLine(str);     
        }
        if(Error)
        {
            ErrorLog = message;
        }

    }
    // Private Sub ServicePanel_Disposed(sender As Object, e As EventArgs) Handles Me.Disposed
    // Me.Dispose(True)
    // End Sub
    private static void WriteHelp()
    {
        Console.WriteLine("параметры:");
        Console.WriteLine("-I | -i | -instal    | -Install   -- Установка сервиса");
        Console.WriteLine("-U | -u | -uninstall | -Uninstall -- Удаление сервиса");
        WriteLog("Запрос справки.");
    }
    private static void Install()
    {
        try
        {
            string exeFileName = System.Reflection.Assembly.GetExecutingAssembly().Location;
            ManagedInstallerClass.InstallHelper(new string[] { exeFileName });
            WriteLog("Сервис инсталлирован.");
        }
        catch (Exception ex)
        {
            WriteLog(ex.Message);
        }
    }
    private static void UnInstall()
    {
        try
        {
            string exeFileName = System.Reflection.Assembly.GetExecutingAssembly().Location;
            ManagedInstallerClass.InstallHelper(new string[] { "/u", exeFileName });
            _socketServerAutomobile.Stop();
            _socketServerRailway.Stop();
            TimerErrorLog.Stop();
            TimerErrorLog.Dispose();
            WriteLog("Сервис деинсталлирован.");
        }
        catch (Exception ex)
        {
            WriteLog(ex.Message);
        }
    }

    private static void TimerErrorLogClear(object sender, EventArgs e)
    {
        ErrorLog = string.Empty;
    }
    private static void CurrentDomain_ProcessExit(object sender, EventArgs e)
    {
        WriteLog("Процесс остановлен");
    }

    public static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        try
        {
            ASHK.AutoRailScales.AssemblyInfo assemblyInfo = new AssemblyInfo();
            int exitCode;
            using (var process = new Process())
            {
                var startInfo = process.StartInfo;
                startInfo.FileName = "sc";
                startInfo.WindowStyle = ProcessWindowStyle.Hidden;

                startInfo.Arguments = string.Format("failure \"{0}\" reset= 0 actions= restart/2000", assemblyInfo.Product);

                process.Start();
                process.WaitForExit();

                exitCode = process.ExitCode;

                process.Close();
            }

            if (exitCode != 0)
                throw new InvalidOperationException();
            var a = (Exception)e.ExceptionObject;
            string[] str = a.StackTrace is null ? new string[] { "Нет значения" } : a.StackTrace.Split('\n');
            WriteLog(a.Message + " место ошибки:" + str[str.Length - 1], true);
            
            
        }
        catch (Exception ex)
        {
        }
    }

}
