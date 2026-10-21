import { useEffect, useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import { clearAuth, getCategories } from "../services/api";
import "../styles/CategorySelect.css";
//summary: This file contains the CategorySelect component, which allows users to select a category for their idea. It uses React hooks to manage state and side effects. The component fetches categories from the API when it mounts and displays them in a grid. Users can select a category to navigate to the main page filtered by that category. Error handling is implemented to provide feedback to the user in case of issues during fetching.
export default function CategorySelect() {
    const navigate = useNavigate();

    const [categories, setCategories] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState("");

    useEffect(() => {
        let cancelled = false;

        async function loadCategories() {
            setLoading(true);
            setError("");

            try {
                const data = await getCategories();

                if (!cancelled) {
                    setCategories(data);
                }
            } catch (err) {
                if (!cancelled) {
                    setError(err.message);
                }
            } finally {
                if (!cancelled) {
                    setLoading(false);
                }
            }
        }

        loadCategories();

        return () => {
            cancelled = true;
        };
    }, []);

    function handleSelect(categoryId) {
        navigate(`/main?category=${categoryId}`);
    }

    function handleLogout() {
        clearAuth();
        navigate("/");
    }

    if (loading) {
        return <main className="category-container">Loading categories...</main>;
    }

    if (error) {
        return (
            <main className="category-container">
                <p role="alert">Could not load categories: {error}</p>
                <button onClick={() => window.location.reload()}>
                    Retry
                </button>
            </main>
        );
    }

    return (
        <main className="category-container">
            <header>
                <h1>Share your idea with the world!</h1>
                <p>Please select a category</p>

                <button onClick={handleLogout}>Log out</button>
            </header>

            <div className="category-grid">
                {categories.map((category) => (
                    <button
                        type="button"
                        key={category.id}
                        className="category-card"
                        onClick={() => handleSelect(category.id)}
                    >
                        <h3>{category.name}</h3>
                    </button>
                ))}
            </div>

            <p>
                <Link to="/main">View all ideas</Link>
            </p>
        </main>
    );
}