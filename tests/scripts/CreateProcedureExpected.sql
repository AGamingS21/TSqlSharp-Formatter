CREATE PROCEDURE dbo.create_script1
AS
BEGIN
    /*
        THIS iS A MULTILINE COmment
*/
    DROP TABLE IF EXISTS dbo.script1
    -- single line comment
    SELECT
        *        -- Comment about this line
    INTO
        dbo.script1
    FROM
        dbo.script_staging    -- Comment at the end of select
END
-- Comment at the end of create proc