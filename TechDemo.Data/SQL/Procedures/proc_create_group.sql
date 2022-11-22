CREATE OR REPLACE PROCEDURE proc_create_group(
    IN "@group" c_group
)
LANGUAGE SQL
AS $$
INSERT INTO "group"
(
    "id",
    "name"
)
VALUES
    (
        "@group".id,
        "@group".name
    )
$$;


alter procedure proc_create_group(c_group) owner to sa;
