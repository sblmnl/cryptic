#!/bin/sh

dotnet tool restore
dotnet husky install

if [ ! -d .docker/https ]; then
  mkdir .docker/https
fi

https_cert_path="$HOME/.aspnet/https/localhost.pem"
https_key_path="$HOME/.aspnet/https/localhost.key"

if [ ! -f $https_cert_path ] || [ ! -f $https_cert_path ]; then
  dotnet dev-certs https \
    --export-path $cert_file_path \
    --format Pem \
    --no-password
fi

cp $https_cert_path .docker/https/
cp $https_key_path .docker/https/
chmod 644 .docker/https/*

cat <<EOF > .env
VITE_APP_PATH_BASE=""
EOF

cd client
npm install

cat <<EOF > .env
VITE_APP_PATH_BASE=""
EOF

cd ..
