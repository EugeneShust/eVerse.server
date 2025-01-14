using Core.Firebase;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace api.server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationsController : ControllerBase
    {
        private readonly FirebaseService _firebaseService;

        public NotificationsController(FirebaseService firebaseService)
        {
            _firebaseService = firebaseService;
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendNotification([FromBody] NotificationRequest request)
        {
            if (string.IsNullOrEmpty(request.Token))
                return BadRequest("Token is required");

            var response = await _firebaseService.SendNotificationAsync(request.Token, request.Title, request.Body);
            return Ok(new { Response = response });
        }
    }

    public class NotificationRequest
    {
        public string Token { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
    }
}
