using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SharedLib.Entities
{
    public class WReportDetail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public DateTime? SubmissionDate { get; set; } = DateTime.Now;

        public string? OverallImplementation { get; set; }

        public string? EducationalAchievement { get; set; }

        public string? PsychosocialAchievement { get; set; }

        public string? MainChallenge { get; set; }

        public string? NextWeekPriority { get; set; }

        public long? PlanImplementation { get; set; }

        public long? BalanceActivities { get; set; }

        public long? MaterialsAvailability { get; set; }

        public long? TimeSuitability { get; set; }

        public long? RoleDistribution { get; set; }

        public long? CurriculumCompliance { get; set; }

        public long? UNRWAPrinciplesCompliance { get; set; }

        public long? DocumentationQuality { get; set; }

        public string? PlanningNotes { get; set; }

        public long? AttendanceCommitment { get; set; }

        public long? ClassroomManagement { get; set; }

        public long? TeachingStrategies { get; set; }

        public long? RecreationalStrategies { get; set; }

        public long? IndividualDifferences { get; set; }

        public long? DisabilityInclusion { get; set; }

        public long? FeedbackQuality { get; set; }

        public long? TeamworkCoordination { get; set; }

        public string? StaffNotes { get; set; }

        public string? TrainingNeed { get; set; }

        [StringLength(200)]
        public string? SupportType { get; set; }

        [StringLength(200)]
        public string? TargetGroup { get; set; }

        [StringLength(200)]
        public string? TrainingResponsible { get; set; }

        public DateTime? TrainingDate { get; set; }

        public string? AchievementEvidence { get; set; }

        public long? WaterSanitationCleanliness { get; set; }

        public long? VentilationShadeSeating { get; set; }

        public long? FirstAidEmergencyProcedures { get; set; }

        public long? ChildProtectionProfessionalConduct { get; set; }

        public long? ComplaintReferralMechanism { get; set; }

        public long? DisabilityAccessActivities { get; set; }

        public long? DignityNonDiscrimination { get; set; }

        public string? EnvironmentNotes { get; set; }

        public string? RiskChallenge { get; set; }

        public string? RiskImpact { get; set; }

        public long? RiskProbability { get; set; }

        public long? RiskPriority { get; set; }

        public string? RiskAction { get; set; }

        [StringLength(200)]
        public string? RiskResponsible { get; set; }

        public long? RiskStatus { get; set; }

        public string? BestPracticeName { get; set; }

        public string? PracticeDescription { get; set; }

        public string? SuccessImplementation { get; set; }

        public string? ImpactEvidenceData { get; set; }

        public string? LessonsLearned { get; set; }

        public long? CenterId { get; set; }

        // Foreign Key & Navigation Property للربط مع جدول WReport
        public long? WReportId { get; set; }

        [ForeignKey("WReportId")]
        public virtual WReport? WReport { get; set; }
    }
}
