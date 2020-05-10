

namespace Common
{
    using System;
    using System.Data;

    /// <summary>
    /// Defines the interface for IDatabaseUnitOfWork
    /// </summary>
    public interface IDatabaseUnitOfWork : IDisposable
    {
        IDbConnection Connection { get; }
        void BeginTransaction(IsolationLevel level = IsolationLevel.ReadCommitted);
        void CommitTransaction();
        void RollbackTransaction();
    }
}
