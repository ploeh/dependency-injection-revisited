CREATE DATABASE [Booking]
GO

USE [Booking]
GO

CREATE TABLE [dbo].[Reservations] (
    [Id]       INT                NOT NULL IDENTITY,
    [Date]     DATETIMEOFFSET (7) NOT NULL,
    [Name]     NVARCHAR (50)      NOT NULL,
    [Email]    NVARCHAR (50)      NOT NULL,
    [Quantity] INT                NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
)
GO