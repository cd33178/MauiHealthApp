-- =============================================================================
-- 001_Initial_Schema.sql
-- MauiHealthApp – initial database schema
-- Target: SQL Server 2019+ / Azure SQL
-- Run order: 1
-- =============================================================================

SET NOCOUNT ON;
GO

-- ---------------------------------------------------------------------------
-- Ensure we are in the correct database context before creating objects.
-- Replace 'MauiHealthApp' with your actual database name when executing
-- outside of an automated migration tool.
-- ---------------------------------------------------------------------------
-- USE [MauiHealthApp];
-- GO

-- ===========================================================================
-- TABLE: Users
-- Core identity record; authentication details live in a separate identity
-- provider (e.g. ASP.NET Core Identity / Azure AD B2C).
-- ===========================================================================
IF NOT EXISTS (
    SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Users]') AND type = 'U'
)
BEGIN
    CREATE TABLE [dbo].[Users] (
        [Id]          UNIQUEIDENTIFIER NOT NULL CONSTRAINT [DF_Users_Id]          DEFAULT NEWSEQUENTIALID(),
        [Email]       NVARCHAR(256)    NOT NULL,
        [DisplayName] NVARCHAR(256)    NULL,
        [CreatedAt]   DATETIME2(7)     NOT NULL CONSTRAINT [DF_Users_CreatedAt]   DEFAULT GETUTCDATE(),
        [UpdatedAt]   DATETIME2(7)     NULL,
        [IsActive]    BIT              NOT NULL CONSTRAINT [DF_Users_IsActive]    DEFAULT 1,

        CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [UQ_Users_Email] UNIQUE NONCLUSTERED ([Email] ASC)
    );

    PRINT 'Created table [dbo].[Users]';
END
ELSE
    PRINT 'Table [dbo].[Users] already exists – skipped.';
GO

-- ===========================================================================
-- TABLE: Profiles
-- Extended health profile linked 1-to-1 with a User.
-- ===========================================================================
IF NOT EXISTS (
    SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Profiles]') AND type = 'U'
)
BEGIN
    CREATE TABLE [dbo].[Profiles] (
        [Id]          UNIQUEIDENTIFIER NOT NULL CONSTRAINT [DF_Profiles_Id]        DEFAULT NEWID(),
        [UserId]      UNIQUEIDENTIFIER NOT NULL,
        [DateOfBirth] DATE             NULL,
        [WeightKg]    DECIMAL(5, 2)    NULL,
        [HeightCm]    DECIMAL(5, 2)    NULL,
        [BloodType]   NVARCHAR(10)     NULL,
        [Notes]       NVARCHAR(MAX)    NULL,
        [CreatedAt]   DATETIME2(7)     NOT NULL CONSTRAINT [DF_Profiles_CreatedAt] DEFAULT GETUTCDATE(),
        [UpdatedAt]   DATETIME2(7)     NULL,

        CONSTRAINT [PK_Profiles] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [FK_Profiles_Users] FOREIGN KEY ([UserId])
            REFERENCES [dbo].[Users] ([Id])
            ON DELETE CASCADE
            ON UPDATE NO ACTION
    );

    PRINT 'Created table [dbo].[Profiles]';
END
ELSE
    PRINT 'Table [dbo].[Profiles] already exists – skipped.';
GO

-- ===========================================================================
-- TABLE: MedicalConditions
-- Health conditions associated with a Profile.
-- ===========================================================================
IF NOT EXISTS (
    SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[MedicalConditions]') AND type = 'U'
)
BEGIN
    CREATE TABLE [dbo].[MedicalConditions] (
        [Id]           UNIQUEIDENTIFIER NOT NULL CONSTRAINT [DF_MedicalConditions_Id]        DEFAULT NEWID(),
        [ProfileId]    UNIQUEIDENTIFIER NOT NULL,
        [Name]         NVARCHAR(256)    NOT NULL,
        [DiagnosedAt]  DATE             NULL,
        [Severity]     NVARCHAR(50)     NULL,   -- e.g. Mild, Moderate, Severe
        [Notes]        NVARCHAR(MAX)    NULL,
        [CreatedAt]    DATETIME2(7)     NOT NULL CONSTRAINT [DF_MedicalConditions_CreatedAt] DEFAULT GETUTCDATE(),

        CONSTRAINT [PK_MedicalConditions] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [FK_MedicalConditions_Profiles] FOREIGN KEY ([ProfileId])
            REFERENCES [dbo].[Profiles] ([Id])
            ON DELETE CASCADE
            ON UPDATE NO ACTION,
        CONSTRAINT [CK_MedicalConditions_Severity] CHECK (
            [Severity] IS NULL OR [Severity] IN (N'Mild', N'Moderate', N'Severe')
        )
    );

    PRINT 'Created table [dbo].[MedicalConditions]';
END
ELSE
    PRINT 'Table [dbo].[MedicalConditions] already exists – skipped.';
GO

-- ===========================================================================
-- TABLE: Questions
-- Health questions submitted by users; may be answered inline or via Answers.
-- ===========================================================================
IF NOT EXISTS (
    SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Questions]') AND type = 'U'
)
BEGIN
    CREATE TABLE [dbo].[Questions] (
        [Id]           UNIQUEIDENTIFIER NOT NULL CONSTRAINT [DF_Questions_Id]         DEFAULT NEWID(),
        [UserId]       UNIQUEIDENTIFIER NOT NULL,
        [QuestionText] NVARCHAR(MAX)    NOT NULL,
        [AnswerText]   NVARCHAR(MAX)    NULL,    -- Denormalised quick-answer field
        [Category]     NVARCHAR(100)    NULL,
        [Tags]         NVARCHAR(500)    NULL,    -- Comma-separated tag list
        [IsAnswered]   BIT              NOT NULL CONSTRAINT [DF_Questions_IsAnswered] DEFAULT 0,
        [CreatedAt]    DATETIME2(7)     NOT NULL CONSTRAINT [DF_Questions_CreatedAt]  DEFAULT GETUTCDATE(),
        [UpdatedAt]    DATETIME2(7)     NULL,

        CONSTRAINT [PK_Questions] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [FK_Questions_Users] FOREIGN KEY ([UserId])
            REFERENCES [dbo].[Users] ([Id])
            ON DELETE CASCADE
            ON UPDATE NO ACTION
    );

    PRINT 'Created table [dbo].[Questions]';
END
ELSE
    PRINT 'Table [dbo].[Questions] already exists – skipped.';
GO

-- ===========================================================================
-- TABLE: Answers
-- Detailed answers to a Question (one-to-many to support multiple sources).
-- ===========================================================================
IF NOT EXISTS (
    SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Answers]') AND type = 'U'
)
BEGIN
    CREATE TABLE [dbo].[Answers] (
        [Id]         UNIQUEIDENTIFIER NOT NULL CONSTRAINT [DF_Answers_Id]        DEFAULT NEWID(),
        [QuestionId] UNIQUEIDENTIFIER NOT NULL,
        [AnswerText] NVARCHAR(MAX)    NOT NULL,
        [Source]     NVARCHAR(256)    NULL,
        [Confidence] DECIMAL(3, 2)   NULL,  -- 0.00–1.00
        [CreatedAt]  DATETIME2(7)    NOT NULL CONSTRAINT [DF_Answers_CreatedAt] DEFAULT GETUTCDATE(),

        CONSTRAINT [PK_Answers] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [FK_Answers_Questions] FOREIGN KEY ([QuestionId])
            REFERENCES [dbo].[Questions] ([Id])
            ON DELETE CASCADE
            ON UPDATE NO ACTION,
        CONSTRAINT [CK_Answers_Confidence] CHECK (
            [Confidence] IS NULL OR ([Confidence] >= 0.00 AND [Confidence] <= 1.00)
        )
    );

    PRINT 'Created table [dbo].[Answers]';
END
ELSE
    PRINT 'Table [dbo].[Answers] already exists – skipped.';
GO

-- ===========================================================================
-- INDEXES
-- ===========================================================================

-- Users ── email lookup (covered by the unique constraint, adding explicit
--          non-clustered index on IsActive for active-user filters)
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Users_Email' AND object_id = OBJECT_ID(N'[dbo].[Users]'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Users_Email]
        ON [dbo].[Users] ([Email] ASC)
        INCLUDE ([DisplayName], [IsActive]);
    PRINT 'Created index [IX_Users_Email]';
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Users_CreatedAt' AND object_id = OBJECT_ID(N'[dbo].[Users]'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Users_CreatedAt]
        ON [dbo].[Users] ([CreatedAt] DESC)
        INCLUDE ([Email], [IsActive]);
    PRINT 'Created index [IX_Users_CreatedAt]';
END
GO

-- Profiles ── FK + filtered index for weight/height analytics
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Profiles_UserId' AND object_id = OBJECT_ID(N'[dbo].[Profiles]'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Profiles_UserId]
        ON [dbo].[Profiles] ([UserId] ASC)
        INCLUDE ([DateOfBirth], [WeightKg], [HeightCm], [BloodType]);
    PRINT 'Created index [IX_Profiles_UserId]';
END
GO

-- MedicalConditions ── FK + name search
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_MedicalConditions_ProfileId' AND object_id = OBJECT_ID(N'[dbo].[MedicalConditions]'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_MedicalConditions_ProfileId]
        ON [dbo].[MedicalConditions] ([ProfileId] ASC)
        INCLUDE ([Name], [Severity], [DiagnosedAt]);
    PRINT 'Created index [IX_MedicalConditions_ProfileId]';
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_MedicalConditions_Name' AND object_id = OBJECT_ID(N'[dbo].[MedicalConditions]'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_MedicalConditions_Name]
        ON [dbo].[MedicalConditions] ([Name] ASC)
        INCLUDE ([ProfileId], [Severity]);
    PRINT 'Created index [IX_MedicalConditions_Name]';
END
GO

-- Questions ── FK, category filter, and recency sort
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Questions_UserId' AND object_id = OBJECT_ID(N'[dbo].[Questions]'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Questions_UserId]
        ON [dbo].[Questions] ([UserId] ASC)
        INCLUDE ([Category], [IsAnswered], [CreatedAt]);
    PRINT 'Created index [IX_Questions_UserId]';
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Questions_Category' AND object_id = OBJECT_ID(N'[dbo].[Questions]'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Questions_Category]
        ON [dbo].[Questions] ([Category] ASC)
        INCLUDE ([UserId], [IsAnswered], [CreatedAt]);
    PRINT 'Created index [IX_Questions_Category]';
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Questions_CreatedAt' AND object_id = OBJECT_ID(N'[dbo].[Questions]'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Questions_CreatedAt]
        ON [dbo].[Questions] ([CreatedAt] DESC)
        INCLUDE ([UserId], [Category], [IsAnswered]);
    PRINT 'Created index [IX_Questions_CreatedAt]';
END
GO

-- Answers ── FK lookup
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Answers_QuestionId' AND object_id = OBJECT_ID(N'[dbo].[Answers]'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Answers_QuestionId]
        ON [dbo].[Answers] ([QuestionId] ASC)
        INCLUDE ([Source], [Confidence], [CreatedAt]);
    PRINT 'Created index [IX_Answers_QuestionId]';
END
GO

PRINT '001_Initial_Schema.sql completed successfully.';
GO
