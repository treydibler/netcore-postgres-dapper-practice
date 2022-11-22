create procedure proc_create_document(IN "@document" c_document)
    language sql
as $$
INSERT INTO document
(
    "id",
    "name",
    "description",
    "category",
    "location",
    "hash",
    "added",
    "tags"
)
VALUES (
           "@document".id,
           "@document".name,
           "@document".description,
           "@document".category,
           "@document".location,
           "@document".hash,
           "@document".added,
           "@document".tags
       );
$$;

alter procedure proc_create_document(c_document) owner to sa;

