const API_URL = "http://localhost:5104/api";
// This file contains functions for interacting with the backend API, including authentication, idea management, comments, ratings, and collaboration requests. It uses the Fetch API to make HTTP requests and handles token storage in localStorage.
export function getToken() {
    return localStorage.getItem("ideon_token");
}//gets the authentication token from localStorage, which is used for making authenticated API requests.

export function saveAuth(auth) {
    localStorage.setItem("ideon_token", auth.token);
    localStorage.setItem("ideon_user", JSON.stringify({
        userId: auth.userId,
        username: auth.username,
        email: auth.email
    }));
}//saves the authentication token and user information to localStorage after a successful login or registration, allowing the application to persist the user's session across page reloads.

export function clearAuth() {
    localStorage.removeItem("ideon_token");
    localStorage.removeItem("ideon_user");
}//clears the authentication token and user information from localStorage, effectively logging the user out of the application.

export function getCurrentUser() {
    const raw = localStorage.getItem("ideon_user");

    if (!raw) {
        return null;
    }

    try {
        return JSON.parse(raw);
    } catch {
        return null;
    }
}

async function request(path, options = {}) {
    const token = getToken();

    const headers = {
        ...(options.body ? { "Content-Type": "application/json" } : {}),
        ...(options.headers || {}),
        ...(token ? { Authorization: `Bearer ${token}` } : {})
    };

    const response = await fetch(`${API_URL}${path}`, {
        ...options,
        headers
    });

    if (response.status === 204) {
        return null;
    }

    const text = await response.text();

    let data = null;

    if (text) {
        try {
            data = JSON.parse(text);
        } catch {
            data = text;
        }
    }

    if (!response.ok) {
        const message =
            data?.message ||
            data?.title ||
            (typeof data === "string" ? data : null) ||
            `Request failed: ${response.status}`;

        if (response.status === 401) {
            clearAuth();
        }

        throw new Error(message);
    }

    return data;
}

export async function register(data) {
    return request("/auth/register", {
        method: "POST",
        body: JSON.stringify(data)
    });
}

export async function login(data) {
    return request("/auth/login", {
        method: "POST",
        body: JSON.stringify(data)
    });
}

export async function getMe() {
    return request("/auth/me");
}

export async function getIdeas() {
    return request("/ideas");
}

export async function getIdea(id) {
    return request(`/ideas/${id}`);
}

export async function getIdeaById(id) {
    return getIdea(id);
}

export async function getCategories() {
    return request("/categories");
}

export async function createIdea(data) {
    return request("/ideas", {
        method: "POST",
        body: JSON.stringify(data)
    });
}

export async function updateIdea(id, data) {
    return request(`/ideas/${id}`, {
        method: "PUT",
        body: JSON.stringify(data)
    });
}

export async function deleteIdea(id) {
    return request(`/ideas/${id}`, {
        method: "DELETE"
    });
}

export async function getComments(ideaId) {
    return request(`/ideas/${ideaId}/comments`);
}

export async function createComment(ideaId, content) {
    return request(`/ideas/${ideaId}/comments`, {
        method: "POST",
        body: JSON.stringify({ content })
    });
}

export async function getRatings(ideaId) {
    return request(`/ideas/${ideaId}/ratings`);
}

export async function createRating(ideaId, data) {
    return request(`/ideas/${ideaId}/ratings`, {
        method: "POST",
        body: JSON.stringify(data)
    });
}

export async function updateMyRating(ideaId, data) {
    return request(`/ideas/${ideaId}/ratings/mine`, {
        method: "PUT",
        body: JSON.stringify(data)
    });
}

export async function createCollaborationRequest(ideaId) {
    return request(`/ideas/${ideaId}/collaboration-requests`, {
        method: "POST",
        body: JSON.stringify({})
    });
}

export async function getMyCollaborationRequests() {
    return request("/collaboration-requests/mine");
}