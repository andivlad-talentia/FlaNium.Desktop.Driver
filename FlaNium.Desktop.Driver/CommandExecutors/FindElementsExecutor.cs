namespace FlaNium.Desktop.Driver.CommandExecutors
{
    using System;
    #region using

    using System.Linq;
    using global::FlaUI.Core.AutomationElements;
    using FlaNium.Desktop.Driver.Extensions;
    using FlaNium.Desktop.Driver.FlaUI;
    using FlaNium.Desktop.Driver.Common;
    using FlaNium.Desktop.Driver.Exceptions;

    #endregion

    internal class FindElementsExecutor : CommandExecutorBase
    {
        #region Methods

        protected override string DoImpl()
        {
            var searchValue = this.ExecutedCommand.Parameters["value"].ToString();
            var searchStrategy = this.ExecutedCommand.Parameters["using"].ToString();


            AutomationElement rootElement = DriverManager.GetRootElement();
            AutomationElement[] elements;

            if (searchStrategy.Equals("xpath"))
            {
                if (searchValue.StartsWith("#"))
                {
                    searchValue = searchValue.TrimStart('#');
                    rootElement = rootElement.Automation.GetDesktop();
                }

                elements = ByXpath.FindAllByXPath(searchValue, rootElement);
            }
            else
            {
                var condition = ByHelper.GetStrategy(searchStrategy, searchValue);

                // For the root session don't search everything as it will timeout
                elements = (DriverManager.Application == null) ?
                    rootElement.FindAllChildren(condition) :
                    rootElement.FindAllDescendants(condition);
            }

            if (elements == null)
            {
                throw new AutomationException("Element cannot be found", ResponseStatus.NoSuchElement);
            }

            
            var flaUiDriverElementList = elements
                .Select<AutomationElement, FlaUIDriverElement>((Func<AutomationElement, FlaUIDriverElement>)(x => new FlaUIDriverElement(x)))
                .ToList<FlaUIDriverElement>();

            var registeredKeys = this.Automator.ElementsRegistry.RegisterElements(flaUiDriverElementList, this.ExecutedCommand.SessionId);

            var registeredObjects = registeredKeys.Select(e => new JsonElementContent(e));
            return this.JsonResponse(ResponseStatus.Success, registeredObjects);
        }

        #endregion
    }
}
