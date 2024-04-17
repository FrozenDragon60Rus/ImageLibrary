USE ImageLibrary
GO

CREATE OR ALTER PROCEDURE GetImages(@offset INT)
AS
BEGIN
	SELECT [dbo].[Image].Address
	FROM [dbo].[Image]

	LEFT JOIN (
		SELECT [dbo].[ImageByTag].Image_Id, STRING_AGG(CAST(Tag.Name AS VARCHAR),',') AS Tag
		FROM [dbo].[ImageByTag]

		LEFT JOIN [dbo].[Tag]
		ON [dbo].[ImageByTag].Tag_Id = [dbo].[Tag].Id
		GROUP BY [dbo].[ImageByTag].Image_Id
	) AS JoinTag
	ON [dbo].[Image].Id = JoinTag.Image_Id

	LEFT JOIN (
		SELECT [dbo].[ImageByCharacter].Image_Id, STRING_AGG(CAST(Character.Name AS VARCHAR),',') AS Character
		FROM [dbo].[ImageByCharacter]

		LEFT JOIN [dbo].[Character]
		ON [dbo].[ImageByCharacter].Character_Id = Character.Id
		GROUP BY [dbo].[ImageByCharacter].Image_Id
	) AS JoinCharacter
	ON [dbo].[Image].Id = JoinCharacter.Image_Id

	LEFT JOIN (
		SELECT [dbo].[ImageByAuthor].Image_Id, STRING_AGG(CAST(Author.Name AS VARCHAR),',') AS Author
		FROM [dbo].[ImageByAuthor]

		LEFT JOIN [dbo].[Author]
		ON [dbo].[ImageByAuthor].Author_Id = [dbo].[Author].Id
		GROUP BY [dbo].[ImageByAuthor].Image_Id
	) AS JoinAuthor
	ON [dbo].[Image].Id = JoinAuthor.Image_Id
	ORDER BY Address
	OFFSET @offset ROWS
END;