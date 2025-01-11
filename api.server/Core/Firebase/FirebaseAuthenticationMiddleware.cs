using FirebaseAdmin.Auth;
using Microsoft.AspNetCore.Authorization;

namespace API.Server.Core.Firebase
{
    public class FirebaseAuthenticationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly FirebaseAuth _firebaseAuth;

        public FirebaseAuthenticationMiddleware(RequestDelegate next, FirebaseAuth firebaseAuth)
        {
            _next = next;
            _firebaseAuth = firebaseAuth;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var endpoint = context.GetEndpoint();

            if (endpoint?.Metadata?.GetMetadata<IAllowAnonymous>() == null)
            {
                await _next(context);
                return;
            }

            var authorizationHeader = context.Request.Headers["Authorization"].ToString();
            if (string.IsNullOrEmpty(authorizationHeader) || !authorizationHeader.StartsWith("Bearer "))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Missing or invalid Authorization header");
                return;
            }

            var token = authorizationHeader["Bearer ".Length..].Trim();

            try
            {
                context.Items["FirebaseToken"] = await _firebaseAuth.VerifyIdTokenAsync(token);

                await _next(context);
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync($"Invalid Firebase token: {ex.Message}");
            }
        }
    }
}
