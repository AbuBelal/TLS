namespace APIServerLib.ProgramSettings
{
    public static class AllowCors
    {
        const string CorsPolicyName = "AllowManApp";
        public static WebApplicationBuilder AddCorsPolicy(this WebApplicationBuilder builder)
        {
            string[] WebAppUrl = builder.Configuration.GetSection("CorsOrigins").Get<string[]>();

            builder.Services.AddCors(options =>
            {
                options.AddPolicy(CorsPolicyName,
                    policy => policy.WithOrigins(WebAppUrl!) // تأكد من مطابقة بورت الـ Client
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials());
            });

            return builder;
        }

        public static WebApplication UseCorsPolicy(this WebApplication app)
        {
            app.UseCors(CorsPolicyName);
            return app;
        }

    }
}
