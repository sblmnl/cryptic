# Cryptic

Utility for securely sharing notes over the web

## Getting Started

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
