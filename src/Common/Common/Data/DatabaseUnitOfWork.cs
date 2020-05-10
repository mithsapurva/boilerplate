

namespace Common
{
    using System;
    using System.Data;

    /// <summary>
    /// Defines the class for DatabaseUnitOfWork
    /// </summary>
    public class DatabaseUnitOfWork : IDatabaseUnitOfWork, IDisposable
    {
        private readonly Func<IDbConnection> connFactory;
        private IDbConnection dbConnection;
        private IDbTransaction transaction;

        public DatabaseUnitOfWork(Func<IDbConnection> connFactory)
        {
            this.connFactory = connFactory ?? throw new ArgumentNullException(nameof(connFactory));
        }

        public IDbConnection Connection
        {
            get
            {
                if (dbConnection == null)
                {
                    dbConnection = connFactory();
                }
                return dbConnection;
            }
        }

        public void BeginTransaction(IsolationLevel level = IsolationLevel.ReadCommitted)
        {
            transaction = dbConnection.BeginTransaction(level);
        }

        public void CommitTransaction()
        {
            transaction.Commit();
        }

        public void RollbackTransaction()
        {
            transaction.Rollback();
        }

        public void Dispose()
        {
            if (transaction != null)
            {
                transaction.Dispose();
            }
            transaction = null;
            if (dbConnection != null)
            {
                dbConnection.Close();
                dbConnection.Dispose();
            }
            dbConnection = null;
        }
    }
}
