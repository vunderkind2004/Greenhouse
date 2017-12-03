CREATE TABLE [dbo].[Device]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY,
    [RegistrationDate] DATETIME NOT NULL, 
    [Name] NVARCHAR(50) NOT NULL, 
    [Summary] NVARCHAR(MAX) NULL, 
    [Token] NVARCHAR(50) NOT NULL, 
    [UserId] INT NOT NULL, 
    [ViewId] UNIQUEIDENTIFIER NULL, 
    CONSTRAINT [FK_Device_ToUser] FOREIGN KEY ([UserId]) REFERENCES [User]([Id])
)
