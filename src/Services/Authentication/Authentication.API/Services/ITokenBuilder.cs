

namespace Authentication.API
{
    public interface ITokenBuilder
    {
        /// <summary>
        /// Defines a method to build/generate tokens based on the system name
        /// </summary>
        /// <param name="systemName">name of the system from where the request is coming</param>
        /// <returns>JWT token</returns>
        string BuildToken(string systemName);
    }
}
