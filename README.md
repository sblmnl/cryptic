# Cryptic

Utility for securely sharing notes over the web

## Getting Started

### Development Environment Setup

Prerequisites:

- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- [Node.js](https://nodejs.org)

Run `npm install` in the root of the repository to enable git hooks.

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
