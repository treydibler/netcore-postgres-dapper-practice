CREATE TABLE IF NOT EXISTS "user" (
    "id" uuid DEFAULT gen_random_uuid() PRIMARY KEY,
    "username" varchar(320) UNIQUE not null,
    "credentials" jsonb not null,
    "role" "role" not null DEFAULT ('USER')
);

DO $$ BEGIN
    CREATE TYPE "c_user" AS (
                                "id" uuid,
                                "username" varchar(320),
                                "credentials" jsonb,
                                "role" "role"
                              );
EXCEPTION
    WHEN duplicate_object THEN null;
END $$;