//using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
//using ServerLibrary.Helpers;
using System.Text;

namespace Server.ProgramSettings
{
    public static  class AddAuthBuilder
    {
        //public static WebApplicationBuilder AddAuth(this WebApplicationBuilder builder)
        //{
        //    // ربط القسم من appsettings.json بالكلاس البرمجي
        //    builder.Services.Configure<Jwt>(builder.Configuration.GetSection("Jwt"));

        //    // إعداد JWT Authentication
        //    builder.Services.AddAuthentication(options =>
        //    {
        //        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        //        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        //    })
        //    .AddJwtBearer(o =>
        //    {
        //        o.RequireHttpsMetadata = false;
        //        o.SaveToken = false;
        //        o.TokenValidationParameters = new TokenValidationParameters
        //        {
        //            ValidateIssuerSigningKey = true,
        //            ValidateIssuer = true,
        //            ValidateAudience = true,
        //            ValidateLifetime = true,
        //            ValidIssuer = builder.Configuration["Jwt:Issuer"],
        //            ValidAudience = builder.Configuration["Jwt:Audience"],
        //            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        //        };
        //    });
        //    return builder;
        //}
    }
}
