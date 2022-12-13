﻿namespace FlaNium.Desktop.Driver.Automator
{
    #region using

    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;


    #endregion

    internal class Capabilities
    {
        #region Constructors and Destructors

        internal Capabilities()
        {
            this.App = string.Empty;
            this.Arguments = string.Empty;
            this.LaunchDelay = 0;
            this.DebugConnectToRunningApp = false;
            this.InnerPort = 9998;
            this.ProcessName = string.Empty;
            this.ResponseTimeout = 300000;
        }

        #endregion

        #region Public Properties

        [JsonProperty("app")]
        public string App { get; set; }

        [JsonProperty("appTopLevelWindow")]
        public string AppTopLevelWindow { get; set; }

        [JsonProperty("appArguments")]
        public string Arguments { get; set; }

        [JsonProperty("mainWindowClassName")]
        public string MainWindowClassName { get; set; }

        [JsonProperty("debugConnectToRunningApp")]
        public bool DebugConnectToRunningApp { get; set; }

        [JsonProperty("innerPort")]
        public int InnerPort { get; set; }
        
        [JsonProperty("ms:waitForAppLaunch")]
        public int LaunchDelay { get; set; }

        [JsonProperty("processName")]
        public string ProcessName { get; set; }

        [JsonProperty("responseTimeout")]
        public int ResponseTimeout { get; set; }

        #endregion

        #region Public Methods and Operators

        public static Capabilities CapabilitiesFromJsonString(string jsonString)
        {
            var capabilities = JsonConvert.DeserializeObject<Capabilities>(
                jsonString, 
                new JsonSerializerSettings
                    {
                        Error =
                            delegate(object sender, ErrorEventArgs args)
                                {
                                    args.ErrorContext.Handled = true;
                                }
                    });

            return capabilities;
        }

        public string CapabilitiesToJsonString()
        {
            return JsonConvert.SerializeObject(this);
        }

        #endregion
    }
}
