CREATE OR REPLACE PROCEDURE proc_list_users()
LANGUAGE SQL
AS $$
SELECT
    "id",
    "username",
    "role"
FROM
    "user" u
ORDER BY 
    u.username
$$;