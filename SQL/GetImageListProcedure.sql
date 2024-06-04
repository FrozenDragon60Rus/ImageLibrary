USE ImageLibrary
GO

/*CREATE OR ALTER PROCEDURE GetImageList(@Id INT)
AS
BEGIN
	SELECT [dbo].[Image].Id, 
		 [dbo].[Image].Address,
		 [dbo].[Image].Rating,
		 JoinTag.Tag,
		 JoinCharacter.Character,
		 JoinAuthor.Author
	FROM [dbo].[Image]

	LEFT JOIN (
		SELECT [dbo].[ImageByTag].Image_Id, STRING_AGG(CAST(Tag.Name AS VARCHAR(1024)),',') AS Tag
		FROM [dbo].[ImageByTag]

		LEFT JOIN [dbo].[Tag]
		ON [dbo].[ImageByTag].Tag_Id = [dbo].[Tag].Id
		GROUP BY [dbo].[ImageByTag].Image_Id
	) AS JoinTag
	ON [dbo].[Image].Id = JoinTag.Image_Id

	LEFT JOIN (
		SELECT [dbo].[ImageByCharacter].Image_Id, STRING_AGG(CAST(Character.Name AS VARCHAR(1024)),',') AS Character
		FROM [dbo].[ImageByCharacter]

		LEFT JOIN [dbo].[Character]
		ON [dbo].[ImageByCharacter].Character_Id = Character.Id
		GROUP BY [dbo].[ImageByCharacter].Image_Id
	) AS JoinCharacter
	ON [dbo].[Image].Id = JoinCharacter.Image_Id

	LEFT JOIN (
		SELECT [dbo].[ImageByAuthor].Image_Id, STRING_AGG(CAST(Author.Name AS VARCHAR(1024)),',') AS Author
		FROM [dbo].[ImageByAuthor]

		LEFT JOIN [dbo].[Author]
		ON [dbo].[ImageByAuthor].Author_Id = [dbo].[Author].Id
		GROUP BY [dbo].[ImageByAuthor].Image_Id
	) AS JoinAuthor
	ON [dbo].[Image].Id = JoinAuthor.Image_Id
	WHERE [dbo].[Image].Id = ISNULL(@Id, [dbo].[Image].Id)
END;

GO*/

create or alter function GetImageList()
returns table
as
return
	select 
		i.Id, 
		i.Address,
		i.Rating,
		JoinTag.Tag,
		JoinCharacter.Character,
		JoinAuthor.Author
	from [dbo].[Image] as i

	left join (
		SELECT ibt.Image_Id, STRING_AGG(CAST(t.Name AS VARCHAR),',') AS Tag
		FROM [dbo].[ImageByTag] as ibt

		LEFT JOIN [dbo].[Tag] as t
		ON t.Id = ibt.Tag_Id
		group by ibt.Image_Id
	) AS JoinTag
	ON JoinTag.Image_Id = i.Id

	left join (
		SELECT [dbo].[ImageByCharacter].Image_Id, STRING_AGG(CAST(Character.Name AS VARCHAR(1024)),',') AS Character
		FROM [dbo].[ImageByCharacter]

		LEFT JOIN [dbo].[Character]
		ON [dbo].[ImageByCharacter].Character_Id = Character.Id
		GROUP BY [dbo].[ImageByCharacter].Image_Id
	) AS JoinCharacter
	ON JoinCharacter.Image_Id = i.Id

	left join (
		SELECT [dbo].[ImageByAuthor].Image_Id, STRING_AGG(CAST(Author.Name AS VARCHAR(1024)),',') AS Author
		FROM [dbo].[ImageByAuthor]

		LEFT JOIN [dbo].[Author]
		ON [dbo].[ImageByAuthor].Author_Id = [dbo].[Author].Id
		GROUP BY [dbo].[ImageByAuthor].Image_Id
	) AS JoinAuthor
	ON JoinAuthor.Image_Id = i.Id