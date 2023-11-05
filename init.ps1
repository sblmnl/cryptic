# generate the ssl/tls dev cert
dotnet dev-certs https -ep $env:USERPROFILE\.aspnet\https\cryptic.pfx -p password
dotnet dev-certs https --trust

# populate the .env file
Add-Content -Path ".env" -Value @"
# database
DB_NAME=cryptic
DB_USER=postgres
DB_PASSWORD=password

# ssl/tls
CERT_PASSWORD=password
CERT_DIR=/https
CERT_FILE=cryptic.pfx
"@