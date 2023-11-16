CREATE TABLE [dbo].[Patients] (
    [Id]           INT            IDENTITY (1, 1) NOT NULL,
    [FirstName]    NVARCHAR (MAX) NOT NULL,
    [LastName]     NVARCHAR (MAX) NOT NULL,
    [DateOfBirth]  DATE       NOT NULL,
    [MobileNumber] NVARCHAR (MAX) NOT NULL,
    [UserId]       NVARCHAR (MAX) NOT NULL,
    CONSTRAINT [PK_Patients] PRIMARY KEY CLUSTERED ([Id] ASC)
);

