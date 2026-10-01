-- PostgreSQL initialization. EF Core applies the full schema on first API run.
-- This script is intentionally idempotent and establishes the database extension baseline.
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";
