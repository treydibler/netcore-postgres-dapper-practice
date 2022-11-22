CREATE OR REPLACE PROCEDURE proc_get_document_by_id(@user_id UUID, @document_id UUID)
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
        d.id = @document_id
  AND
        u.user_id = @user_id
ORDER BY d.added DESC
$$;