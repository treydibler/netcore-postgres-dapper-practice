CREATE OR REPLACE PROCEDURE proc_list_document_by_category(
    "@user_id" UUID, 
    "@category" category
)
LANGUAGE SQL
AS $$
SELECT
    (d).*
FROM
    "document" d
        LEFT OUTER JOIN "document_access" a
                        ON a.document_id = d.id
        LEFT OUTER JOIN "group" g
                        ON g.id = a.group_id
        LEFT OUTER JOIN "group_user" u
                        ON u.group_id = g.id
WHERE
    d.category = "@category"
AND
    u.user_id = "@user_id"
ORDER BY 
    d.added DESC 
$$;