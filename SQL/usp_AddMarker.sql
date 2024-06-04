use ImageLibrary
go

create or alter procedure AddMarker(
	@ImageId int
	,@MarkerId int
	,@MarkerName varchar(20)
)
as
begin
	if @MarkerName = 'Tag'
		exec AddTag @ImageId, @MarkerId
	else
	begin
		if @MarkerName = 'Character'
			exec AddCharacter @ImageId, @MarkerId
		else
		begin
			if @MarkerName = 'Author'
				exec AddAuthor @ImageId, @MarkerId
		end
	end	
end;