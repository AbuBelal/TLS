using APIServerLib.Data;
using APIServerLib.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using SharedLib.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace APIServerLib.Repositories.Implemntations
{
    public class WReportDetailRepository : IWReportDetailRepository
    {
        private readonly ApplicationDbContext _context;

        public WReportDetailRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<WReportDetail>> GetAllAsync()
        {
            return await _context.WReportDetails
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<WReportDetail?> GetByIdAsync(long id)
        {
            return await _context.WReportDetails
                .Include(d => d.WReport) // جلب بيانات التقرير الرئيسي المرتبط
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        // جلب جميع التفاصيل التابعة لتقرير محدد
        public async Task<IEnumerable<WReportDetail>> GetByWReportIdAsync(long wReportId)
        {
            return await _context.WReportDetails
                .Where(d => d.WReportId == wReportId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<WReportDetail> AddAsync(WReportDetail reportDetail)
        {
            // ضمان تعيين تاريخ الإدخال إذا لم يكن مرسلاً
            reportDetail.SubmissionDate ??= DateTime.Now;

            await _context.WReportDetails.AddAsync(reportDetail);
            await _context.SaveChangesAsync();
            return reportDetail;
        }

        public async Task UpdateAsync(WReportDetail reportDetail)
        {
            var existingDetail1 = await _context.WReportDetails.FindAsync(reportDetail.Id);
            //if(existingDetail != null)
            //{
            //    reportDetail.Id = 0;
            //    _context.WReportDetails.Remove(existingDetail);
            //    _context.WReportDetails.Add(reportDetail);
            //    await _context.SaveChangesAsync();
            //}

            if (existingDetail1 != null)
            {
                var existingDetail = new WReportDetail();
                // 2. نسخ كل قيمة يدوياً من الكائن الجديد إلى الكائن الموجود
                existingDetail.SubmissionDate = reportDetail.SubmissionDate;
                existingDetail.OverallImplementation = reportDetail.OverallImplementation;
                existingDetail.EducationalAchievement = reportDetail.EducationalAchievement;
                existingDetail.PsychosocialAchievement = reportDetail.PsychosocialAchievement;
                existingDetail.MainChallenge = reportDetail.MainChallenge;
                existingDetail.NextWeekPriority = reportDetail.NextWeekPriority;

                // التخطيط وجودة التنفيذ
                existingDetail.PlanImplementation = reportDetail.PlanImplementation;
                existingDetail.BalanceActivities = reportDetail.BalanceActivities;
                existingDetail.MaterialsAvailability = reportDetail.MaterialsAvailability;
                existingDetail.TimeSuitability = reportDetail.TimeSuitability;
                existingDetail.RoleDistribution = reportDetail.RoleDistribution;
                existingDetail.CurriculumCompliance = reportDetail.CurriculumCompliance;
                existingDetail.UNRWAPrinciplesCompliance = reportDetail.UNRWAPrinciplesCompliance;
                existingDetail.DocumentationQuality = reportDetail.DocumentationQuality;
                existingDetail.PlanningNotes = reportDetail.PlanningNotes;

                // أداء العاملين
                existingDetail.AttendanceCommitment = reportDetail.AttendanceCommitment;
                existingDetail.ClassroomManagement = reportDetail.ClassroomManagement;
                existingDetail.TeachingStrategies = reportDetail.TeachingStrategies;
                existingDetail.RecreationalStrategies = reportDetail.RecreationalStrategies;
                existingDetail.IndividualDifferences = reportDetail.IndividualDifferences;
                existingDetail.DisabilityInclusion = reportDetail.DisabilityInclusion;
                existingDetail.FeedbackQuality = reportDetail.FeedbackQuality;
                existingDetail.TeamworkCoordination = reportDetail.TeamworkCoordination;
                existingDetail.StaffNotes = reportDetail.StaffNotes;

                // الاحتياجات التدريبية
                existingDetail.TrainingNeed = reportDetail.TrainingNeed;
                existingDetail.SupportType = reportDetail.SupportType;
                existingDetail.TargetGroup = reportDetail.TargetGroup;
                existingDetail.TrainingResponsible = reportDetail.TrainingResponsible;
                existingDetail.TrainingDate = reportDetail.TrainingDate;
                existingDetail.AchievementEvidence = reportDetail.AchievementEvidence;

                // البيئة المدرسية
                existingDetail.WaterSanitationCleanliness = reportDetail.WaterSanitationCleanliness;
                existingDetail.VentilationShadeSeating = reportDetail.VentilationShadeSeating;
                existingDetail.FirstAidEmergencyProcedures = reportDetail.FirstAidEmergencyProcedures;
                existingDetail.ChildProtectionProfessionalConduct = reportDetail.ChildProtectionProfessionalConduct;
                existingDetail.ComplaintReferralMechanism = reportDetail.ComplaintReferralMechanism;
                existingDetail.DisabilityAccessActivities = reportDetail.DisabilityAccessActivities;
                existingDetail.DignityNonDiscrimination = reportDetail.DignityNonDiscrimination;
                existingDetail.EnvironmentNotes = reportDetail.EnvironmentNotes;

                // التحديات والمخاطر
                existingDetail.RiskChallenge = reportDetail.RiskChallenge;
                existingDetail.RiskImpact = reportDetail.RiskImpact;
                existingDetail.RiskProbability = reportDetail.RiskProbability;
                existingDetail.RiskPriority = reportDetail.RiskPriority;
                existingDetail.RiskAction = reportDetail.RiskAction;
                existingDetail.RiskResponsible = reportDetail.RiskResponsible;
                existingDetail.RiskStatus = reportDetail.RiskStatus;

                // الممارسات المتميزة
                existingDetail.BestPracticeName = reportDetail.BestPracticeName;
                existingDetail.PracticeDescription = reportDetail.PracticeDescription;
                existingDetail.SuccessImplementation = reportDetail.SuccessImplementation;
                existingDetail.ImpactEvidenceData = reportDetail.ImpactEvidenceData;
                existingDetail.LessonsLearned = reportDetail.LessonsLearned;

                // المعرفات الإضافية
                existingDetail.CenterId = reportDetail.CenterId;

                // تحديث المفتاح الأجنبي WReportId فقط إذا جاء بقيمة صالحة
                //if (reportDetail.WReportId.HasValue && reportDetail.WReportId.Value > 0)

                existingDetail.WReportId = reportDetail.WReportId;

                _context.WReportDetails.Add(existingDetail);
                // 3. حفظ التعديلات في قاعدة البيانات
                await _context.SaveChangesAsync();
            }
        }
        


        public async Task DeleteAsync(long id)
        {
            var reportDetail = await _context.WReportDetails.FindAsync(id);
            if (reportDetail != null)
            {
                _context.WReportDetails.Remove(reportDetail);
                await _context.SaveChangesAsync();
            }
        }
    }
}
