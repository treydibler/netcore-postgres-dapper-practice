CREATE OR REPLACE PROCEDURE proc_get_group_by_id(
    "@id" UUID
)
    LANGUAGE SQL
AS $$
SELECT
    "id",
    "name"
FROM "group" g
WHERE g.id = "@id"
$$;