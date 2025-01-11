using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace Internal.Middleware
{
    public class JwtAuthenticationMiddleware(RequestDelegate next, IConfiguration configuration)
    {
        private readonly RequestDelegate _next = next;
        private readonly string _secretKey = configuration["Jwt:Secret"];

        public async Task InvokeAsync(HttpContext context)
        {
            var endpoint = context.GetEndpoint();

            if (endpoint?.Metadata?.GetMetadata<IAllowAnonymous>() != null)
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
            var tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                var key = Encoding.ASCII.GetBytes(_secretKey);
                var claimsPrincipal = tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                }, out _);

                Console.WriteLine(claimsPrincipal);
                context.User = claimsPrincipal;

                await _next(context);
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync($"Invalid token: {ex.Message}");
            }
        }
    }
}
