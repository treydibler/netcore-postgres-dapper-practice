CREATE OR REPLACE PROCEDURE proc_grant_document_group(
    "@document_id" UUID,
    "@group_id" UUID
)
    LANGUAGE SQL
AS $$
INSERT INTO "document_access"
(
    "document_id",
    "group_id"
)
VALUES
    (
        "@document_id",
        "@group_id"
    )
ON CONFLICT ("document_id", "group_id") DO NOTHING;
$$;