#!/bin/sh

DEFAULT_PASSWORD=password

# generate the ssl/tls dev cert
dotnet dev-certs https -ep "${HOME}"/.aspnet/https/cryptic.pfx -p $DEFAULT_PASSWORD
dotnet dev-certs https --trust

# populate the .env file
cat <<EOF > .env
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
EOF

# populate the pgAdmin servers file
cat <<EOF > pgadmin.json
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
EOF
