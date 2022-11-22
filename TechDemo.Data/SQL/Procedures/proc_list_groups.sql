CREATE OR REPLACE PROCEDURE proc_list_groups()
    LANGUAGE SQL
AS $$
SELECT
    "id",
    "name"
FROM
    "group" g
ORDER BY
    g.name
$$;