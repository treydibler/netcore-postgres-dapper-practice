CREATE OR REPLACE PROCEDURE proc_list_group_users(
    "@group_id" UUID
)
    LANGUAGE SQL
AS $$
SELECT
    u."id",
    u."username"
FROM "user" u
         INNER JOIN "document_access" a
                    ON a.user_id = u.id
         INNER JOIN "group_user" gu
                    ON a.user_id = gu.user_id
         INNER JOIN "group" g
                    ON g.id = gu.group_id
WHERE
        g.id = "@group_id"
ORDER BY
    u.username
$$;