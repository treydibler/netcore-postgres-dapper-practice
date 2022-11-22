CREATE OR REPLACE PROCEDURE proc_find_user_by_username(
    "@username" text
)
LANGUAGE SQL
AS $$
SELECT
    "id",
    "username",
    "credentials",
    "role"
FROM
    "user" u
WHERE
    u.username = "@username"
$$;