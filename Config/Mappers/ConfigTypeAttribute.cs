using System;

namespace OXStack.Config.Mappers
{
    /// <summary>
    /// Types of config
    /// </summary>
    public enum ConfigTypes
    {
        DataBaseConfig,
        // append etc.
    };

    /// <summary>
    /// Type of config
    /// </summary>
    public class ConfigTypeAttribute : Attribute
    {
        /// <summary>
        /// Desired type..
        /// </summary>
        public ConfigTypes ConfigType { get; set; }
        /// <summary>
        /// Map this property with this type.
        /// </summary>
        /// <param name="confType"></param>
        public ConfigTypeAttribute(ConfigTypes confType)
        {
            ConfigType = confType;
        }
    }
}
