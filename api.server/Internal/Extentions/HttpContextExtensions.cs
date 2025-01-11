using FirebaseAdmin.Auth;

namespace Internal.Extentions
{
    // todo partial class
    public static class HttpContextExtensions
    {
        public static FirebaseToken? GetFirebaseToken(this HttpContext context)
        {
            return context.Items["FirebaseToken"] as FirebaseToken;
        }

        public static string? GetUserId(this HttpContext context)
        {
            if (context.User == null || !context.User.Identity.IsAuthenticated)
            {
                return null;
            }

            return context.User.FindFirst("id")?.Value;
        }
    }
}
