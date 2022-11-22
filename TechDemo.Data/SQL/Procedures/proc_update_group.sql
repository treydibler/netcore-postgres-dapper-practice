CREATE OR REPLACE PROCEDURE proc_update_group(
    "@group_id" UUID,
    "@name" varchar(255)
)
    LANGUAGE SQL
AS $$
UPDATE "group"
SET
    "name" = "@name"
WHERE "id" = "@group_id"
$$;

alter procedure proc_update_group(uuid, varchar(255)) owner to sa;
