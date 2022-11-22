CREATE OR REPLACE PROCEDURE proc_get_user_by_id(
    "@user_id" UUID
)
    LANGUAGE SQL
AS $$
SELECT
    (u).*
FROM
    "user" u
WHERE
    u.id = "@user_id"
$$;