$DEFAULT_PASSWORD = "password"

# populate the .env file
New-Item -Path ".env" -Value @"
# app
APP_ENVIRONMENT=Development

# database
DB_NAME=cryptic
DB_USER=postgres
DB_PASSWORD=$DEFAULT_PASSWORD

# https
CERT_HOST_DIR=./.dev/https
CERT_FILE=cryptic.pfx
CERT_PASSWORD=$DEFAULT_PASSWORD

# ports
WEBAPI_HTTPS_PORT=8443
WEBAPI_HTTP_PORT=8080
"@ -Force
