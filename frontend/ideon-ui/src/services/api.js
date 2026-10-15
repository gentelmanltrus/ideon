//summary: This file contains functions to interact with the API for fetching ideas and categories. It uses the fetch API to make HTTP requests and handles errors appropriately. The base URL for the API is defined at the top of the file.
const API_URL = "http://localhost:5104/api";//Base URL for the API
// Function to fetch ideas from the API. fetch is a built-in JavaScript function that makes HTTP requests. It returns a promise that resolves to the response of the request. In this case, we are making a GET request to the /ideas endpoint of the API. If the response is not ok (i.e., the status code is not in the range 200-299), we throw an error. Otherwise, we return the JSON data from the response.
export async function getIdeas() {
    const response = await fetch(`${API_URL}/ideas`);
    //await - Waits for the fetch request to complete and returns the response object. 
    if (!response.ok) {
        throw new Error("Failed to fetch ideas");
    }

    return response.json();
}
// Function to fetch a single idea by its ID from the API. Similar to getIdeas, but it takes an id parameter and makes a GET request to the /ideas/:id endpoint. If the response is not ok, we throw an error. Otherwise, we return the JSON data from the response.
export async function getIdea(id) {
    const response = await fetch(`${API_URL}/ideas/${id}`);

    if (!response.ok) {
        throw new Error("Failed to fetch idea");
    }

    return response.json();
}
// Function to fetch a single idea by its ID from the API. Similar to getIdeas, but it takes an id parameter and makes a GET request to the /ideas/:id endpoint. If the response is not ok, we throw an error. Otherwise, we return the JSON data from the response.
export async function getIdeaById(id) {
    const response = await fetch(`${API_URL}/ideas/${id}`);

    if (!response.ok) {
        throw new Error(`Failed to fetch idea: ${response.status}`);
    }

    return response.json();
}

// Function to fetch categories from the API. Similar to getIdeas, but it makes a GET request to the /categories endpoint. If the response is not ok, we throw an error. Otherwise, we return the JSON data from the response.
export async function getCategories() {
    const response = await fetch(`${API_URL}/categories`);

    if (!response.ok) {
        throw new Error("Failed to fetch categories");
    }

    return response.json();
}
// Function to create a new idea by sending a POST request to the API. It takes a data parameter, which is an object containing the idea's title, description, category ID, and whether it is looking for a builder. We use fetch to make a POST request to the /ideas endpoint, setting the Content-Type header to application/json and sending the data as a JSON string in the request body. If the response is not ok, we throw an error with the response message or status code. Otherwise, we return the JSON data from the response.

export async function createIdea(data) {
    const response = await fetch(`${API_URL}/ideas`, {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify(data)
    });

    if (!response.ok) {
        const message = await response.text();

        throw new Error(
            message || `Failed to create idea: ${response.status}`
        );
    }

    return response.json();
}
// Function to fetch comments for a specific idea by its ID from the API. It takes an ideaId parameter and makes a GET request to the /ideas/:ideaId/comments endpoint. If the response is not ok, we throw an error with the response status code. Otherwise, we return the JSON data from the response.
export async function getComments(ideaId) {
    const response = await fetch(
        `${API_URL}/ideas/${ideaId}/comments`
    );

    if (!response.ok) {
        throw new Error(
            `Failed to fetch comments: ${response.status}`
        );
    }

    return response.json();
}
// Function to create a new comment for a specific idea by sending a POST request to the API. It takes an ideaId parameter and a content parameter, which is the text of the comment. We use fetch to make a POST request to the /ideas/:ideaId/comments endpoint, setting the Content-Type header to application/json and sending the content as a JSON string in the request body. If the response is not ok, we throw an error with the response message or status code. Otherwise, we return the JSON data from the response.
export async function createComment(ideaId, content) {
    const response = await fetch(
        `${API_URL}/ideas/${ideaId}/comments`,
        {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify({ content })
        }
    );

    if (!response.ok) {
        const message = await response.text();
        throw new Error(
            message || `Failed to create comment: ${response.status}`
        );
    }

    return response.json();
}