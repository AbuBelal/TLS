
/****** Object:  Table [dbo].[WReportDetails]    Script Date: 27/07/2026 8:29:24 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[WReportDetails](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[SubmissionDate] [datetime] NULL,
	[OverallImplementation] [nvarchar](max) NULL,
	[EducationalAchievement] [nvarchar](max) NULL,
	[PsychosocialAchievement] [nvarchar](max) NULL,
	[MainChallenge] [nvarchar](max) NULL,
	[NextWeekPriority] [nvarchar](max) NULL,
	[PlanImplementation] [bigint] NULL,
	[BalanceActivities] [bigint] NULL,
	[MaterialsAvailability] [bigint] NULL,
	[TimeSuitability] [bigint] NULL,
	[RoleDistribution] [bigint] NULL,
	[CurriculumCompliance] [bigint] NULL,
	[UNRWAPrinciplesCompliance] [bigint] NULL,
	[DocumentationQuality] [bigint] NULL,
	[PlanningNotes] [nvarchar](max) NULL,
	[AttendanceCommitment] [bigint] NULL,
	[ClassroomManagement] [bigint] NULL,
	[TeachingStrategies] [bigint] NULL,
	[RecreationalStrategies] [bigint] NULL,
	[IndividualDifferences] [bigint] NULL,
	[DisabilityInclusion] [bigint] NULL,
	[FeedbackQuality] [bigint] NULL,
	[TeamworkCoordination] [bigint] NULL,
	[StaffNotes] [nvarchar](max) NULL,
	[TrainingNeed] [nvarchar](max) NULL,
	[SupportType] [nvarchar](200) NULL,
	[TargetGroup] [nvarchar](200) NULL,
	[TrainingResponsible] [nvarchar](200) NULL,
	[TrainingDate] [date] NULL,
	[AchievementEvidence] [nvarchar](max) NULL,
	[WaterSanitationCleanliness] [bigint] NULL,
	[VentilationShadeSeating] [bigint] NULL,
	[FirstAidEmergencyProcedures] [bigint] NULL,
	[ChildProtectionProfessionalConduct] [bigint] NULL,
	[ComplaintReferralMechanism] [bigint] NULL,
	[DisabilityAccessActivities] [bigint] NULL,
	[DignityNonDiscrimination] [bigint] NULL,
	[EnvironmentNotes] [nvarchar](max) NULL,
	[RiskChallenge] [nvarchar](max) NULL,
	[RiskImpact] [nvarchar](max) NULL,
	[RiskProbability] [bigint] NULL,
	[RiskPriority] [bigint] NULL,
	[RiskAction] [nvarchar](max) NULL,
	[RiskResponsible] [nvarchar](200) NULL,
	[RiskStatus] [bigint] NULL,
	[BestPracticeName] [nvarchar](max) NULL,
	[PracticeDescription] [nvarchar](max) NULL,
	[SuccessImplementation] [nvarchar](max) NULL,
	[ImpactEvidenceData] [nvarchar](max) NULL,
	[LessonsLearned] [nvarchar](max) NULL,
	[CenterId] [bigint] NULL,
	[WReportId] [bigint] NULL,
 CONSTRAINT [PK__weekly_R__3214EC071B950992] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[WReportDetails] ADD  CONSTRAINT [DF__weekly_Re__Submi__4F47C5E3]  DEFAULT (getdate()) FOR [SubmissionDate]
GO

ALTER TABLE [dbo].[WReportDetails]  WITH CHECK ADD  CONSTRAINT [FK_WReport_Details_WReport] FOREIGN KEY([WReportId])
REFERENCES [dbo].[WReports] ([Id])
GO

ALTER TABLE [dbo].[WReportDetails] CHECK CONSTRAINT [FK_WReport_Details_WReport]
GO


