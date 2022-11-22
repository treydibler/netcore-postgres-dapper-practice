CREATE OR REPLACE PROCEDURE proc_create_user(
    IN "@user" "c_user"
)
LANGUAGE SQL
AS $$
INSERT INTO "user"
(
 "id",
 "username",
 "credentials",
 "role"
)
VALUES
    (
        "@user".id,
        "@user".username,
        "@user".credentials,
        "@user".role
    )
$$;




alter procedure proc_create_user(c_user) owner to sa;
