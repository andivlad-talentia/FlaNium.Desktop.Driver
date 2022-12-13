namespace FlaNium.Desktop.Driver
{
    #region using

    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Threading;
    using FlaNium.Desktop.Driver.FlaUI;
    using FlaNium.Desktop.Driver.Common;
    using FlaNium.Desktop.Driver.Exceptions;

    #endregion

    internal class ElementsRegistry
    {
        #region Static Fields

        private static int safeInstanceCount;

        #endregion

        #region Fields

        private readonly Dictionary<string, Dictionary<string, FlaUIDriverElement>> registeredElements;

        #endregion

        #region Constructors and Destructors

        public ElementsRegistry()
        {
            this.registeredElements = new Dictionary<string, Dictionary<string, FlaUIDriverElement>>();
        }

        #endregion

        #region Public Methods and Operators

        public void Clear(string sessionId)
        {
            if (this.registeredElements.ContainsKey(sessionId))
            {
                this.registeredElements[sessionId].Clear();
                this.registeredElements.Remove(sessionId);
            }
        }

        /// <summary>
        /// Returns CruciatusElement registered with specified key if any exists. Throws if no element is found.
        /// </summary>
        /// <exception cref="AutomationException">
        /// Registered element is not found or element has been garbage collected.
        /// </exception>
        public FlaUIDriverElement GetRegisteredElement(string registeredKey, string sessionId)
        {
            var element = this.GetRegisteredElementOrNull(registeredKey, sessionId);
            if (element != null)
            {
                return element;
            }

            throw new AutomationException("Stale element reference", ResponseStatus.StaleElementReference);
        }

        public string RegisterElement(FlaUIDriverElement element, string sessionId)
        {
            Interlocked.Increment(ref safeInstanceCount);

            var registeredKey = element.GetHashCode() + "-"
                             + safeInstanceCount.ToString(string.Empty, CultureInfo.InvariantCulture);

            EnsureSessionRegistry(sessionId);
            this.registeredElements[sessionId].Add(registeredKey, element);

            return registeredKey;

        }

        public IEnumerable<string> RegisterElements(IEnumerable<FlaUIDriverElement> elements, string sessionId)
        {
            EnsureSessionRegistry(sessionId);
            return elements.Select(e => this.RegisterElement(e, sessionId));
        }

        #endregion

        #region Methods

        internal FlaUIDriverElement GetRegisteredElementOrNull(string registeredKey, string sessionId)
        {
            FlaUIDriverElement element;
            if (this.registeredElements.ContainsKey(sessionId))
            {
                this.registeredElements[sessionId].TryGetValue(registeredKey, out element);
                return element;
            }
            return null;
        }

        private void EnsureSessionRegistry(string sessionId)
        {
            // Initialize session specific registry
            if (!this.registeredElements.ContainsKey(sessionId))
            {
                this.registeredElements[sessionId] = new Dictionary<string, FlaUIDriverElement>();
            }
        }

        #endregion
    }
}
