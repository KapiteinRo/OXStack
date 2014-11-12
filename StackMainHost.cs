using System;
using System.Reflection;
using OXStack.Config;

namespace OXStack
{
    /// <summary>
    /// Main Base Class
    /// </summary>
    public class StackMainHost
    {
        private OXConfig _config = null;
        public OXConfig Config { get { return _config ?? (_config = new OXConfig()); } }

        public StackMainHost()
        {
            // load config like things
            // load dataconnectors
            foreach (PropertyInfo info in this.GetType().GetProperties())
            {
                Type t = info.GetType();
                // nahh, this for lator
            }
        }
    }
}
