SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- Creating the Countries table
CREATE TABLE [dbo].[Countries](
    [Id] INT IDENTITY(1,1) NOT NULL,
    [Name] VARCHAR(50) NOT NULL,
    [TwoLetterCode] CHAR(2) NOT NULL,
    [ThreeLetterCode] CHAR(3) NOT NULL,
    [CreatedAt] DATETIME2(7) NOT NULL,
    CONSTRAINT [PK_Countries] PRIMARY KEY CLUSTERED (
        [Id] ASC
    ) WITH (
        PAD_INDEX = OFF, 
        STATISTICS_NORECOMPUTE = OFF, 
        IGNORE_DUP_KEY = OFF, 
        ALLOW_ROW_LOCKS = ON, 
        ALLOW_PAGE_LOCKS = ON, 
        FILLFACTOR = 95, 
        OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF
    ) ON [PRIMARY]
) ON [PRIMARY]
GO

-- Creating the IPAddresses table
CREATE TABLE [dbo].[IPAddresses](
    [Id] INT IDENTITY(1,1) NOT NULL,
    [CountryId] INT NOT NULL,
    [IP] VARCHAR(15) NOT NULL,
    [CreatedAt] DATETIME2(7) NOT NULL,
    [UpdatedAt] DATETIME2(7) NOT NULL,
    CONSTRAINT [PK_IPAddresses] PRIMARY KEY CLUSTERED (
        [Id] ASC
    ) WITH (
        PAD_INDEX = OFF, 
        STATISTICS_NORECOMPUTE = OFF, 
        IGNORE_DUP_KEY = OFF, 
        ALLOW_ROW_LOCKS = ON, 
        ALLOW_PAGE_LOCKS = ON, 
        FILLFACTOR = 95, 
        OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF
    ) ON [PRIMARY],
    CONSTRAINT [FK_IPAddresses_Countries] FOREIGN KEY ([CountryId]) REFERENCES [dbo].[Countries]([Id])
) ON [PRIMARY]
GO

-- Enable explicit value insertion into the identity column
SET IDENTITY_INSERT [dbo].[Countries] ON;
GO

-- Inserting values into the Countries table
INSERT [dbo].[Countries] ([Id], [Name], [TwoLetterCode], [ThreeLetterCode], [CreatedAt]) VALUES 
(1, N'Greece', N'GR', N'GRC', CAST(N'2022-10-12T06:46:10.5000000' AS DateTime2)),
(2, N'Germany', N'DE', N'DEU', CAST(N'2022-10-12T06:46:10.5000000' AS DateTime2)),
(3, N'Cyprus', N'CY', N'CYP', CAST(N'2022-10-12T06:46:10.5000000' AS DateTime2)),
(4, N'United States', N'US', N'USA', CAST(N'2022-10-12T06:46:10.5000000' AS DateTime2)),
(6, N'Spain', N'ES', N'ESP', CAST(N'2022-10-12T06:46:10.5000000' AS DateTime2)),
(7, N'France', N'FR', N'FRA', CAST(N'2022-10-12T06:46:10.5000000' AS DateTime2)),
(8, N'Italy', N'IT', N'ITA', CAST(N'2022-10-12T06:46:10.5000000' AS DateTime2)),
(9, N'Japan', N'JP', N'JPN', CAST(N'2022-10-12T06:46:10.5000000' AS DateTime2)),
(10, N'China', N'CN', N'CHN', CAST(N'2022-10-12T06:46:10.5000000' AS DateTime2));
GO

-- Disable explicit value insertion into the identity column
SET IDENTITY_INSERT [dbo].[Countries] OFF;
GO

-- Enable explicit value insertion into the identity column for IPAddresses
SET IDENTITY_INSERT [dbo].[IPAddresses] ON;
GO

-- Inserting values into the IPAddresses table
INSERT [dbo].[IPAddresses] ([Id], [CountryId], [IP], [CreatedAt], [UpdatedAt]) VALUES 
(6, 1, N'44.255.255.254', CAST(N'2022-10-12T07:04:06.8566667' AS DateTime2), CAST(N'2022-10-12T07:04:06.8566667' AS DateTime2)),
(7, 2, N'45.255.255.254', CAST(N'2022-10-12T07:04:06.8566667' AS DateTime2), CAST(N'2022-10-12T07:04:06.8566667' AS DateTime2)),
(8, 3, N'46.255.255.254', CAST(N'2022-10-12T07:04:06.8566667' AS DateTime2), CAST(N'2022-10-12T07:04:06.8566667' AS DateTime2)),
(9, 4, N'47.255.255.254', CAST(N'2022-10-12T07:04:06.8566667' AS DateTime2), CAST(N'2022-10-12T07:04:06.8566667' AS DateTime2)),
(10, 6, N'49.255.255.254', CAST(N'2022-10-12T07:04:06.8566667' AS DateTime2), CAST(N'2022-10-12T07:04:06.8566667' AS DateTime2)),
(11, 7, N'41.255.255.254', CAST(N'2022-10-12T07:04:06.8566667' AS DateTime2), CAST(N'2022-10-12T07:04:06.8566667' AS DateTime2)),
(12, 8, N'42.255.255.254', CAST(N'2022-10-12T07:04:06.8566667' AS DateTime2), CAST(N'2022-10-12T07:04:06.8566667' AS DateTime2)),
(13, 9, N'43.255.255.254', CAST(N'2022-10-12T07:04:06.8566667' AS DateTime2), CAST(N'2022-10-12T07:04:06.8566667' AS DateTime2)),
(14, 10, N'50.255.255.254', CAST(N'2022-10-12T07:04:06.8566667' AS DateTime2), CAST(N'2022-10-12T07:04:06.8566667' AS DateTime2));
GO

-- Disable explicit value insertion into the identity column
SET IDENTITY_INSERT [dbo].[IPAddresses] OFF;
GO

-- Set constraints on the tables
SET ANSI_PADDING ON;
GO

-- Creating a unique non-clustered index on the IP field
ALTER TABLE [dbo].[IPAddresses] 
ADD CONSTRAINT [IX_IPAddresses] UNIQUE NONCLUSTERED ([IP] ASC)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 95, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) 
ON [PRIMARY];
GO

-- Adding default constraints for CreatedAt and UpdatedAt columns
ALTER TABLE [dbo].[Countries] 
ADD CONSTRAINT [DF_Countries_CreatedAt] DEFAULT (GETUTCDATE()) FOR [CreatedAt];
GO

ALTER TABLE [dbo].[IPAddresses] 
ADD CONSTRAINT [DF_IPAddresses_CreatedAt] DEFAULT (GETUTCDATE()) FOR [CreatedAt];
GO

ALTER TABLE [dbo].[IPAddresses] 
ADD CONSTRAINT [DF_IPAddresses_UpdatedAt] DEFAULT (GETUTCDATE()) FOR [UpdatedAt];
GO

-- Adding foreign key constraint for the CountryId column in the IPAddresses table
--ALTER TABLE [dbo].[IPAddresses] 
--WITH CHECK ADD CONSTRAINT [FK_IPAddresses_Countries] FOREIGN KEY([CountryId])
--REFERENCES [dbo].[Countries] ([Id]);
--GO

--ALTER TABLE [dbo].[IPAddresses] 
--CHECK CONSTRAINT [FK_IPAddresses_Countries];
--GO

