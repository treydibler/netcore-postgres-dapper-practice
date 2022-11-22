CREATE OR REPLACE PROCEDURE proc_update_user(
    IN "@user" c_user
)
    LANGUAGE SQL
AS $$
UPDATE "user"
SET
    "credentials" = "@user".credentials,
    "role" = "@user".role
WHERE "id" = "@user".id
$$;

alter procedure proc_update_user(c_user) owner to sa;
