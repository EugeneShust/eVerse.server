using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Google.Apis.Auth.OAuth2;

namespace Core.Firebase
{
    public static class StartupExtentions
    {
        public static IServiceCollection AddFirebaseApp(this IServiceCollection services, IConfiguration configuration)
        {
            var path = configuration.GetValue<string>("FirebaseConfigPath");

            services.AddSingleton<FirebaseApp>(sp =>
            {
                if (FirebaseApp.DefaultInstance == null)
                {
                    var path = sp.GetRequiredService<IConfiguration>().GetValue<string>("FirebaseConfigPath");
                    return FirebaseApp.Create(new AppOptions
                    {
                        Credential = GoogleCredential.FromFile(path)
                    });
                }

                return FirebaseApp.DefaultInstance;
            });

            services.AddSingleton(sp =>
            {
                var firebaseApp = sp.GetRequiredService<FirebaseApp>();
                return FirebaseAuth.GetAuth(firebaseApp);
            });


            return services;
        }
    }
}
