const BASE_URL = import.meta.env.VITE_API_BASE_URL;

// Helper function to grab the token managed by react-oidc-context
const getOidcToken = () => {
    // The key is always formatted as "oidc.user:YOUR_AUTHORITY_URL:YOUR_CLIENT_ID"
    const oidcStorageKey = "oidc.user:https://localhost:7036:nexawork_react_web";
    const oidcStorage = sessionStorage.getItem(oidcStorageKey);
    
    if (!oidcStorage) {
        return null;
    }

    try {
        const user = JSON.parse(oidcStorage);
        return user.access_token; // This is the JWT you send to your business API!
    } catch (e) {
        console.error("Failed to parse OIDC user session", e);
        return null;
    }
};

export const apiClient = async (endpoint: string, options: RequestInit = {}) => {
    // 1. Get the token from OIDC session storage
    const token = getOidcToken();

    const headers = new Headers(options.headers);

    if (!headers.has('Content-Type')) {
        headers.set('Content-Type', 'application/json');
    }

    // 2. Attach the token
    if (token) {
        headers.set('Authorization', `Bearer ${token}`);
    }

    const config: RequestInit = {
        ...options,
        headers,
    };

    try {
        const response = await fetch(`${BASE_URL}${endpoint}`, config);

        // 3. Handle Unauthorized
        if (response.status === 401) {
            console.warn("User session has expired or is invalid.");
            
            // Clear the OIDC storage so the app knows we are logged out
            sessionStorage.removeItem("oidc.user:https://localhost:7036:nexawork_react_web");

            // Redirect back to the home page (which will trigger ProtectedRoute to send them to login)
            window.location.href = '/';

            throw new Error('Unauthorized');
        }

        return response;
    } catch (error) {
        console.error(`Error calling API [${endpoint}]:`, error);
        throw error;
    }
};