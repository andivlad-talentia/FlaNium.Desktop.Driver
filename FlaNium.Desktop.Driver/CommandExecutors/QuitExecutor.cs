namespace FlaNium.Desktop.Driver.CommandExecutors
{
    internal class QuitExecutor : CommandExecutorBase
    {
        #region Methods

        protected override string DoImpl()
        {
            if (!this.Automator.ActualCapabilities.DebugConnectToRunningApp)
            {
                this.DriverManager?.CloseDriver();
                this.Automator.ElementsRegistry.Clear();
            }

            return this.JsonResponse();
        }

        #endregion
    }
}
