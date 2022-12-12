
namespace FlaNium.Desktop.Driver.Common
{
    using System;
    #region

    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text.Json;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using NLog.Targets;
    using OpenQA.Selenium;
    using JsonSerializer = System.Text.Json.JsonSerializer;

    #endregion

    public class Command
    {
        public static readonly JsonSerializerOptions jsonLoadSettings = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };

        #region Fields

        private IDictionary<string, JToken> commandParameters = new JObject();

        #endregion

        #region Constructors and Destructors

        public Command(string name, IDictionary<string, JToken> parameters)
        {
            this.Name = name;
            if (parameters != null)
            {
                this.Parameters = parameters;
            }
        }

        public Command(string name, string jsonParameters)
            : this(name, string.IsNullOrEmpty(jsonParameters) ? null : JObject.Parse(jsonParameters))
        {
        }

        public Command(string name)
        {
            this.Name = name;
        }

        public Command()
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the command name
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets the parameters of the command
        /// </summary>
        [JsonProperty("parameters")]
        public IDictionary<string, JToken> Parameters
        {
            get
            {
                return this.commandParameters;
            }

            set
            {
                this.commandParameters = value;
            }
        }

        /// <summary>
        /// Gets the SessionID of the command
        /// </summary>
        [JsonProperty("sessionid")]
        public string SessionId { get; set; }

        internal void TrySetSessionIdFromParameters()
        {
            SessionId = Parameters.FirstOrDefault(kvp => kvp.Key.ToLower() == "sessionid").Value?.ToString();
        }

        #endregion
    }
}
