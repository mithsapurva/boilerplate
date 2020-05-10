

namespace Authentication.API
{
    using Microsoft.AspNetCore.Mvc;

    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly ITokenBuilder tokenBuilder;

        public AuthenticationController(ApplicationDbContext context, ITokenBuilder tokenBuilder)
        {
            this.context = context;
            this.tokenBuilder = tokenBuilder;
        }

        [HttpGet]
        public string Get(string systemName)
        {
            return tokenBuilder.BuildToken(systemName);
        }
    }
}