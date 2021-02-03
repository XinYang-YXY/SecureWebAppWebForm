CREATE TABLE [dbo].[Account]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [firstName] NCHAR(50) NULL, 
    [lastName] NCHAR(50) NULL, 
    [creditCardNum] NCHAR(20) NULL, 
    [email] NCHAR(20) NULL, 
    [passwordHash] NVARCHAR(MAX) NULL, 
    [passwordSalt] NVARCHAR(MAX) NULL, 
    [dob] DATE NULL, 
    [creditCardPin] NCHAR(10) NULL, 
    [creditCardExpireDate] NCHAR(10) NULL
)
