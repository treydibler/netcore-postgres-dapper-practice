CREATE OR REPLACE PROCEDURE proc_delete_group_user(
    "@group_id" UUID,
    "@user_id" UUID
)
LANGUAGE SQL
AS $$
DELETE FROM "group_user" gu
WHERE 
    gu.group_id = "@group_id"
AND 
    gu.user_id = "@user_id"
$$;