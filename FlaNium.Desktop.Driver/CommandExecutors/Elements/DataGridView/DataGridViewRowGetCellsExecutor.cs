﻿
namespace FlaNium.Desktop.Driver.CommandExecutors.Elements.DataGridView
{
    using System;
    using System.Linq;
    using global::FlaUI.Core.AutomationElements;
    using FlaNium.Desktop.Driver.FlaUI;
    using FlaNium.Desktop.Driver.Common;

    class DataGridViewRowGetCellsExecutor : CommandExecutorBase
    {
        #region Methods

        protected override string DoImpl()
        {
            var registeredKey = this.ExecutedCommand.Parameters["ID"].ToString();

            var element = this.Automator.ElementsRegistry.GetRegisteredElement(registeredKey, this.ExecutedCommand.SessionId);

            DataGridViewRow item = (DataGridViewRow)element.FlaUIElement;

            var cells = item.Cells;

            var flaUiDriverElementList = cells
                .Select<AutomationElement, FlaUIDriverElement>((Func<AutomationElement, FlaUIDriverElement>)(x => new FlaUIDriverElement(x)))
                .ToList<FlaUIDriverElement>();

            var registeredKeys = Automator.ElementsRegistry.RegisterElements(flaUiDriverElementList, this.ExecutedCommand.SessionId);

            var registeredObjects = registeredKeys.Select(e => new JsonElementContent(e));

            return this.JsonResponse(ResponseStatus.Success, registeredObjects);
        }

        #endregion
    }
}
