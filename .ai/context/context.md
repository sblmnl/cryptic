# Global AI Agent Context

This file is the canonical source for agent context. `CLAUDE.md` and `AGENTS.md` at the repo root are symlinks pointing here — do not edit those files directly.

This file serves as the main context entrypoint for AI agents and provides a high-level overview of the project.

## Overview

**Cryptic** is a web application for securely sharing data over the web using end-to-end encryption. Content is encrypted client-side before transmission; the server stores only opaque ciphertext and never has access to decryption keys.

Core Features:

- Notes (implemented)
- Files (future)
- Chat (future)

## Architecture

The application follows a clean separation of concerns across three layers:

1. **`Cryptic.Core`** — Domain logic, persistence (EF Core), commands/handlers, value objects
2. **`Cryptic.Web.Server`** — ASP.NET Core Minimal API HTTP layer; maps HTTP requests to commands
3. **Frontend (`client/`)** — Vue 3 SPA; handles all client-side encryption before sending to server

Key architectural patterns:

- **CQRS-lite via LiteBus**: Commands encapsulate intent; handlers return `Result<T>` (FluentResults)
- **Results over exceptions**: `Result<T>` propagates success/failure with typed errors; no throwing
- **Strongly-typed IDs**: Domain entities use `StronglyTypedId`-generated types (e.g. `NoteId`)
- **Client-side E2E encryption**: AES-GCM with Argon2id KDF; encryption parameters stored in `clientMetadata`

## Tech Stack

### Backend

- **Language & Framework**: C# / ASP.NET Core Minimal API / .NET 10
- **ORM**: Entity Framework Core 10 + Npgsql (PostgreSQL)
- **Command bus**: LiteBus 4.3.0
- **Result type**: FluentResults 4.0.0
- **Cryptography**: BouncyCastle.Cryptography 2.6.2 (Argon2id password hashing)
- **Serialization**: `System.Text.Json` (camelCase) + `Newtonsoft.Json` for custom types
- **ID generation**: StronglyTypedId 1.0.0-beta08
- **DB naming**: `EFCore.NamingConventions` (snake_case)

### Frontend

- **Framework**: TypeScript + Vue 3 (Composition API, `<script setup>`)
- **UI**: Quasar Framework 2 (Material Design components)
- **Routing**: Vue Router 4
- **State**: Pinia 3 (installed; not actively used — state managed locally in components)
- **HTTP**: Axios 1.13 with a preconfigured `/api` base instance
- **Cryptography**: libsodium-wrappers-sumo 0.8 (Argon2id KDF) + Web Crypto API (AES-GCM)
- **Build**: Vite (via Quasar CLI)

### Infrastructure

- **Database**: PostgreSQL 18 (Alpine, Docker)
- **Containers**: Docker Compose — `db`, `server`, `cron` (Supercronic for scheduled tasks)
- **CI**: GitHub Actions — build on push/PR to `main` (.NET 10 + Node 22)

## Project Layout

```text
/
├── client/               # Vue 3 frontend (TypeScript)
│   ├── src/
│   │   ├── boot/         # Axios instance, dark mode, Quasar defaults
│   │   ├── components/   # App-level components (e.g. AppHeader)
│   │   ├── css/          # Global SCSS (app.scss, quasar.variables.scss, typography.scss)
│   │   ├── layouts/      # DefaultAppLayout + AppLayout enum
│   │   ├── pages/        # App-level pages (ContributePage, NotFoundPage)
│   │   ├── router/       # Vue Router instance + route definitions
│   │   ├── features/     # Vertical slices — one folder per domain feature
│   │   │   └── notes/
│   │   │       ├── api/        # HTTP request/response types + send*Request functions
│   │   │       ├── components/ # Feature-scoped Vue components
│   │   │       ├── pages/      # Route-level page components
│   │   │       └── *.ts        # Domain/business logic at feature root (e.g. note.ts)
│   │   └── shared/       # Cross-feature utilities and types
│   │       ├── api/      # Shared HTTP envelope types (OkHttpResponseBody, etc.)
│   │       ├── components/ # Reusable components (PasswordEntryPopup)
│   │       ├── types/    # Shared domain types (DeleteAfter, CodedError)
│   │       └── util/     # Utilities — crypto (AES-GCM, Argon2), encoding, json, time
│   └── tests/
│       ├── unit/         # Vitest unit tests — mirrors src/ structure
│       │   ├── shared/   # Tests for shared/
│       │   └── features/ # Tests for features/
│       └── e2e/          # Playwright end-to-end tests
├── server/
│   ├── src/
│   │   ├── Cryptic.Core/             # Domain + persistence
│   │   └── Cryptic.Web.Server/       # HTTP layer (Minimal API)
│   └── tests/
│       ├── Cryptic.Core.Tests/       # Domain unit/integration tests (xUnit)
│       └── Cryptic.Web.Server.Tests/ # Endpoint tests (xUnit)
├── .docker/              # Docker support files (SSL certs, etc.)
├── .github/workflows/    # CI/CD (GitHub Actions)
├── docker-compose.yml
├── .editorconfig         # Code style (C# + general)
├── Directory.Build.props
└── Directory.Packages.props  # Centralized NuGet versioning
```

## HTTP API Conventions

All responses follow a consistent envelope:

```json
{ "status": "ok", "data": { ... } }          // success
{ "status": "failed", "errors": [ ... ] }    // failure
```

Errors include a `code` field and optional `metadata` for client-side handling.

## Additional Context Files

- [Development](development.md) — Code style, naming conventions, patterns for C# and TypeScript
- [Testing](testing.md) — Testing frameworks, patterns, and conventions for backend and frontend
