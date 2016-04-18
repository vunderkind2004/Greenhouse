CREATE TABLE [dbo].[User]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [Login] NVARCHAR(50) NOT NULL, 
    [FirstName] NVARCHAR(50) NOT NULL, 
    [LastName] NVARCHAR(50) NOT NULL, 
    [Email] NVARCHAR(50) NOT NULL, 
    [PasswordHash] NVARCHAR(50) NOT NULL, 
    [RegistrationDate] DATETIME NOT NULL, 
    [IsAdmin] BIT NOT NULL DEFAULT 0, 
    [Phone] NVARCHAR(50) NULL
)
