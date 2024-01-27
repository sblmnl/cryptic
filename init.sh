#!/bin/sh

DEFAULT_PASSWORD=password
CERT_FILE=cryptic.pfx

# generate the https certificate
if [ "$OSTYPE" = "darwin" ]; then
    if [ ! -f ~/.aspnet/https/$CERT_FILE ]; then
        dotnet dev-certs https -ep ~/.aspnet/https/$CERT_FILE -p $DEFAULT_PASSWORD
    fi

    dotnet dev-certs https --trust
else
    if  [ ! -f ~/.aspnet/https/$CERT_FILE ]; then
        cat <<EOF > localhost.conf
[req]
default_bits        = 2048
default_keyfile     = localhost.key
distinguished_name  = req_distinguished_name
req_extensions      = req_ext
x509_extensions     = v3_ca

[req_distinguished_name]
commonName          = Common Name (e.g. server FQDN or YOUR name)
commonName_default  = localhost
commonName_max      = 64

[req_ext]
subjectAltName      = @alt_names

[v3_ca]
subjectAltName      = @alt_names
basicConstraints    = critical, CA:false
keyUsage            = keyCertSign, cRLSign, digitalSignature,keyEncipherment

[alt_names]
DNS.1               = localhost
DNS.2               = 127.0.0.1
EOF

        openssl req -x509 -nodes -days 365 -newkey rsa:2048 -keyout localhost.key -out localhost.crt -config localhost.conf
        openssl pkcs12 -export -out $CERT_FILE -inkey localhost.key -in localhost.crt -password pass:${DEFAULT_PASSWORD}
        sudo cp localhost.crt /usr/local/share/ca-certificates/
        sudo update-ca-certificates
        
        if  [ ! -f /etc/ssl/certs/localhost.pem ]; then
            echo "ERROR: Failed to generate self-signed development certificate!"
            return
        fi

        if [ ! -d ${HOME}/.aspnet/https/ ]; then
            mkdir -p ${HOME}/.aspnet/https
        fi

        mv $CERT_FILE ${HOME}/.aspnet/https/
        rm localhost.crt localhost.key localhost.conf
    fi
fi

# populate the .env file
cat <<EOF > .env
# app
APP_ENVIRONMENT=Development

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
EOF
