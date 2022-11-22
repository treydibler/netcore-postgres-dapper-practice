CREATE OR REPLACE PROCEDURE proc_delete_user_by_id(
    "@user_id" UUID
)
LANGUAGE SQL
AS $$
DELETE
FROM
    "user" u
WHERE
    u.id = "@user_id"
$$;