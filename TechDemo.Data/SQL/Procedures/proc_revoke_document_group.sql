CREATE OR REPLACE PROCEDURE proc_revoke_document_group(
    "@document_id" UUID, 
    "@group_id" UUID
)
LANGUAGE SQL
AS $$
DELETE FROM "document_access" a
WHERE 
    a.group_id = "@group_id"
AND
    a.document_id = "@document_id"
$$;