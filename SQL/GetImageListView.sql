USE ImageLibrary
GO

create or alter view ImageList
as
	select 
		i.Id, 
		i.Address,
		i.Rating,
		JoinTag.Tag,
		JoinCharacter.Character,
		JoinAuthor.Author
	from [dbo].[Image] as i

	left join (
		SELECT ibt.Image_Id, string_agg(cast(t.Name as varchar),',') as Tag
		FROM [dbo].[ImageByTag] as ibt

		LEFT JOIN [dbo].[Tag] as t
		on t.Id = ibt.Tag_Id
		group by ibt.Image_Id
	) as JoinTag
	on JoinTag.Image_Id = i.Id

	left join (
		select ibc.Image_Id, string_agg(cast(c.Name as varchar),',') as Character
		from [dbo].[ImageByCharacter] as ibc

		left join [dbo].[Character] as c
		on c.Id = ibc.Character_Id
		group by ibc.Image_Id
	) as JoinCharacter
	on JoinCharacter.Image_Id = i.Id

	left join (
		select iba.Image_Id, string_agg(cast(a.Name as varchar),',') as Author
		from [dbo].[ImageByAuthor] as iba

		left join [dbo].[Author] as a
		on a.Id = iba.Author_Id 
		group by iba.Image_Id
	) as JoinAuthor
	on JoinAuthor.Image_Id = i.Id;

go

select *
from ImageList