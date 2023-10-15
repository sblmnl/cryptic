#!/bin/bash

# populate the .env file
cat <<EOF > .env
DB_NAME=cryptic
DB_USER=postgres
DB_PASSWORD=password
EOF
