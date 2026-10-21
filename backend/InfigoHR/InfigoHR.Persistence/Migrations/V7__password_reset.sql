ALTER TABLE "Auths"
    ADD COLUMN "ResetCodeHash" VARCHAR(64),
    ADD COLUMN "ResetCodeExpiresAtUtc" TIMESTAMPTZ,
    ADD COLUMN "ResetRequestedAtUtc" TIMESTAMPTZ,
    ADD COLUMN "ResetAttempts" INT NOT NULL DEFAULT 0,
    ADD COLUMN "TokenVersion" UUID NOT NULL DEFAULT gen_random_uuid(),
    ADD COLUMN "ConcurrencyVersion" UUID NOT NULL DEFAULT gen_random_uuid();