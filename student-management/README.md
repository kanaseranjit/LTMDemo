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

This app is ready to deploy to Azure as a single-page React application.

### Build-time API configuration

The app reads its API base URL from `REACT_APP_API_BASE_URL`. Set it before building for Azure:

```bash
REACT_APP_API_BASE_URL=https://your-api-host/api npm run build
```

For local development, the app falls back to `http://localhost:7093/api`.

### Azure App Service

Deploy the app as a static React build to a Windows Azure App Service.
The included `public/web.config` file rewrites all client-side routes back to `index.html`, so refreshes on routes like `/studentsList` keep working under IIS.

A GitHub Actions workflow is included at `.github/workflows/azure-webapp.yml` for push-to-deploy publishing.

1. Create an Azure App Service running on **Windows**.
2. In the Azure portal, open the App Service and download its **publish profile**.
3. In GitHub, add these repository secrets:
- `AZURE_WEBAPP_NAME`
- `AZURE_WEBAPP_PUBLISH_PROFILE`
- `REACT_APP_API_BASE_URL`
4. Push to `main`, or run the workflow manually from the **Actions** tab.
5. After deployment completes, open `https://<your-app-name>.azurewebsites.net`.

The workflow fails early if any of the required secrets are missing, so configuration issues show up before the deploy step runs.

### Azure Static Web Apps

The included `public/staticwebapp.config.json` enables SPA route fallback for Azure Static Web Apps.
Set `REACT_APP_API_BASE_URL` in the Azure Static Web Apps app settings if your API is hosted separately.

### `npm run build` fails to minify

This section has moved here: [https://facebook.github.io/create-react-app/docs/troubleshooting#npm-run-build-fails-to-minify](https://facebook.github.io/create-react-app/docs/troubleshooting#npm-run-build-fails-to-minify)
