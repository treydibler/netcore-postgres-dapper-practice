CREATE TABLE IF NOT EXISTS "document_access" (
    "id" uuid DEFAULT gen_random_uuid() PRIMARY KEY,
    "document_id" uuid not null,
    "user_id" uuid null,
    "group_id" uuid null,
    
    CONSTRAINT "fk_document"
      FOREIGN KEY("document_id")
          REFERENCES "document"("id")
          ON DELETE CASCADE,
          
    CONSTRAINT "fk_group"
      FOREIGN KEY("group_id")
          REFERENCES "group"("id")
          ON DELETE CASCADE,
    UNIQUE ("document_id", "user_id"),
    UNIQUE ("document_id", "group_id")
);


DO $$ BEGIN
    CREATE TYPE "c_document_access" AS (
                                    "id" uuid,
                                    "document_id" uuid,
                                    "user_id" uuid,
                                    "group_id" uuid
                                );
EXCEPTION
    WHEN duplicate_object THEN null;
END $$;