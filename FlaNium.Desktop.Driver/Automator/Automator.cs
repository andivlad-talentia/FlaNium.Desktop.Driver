using FlaUI.Core;

namespace FlaNium.Desktop.Driver.Automator
{
    using System;
    #region using

    using System.Collections.Generic;
      
    using FlaNium.Desktop.Driver.Input;

    #endregion

    internal class Automator
    {
        #region Static Fields

        private static readonly object LockObject = new object();

        private static volatile Automator instance;

        #endregion

        #region Constructors and Destructors

        public Automator()
        {
            this.ElementsRegistry = new ElementsRegistry();
            this.FlaNiumKeyboard = new FlaNiumKeyboard();
        }

        #endregion

        #region Public Properties

        public Capabilities ActualCapabilities { get; set; }

        public Application Application { get; set; }

        public ElementsRegistry ElementsRegistry { get; private set; }

        public FlaNiumKeyboard FlaNiumKeyboard { get; set; }

        #endregion

        #region Public Methods and Operators

        public static T GetValue<T>(IReadOnlyDictionary<string, object> parameters, string key) where T : class
        {
            object valueObject;
            parameters.TryGetValue(key, out valueObject);

            return valueObject as T;
        }

        public static Automator GetInstance()
        {
            if (instance == null)
            {
                lock (LockObject)
                {
                    if (instance == null)
                        instance = new Automator();
                }
            }
            return instance;
        }

        #endregion
    }
}
