CReate procedure [dbo].[create_script1]
AS
begin
/*
        THIS iS A MULTILINE COmment
*/
drop table if exists dbo.script1 
-- single line comment
select * 
  -- Comment about this line
into dbo.script1 from dbo.script_staging

-- Comment at the end of select

end

-- Comment at the end of create proc