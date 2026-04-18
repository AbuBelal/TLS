using APIServerLib.Repositories.Implemntations;
using APIServerLib.Repositories.Interfaces;
using APIServerLib.Services;
using Microsoft.AspNetCore.Identity;
using SharedLib.Entities;
namespace APIServerLib.ProgramSettings
{
    public static class AddAppServices
    {
        public static WebApplicationBuilder AddAppServicesToContainer(this WebApplicationBuilder builder)
        {
            // إضافة خدمات أخرى هنا إذا لزم الأمر
            builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
            builder.Services.AddScoped<ICenterRepository, CenterRepository>();
            builder.Services.AddScoped<IStudentRepository, StudentRepository>();
            builder.Services.AddScoped<IEmpCenterRepository, EmpCenterRepository>();
            builder.Services.AddScoped<IStdCenterRepository, StdCenterRepository>();
            builder.Services.AddScoped<ILookupValueRepository, LookupValueRepository>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IDashboardRepository, DashboardRepository>();
            builder.Services.AddScoped<IAdminDashboardRepository, AdminDashboardRepository>();

            builder.Services.AddScoped<IAuditLogRepository, AuditLogRepository>();
            builder.Services.AddScoped<AuditLogService>();
            builder.Services.AddHttpContextAccessor();

            // أضف هذا السطر مع باقي تسجيلات الخدمات:
            //builder.Services.AddScoped<IRolesRepository, RolesRepository>();

            return builder;
        }
    }
}
