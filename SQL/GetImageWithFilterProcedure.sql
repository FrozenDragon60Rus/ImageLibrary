USE ImageLibrary
GO

IF TYPE_ID('VCHAR') IS NULL
	CREATE TYPE VCHAR FROM VARCHAR(1024);
GO

CREATE OR ALTER PROC GetImageListWithFilter(
	@Tag VCHAR, 
	@Character VCHAR,
	@Author VCHAR,
	@RatingFrom INT,
	@RatingTo INT,
	@Offset INT,
	@Count INT
)
AS
BEGIN

	IF OBJECT_ID('tempdb.dbo.#ListTag') IS NOT NULL
			DROP TABLE dbo.#ListTag;
		CREATE TABLE dbo.#ListTag(value VARCHAR(50));

	IF @Tag IS NOT NULL
	BEGIN
		
		INSERT INTO dbo.#ListTag
		SELECT *
		FROM STRING_SPLIT(@Tag,',');

		DECLARE @TagCount INT;
		SET @TagCount = (SELECT COUNT(dbo.#ListTag.value) FROM dbo.#ListTag)
	END;

	IF OBJECT_ID('tempdb.dbo.#ListCharacter') IS NOT NULL
			DROP TABLE dbo.#ListCharacter;
		CREATE TABLE dbo.#ListCharacter(value VARCHAR(50));

	IF @Character IS NOT NULL
	BEGIN
		INSERT INTO dbo.#ListCharacter
		SELECT *
		FROM STRING_SPLIT(@Character,',');

		DECLARE @CharacterCount INT;
		SET @CharacterCount = (SELECT COUNT(dbo.#ListCharacter.value) FROM dbo.#ListCharacter)
	END;

	IF OBJECT_ID('tempdb.dbo.#ListAuthor') IS NOT NULL
			DROP TABLE dbo.#ListAuthor;
		CREATE TABLE dbo.#ListAuthor(value VARCHAR(50));

	IF @Author IS NOT NULL
	BEGIN

		INSERT INTO dbo.#ListAuthor
		SELECT *
		FROM STRING_SPLIT(@Author,',');

		DECLARE @AuthorCount INT;
		SET @AuthorCount = (SELECT COUNT(dbo.#ListAuthor.value) FROM dbo.#ListAuthor)
	END;

	SELECT [Address]
	FROM Image
	LEFT JOIN(
		SELECT ImageByTag.Image_Id, COUNT(dbo.#ListTag.value) AS Tag_Count
		FROM ImageByTag
		LEFT JOIN Tag
		RIGHT JOIN dbo.#ListTag
		ON Tag.Name = dbo.#ListTag.value
		ON Tag.Id = ImageByTag.Tag_Id
		GROUP BY ImageByTag.Image_Id
		HAVING COUNT(dbo.#ListTag.value) = @TagCount
	) AS JoinTag 
	ON Image.Id = JoinTag.Image_Id
	LEFT JOIN(
		SELECT ImageByCharacter.Image_Id, COUNT(dbo.#ListCharacter.value) AS Character_Count
		FROM ImageByCharacter
		LEFT JOIN Character
		RIGHT JOIN dbo.#ListCharacter
		ON Character.Name = dbo.#ListCharacter.value
		ON Character.Id = ImageByCharacter.Character_Id
		GROUP BY ImageByCharacter.Image_Id
		HAVING COUNT(dbo.#ListCharacter.value) = @CharacterCount
	) AS JoinCharacter 
	ON Image.Id = JoinCharacter.Image_Id 
	LEFT JOIN(
		SELECT ImageByAuthor.Image_Id, COUNT(dbo.#ListAuthor.value) AS Author_Count
		FROM ImageByAuthor
		LEFT JOIN Author
		RIGHT JOIN dbo.#ListAuthor
		ON Author.Name = dbo.#ListAuthor.value
		ON Author.Id = ImageByAuthor.Author_Id
		GROUP BY ImageByAuthor.Image_Id
		HAVING COUNT(dbo.#ListAuthor.value) = @AuthorCount
	) AS JoinAuthor 
	ON Image.Id = JoinAuthor.Image_Id 
	WHERE Rating BETWEEN @RatingFrom AND @RatingTo
		AND dbo.OnMarker(@Tag, JoinTag.Tag_Count, @TagCount) = 1
		AND dbo.OnMarker(@Character, JoinCharacter.Character_Count, @CharacterCount) = 1
		AND dbo.OnMarker(@Author, JoinAuthor.Author_Count, @AuthorCount) = 1
	ORDER BY Rating DESC 
	OFFSET @Offset ROWS FETCH NEXT @Count ROWS ONLY
END;
GO

CREATE OR ALTER FUNCTION OnMarker(@marker VCHAR, @count INT, @markerCount INT)
RETURNS BIT
AS
BEGIN
	IF @marker IS NULL RETURN 1;
	IF @count = @markerCount RETURN 1
	RETURN 0;
END;

GO
EXEC GetImageListWithFilter 'android,angel', NULL, NULL, 0, 10, 0, 10;
EXEC GetImageListWithFilter NULL, NULL, NULL, 0, 10, 0, 10;
EXEC GetImageListWithFilter 'android,angel', NULL, NULL, 0, 10, 0, 10;