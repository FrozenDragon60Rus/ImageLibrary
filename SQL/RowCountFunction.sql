USE [ImageLibrary]
GO
/****** Object:  UserDefinedFunction [dbo].[TestFunc]    Script Date: 08.04.2024 9:34:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

create or alter function dbo.MyRowCount()
	returns int
as
begin
	declare @Count int = 0;

	select @Count += 1
	from dbo.Image
	return (@Count)
end;