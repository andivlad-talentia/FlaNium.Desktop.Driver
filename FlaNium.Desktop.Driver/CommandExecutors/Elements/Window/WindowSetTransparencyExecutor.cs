﻿
using FlaUI.Core.AutomationElements;

namespace FlaNium.Desktop.Driver.CommandExecutors.Elements.Window
{
    class WindowSetTransparencyExecutor : CommandExecutorBase
    {
        #region Methods

        protected override string DoImpl()
        {
            var registeredKey = this.ExecutedCommand.Parameters["ID"].ToString();

            var alpha = this.ExecutedCommand.Parameters["index"].ToString();

            var element = this.Automator.ElementsRegistry.GetRegisteredElement(registeredKey, this.ExecutedCommand.SessionId);

            var window = element.FlaUIElement.AsWindow();

            window.SetTransparency(byte.Parse(alpha));

            return this.JsonResponse();
        }

        #endregion
    }
}
