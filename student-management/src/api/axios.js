import axios from 'axios';

const defaultApiBaseUrl = 'https://studentsappservice-b2egatcuh7hkgpfx.southindia-01.azurewebsites.net/api';
const authCredentials = {
    username: 'admin',
    password: 'password',
};
const tokenStorageKey = 'student_management_jwt';

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

const authApi = axios.create({
    baseURL: apiBaseUrl,
    headers: {
        'Content-Type': 'application/json',
    },
});

const api = axios.create({
    baseURL: apiBaseUrl,
    headers: {
        'Content-Type': 'application/json',
    },
});

let loginRequestInFlight = null;

function readStoredToken() {
    if (typeof window === 'undefined') {
        return '';
    }
    return window.localStorage.getItem(tokenStorageKey) || '';
}

function storeToken(token) {
    if (typeof window === 'undefined' || !token) {
        return;
    }
    window.localStorage.setItem(tokenStorageKey, token);
}

function clearToken() {
    if (typeof window === 'undefined') {
        return;
    }
    window.localStorage.removeItem(tokenStorageKey);
}

function extractToken(payload) {
    return payload?.token || payload?.accessToken || payload?.jwt || payload?.data?.token || '';
}

async function loginAndGetToken() {
    const response = await authApi.post('/auth/login', authCredentials);
    const token = extractToken(response?.data);

    if (!token) {
        throw new Error('Login succeeded but JWT token was not found in response payload.');
    }

    storeToken(token);
    return token;
}

async function ensureToken() {
    const existing = readStoredToken();
    if (existing) {
        return existing;
    }

    if (!loginRequestInFlight) {
        // Share a single login request across concurrent API calls.
        loginRequestInFlight = loginAndGetToken().finally(() => {
            loginRequestInFlight = null;
        });
    }

    return loginRequestInFlight;
}

api.interceptors.request.use(async (config) => {
    const token = await ensureToken();

    config.headers = config.headers || {};
    config.headers.Authorization = `Bearer ${token}`;
    return config;
});

api.interceptors.response.use(
    (response) => response,
    async (error) => {
        const originalRequest = error?.config;
        const status = error?.response?.status;

        if (status === 401 && originalRequest && !originalRequest._retry) {
            originalRequest._retry = true;
            clearToken();

            const token = await ensureToken();
            originalRequest.headers = originalRequest.headers || {};
            originalRequest.headers.Authorization = `Bearer ${token}`;
            return api(originalRequest);
        }

        return Promise.reject(error);
    }
);

export default api;