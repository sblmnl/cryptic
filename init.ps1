$DEFAULT_PASSWORD="password"

# generate the ssl/tls dev cert
dotnet dev-certs https -ep $env:USERPROFILE\.aspnet\https\cryptic.pfx -p $DEFAULT_PASSWORD
dotnet dev-certs https --trust

# populate the .env file
New-Item -Path ".env" -Value @"
# database
DB_NAME=cryptic
DB_USER=postgres
DB_PASSWORD=$DEFAULT_PASSWORD

# ssl/tls
CERT_PASSWORD=$DEFAULT_PASSWORD
CERT_DIR=/https
CERT_FILE=cryptic.pfx

# pgadmin
PGADMIN_USER=admin@pgadmin.org
PGADMIN_PASSWORD=$DEFAULT_PASSWORD
"@ -Force

# populate the pgAdmin servers file
New-Item -Path "pgadmin.json" -ItemType "File" -Value @"
{
    "Servers": {
        "1": {
            "Name": "cryptic",
            "Group": "Servers",
            "Port": 5432,
            "Username": "postgres",
            "Host": "cryptic-db",
            "SSLMode": "prefer",
            "MaintenanceDB": "postgres"
        }
    }
}
"@ -Force
