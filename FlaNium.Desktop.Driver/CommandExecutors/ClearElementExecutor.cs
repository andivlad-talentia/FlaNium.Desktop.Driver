namespace FlaNium.Desktop.Driver.CommandExecutors
{
    internal class ClearElementExecutor : CommandExecutorBase
    {
        #region Methods

        protected override string DoImpl()
        {
            var registeredKey = this.ExecutedCommand.Parameters["ID"].ToString();

            var element = this.Automator.ElementsRegistry.GetRegisteredElement(registeredKey, this.ExecutedCommand.SessionId);
            element.Clear(DriverManager);

            return this.JsonResponse();
        }

        #endregion
    }
}
