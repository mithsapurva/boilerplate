
namespace Data
{
    /// <summary>
    /// Defines interface for IDapperUnitOfWork
    /// </summary>
    public interface IDapperUnitOfWork : IUnitOfWork
    {
        ISqlConnectionFactory SqlConnectionFactory { get; }
    }
}
