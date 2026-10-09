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


// Function to fetch categories from the API. Similar to getIdeas, but it makes a GET request to the /categories endpoint. If the response is not ok, we throw an error. Otherwise, we return the JSON data from the response.
export async function getCategories() {
    const response = await fetch(`${API_URL}/categories`);

    if (!response.ok) {
        throw new Error("Failed to fetch categories");
    }

    return response.json();
}