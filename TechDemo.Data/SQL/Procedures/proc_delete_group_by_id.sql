CREATE OR REPLACE PROCEDURE proc_delete_group_by_id(
    "@group_id" UUID
)
LANGUAGE SQL
AS $$
DELETE FROM
    "group" g
WHERE
    g.id = "@group_id"
$$;