namespace OXStack.Config
{
    /// <summary>
    /// Standard database config
    /// </summary>
    public interface IDataConnectorConfig
    {
        string ConnectionString { get; }
    }
}
