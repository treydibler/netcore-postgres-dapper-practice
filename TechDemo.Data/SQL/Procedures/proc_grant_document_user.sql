CREATE OR REPLACE PROCEDURE proc_grant_document_user(
    "@document_id" UUID, 
    "@user_id" UUID
)
LANGUAGE SQL
AS $$
INSERT INTO "document_access"
    (
        "document_id",
        "user_id"
    )
VALUES
    (
        "@document_id",
        "@user_id"
    )
ON CONFLICT ("document_id", "user_id") DO NOTHING;
$$;