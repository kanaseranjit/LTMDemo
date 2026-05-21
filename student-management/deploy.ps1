param(
    [string]$ProjectRoot = "E:\Ranjit\LTI\student-management",
    [string]$ApiBaseUrl = "https://studentsappservice-b2egatcuh7hkgpfx.southindia-01.azurewebsites.net/",
    [string]$PublishProfilePath = "$HOME\Downloads\ReactStudentApp.PublishSettings",
    [string]$SiteUrl = "https://reactstudentapp-c6dah9bwhthnbgfg.austriaeast-01.azurewebsites.net/"
)

$ErrorActionPreference = 'Stop'

Write-Host "[1/6] Preparing build..." -ForegroundColor Cyan
Set-Location $ProjectRoot
$env:REACT_APP_API_BASE_URL = $ApiBaseUrl

npm run build
if ($LASTEXITCODE -ne 0) {
    throw "Build failed."
}

Write-Host "[2/6] Preparing deploy-runtime package..." -ForegroundColor Cyan
$deployDir = Join-Path (Get-Location) "deploy-runtime"
if (Test-Path $deployDir) {
    Remove-Item $deployDir -Recurse -Force
}
New-Item -ItemType Directory -Path $deployDir | Out-Null
Copy-Item -Path "build\*" -Destination $deployDir -Recurse -Force

@'
const http = require('http');
const fs = require('fs');
const path = require('path');

const port = process.env.PORT || 8080;
const root = __dirname;

const mimeTypes = {
  '.html': 'text/html; charset=utf-8',
  '.js': 'application/javascript; charset=utf-8',
  '.css': 'text/css; charset=utf-8',
  '.json': 'application/json; charset=utf-8',
  '.png': 'image/png',
  '.jpg': 'image/jpeg',
  '.jpeg': 'image/jpeg',
  '.gif': 'image/gif',
  '.svg': 'image/svg+xml',
  '.ico': 'image/x-icon',
  '.txt': 'text/plain; charset=utf-8',
  '.map': 'application/json; charset=utf-8'
};

function serveFile(filePath, res) {
  fs.readFile(filePath, (err, data) => {
    if (err) {
      res.writeHead(404, { 'Content-Type': 'text/plain; charset=utf-8' });
      res.end('Not Found');
      return;
    }

    const ext = path.extname(filePath).toLowerCase();
    const contentType = mimeTypes[ext] || 'application/octet-stream';
    res.writeHead(200, { 'Content-Type': contentType });
    res.end(data);
  });
}

const server = http.createServer((req, res) => {
  const requestPath = decodeURIComponent((req.url || '/').split('?')[0]);
  const normalized = path.normalize(requestPath).replace(/^\/+/, '');
  let filePath = path.join(root, normalized);

  if (!filePath.startsWith(root)) {
    res.writeHead(403, { 'Content-Type': 'text/plain; charset=utf-8' });
    res.end('Forbidden');
    return;
  }

  fs.stat(filePath, (err, stats) => {
    if (!err && stats.isDirectory()) {
      filePath = path.join(filePath, 'index.html');
      return serveFile(filePath, res);
    }

    if (!err && stats.isFile()) {
      return serveFile(filePath, res);
    }

    serveFile(path.join(root, 'index.html'), res);
  });
});

server.listen(port, () => {
  console.log(`Server listening on port ${port}`);
});
'@ | Set-Content -Path (Join-Path $deployDir "server.js") -Encoding UTF8

@'
{
  "name": "reactstudentapp-runtime",
  "version": "1.0.0",
  "private": true,
  "main": "server.js",
  "scripts": {
    "start": "node server.js"
  }
}
'@ | Set-Content -Path (Join-Path $deployDir "package.json") -Encoding UTF8

Write-Host "[3/6] Creating deployment zip..." -ForegroundColor Cyan
$zipPath = Join-Path (Get-Location) "deploy-runtime.zip"
if (Test-Path $zipPath) {
    Remove-Item $zipPath -Force
}
tar -a -c -f $zipPath -C $deployDir .

Write-Host "[4/6] Reading publish profile..." -ForegroundColor Cyan
if (!(Test-Path $PublishProfilePath)) {
    throw "Publish profile not found: $PublishProfilePath"
}

[xml]$xml = Get-Content -Path $PublishProfilePath
$zipProfile = $xml.publishData.publishProfile |
    Where-Object { $_.publishMethod -eq 'ZipDeploy' } |
    Select-Object -First 1

if (-not $zipProfile) {
    throw "No ZipDeploy profile found in publish settings file."
}

$scmHost = $zipProfile.publishUrl.Replace(':443', '')
$pair = "{0}:{1}" -f $zipProfile.userName, $zipProfile.userPWD
$auth = [Convert]::ToBase64String([Text.Encoding]::ASCII.GetBytes($pair))
$headers = @{ Authorization = "Basic $auth" }

Write-Host "[5/6] Deploying to Azure via ZipDeploy..." -ForegroundColor Cyan
$resp = Invoke-WebRequest -Uri ("https://" + $scmHost + "/api/zipdeploy") -Method POST -InFile $zipPath -ContentType "application/zip" -Headers $headers -UseBasicParsing
Write-Host ("ZipDeploy status=" + $resp.StatusCode) -ForegroundColor Green

Write-Host "[6/6] Verifying deployed app..." -ForegroundColor Cyan
$root = Invoke-WebRequest -Uri $SiteUrl -UseBasicParsing
$studentsRoute = Invoke-WebRequest -Uri ($SiteUrl.TrimEnd('/') + "/studentsList") -UseBasicParsing

Write-Host ("RootStatus=" + $root.StatusCode) -ForegroundColor Green
Write-Host ("StudentsListStatus=" + $studentsRoute.StatusCode) -ForegroundColor Green
Write-Host "Deployment complete." -ForegroundColor Green
