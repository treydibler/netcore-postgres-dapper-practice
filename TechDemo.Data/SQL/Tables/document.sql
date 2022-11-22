CREATE TABLE IF NOT EXISTS "document" (
      "id" uuid DEFAULT gen_random_uuid() PRIMARY KEY,
      "name" varchar(255) not null,
      "description" varchar(1000) not null,
      "category" category null,
      "location" varchar(2048) not null,
      "hash" text null,
      "added" timestamp with time zone default (now() at time zone 'utc'),
      "tags" text not null
);

DO $$ BEGIN
    CREATE TYPE "c_document" AS (
            "id" uuid,
            "name" varchar(255),
            "description" varchar(1000),
            "category" category,
            "location" varchar(2048),
            "hash" text,
            "added" timestamp with time zone,
            "tags" text
        );
EXCEPTION
    WHEN duplicate_object THEN null;
END $$;