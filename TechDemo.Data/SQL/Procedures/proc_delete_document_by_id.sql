CREATE OR REPLACE PROCEDURE proc_delete_document_by_id(
    "@id" UUID
)
    LANGUAGE SQL
AS $$
DELETE FROM "document" d
WHERE d.id = "@id"
$$;