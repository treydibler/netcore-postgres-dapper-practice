CREATE TABLE IF NOT EXISTS "group_user" (
    "id" uuid DEFAULT gen_random_uuid() PRIMARY KEY,
    "group_id" uuid not null,
    "user_id" uuid not null,
    UNIQUE ("group_id", "user_id")
);

DO $$ BEGIN
    CREATE TYPE "c_group_user" AS (
                                 "id" uuid,
                                 "group_id" uuid,
                                 "user_id" uuid
                             );
EXCEPTION
    WHEN duplicate_object THEN null;
END $$;