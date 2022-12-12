using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Instrumentation;
using System.Text;
using System.Threading.Tasks;

namespace FlaNium.Desktop.Driver.FlaUI
{
    internal class DriverManagerCollection
    {
        private static DriverManagerCollection _instance;
        public static DriverManagerCollection Instance => _instance ?? (_instance = new DriverManagerCollection());

        private Dictionary<string, DriverManager> _drivers = new Dictionary<string, DriverManager>();

        public DriverManager this[string sessionId]
        {
            get => _drivers.ContainsKey(sessionId) ? _drivers [sessionId] : null;
            set
            {
                if (value == null && _drivers.ContainsKey(sessionId))
                    _drivers.Remove(sessionId);
                else
                    _drivers[sessionId] = value;
            }
        }
    }
}
