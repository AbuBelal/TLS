using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;
using Refit;
using TLSClientSharedLib.Helpers;
using TLSClientSharedLib.Services.Apis;
using TLSWeb.Identity;



namespace TLSWeb.Helpers
{
    public static class AddAppServices
    {
        // دالة موحدة لإضافة الخدمات المشتركة
        public static void AddCommonServices(this WebAssemblyHostBuilder builder)
        {
            builder.Services.AddTransient<CookieHandler>();

            builder.Services.AddAuthorizationCore();

            builder.Services.AddScoped<AuthenticationStateProvider, CookieAuthenticationStateProvider>();

            builder.Services.AddScoped(sp => (IAccountManagement)sp.GetRequiredService<AuthenticationStateProvider>());

            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
            
           
            builder.Services.AddMudServices();

            builder.Services.AddHttpClient("Auth", options =>
                options.BaseAddress = new Uri(ApiUrls.BaseUrl)
            )
            .AddHttpMessageHandler<CookieHandler>();

            builder.Services.AddRefitClient<IUserApi>()
               .ConfigureHttpClient(c =>
               {
                   c.BaseAddress = new Uri(ApiUrls.BaseUrl);
               }).AddHttpMessageHandler<CookieHandler>();

            builder.Services.AddRefitClient<IEmployeeApi>()
                .ConfigureHttpClient(c =>
                {
                    c.BaseAddress = new Uri(ApiUrls.BaseUrl);
                }).AddHttpMessageHandler<CookieHandler>();

            builder.Services.AddRefitClient<IStudentApi>()
                .ConfigureHttpClient(c =>
                {
                    c.BaseAddress = new Uri(ApiUrls.BaseUrl);
                }).AddHttpMessageHandler<CookieHandler>();

            builder.Services.AddRefitClient<IAccountApi>()
               .ConfigureHttpClient(c =>
               {
                   c.BaseAddress = new Uri(ApiUrls.BaseUrl);
               }).AddHttpMessageHandler<CookieHandler>();

            builder.Services.AddRefitClient<IRolesApi>()
               .ConfigureHttpClient(c =>
               {
                   c.BaseAddress = new Uri(ApiUrls.BaseUrl);
               }).AddHttpMessageHandler<CookieHandler>();


            builder.Services.AddRefitClient<ILookupValueApi>()
               .ConfigureHttpClient(c =>
               {
                   c.BaseAddress = new Uri(ApiUrls.BaseUrl);
               }).AddHttpMessageHandler<CookieHandler>();

            builder.Services.AddRefitClient<IDashboardApi>()
               .ConfigureHttpClient(c =>
               {
                   c.BaseAddress = new Uri(ApiUrls.BaseUrl);
               }).AddHttpMessageHandler<CookieHandler>();

            builder.Services.AddRefitClient<IAdminDashboardApi>()
   .ConfigureHttpClient(c =>
   {
       c.BaseAddress = new Uri(ApiUrls.BaseUrl);
   }).AddHttpMessageHandler<CookieHandler>();


            //services.AddTransient<CookieHandler>();
            builder.Services.AddScoped<IUserService, UserService>();

            //services.AddAuthentication("Cookies")
            //.AddCookie("Cookies", options =>
            //{
            //    options.LoginPath = PagesUris.AccountPages.Login;
            //    options.Cookie.Name = "MyBffAuthCookie"; // اسم الكوكي
            //    options.Cookie.HttpOnly = true; // حماية من XSS
            //    options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // يعمل فقط مع HTTPS
            //    options.Cookie.SameSite = SameSiteMode.Strict; // حماية من CSRF
            //    /* إعدادات الكوكيز */
            //});
            //services.AddAuthorizationCore();
            //services.AddScoped<AuthenticationStateProvider, CookieAuthenticationStateProvider>();
            //services.AddScoped(sp => (IAccountManagement)sp.GetRequiredService<AuthenticationStateProvider>());

            //services.AddHttpClient("Auth", options =>
            //  options.BaseAddress = new Uri(ApiUrls.BaseUrl))
            //   .AddHttpMessageHandler<CookieHandler>();




            //services.AddRefitClient<IEmployeeApi>()
            //    .ConfigureHttpClient(c =>
            //    {

            //        c.BaseAddress = new Uri(ApiUrls.BaseUrl);
            //    }).AddHttpMessageHandler<CookieHandler>();

            //services.AddRefitClient<ILookupValueApi>()
            //    .ConfigureHttpClient(c =>
            //    {
            //        c.BaseAddress = new Uri(ApiUrls.BaseUrl);
            //    }).AddHttpMessageHandler<CookieHandler>();

            // أضف هنا أي خدمات مشتركة أخرى
        }
    }
}