CREATE OR REPLACE PROCEDURE proc_revoke_document_user(
    "@document_id" UUID, 
    "@user_id" UUID
)
LANGUAGE SQL
AS $$
DELETE FROM "document_access" a
WHERE
    a.user_id = "@user_id"
AND
    a.document_id = "@document_id"
$$;