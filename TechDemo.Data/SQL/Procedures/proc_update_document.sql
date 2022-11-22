CREATE OR REPLACE PROCEDURE proc_update_document(
    IN "@document" c_document
)
LANGUAGE SQL
AS $$
UPDATE document
SET
    "name" = "@document".name,
    "description" = "@document".description,
    "category" = "@document".category,
    "location" = "@document".location,
    "hash" = "@document".hash,
    "tags" = "@document".tags
WHERE "id" = "@document".id
$$;


alter procedure proc_update_document(c_document) owner to sa;

