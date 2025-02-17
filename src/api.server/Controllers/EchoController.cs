using Microsoft.AspNetCore.Mvc;

namespace api.server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EchoController : ControllerBase
    {
        private readonly ILogger<EchoController> _logger;

        public EchoController(ILogger<EchoController> logger)
        {
            _logger = logger;
        }

        [HttpPost(Name = "PostEcho")]
        public ActionResult<Echo> Post([FromBody] Echo echo)
        {
            if (echo == null)
            {
                return BadRequest("Echo body is missing.");
            }

            return Ok(new Echo
            {
                Body = echo.Body,
            });
        }
    }
}
