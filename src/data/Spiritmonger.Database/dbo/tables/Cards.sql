CREATE TABLE [dbo].[Cards]
(
	[Id] UNIQUEIDENTIFIER NOT NULL, 
	[Name] NVARCHAR(128) NOT NULL,
	[MultiverseId] INT NULL,
	[Expansion] NVARCHAR(MAX) NULL,
	[ImageUrl] NVARCHAR(MAX) NULL,
	[CardNameId] UNIQUEIDENTIFIER NULL,
    CONSTRAINT [PK_Cards] PRIMARY KEY CLUSTERED ([Id]),
	CONSTRAINT [FK_CardNames_Cards_CardNameId] FOREIGN KEY ([CardNameId]) REFERENCES [dbo].[CardNames] ([Id])
)
GO

CREATE INDEX [IX_Cards_Name] ON [dbo].[Cards] ([Name])
GO
