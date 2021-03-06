﻿CREATE TABLE [dbo].[Cards]
(
	[Id] UNIQUEIDENTIFIER NOT NULL, 
	[Name] NVARCHAR(256) NOT NULL,
	[MultiverseId] INT NULL,
	[Expansion] NVARCHAR(MAX) NULL,
	[ImageUrl] NVARCHAR(MAX) NULL,
	[CardNameId] UNIQUEIDENTIFIER NULL,
	[CardType] NVARCHAR(256) NULL,
	[ManaCost] NVARCHAR(64) NULL,
	[Mana] NVARCHAR(64) NULL,
	[MKM_price] DECIMAL(19, 5) NULL,
	[CKD_price] DECIMAL(19, 5) NULL,
	[TIX_price] DECIMAL(19, 5) NULL,	
    CONSTRAINT [PK_Cards] PRIMARY KEY CLUSTERED ([Id]),
	CONSTRAINT [FK_CardNames_Cards_CardNameId] FOREIGN KEY ([CardNameId]) REFERENCES [dbo].[CardNames] ([Id])
)
GO

CREATE INDEX [IX_Cards_Name] ON [dbo].[Cards] ([Name])
GO
