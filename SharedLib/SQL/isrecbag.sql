alter TABLE [dbo].[Students]
	add [IsReceviedBag] [bit] NULL;
go
ALTER TABLE [dbo].[Students] ADD  CONSTRAINT [DF_Students_IsReceviedBag]  DEFAULT ((0)) FOR [IsReceviedBag]
GO
update Students
set IsReceviedBag=0;

