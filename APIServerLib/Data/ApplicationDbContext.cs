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
        }

    }
}
