﻿
namespace FlaNium.Desktop.Driver.CommandExecutors.Elements.Grid
{
    using global::FlaUI.Core.AutomationElements;
    using FlaNium.Desktop.Driver.FlaUI;
    using FlaNium.Desktop.Driver.Common;
    using FlaNium.Desktop.Driver.Exceptions;

    class GridRowFindCellByTextExecutor : CommandExecutorBase
    {
        #region Methods

        protected override string DoImpl()
        {
            var registeredKey = this.ExecutedCommand.Parameters["ID"].ToString();
            
            var value = this.ExecutedCommand.Parameters["value"].ToString();

            var element = this.Automator.ElementsRegistry.GetRegisteredElement(registeredKey, this.ExecutedCommand.SessionId);

            GridRow gridRow = element.FlaUIElement.AsGridRow();

            var result = gridRow.FindCellByText(value);

            if (result == null)
            {
                throw new AutomationException("Element cannot be found", ResponseStatus.NoSuchElement);
            }

            var itemRegisteredKey = this.Automator.ElementsRegistry.RegisterElement(new FlaUIDriverElement(result), this.ExecutedCommand.SessionId);

            var registeredObject = new JsonElementContent(itemRegisteredKey);

            return this.JsonResponse(ResponseStatus.Success, registeredObject);
        }

        #endregion
    }
}
