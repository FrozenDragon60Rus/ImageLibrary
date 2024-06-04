USE ImageLibrary
GO

create or alter procedure AddTag(@imageId INT, @tagId INT)
as
begin
	merge into [ImageByTag] as ibt
	using (select Tag_Id = @tagId, Image_Id = @imageId) as new
	on ibt.Image_Id = new.Image_Id
		and ibt.Tag_Id = new.Tag_Id
	when matched then
		delete
	when not matched then
		insert
		values (@imageId, @tagId);
end;

go

create or alter procedure AddCharacter(@imageId INT, @characterId INT)
as
begin
	merge into [ImageByCharacter] as ibc
	using (select Tag_Id = @characterId, Image_Id = @imageId) as new
	on ibc.Image_Id = new.Image_Id
		and ibc.Character_Id = new.Tag_Id
	when matched then
		delete
	when not matched then
		insert
		values (@imageId, @characterId);
end;
go

create or alter procedure AddAuthor(@imageId INT, @authorId INT)
as
begin
	merge into [ImageByAuthor] as iba
	using (select Tag_Id = @authorId, Image_Id = @imageId) as new
	on iba.Image_Id = new.Image_Id
		and iba.Author_Id = new.Tag_Id
	when matched then
		delete
	when not matched then
		insert
		values (@imageId, @authorId);
end;