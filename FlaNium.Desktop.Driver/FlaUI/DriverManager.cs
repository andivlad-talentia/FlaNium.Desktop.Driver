using System;
using System.Collections.Generic;
using System.Linq;

using System.Threading.Tasks;

using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Input;
using FlaUI.Core.Tools;
using FlaUI.UIA2;


using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;

using System.Threading;
using System.Xml.Linq;
using OpenQA.Selenium;


namespace FlaNium.Desktop.Driver.FlaUI
{
    class DriverManager
    {
        private static TimeSpan implicitTimeout = new TimeSpan(0, 0, 30);       
        private static readonly string ROOT_APP_NAME = "root";

        public static TimeSpan ImplicitTimeout
        {
            get => DriverManager.implicitTimeout;
            set
            {
                DriverManager.implicitTimeout = value;
                Console.WriteLine(string.Format(" setImplicitTimeout: {0}", (object)DriverManager.ImplicitTimeout));
            }
        }

        public static void refreshImplicitTimeout() => DriverManager.ImplicitTimeout = TimeSpan.FromSeconds(30.0);

        public static string AutomationIdWindowForIgnore { get; } = "ProgressWindowForm";

        public static AutomationBase Automation { get; } = (AutomationBase)new UIA2Automation();

        public Application Application { get; private set; }
        public string ApplicationName { get; private set; }
        public Window RootElement { get; private set; }
        public string SessionId { get; set; }

        private DriverManager(string sessionId)
        {
            if (string.IsNullOrEmpty(sessionId))
            {
                throw new ArgumentNullException(nameof(sessionId));
            }
            SessionId = sessionId;
            DriverManagerCollection.Instance[SessionId] = this;
        }

        public void CloseDriver(bool isDebug = false)
        {
            CloseApplication();
            DriverManagerCollection.Instance[SessionId] = null;
        }

        public void CloseApplication()
        {
            RootElement = null;
            Application?.Close();
            Application = null;
            GC.Collect();
        }

        public Window[] GetWindows()
        {
            try
            {
                Window mainWindow = Application.GetMainWindow(DriverManager.Automation);
                return new List<Window>() 
                {
                    mainWindow
                }
                .Union<Window>((IEnumerable<Window>)mainWindow.ModalWindows).ToArray();
            }
            catch
            {
                return GetAllProcessWindows();
            }
        }

        private Window[] GetAllProcessWindows()
        {
            Window[] allTopLevelWindows = Application.GetAllTopLevelWindows(DriverManager.Automation);
            return ((IEnumerable<Window>)allTopLevelWindows).Union<Window>(((IEnumerable<Window>)allTopLevelWindows).SelectMany<Window, Window>((Func<Window, IEnumerable<Window>>)(w => (IEnumerable<Window>)w.ModalWindows))).Where<Window>((Func<Window, bool>)(x => x.Properties.AutomationId?.ValueOrDefault != DriverManager.AutomationIdWindowForIgnore)).ToArray<Window>();
        }

        public Window GetRootElement()
        {
            if (RootElement == null)
            {
                RootElement = Retry.While<Window>((Func<Window>)(() => Application.GetMainWindow(DriverManager.Automation)), (Func<Window, bool>)(x =>
                {
                    try
                    {
                        return x.Properties.AutomationId?.ValueOrDefault == DriverManager.AutomationIdWindowForIgnore;
                    }
                    catch
                    {
                        return true;
                    }
                }), new TimeSpan?(TimeSpan.FromMinutes(5.0))).Result;

                if (RootElement.FindAllDescendants().Length == 0)
                {
                    RootElement = Application.GetAllTopLevelWindows(DriverManager.Automation)[0];
                }

            }
            return RootElement;
        }

        public static void StartApp(string appPath, string arguments, string sessionId, int launchDelay, string mainWindowClassName)
        {
            appPath = appPath.Replace("\\\\", "\\");
            string name = appPath.Substring(appPath.LastIndexOf('\\') + 1);

            var driverManager = new DriverManager(sessionId);
            if (appPath.ToLower() == ROOT_APP_NAME)
            {
                driverManager.RootElement = Automation.GetDesktop().AsWindow();
                driverManager.RootElement.FindAllChildren();
                launchDelay = 0;
            } 
            else if (!File.Exists(appPath))
            {
                // WindowsStore
                try
                {
                    driverManager.Application = Application.LaunchStoreApp(name);
                    driverManager.Application.WaitWhileBusy(new TimeSpan?(TimeSpan.FromSeconds(launchDelay)));
                }

                catch (Win32Exception)
                {
                    driverManager.CloseDriver();
                }

                catch (Exception)
                {
                    throw new FileNotFoundException(appPath);
                }
            }
            else
            {
                Directory.SetCurrentDirectory(Path.GetDirectoryName(appPath));
                ProcessStartInfo processStartInfo = new ProcessStartInfo()
                {
                    FileName = appPath,
                    Arguments = arguments
                };
                try
                {
                    driverManager.Application = Application.Launch(processStartInfo);
                    if (launchDelay == 0)
                    {
                        return;
                    }
                    RetrySettings retrySettings = new RetrySettings()
                    {
                        IgnoreException = true,
                        Timeout = TimeSpan.FromSeconds(launchDelay),
                        ThrowOnTimeout = true,
                        TimeoutMessage = "Unable to start application " + name
                    };

                    Retry.While(() =>
                    {
                        try
                        {
                            driverManager.Application.WaitWhileBusy(new TimeSpan?(TimeSpan.FromSeconds(5)));

                            if (mainWindowClassName != null)
                            {
                                var mainWindow = driverManager.Application.GetMainWindow(Automation);
                                if (mainWindow.ClassName.ToLower() == mainWindowClassName.ToLower())
                                {
                                    driverManager.RootElement = mainWindow;
                                    mainWindow.FindAllChildren();
                                    return true;
                                }
                                else
                                {
                                    return false;
                                }
                            }
                            else
                            {
                                Task.Delay(launchDelay * 1000).Wait();
                                return !driverManager.Application.HasExited;
                            }
                        }
                        catch
                        {
                            if (driverManager.Application.HasExited)
                            {
                                driverManager.Application = Application.AttachOrLaunch(processStartInfo);
                                driverManager.Application.WaitWhileBusy(new TimeSpan?(TimeSpan.FromSeconds(5)));
                            }
                            return false;
                        }
                    },
                    (rv) => rv == false,
                    retrySettings);
                }
                catch (Win32Exception)
                {
                    driverManager.CloseDriver();
                }
            }
            if (null == driverManager.RootElement && null != driverManager?.Application?.MainWindowHandle)
            {
                var mainWindow = driverManager.Application.GetMainWindow(Automation);
                driverManager.RootElement = mainWindow;
                mainWindow.FindAllChildren();
            }
        }

        public static void AttachToWindowHandle(IntPtr handle, string sessionId, int launchDelay)
        {
            RetrySettings retrySettings = new RetrySettings()
            {
                IgnoreException = true,
                Timeout = TimeSpan.FromSeconds(launchDelay),
                ThrowOnTimeout = true,
                TimeoutMessage = "Unable to attach to window with handle " + handle.ToString()
            };
            var driverManager = new DriverManager(sessionId);
            Retry.WhileException(() =>
            {
                var mainWindow = Automation.FromHandle(handle).AsWindow();
                driverManager.Application = Application.Attach(mainWindow.Properties.ProcessId);
                driverManager.RootElement = mainWindow;
                mainWindow.FindAllChildren();

            },
            TimeSpan.FromSeconds(launchDelay));
        }

        public void Click(Point p)
        {
            (GetRootElement() as Window)?.SetForeground();
            Mouse.Position = p;
            Mouse.Click(MouseButton.Left);
        }

        public async Task<Task<RetryResult<Window>>> FindWindow(
          string title,
          TimeSpan timeout)
        {
            Func<RetryResult<Window>> function1 = (Func<RetryResult<Window>>)(() => Retry.While<Window>((Func<Window>)(() => ((IEnumerable<Window>)GetAllProcessWindows()).FirstOrDefault<Window>((Func<Window, bool>)(x => x.Title == title))), (Func<Window, bool>)(x => x == null), new TimeSpan?(timeout)));
            Func<RetryResult<Window>> function2 = (Func<RetryResult<Window>>)(() => Retry.While<Window>((Func<Window>)(() => ((IEnumerable<Window>)GetWindows()).FirstOrDefault<Window>((Func<Window, bool>)(x => x.Title == title))), (Func<Window, bool>)(x => x == null), new TimeSpan?(timeout)));
            return await Task.WhenAny<RetryResult<Window>>(Task.Run<RetryResult<Window>>(function1), Task.Run<RetryResult<Window>>(function2));
        }

        public void SwitchWindow(string title)
        {
            long timeMilliseconds = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            Task<RetryResult<Window>> task = (Task<RetryResult<Window>>)null;
            while ((double)timeMilliseconds > (double)DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - DriverManager.ImplicitTimeout.TotalMilliseconds)
            {
                if (title == "")
                {
                    RootElement = Application.GetMainWindow(DriverManager.Automation);
                    RootElement.SetForeground();
                    return;
                }
                Task<Task<RetryResult<Window>>> window = FindWindow(title, DriverManager.ImplicitTimeout);
                window.Wait();
                task = !window.IsFaulted ? window.Result : throw window.Exception;
                if (task.IsFaulted)
                    Thread.Sleep(1000);
                else
                    break;
            }
            if (task.IsFaulted)
                throw new Exception(string.Format("Fail to get Window {0}", (object)title), (Exception)task.Exception);
            RootElement = task.Result.Result;
            RootElement.SetForeground();
        }

        public void PrintTimestamp(string comment)
        {
        }

        public string DownloadTempFile(string filename)
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory.Replace("\\", "\\\\").TrimEnd('\\', '/'), filename);
            if (!File.Exists(path))
            {
                Logger.Error(string.Format("File does not exist {0}", (object)path));
                return (string)null;
            }
            byte[] inArray = File.ReadAllBytes(path);
            File.Delete(path);
            return Convert.ToBase64String(inArray);
        }
    }
}
