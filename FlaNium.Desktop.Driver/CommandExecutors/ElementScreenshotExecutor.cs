﻿namespace FlaNium.Desktop.Driver.CommandExecutors
{

    using System;
    using System.Drawing.Imaging;
    using System.IO;
    using global::FlaUI.Core.Capturing;
    using FlaNium.Desktop.Driver.Common;

    internal class ElementScreenshotExecutor : CommandExecutorBase
    {
       
        protected override string DoImpl()
        {
            var elementId = this.ExecutedCommand.Parameters["ID"].ToString();
            var imageFormatStr = this.ExecutedCommand.Parameters["format"].ToString();
            var foreground = Boolean.Parse(this.ExecutedCommand.Parameters["foreground"].ToString());

            var element = this.Automator.ElementsRegistry.GetRegisteredElement(elementId, this.ExecutedCommand.SessionId);

            ImageFormat imageFormat = ImFormat.GetImageFormat(imageFormatStr);

            MemoryStream memoryStream = new MemoryStream();
            CaptureImage captureImage;

            if (foreground)
            {
                captureImage = Capture.Element(element.FlaUIElement);
            }
            else
            {
                captureImage = ElementCapture.CaptureImageOfElement(element.FlaUIElement);
            }

            captureImage.Bitmap.Save((Stream)memoryStream, imageFormat); 

            return this.JsonResponse(ResponseStatus.Success, (object)Convert.ToBase64String(memoryStream.ToArray()));
        }


    }
}
