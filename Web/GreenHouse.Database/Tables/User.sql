CREATE TABLE [dbo].[User] (
    [Id]               INT           IDENTITY (1, 1) NOT NULL,
    [Login]            NVARCHAR (50) NOT NULL,
    [FirstName]        NVARCHAR (50) NOT NULL,
    [LastName]         NVARCHAR (50) NOT NULL,
    [Email]            NVARCHAR (50) NOT NULL,
    [PasswordHash]     NVARCHAR (50) NOT NULL,
    [RegistrationDate] DATETIME      NOT NULL,
    [IsAdmin]          BIT           DEFAULT ((0)) NOT NULL,
    [Phone]            NVARCHAR (50) NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);



GO
CREATE NONCLUSTERED INDEX [IX_UserLogin]
    ON [dbo].[User]([Login] ASC);

