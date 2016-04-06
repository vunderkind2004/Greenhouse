CREATE TABLE [dbo].[Device]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY,
    [RegistrationDate] DATETIME NOT NULL, 
    [Name] NVARCHAR(50) NOT NULL, 
    [Summary] NVARCHAR(MAX) NULL, 
    [Token] NVARCHAR(50) NOT NULL
)
