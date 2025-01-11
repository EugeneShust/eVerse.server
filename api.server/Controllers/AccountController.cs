using API.Server.Models;
using API.Server.Repository;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using FirebaseAdmin.Auth;
using Shared.Protocol.Requests;
using FirebaseAdmin;
using System.Numerics;
using Internal.Extentions;
using Microsoft.AspNetCore.Authorization;
using API.Server.Internal.Utils;
using static Google.Apis.Requests.BatchRequest;

namespace api.server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController(IAccountRepository repository, IUserRepository userRepository, FirebaseApp firebaseApp, JwtSecurityService jwtService) : ControllerBase
    {
        private readonly IAccountRepository _accounts = repository;
        private readonly IUserRepository _users = userRepository;
        private readonly FirebaseApp _firebaseApp = firebaseApp;
        private readonly JwtSecurityService _jwtService = jwtService;

        [AllowAnonymous]
        [HttpPost]
        [Route("register")]
        [ProducesResponseType(typeof(RegistrationResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(StatusCodeResult), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(StatusCodeResult), (int)HttpStatusCode.UpgradeRequired)]
        [ProducesResponseType(typeof(StatusCodeResult), (int)HttpStatusCode.ServiceUnavailable)]
        public async Task<ActionResult<RegistrationResponse>> Registration([FromBody] RegistrationRequest request)
        {
            Account? account = null;

            try
            {
                var token = HttpContext.GetFirebaseToken();
                
                if (token == null)
                {
                    return Unauthorized("Firebase user not found");
                }

                try
                {
                    account = await _accounts.FindOneAsync(x => x.FirebaseUid == token.Uid);

                    if (account == null)
                    {
                        var now = DateTime.UtcNow;
                        account = new Account { FirebaseUid = token.Uid, CreatedAt = now };

                        await _accounts.CreateAsync(account);
                    }
                }
                catch (Exception accountError)
                {
                    Console.WriteLine($"Error handling account: {accountError}");
                    return StatusCode((int)HttpStatusCode.ServiceUnavailable, "Error processing account");
                }

                // Here, an account creation event should be initiated and sent over the service bus(Rabbit or etc).
                var auth = FirebaseAuth.GetAuth(_firebaseApp);
                var firebaseUser = await auth.GetUserAsync(token.Uid);

                var user = new User
                {
                    Id = account.Id,
                    Email = firebaseUser.Email,
                    Phone = firebaseUser.PhoneNumber,
                    Avatar = firebaseUser.PhotoUrl,
                    DisplayName = firebaseUser.DisplayName,
                };

                await _users.CreateAsync(user);
                
                var response = new RegistrationResponse { UserId = user.Id };
                
                return CreatedAtAction(nameof(Authentication), response);
            }
            catch (Exception error)
            {
                Console.WriteLine($"Unexpected error occurred: {error}");
                return StatusCode((int)HttpStatusCode.InternalServerError, "An unexpected error occurred");
            }
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("authentication")]
        public async Task<IActionResult> Authentication(string? userId)
        {
            var token = HttpContext.GetFirebaseToken();

            if (token == null)
            {
                return Unauthorized("Firebase user not found");
            }

            var account = await _accounts.GetByFirebaseUidAsync(token.Uid);

            if (account == null) return NotFound();

            var user = await _users.FindOneAsync(x => x.Id == account.Id);


            if (account.Id != userId)
            {
                Console.WriteLine($"{nameof(Authentication)}: {userId}");
                //return Unauthorized("Incorrect userId");
            }

            var jwt = _jwtService.GenerateJwt(user.Id);

            return Ok(new { token = jwt, userId = user.Id, location = $"/user/profile" });
        }
    }
}