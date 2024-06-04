USE ImageLibrary
GO

create or alter procedure GetImages(@offset INT)
as
begin
	select i.Address
	from dbo.Image as i

	left join (
		select ibt.Image_Id, string_agg(CAST(t.Name AS varchar),',') AS Tag
		from dbo.ImageByTag as ibt

		left join dbo.Tag as t
		on t.Id = ibt.Tag_Id
		group by ibt.Image_Id
	) as JoinTag
	on JoinTag.Image_Id = i.Id

	left join (
		select ibc.Image_Id, string_agg(cast(c.Name AS varchar),',') AS Character
		from dbo.ImageByCharacter as ibc

		left join dbo.Character as c
		on c.Id = ibc.Character_Id
		group by ibc.Image_Id
	) as JoinCharacter
	on JoinCharacter.Image_Id = i.Id

	left join (
		select iba.Image_Id, string_agg(cast(a.Name AS varchar),',') AS Author
		from dbo.ImageByAuthor as iba

		left join dbo.Author as a
		on a.Id = iba.Author_Id
		group by iba.Image_Id
	) as JoinAuthor
	on JoinAuthor.Image_Id = i.Id
	order by Address
	offset @offset rows
end;