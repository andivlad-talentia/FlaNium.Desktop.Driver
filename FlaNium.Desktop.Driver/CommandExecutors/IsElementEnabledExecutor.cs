﻿namespace FlaNium.Desktop.Driver.CommandExecutors
{
    #region using

    using FlaNium.Desktop.Driver.Common;

    #endregion

    internal class IsElementEnabledExecutor : CommandExecutorBase
    {
        #region Methods

        protected override string DoImpl()
        {
            var registeredKey = this.ExecutedCommand.Parameters["ID"].ToString();

            var element = this.Automator.ElementsRegistry.GetRegisteredElement(registeredKey, this.ExecutedCommand.SessionId);

            return this.JsonResponse(ResponseStatus.Success, element.Properties.IsEnabled);
        }

        #endregion
    }
}
