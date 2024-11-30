#!/bin/sh

# create .dev directory
[ ! -d .dev ] && mkdir .dev

# generate ssl certificate
[ ! -d .dev/ssl ] && mkdir .dev/ssl

cat <<EOF > .dev/ssl/localhost.conf
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

openssl genrsa -out .dev/ssl/localhost.key
openssl req -new -key .dev/ssl/localhost.key -out .dev/ssl/localhost.csr -config .dev/ssl/localhost.conf
openssl x509 -req -in .dev/ssl/localhost.csr -signkey .dev/ssl/localhost.key -out .dev/ssl/localhost.crt -extensions req_ext -extfile .dev/ssl/localhost.conf
openssl pkcs12 -export -out .dev/ssl/localhost.pfx -inkey .dev/ssl/localhost.key -in .dev/ssl/localhost.crt -passout pass:password
