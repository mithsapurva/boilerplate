
namespace Data
{
    public interface IConnectionFactory
    {
        IConnectionWrapper GetConnection();
        bool ConnectionStatus();
    }
}
