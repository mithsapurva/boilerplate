
namespace Data
{
    using System;
    using System.Data;

    /// <summary>
    /// Defines an interface IConnectionWrapper
    /// </summary>
    public interface IConnectionWrapper : IDisposable
    {
        IDbConnection Connection { get; }
    }
}
