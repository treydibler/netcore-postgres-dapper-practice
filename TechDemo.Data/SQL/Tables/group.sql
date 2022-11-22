CREATE TABLE IF NOT EXISTS "group" (
    "id" uuid DEFAULT gen_random_uuid() PRIMARY KEY,
    "name" varchar(255) not null
);


DO $$ BEGIN
    CREATE TYPE "c_group" AS (
                               "id" uuid,
                               "name" varchar(255)
                           );
EXCEPTION
    WHEN duplicate_object THEN null;
END $$;