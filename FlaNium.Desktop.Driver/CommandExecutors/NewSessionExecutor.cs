namespace FlaNium.Desktop.Driver.CommandExecutors
{
    using System.Threading;
    using Newtonsoft.Json;
    using FlaNium.Desktop.Driver.Automator;
    using FlaNium.Desktop.Driver.Common;
    using FlaNium.Desktop.Driver.FlaUI;
    using FlaNium.Desktop.Driver.Input;
    using System;

    internal class NewSessionExecutor : CommandExecutorBase
    {

        protected override string DoImpl()
        {            
            var serializedCapability = JsonConvert.SerializeObject(this.ExecutedCommand.Parameters["desiredCapabilities"]);

            this.Automator.ActualCapabilities = Capabilities.CapabilitiesFromJsonString(serializedCapability);

            this.InitializeApplication();

            FlaNiumKeyboard.SwitchInputLanguageToEng(); // Имеются проблемы ввода текста при активной русской раскладке. Добавлено переключение на английскую раскладку.

            return this.JsonResponse(ResponseStatus.Success, this.Automator.ActualCapabilities);
        }

        private void InitializeApplication()
        {
            var appPath = this.Automator.ActualCapabilities.App;
            var appArguments = this.Automator.ActualCapabilities.Arguments;
            var launchDelay = this.Automator.ActualCapabilities.LaunchDelay;
            var appWindow = this.Automator.ActualCapabilities.AppTopLevelWindow;
            var mainWindowClassName = this.Automator.ActualCapabilities.MainWindowClassName;

            if (appPath != null)
            {
                DriverManager.StartApp(appPath, appArguments, ExecutedCommand.SessionId, launchDelay, mainWindowClassName);
            }
            else if (appWindow != null)
            {
                DriverManager.AttachToWindowHandle(new IntPtr(Convert.ToInt32(appWindow, 16)), ExecutedCommand.SessionId);
            }
            else
            {
                throw new InvalidOperationException("Not sure what to start");
            }
        }

    }
}
