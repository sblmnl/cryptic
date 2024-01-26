# Cryptic

Utility for securely sharing notes over the web

## Getting Started

### Development Environment Setup

Run `./init.sh` (Mac/Linux) or `./init.ps1` (Windows).

### Running the Application

#### Docker Profiles

**Production (prod)**  
Runs the full stack application and all necessary services.

Command:  
`docker compose --profile prod up`  

**Development (dev)**  
Only runs the development services (Postgres and pgAdmin). This is when you might want to be able to debug the app in your IDE.

Command:  
`docker compose --profile dev up`
