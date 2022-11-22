CREATE OR REPLACE PROCEDURE proc_create_group_user(
    IN "@group_user" c_group_user
)
LANGUAGE SQL
AS $$
INSERT INTO "group_user"
(
    "id",
    "group_id",
    "user_id"
)
VALUES
    (
        "@group_user".id,
        "@group_user".group_id,
        "@group_user".user_id
    )
$$;