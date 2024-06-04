USE ImageLibrary
GO

if type_id('vchar') is null
	create type vchar from varchar(1024);
go

create or alter procedure GetImageListWithFilter(
	@Tag vchar 
	,@Character vchar
	,@Author vchar
	,@RatingFrom int
	,@RatingTo int
	--,@Offset int
	--,@Fetch int
	--,@Count int output
)
as
begin
	set nocount on

	if object_id('tempdb..#ListTag') is not null
		drop table dbo.#ListTag;

	create table dbo.#ListTag(value varchar(50));

	if @Tag is not null
	begin
		insert into #ListTag
		select t.value
		from string_split(@Tag,',') as t;

		declare @TagCount int = @@rowcount;
	end;

	if object_id('tempdb..#ListCharacter') is not null
		drop table dbo.#ListCharacter;

	create table dbo.#ListCharacter(value varchar(50));

	if @Character is not null
	begin
		insert into #ListCharacter
		select c.value
		from string_split(@Character,',') as c;

		declare @CharacterCount int = @@rowcount;
	end;

	if object_id('tempdb..#ListAuthor') is not null
		drop table #ListAuthor;

	create table #ListAuthor(value varchar(50));

	if @Author is not null
	begin
		insert into #ListAuthor
		select a.value
		from string_split(@Author,',') as a;

		declare @AuthorCount int = @@rowcount;
	end;

	select i.Address
	from dbo.Image as i
		left join(
			select 
				ibt.Image_Id 
				,count(lt.value) as TagCount
			from dbo.ImageByTag as ibt
				left join dbo.Tag as t on t.Id = ibt.Tag_Id
				right join #ListTag as lt on lt.value = t.Name
			group by ibt.Image_Id
			having count(lt.value) = @TagCount
		) as t on t.Image_Id = i.Id
		left join(
			select 
				ibc.Image_Id 
				,count(lc.value) AS CharacterCount
			from dbo.ImageByCharacter as ibc
				left join dbo.Character as c on c.Id = ibc.Character_Id
				right join #ListCharacter as lc on lc.value = c.Name
			group by ibc.Image_Id
			having count(lc.value) = @CharacterCount
		) as c on c.Image_Id = i.Id 
		left join(
			select 
				iba.Image_Id 
				,count(la.value) as AuthorCount
			from dbo.ImageByAuthor as iba
				left join dbo.Author as a on a.Id = iba.Author_Id
				right join #ListAuthor as la on la.value = a.Name
			group by iba.Image_Id
			having count(la.value) = @AuthorCount
		) as a on a.Image_Id = i.Id 
	where i.Rating between @RatingFrom and @RatingTo
		and dbo.FlagMarker(@Tag, t.TagCount, @TagCount) = 1
		and dbo.FlagMarker(@Character, c.CharacterCount, @CharacterCount) = 1
		and dbo.FlagMarker(@Author, a.AuthorCount, @AuthorCount) = 1
	order by Rating desc;
    --offset @Offset rows fetch next @Fetch rows only;

	set nocount off
end

go

create or alter function FlagMarker(@marker vchar, @count int, @markerCount int)
returns bit
as
begin
	if @marker is null return 1;
	if @count = @markerCount return 1
	return 0;
end

go

exec GetImageListWithFilter 'android,angel', NULL, NULL, 0, 10
exec GetImageListWithFilter NULL, NULL, NULL, 0, 10