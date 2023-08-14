USE [ep5BAS]
GO

/****** Object:  Table [dbo].[ControlEvent]    Script Date: Wednesday, 19 July, 2023 09:18:59 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ControlEvent](
	[recipeID] [int] NOT NULL,
	[dayOfWeek] [tinyint] NOT NULL,
	[channelNmbr] [int] NOT NULL,
	[doAt] [time](7) NOT NULL,
	[whenEntered] [datetime2](2) NOT NULL,
	[whenEdited] [datetime2](2) NOT NULL,
	[enteredBy] [int] NOT NULL,
	[editedBy] [int] NOT NULL,
	[digitalValue] [bit] NOT NULL,
	[notes] [nvarchar](max) NULL,
	[status] [int] NOT NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[ControlEvent]  WITH CHECK ADD  CONSTRAINT [FK_ControlEvent_ControlEventHeader] FOREIGN KEY([recipeID])
REFERENCES [dbo].[ControlEventHeader] ([recipeID])
GO

ALTER TABLE [dbo].[ControlEvent] CHECK CONSTRAINT [FK_ControlEvent_ControlEventHeader]
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'0 = Sunday' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ControlEvent', @level2type=N'COLUMN',@level2name=N'dayOfWeek'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'true = ON; false = OFF' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ControlEvent', @level2type=N'COLUMN',@level2name=N'digitalValue'
GO

