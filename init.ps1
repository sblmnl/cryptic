dotnet tool restore
dotnet husky install

cd client
npm install

(New-Item -Path ".env" -Value @"
VITE_APP_NAME="Cryptic"
VITE_CLIENT_NAME="cryptic"
VITE_CLIENT_VERSION="0.0.1"
#VITE_ROUTER_BASE=""
VITE_ROUTER_MODE="history"
"@ -Force);

cd ..

(New-Item -Path ".env" -Value @"
VITE_APP_NAME="Cryptic"
VITE_CLIENT_NAME="cryptic"
VITE_CLIENT_VERSION="0.0.1"
#VITE_ROUTER_BASE=""
VITE_ROUTER_MODE="history"
"@ -Force);
