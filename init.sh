#!/bin/bash

# generate the ssl/tls dev cert
dotnet dev-certs https -ep "${HOME}"/.aspnet/https/cryptic.pfx -p password
dotnet dev-certs https --trust

# populate the .env file
cat <<EOF > .env
# database
DB_NAME=cryptic
DB_USER=postgres
DB_PASSWORD=password

# ssl/tls
CERT_PASSWORD=password
CERT_DIR=/https
CERT_FILE=cryptic.pfx
EOF
