﻿
using FlaUI.Core.AutomationElements;

namespace FlaNium.Desktop.Driver.CommandExecutors.Elements.Thumb
{
    class ThumbSlideHorizontallyExecutor : CommandExecutorBase
    {
        #region Methods

        protected override string DoImpl()
        {
            var registeredKey = this.ExecutedCommand.Parameters["ID"].ToString();

            var value = this.ExecutedCommand.Parameters["index"].ToString();

            var element = this.Automator.ElementsRegistry.GetRegisteredElement(registeredKey, this.ExecutedCommand.SessionId);

            var thumb = element.FlaUIElement.AsThumb();

            thumb.SlideHorizontally(int.Parse(value));

            return this.JsonResponse();
        }

        #endregion
    }
}
