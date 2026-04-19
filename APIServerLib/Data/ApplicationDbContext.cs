using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SharedLib.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace APIServerLib.Data
{
    public  class ApplicationDbContext (DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
    {
       
        // جداول الكيانات الأساسية
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Center> Centers { get; set; }
        public DbSet<LookupValue> LookupValues { get; set; }

        // جداول الربط (Many-to-Many)
        public DbSet<EmpCenter> EmpCenters { get; set; }
        public DbSet<StdCenter> StdCenters { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<DailyReport> DailyReports { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 1. إعداد المفاتيح المركبة للجداول الوسيطة
            modelBuilder.Entity<EmpCenter>().HasKey(ec => new { ec.EmployeeId, ec.CenterId });
            modelBuilder.Entity<StdCenter>().HasKey(sc => new { sc.StudentId, sc.CenterId });

            // 2. حل مشكلة الـ Multiple Cascade Paths للموظف
            // نقوم بتعريف كل علاقة ونحدد أن OnDelete هو Restrict أو NoAction
            var employee = modelBuilder.Entity<Employee>();

            employee.HasOne(e => e.Gender)
                .WithMany()
                .HasForeignKey(e => e.GenderId)
                .OnDelete(DeleteBehavior.NoAction); // هذا هو السطر السحري

            employee.HasOne(e => e.Job)
                .WithMany()
                .HasForeignKey(e => e.JobId)
                .OnDelete(DeleteBehavior.NoAction);

            employee.HasOne(e => e.OrgJob)
                .WithMany()
                .HasForeignKey(e => e.OrgJobId)
                .OnDelete(DeleteBehavior.NoAction);

            employee.HasOne(e => e.Specialization)
                .WithMany()
                .HasForeignKey(e => e.SpecializationId)
                .OnDelete(DeleteBehavior.NoAction);

            // 3. حل نفس المشكلة للطالب (لتجنب ظهور الخطأ له لاحقاً)
            var student = modelBuilder.Entity<Student>();

            student.HasOne(s => s.Gender)
                .WithMany()
                .HasForeignKey(s => s.GenderId)
                .OnDelete(DeleteBehavior.NoAction);

            student.HasOne(s => s.Level)
                .WithMany()
                .HasForeignKey(s => s.LevelId)
                .OnDelete(DeleteBehavior.NoAction);

            // تعريف المفتاح المركب لجدول StdCenter إذا لزم الأمر
            modelBuilder.Entity<StdCenter>()
                .HasKey(sc => new { sc.StudentId, sc.CenterId, sc.FromDate });

            // إعداد العلاقة بين الطالب ومراكزه
            modelBuilder.Entity<StdCenter>()
                .HasOne(sc => sc.Student)
                .WithMany(s => s.StdCenters)
                .HasForeignKey(sc => sc.StudentId);


            // بيانات ابتدائية للموظفين
            modelBuilder.Entity<Employee>().HasData(
                new Employee { Id = 1, Name = "أحمد", GenderId = 1, JobId = 1, OrgJobId = 1, SpecializationId = 1 },
                new Employee { Id = 2, Name = "سارة", GenderId = 2, JobId = 2, OrgJobId = 2, SpecializationId = 2 }
            );

            // بيانات ابتدائية للطلاب
            modelBuilder.Entity<Student>().HasData(
                new Student { Id = 1, Name = "محمد", GenderId = 1, LevelId = 1 },
                new Student { Id = 2, Name = "ليلى", GenderId = 2, LevelId = 2 }
            );

            // بيانات ابتدائية للمراكز
            modelBuilder.Entity<Center>().HasData(
                new Center { Id = 1, Name = "مركز الرياض" },
                new Center { Id = 2, Name = "مركز جدة" }
            );

            // بيانات ابتدائية للربط بين الموظفين والمراكز
            modelBuilder.Entity<EmpCenter>().HasData(
                new EmpCenter { EmployeeId = 1, CenterId = 1 },
                new EmpCenter { EmployeeId = 2, CenterId = 2 }
            );

            // بيانات ابتدائية للربط بين الطلاب والمراكز
            modelBuilder.Entity<StdCenter>().HasData(
                new StdCenter { StudentId = 1, CenterId = 1, FromDate =DateOnly.Parse("2024-01-01") },
                new StdCenter { StudentId = 2, CenterId = 2, FromDate = DateOnly.Parse("2024-01-01") }
            );

            // بيانات ابتدائية للـ LookupValue
            modelBuilder.Entity<LookupValue>().HasData(
                new LookupValue { Id = 1, Name = "ذكر" },
                new LookupValue { Id = 2, Name = "أنثى" }
            );

            modelBuilder.Entity<AuditLog>(entity =>
            {
                entity.HasOne<ApplicationUser>()
                      .WithMany()
                      .HasForeignKey(a => a.UserId)
                      .OnDelete(DeleteBehavior.NoAction);
            });

            //modelBuilder.Entity<DailyReport>(entity =>
            //{
            //    entity.HasOne<Center>()
            //          .WithMany()
            //          .HasForeignKey(a => a.CenterId)
            //          .OnDelete(DeleteBehavior.NoAction);
            //});
        }

    }
}
