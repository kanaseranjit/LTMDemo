# Getting Started with Create React App

This project was bootstrapped with [Create React App](https://github.com/facebook/create-react-app).

## Available Scripts

In the project directory, you can run:

### `npm start`

Runs the app in the development mode.\
Open [http://localhost:3000](http://localhost:3000) to view it in your browser.

The page will reload when you make changes.\
You may also see any lint errors in the console.

### `npm test`

Launches the test runner in the interactive watch mode.\
See the section about [running tests](https://facebook.github.io/create-react-app/docs/running-tests) for more information.

### `npm run build`

Builds the app for production to the `build` folder.\
It correctly bundles React in production mode and optimizes the build for the best performance.

The build is minified and the filenames include the hashes.\
Your app is ready to be deployed!

See the section about [deployment](https://facebook.github.io/create-react-app/docs/deployment) for more information.

### `npm run eject`

**Note: this is a one-way operation. Once you `eject`, you can't go back!**

If you aren't satisfied with the build tool and configuration choices, you can `eject` at any time. This command will remove the single build dependency from your project.

Instead, it will copy all the configuration files and the transitive dependencies (webpack, Babel, ESLint, etc) right into your project so you have full control over them. All of the commands except `eject` will still work, but they will point to the copied scripts so you can tweak them. At this point you're on your own.

You don't have to ever use `eject`. The curated feature set is suitable for small and middle deployments, and you shouldn't feel obligated to use this feature. However we understand that this tool wouldn't be useful if you couldn't customize it when you are ready for it.

## Learn More

You can learn more in the [Create React App documentation](https://facebook.github.io/create-react-app/docs/getting-started).

To learn React, check out the [React documentation](https://reactjs.org/).

### Code Splitting

This section has moved here: [https://facebook.github.io/create-react-app/docs/code-splitting](https://facebook.github.io/create-react-app/docs/code-splitting)

### Analyzing the Bundle Size

This section has moved here: [https://facebook.github.io/create-react-app/docs/analyzing-the-bundle-size](https://facebook.github.io/create-react-app/docs/analyzing-the-bundle-size)

### Making a Progressive Web App

This section has moved here: [https://facebook.github.io/create-react-app/docs/making-a-progressive-web-app](https://facebook.github.io/create-react-app/docs/making-a-progressive-web-app)

### Advanced Configuration

This section has moved here: [https://facebook.github.io/create-react-app/docs/advanced-configuration](https://facebook.github.io/create-react-app/docs/advanced-configuration)

### Deployment

This section has moved here: [https://facebook.github.io/create-react-app/docs/deployment](https://facebook.github.io/create-react-app/docs/deployment)

## Azure deployment

This application can be deployed to Azure App Service in two ways:

1. GitHub Actions workflow (CI/CD)
2. Manual ZipDeploy script (PowerShell)

Use the manual script below when you want a predictable, one-command deployment from a local machine.

### Prerequisites

1. Node.js and npm installed.
2. Azure App Service (Windows) created for the frontend.
3. Publish profile downloaded for that App Service.
4. API CORS configured to allow the frontend domain.
5. PowerShell 5.1+.

### Build-time API configuration

The app reads API base URL from `REACT_APP_API_BASE_URL`.

Example:

```powershell
$env:REACT_APP_API_BASE_URL = "https://your-api-app.azurewebsites.net/"
npm run build
```

For local development, the API client falls back to `https://studentsappservice-b2egatcuh7hkgpfx.southindia-01.azurewebsites.net/api`.

### Step-by-step manual deployment to Azure (ZipDeploy)

1. Open PowerShell.
2. Go to project root.
3. Set API URL as build-time environment variable.
4. Build the app.
5. Create deployment runtime folder from `build` output.
6. Add `server.js` to serve static files and handle SPA route fallback.
7. Add runtime `package.json` with `start` script.
8. Zip runtime folder.
9. Read publish credentials from publish profile.
10. Push zip to Kudu ZipDeploy endpoint.
11. Verify root and SPA route responses.

### Deployment script (PowerShell)

> Update values for your environment before running:
> - `$projectRoot`
> - `$apiBaseUrl`
> - `$publishProfilePath`
> - `$siteUrl`

You can also run the ready-to-use script in project root:

```powershell
./deploy.ps1
```

Custom parameters example:

```powershell
./deploy.ps1 -ProjectRoot "E:\Ranjit\LTI\student-management" -ApiBaseUrl "https://your-api.azurewebsites.net/" -PublishProfilePath "$HOME\Downloads\ReactStudentApp.PublishSettings" -SiteUrl "https://your-frontend.azurewebsites.net/"
```

```powershell
$projectRoot = "E:\Ranjit\LTI\student-management"
$apiBaseUrl = "https://studentsappservice-b2egatcuh7hkgpfx.southindia-01.azurewebsites.net/"
$publishProfilePath = "$HOME\Downloads\ReactStudentApp.PublishSettings"
$siteUrl = "https://reactstudentapp-c6dah9bwhthnbgfg.austriaeast-01.azurewebsites.net/"

Set-Location $projectRoot
$ErrorActionPreference = 'Stop'

# 1) Build with Azure API URL
$env:REACT_APP_API_BASE_URL = $apiBaseUrl
npm run build
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

# 2) Prepare deploy runtime
$deployDir = Join-Path (Get-Location) "deploy-runtime"
if (Test-Path $deployDir) { Remove-Item $deployDir -Recurse -Force }
New-Item -ItemType Directory -Path $deployDir | Out-Null
Copy-Item -Path "build\*" -Destination $deployDir -Recurse -Force

# 3) Node static server with SPA fallback
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

# 4) Runtime package.json
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

# 5) Zip runtime
$zipPath = Join-Path (Get-Location) "deploy-runtime.zip"
if (Test-Path $zipPath) { Remove-Item $zipPath -Force }
tar -a -c -f $zipPath -C $deployDir .

# 6) Read publish profile and deploy
[xml]$xml = Get-Content -Path $publishProfilePath
$zipProfile = $xml.publishData.publishProfile |
	Where-Object { $_.publishMethod -eq 'ZipDeploy' } |
	Select-Object -First 1

$scmHost = $zipProfile.publishUrl.Replace(':443', '')
$pair = "{0}:{1}" -f $zipProfile.userName, $zipProfile.userPWD
$auth = [Convert]::ToBase64String([Text.Encoding]::ASCII.GetBytes($pair))
$headers = @{ Authorization = "Basic $auth" }

$resp = Invoke-WebRequest -Uri ("https://" + $scmHost + "/api/zipdeploy") -Method POST -InFile $zipPath -ContentType "application/zip" -Headers $headers -UseBasicParsing
Write-Output ("ZipDeploy status=" + $resp.StatusCode)

# 7) Verify deployment
$root = Invoke-WebRequest -Uri $siteUrl -UseBasicParsing
Write-Output ("RootStatus=" + $root.StatusCode)

$studentsRoute = Invoke-WebRequest -Uri ($siteUrl.TrimEnd('/') + "/studentsList") -UseBasicParsing
Write-Output ("StudentsListStatus=" + $studentsRoute.StatusCode)
```

### Deployment using GitHub Actions (optional)

A workflow is available in `.github/workflows/azure-webapp.yml`.

Add these GitHub repository secrets:

1. `AZURE_WEBAPP_NAME`
2. `AZURE_WEBAPP_PUBLISH_PROFILE`
3. `REACT_APP_API_BASE_URL`

Then push to `main` or run the workflow manually from the Actions tab.

### Validation checklist after deployment

1. Open app root URL and verify status 200.
2. Open SPA routes like `/studentsList` and `/admission`.
3. Confirm API calls succeed in browser network tab.
4. If API is cross-origin, verify API CORS allows frontend domain.

### Troubleshooting

1. `401 Unauthorized` on API calls:
	 - Confirm login endpoint `/api/auth/login` is reachable.
	 - Confirm JWT response includes token fields expected by client.
2. CORS errors:
	 - Add frontend URL under API App Service CORS settings.
3. Root shows Azure placeholder page:
	 - Re-run ZipDeploy and verify status 200.
4. Route refresh returns 404:
	 - Ensure SPA fallback is present (web.config or server.js fallback logic).

### Azure Static Web Apps

The included `public/staticwebapp.config.json` enables SPA route fallback for Azure Static Web Apps.
Set `REACT_APP_API_BASE_URL` in the Azure Static Web Apps app settings if your API is hosted separately.

### `npm run build` fails to minify

This section has moved here: [https://facebook.github.io/create-react-app/docs/troubleshooting#npm-run-build-fails-to-minify](https://facebook.github.io/create-react-app/docs/troubleshooting#npm-run-build-fails-to-minify)
