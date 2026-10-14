import { useEffect, useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import { createIdea, getCategories } from "../services/api";
//summary: This file contains the CreateIdea component, which allows users to create a new idea. It uses React hooks to manage state and side effects. The component fetches categories from the API when it mounts and displays a form for users to input the idea's title, description, category, and whether they are looking for a builder. Upon form submission, it sends a POST request to the API to create the idea and navigates to the newly created idea's page if successful. Error handling is implemented to provide feedback to the user in case of issues during fetching or submission.
export default function CreateIdea() {
    const navigate = useNavigate();

    const [title, setTitle] = useState("");
    const [description, setDescription] = useState("");
    const [categoryId, setCategoryId] = useState("");
    const [lookingForBuilder, setLookingForBuilder] = useState(false);

    const [categories, setCategories] = useState([]);
    const [loadingCategories, setLoadingCategories] = useState(true);
    const [submitting, setSubmitting] = useState(false);
    const [error, setError] = useState("");

    useEffect(() => {
        async function loadCategories() {
            try {
                const data = await getCategories();
                setCategories(data);
            } catch (err) {
                setError(err.message);
            } finally {
                setLoadingCategories(false);
            }
        }

        loadCategories();
    }, []);

    async function handleSubmit(event) {
        event.preventDefault();
        setError("");

        if (!title.trim() || !description.trim()) {
            setError("Title and description are required.");
            return;
        }

        if (!categoryId) {
            setError("Please select a category.");
            return;
        }

        setSubmitting(true);

        try {
            const created = await createIdea({
                title: title.trim(),
                description: description.trim(),
                categoryId: Number(categoryId),
                lookingForBuilder
            });

            navigate(`/ideas/${created.id}`);
        } catch (err) {
            setError(err.message);
        } finally {
            setSubmitting(false);
        }
    }

    if (loadingCategories) {
        return <p>Loading categories...</p>;
    }

    return (
        <main>
            <Link to="/">← Back to Home</Link>

            <h1>Create an idea</h1>

            <form onSubmit={handleSubmit}>
                <label htmlFor="title">Title</label>
                <input
                    id="title"
                    value={title}
                    onChange={(e) => setTitle(e.target.value)}
                    maxLength={100}
                    required
                />

                <label htmlFor="description">Description</label>
                <textarea
                    id="description"
                    value={description}
                    onChange={(e) => setDescription(e.target.value)}
                    maxLength={2000}
                    required
                />

                <label htmlFor="category">Category</label>
                <select
                    id="category"
                    value={categoryId}
                    onChange={(e) => setCategoryId(e.target.value)}
                    required
                >
                    <option value="">Select a category</option>

                    {categories.map((category) => (
                        <option key={category.id} value={category.id}>
                            {category.name}
                        </option>
                    ))}
                </select>

                <label>
                    <input
                        type="checkbox"
                        checked={lookingForBuilder}
                        onChange={(e) =>
                            setLookingForBuilder(e.target.checked)
                        }
                    />
                    Looking for a Builder
                </label>

                {error && <p role="alert">{error}</p>}

                <button type="submit" disabled={submitting}>
                    {submitting ? "Creating..." : "Create idea"}
                </button>
            </form>
        </main>
    );
}