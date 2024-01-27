$DEFAULT_PASSWORD = "password"
$CERT_FILE = "cryptic.pfx"

# generate the https certificate
if (!(Test-Path -Path "$env:USERPROFILE\.aspnet\https\$CERT_FILE")) {
    dotnet dev-certs https -ep $env:USERPROFILE\.aspnet\https\$CERT_FILE -p $DEFAULT_PASSWORD
}

dotnet dev-certs https --trust

# populate the .env file
New-Item -Path ".env" -Value @"
# database
DB_NAME=cryptic
DB_USER=postgres
DB_PASSWORD=$DEFAULT_PASSWORD

# https
CERT_FILE=$CERT_FILE
CERT_PASSWORD=$DEFAULT_PASSWORD

# ports
WEBAPI_HTTPS_PORT=8443
WEBAPI_HTTP_PORT=8080
"@ -Force
