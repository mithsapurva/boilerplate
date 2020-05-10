
namespace Data
{
    /// <summary>
    /// Defines the interface for IBaseEntity
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    public interface IBaseEntity<TKey>
    {
        TKey Id { get; set; }
    }
}
