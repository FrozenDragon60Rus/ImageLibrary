USE [ImageLibrary]
GO
/****** Object:  UserDefinedFunction [dbo].[TestFunc]    Script Date: 08.04.2024 9:34:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER FUNCTION [dbo].[MyRowCount]()
	RETURNS INT
AS
BEGIN
	DECLARE @count INT = 0;
	SELECT @count += 1
	FROM dbo.Image
	RETURN (@count)
END;