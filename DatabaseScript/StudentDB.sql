USE [StudentDb]
GO
/****** Object:  Table [dbo].[Admissions]    Script Date: 17-05-2026 22:36:50 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Admissions](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[StudentId] [int] NULL,
	[FeesPaid] [decimal](18, 0) NULL,
	[IsConfirmed] [bit] NULL,
	[AdmissionDate] [date] NULL,
 CONSTRAINT [PK_Admissions] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Streams]    Script Date: 17-05-2026 22:36:50 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Streams](
	[id] [int] NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_Streams] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Students]    Script Date: 17-05-2026 22:36:50 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Students](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NULL,
	[Age] [int] NULL,
	[Email] [nvarchar](50) NULL,
	[IsDeleted] [bit] NULL,
	[StreamId] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[Admissions] ON 
GO
INSERT [dbo].[Admissions] ([id], [StudentId], [FeesPaid], [IsConfirmed], [AdmissionDate]) VALUES (1, 1, CAST(432 AS Decimal(18, 0)), 1, NULL)
GO
INSERT [dbo].[Admissions] ([id], [StudentId], [FeesPaid], [IsConfirmed], [AdmissionDate]) VALUES (2, 1, CAST(432 AS Decimal(18, 0)), 1, NULL)
GO
INSERT [dbo].[Admissions] ([id], [StudentId], [FeesPaid], [IsConfirmed], [AdmissionDate]) VALUES (3, 1, CAST(1223 AS Decimal(18, 0)), 1, CAST(N'2026-05-17' AS Date))
GO
INSERT [dbo].[Admissions] ([id], [StudentId], [FeesPaid], [IsConfirmed], [AdmissionDate]) VALUES (4, 2, CAST(233 AS Decimal(18, 0)), 1, CAST(N'2026-05-17' AS Date))
GO
INSERT [dbo].[Admissions] ([id], [StudentId], [FeesPaid], [IsConfirmed], [AdmissionDate]) VALUES (5, 3, CAST(3434 AS Decimal(18, 0)), 1, CAST(N'2026-05-17' AS Date))
GO
SET IDENTITY_INSERT [dbo].[Admissions] OFF
GO
INSERT [dbo].[Streams] ([id], [Name]) VALUES (1, N'Arts')
GO
INSERT [dbo].[Streams] ([id], [Name]) VALUES (2, N'Commerce')
GO
INSERT [dbo].[Streams] ([id], [Name]) VALUES (3, N'Science')
GO
SET IDENTITY_INSERT [dbo].[Students] ON 
GO
INSERT [dbo].[Students] ([Id], [Name], [Age], [Email], [IsDeleted], [StreamId]) VALUES (1, N'Shekhar', 110, N'string', 0, NULL)
GO
INSERT [dbo].[Students] ([Id], [Name], [Age], [Email], [IsDeleted], [StreamId]) VALUES (2, N'Shekhar1 Chavan', 110, N'string', 0, NULL)
GO
INSERT [dbo].[Students] ([Id], [Name], [Age], [Email], [IsDeleted], [StreamId]) VALUES (3, N'Annaytryt', 23, N'kanase.ranjit@gmail.com', 1, NULL)
GO
INSERT [dbo].[Students] ([Id], [Name], [Age], [Email], [IsDeleted], [StreamId]) VALUES (4, N'wewe', 2334, N'kanasepopatrao@gamil.com', 0, NULL)
GO
INSERT [dbo].[Students] ([Id], [Name], [Age], [Email], [IsDeleted], [StreamId]) VALUES (5, N'test', 654, N'ytr', 1, NULL)
GO
INSERT [dbo].[Students] ([Id], [Name], [Age], [Email], [IsDeleted], [StreamId]) VALUES (6, N'Chetan Kishor Pawar', 2, N'kanase.ranjit@gmail.com', 1, NULL)
GO
INSERT [dbo].[Students] ([Id], [Name], [Age], [Email], [IsDeleted], [StreamId]) VALUES (7, N'Dusare nav', 23, N'dsfre', 0, NULL)
GO
INSERT [dbo].[Students] ([Id], [Name], [Age], [Email], [IsDeleted], [StreamId]) VALUES (8, N'hawaa', 56, N'342323@test.com', 1, NULL)
GO
INSERT [dbo].[Students] ([Id], [Name], [Age], [Email], [IsDeleted], [StreamId]) VALUES (9, N'sdad', 23, N'Test@test.com', 1, NULL)
GO
SET IDENTITY_INSERT [dbo].[Students] OFF
GO
ALTER TABLE [dbo].[Admissions]  WITH CHECK ADD  CONSTRAINT [FK_Admissions_Students] FOREIGN KEY([StudentId])
REFERENCES [dbo].[Students] ([Id])
GO
ALTER TABLE [dbo].[Admissions] CHECK CONSTRAINT [FK_Admissions_Students]
GO
