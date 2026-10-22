-- DO $$
-- BEGIN
--     IF EXISTS (SELECT 1 FROM "TwoFactorAuths") THEN
--         RAISE EXCEPTION
--             'Existing 2FA rows require secret conversion before applying V5';
--     END IF;
-- END $$;

ALTER TABLE "TwoFactorAuths"
    RENAME COLUMN "SecretKey" TO "ProtectedSecret";

ALTER TABLE "TwoFactorAuths"
    ALTER COLUMN "ProtectedSecret" TYPE TEXT,
    ADD COLUMN "IsEnabled" BOOLEAN NOT NULL DEFAULT FALSE,
    ADD COLUMN "SetupId" UUID NOT NULL
        DEFAULT '00000000-0000-0000-0000-000000000000',
    ADD COLUMN "SetupTokenVersion" UUID NOT NULL
        DEFAULT '00000000-0000-0000-0000-000000000000',
    ADD COLUMN "SetupExpiresAtUtc" TIMESTAMPTZ,
    ADD COLUMN "LastAcceptedTimeStep" BIGINT,
    ADD COLUMN "FailedAttempts" INTEGER NOT NULL DEFAULT 0,
    ADD COLUMN "LockedUntilUtc" TIMESTAMPTZ,
    ADD COLUMN "RecoveryCodeHashes" TEXT[] NOT NULL
        DEFAULT ARRAY[]::TEXT[],
    ADD COLUMN "ChallengeTokenHash" VARCHAR(64),
    ADD COLUMN "ChallengeTokenVersion" UUID,
    ADD COLUMN "ChallengeExpiresAtUtc" TIMESTAMPTZ,
    ADD CONSTRAINT "CK_TwoFactorAuths_FailedAttempts"
        CHECK ("FailedAttempts" >= 0),
    ADD CONSTRAINT "UQ_TwoFactorAuths_ChallengeTokenHash"
        UNIQUE ("ChallengeTokenHash");