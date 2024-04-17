USE ImageLibrary
GO

CREATE OR ALTER PROCEDURE AddTag(@imageId INT, @tagId INT)
AS
BEGIN
	MERGE INTO [ImageByTag]
	USING (SELECT Tag_Id = @tagId, Image_Id = @imageId) as new
	ON [dbo].[ImageByTag].Image_Id = new.Image_Id
	AND [dbo].[ImageByTag].Tag_Id = new.Tag_Id
	WHEN MATCHED THEN
		DELETE
	WHEN NOT MATCHED THEN
		INSERT
		VALUES (@imageId, @tagId);
END;
GO

CREATE OR ALTER PROCEDURE AddCharacter(@imageId INT, @characterId INT)
AS
BEGIN
	MERGE INTO [ImageByCharacter]
	USING (SELECT Tag_Id = @characterId, Image_Id = @imageId) as new
	ON [dbo].[ImageByCharacter].Image_Id = new.Image_Id
	AND [dbo].[ImageByCharacter].Character_Id = new.Tag_Id
	WHEN MATCHED THEN
		DELETE
	WHEN NOT MATCHED THEN
		INSERT
		VALUES (@imageId, @characterId);
END;
GO

CREATE OR ALTER PROCEDURE AddAuthor(@imageId INT, @authorId INT)
AS
BEGIN
	MERGE INTO [ImageByAuthor]
	USING (SELECT Tag_Id = @authorId, Image_Id = @imageId) as new
	ON [dbo].[ImageByAuthor].Image_Id = new.Image_Id
	AND [dbo].[ImageByAuthor].Author_Id = new.Tag_Id
	WHEN MATCHED THEN
		DELETE
	WHEN NOT MATCHED THEN
		INSERT
		VALUES (@imageId, @authorId);
END;