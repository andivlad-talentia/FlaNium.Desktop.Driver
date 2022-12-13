
using FlaNium.Desktop.Driver.Extensions;
using FlaNium.Desktop.Driver.FlaUI;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Definitions;
using System;

namespace FlaNium.Desktop.Driver.CommandExecutors.Elements.Window
{
    class MaximizeWindowExecutor : CommandExecutorBase
    {
        #region Methods

        protected override string DoImpl()
        {
            try
            {
                DriverManager.GetRootElement().Patterns.Window.Pattern.SetWindowVisualState(WindowVisualState.Maximized);
            }
            catch (InvalidOperationException)
            {
                PInvoke.User32.ShowWindow(DriverManager.RootElement.FrameworkAutomationElement.NativeWindowHandle, PInvoke.User32.WindowShowStyle.SW_SHOWMAXIMIZED);
            }

            return this.JsonResponse();
        }

        #endregion
    }
}
