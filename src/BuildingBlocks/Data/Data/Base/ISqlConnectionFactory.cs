

namespace Data
{
    using System.Data;

    /// <summary>
    /// Defines the interface for ISqlConnectionFactory
    /// </summary>
    public interface ISqlConnectionFactory
    {
        IDbConnection GetOpenConnection();
    }
}
