CREATE OR REPLACE PROCEDURE proc_list_user_groups(
    "@user_id" UUID
)
    LANGUAGE SQL
AS $$
SELECT
    g."id",
    g."name"
FROM "group" g
         INNER JOIN "document_access" a
                    ON a.group_id = g.id
         INNER JOIN "group_user" u
                    ON u.group_id = g.id
WHERE
        u.user_id = "@user_id"
ORDER BY
    g.name
$$;