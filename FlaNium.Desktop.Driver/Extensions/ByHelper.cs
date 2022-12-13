﻿
namespace FlaNium.Desktop.Driver.Extensions
{
    #region using

    using System;
    using global::FlaUI.Core.Conditions;
    using global::FlaUI.UIA2;
    using global::FlaUI.UIA3;
   
    

    #endregion

    public static class ByHelper
    {
        #region Public Methods and Operators

        public static ConditionBase GetStrategy(string strategy, string value)
        {
            switch (strategy)
            {
                case "id":
                case "accessibility id":
                    return new ConditionFactory(new UIA2PropertyLibrary()).ByAutomationId(value);
                case "name":
                   return new ConditionFactory(new UIA2PropertyLibrary()).ByName(value);
                case "class name":
                    return new ConditionFactory(new UIA2PropertyLibrary()).ByClassName(value);
                case "tag name":
                    return new ConditionFactory(new UIA2PropertyLibrary()).ByLocalizedControlType(value);

                default:
                    throw new NotImplementedException(
                        string.Format("'{0}' is not valid or implemented searching strategy.", strategy));
            }
        }

        #endregion
    }
}
