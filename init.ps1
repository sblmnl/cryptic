dotnet tool restore
dotnet husky install

cd client
npm install

(New-Item -Path ".env" -Value @"
VITE_APP_PATH_BASE=""
"@ -Force);

cd ..
