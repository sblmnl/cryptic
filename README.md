# Cryptic

Utility for securely sharing notes over the web

## Getting Started

### Development Environment Setup

Run `./init.sh` (Mac/Linux) or `./init.ps1` (Windows).

### Running the Application

#### Docker Profiles

**Full Stack (full)**  
Runs the full stack application and all infrastructure services.

Command:  
`docker compose --profile full up`  

**Infrastructure Only (infra)**  
Only runs the infrastructure services (i.e. Postgres). This is useful during development, when you might want to be able to run the front-end or back-end from your IDE.

Command:  
`docker compose --profile infra up`
