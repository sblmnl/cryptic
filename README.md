# Cryptic

Utility for securely sharing notes over the web

## Getting Started

### Development Environment Setup

Prerequisites:

- [.NET 9 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)
- [Node.js](https://nodejs.org)

Run the `init.sh` (Linux/Mac) or `init.ps1` (Windows) to initialize the development environment for the project.

### Running the Application

#### Docker Profiles

**Infrastructure Only**
Only runs the infrastructure services. This allows the application to be run locally and debugged in your IDE.

Command:
`docker compose up`

**Web Application (app)**
Runs the full stack application and all necessary services.

Command:
`docker compose --profile app up`
