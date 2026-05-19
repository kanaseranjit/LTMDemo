import axios from 'axios';

const defaultApiBaseUrl = 'https://studentsappservice-b2egatcuh7hkgpfx.southindia-01.azurewebsites.net/api';

function normalizeApiBaseUrl(rawBaseUrl) {
    const fallback = defaultApiBaseUrl;

    if (!rawBaseUrl || !rawBaseUrl.trim()) {
        return fallback;
    }

    const trimmed = rawBaseUrl.trim().replace(/\/+$/, '');

    // If deploy config provides only host, force the API segment expected by this app.
    if (/\/api$/i.test(trimmed)) {
        return trimmed;
    }

    return `${trimmed}/api`;
}

const apiBaseUrl = normalizeApiBaseUrl(process.env.REACT_APP_API_BASE_URL);

const api = axios.create({
    baseURL: apiBaseUrl,
    headers: {
        'Content-Type': 'application/json',
    },
});

export default api;