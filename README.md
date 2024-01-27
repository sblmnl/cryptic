# Cryptic

Utility for securely sharing notes over the web

## Getting Started

### Development Environment Setup

Run `./init.sh` (Mac/Linux) or `./init.ps1` (Windows).

### Running the Application

#### Docker Profiles

**Infrastructure Only**  
Only runs the infrastructure services. This allows the application to be run locally and debugged in your IDE.

Command:  
`docker compose up`  

**Full Stack (full)**  
Runs the full stack application and all necessary services.

Command:  
`docker compose --profile full up`  

**Web API Only (api)**  
Runs the web API and all necessary services.

Command:  
`docker compose --profile api up`  
