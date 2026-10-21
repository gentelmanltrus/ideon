import { useEffect, useMemo, useState } from "react";
import { Link, useSearchParams } from "react-router-dom";
import { getIdeas } from "../services/api";
import "../styles/MainPage.css";
//summary: This file contains the MainPage component, which displays a list of ideas fetched from the API. It uses React hooks to manage state and side effects. The component fetches ideas when it mounts and filters them based on the selected category from the URL search parameters. Error handling is implemented to provide feedback to the user in case of issues during fetching.
export default function MainPage() {
    const [searchParams] = useSearchParams();
    const categoryParam = searchParams.get("category");

    const [ideas, setIdeas] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState("");

    useEffect(() => {
        let cancelled = false;

        async function loadIdeas() {
            setLoading(true);
            setError("");

            try {
                const data = await getIdeas();

                if (!cancelled) {
                    setIdeas(data);
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

        loadIdeas();

        return () => {
            cancelled = true;
        };
    }, []);

    const filteredIdeas = useMemo(() => {
        if (!categoryParam) {
            return ideas;
        }

        const categoryId = Number(categoryParam);

        if (!Number.isInteger(categoryId)) {
            return [];
        }

        return ideas.filter(
            (idea) => idea.categoryId === categoryId
        );
    }, [ideas, categoryParam]);

    if (loading) {
        return <main className="feed-container">Loading ideas...</main>;
    }

    if (error) {
        return (
            <main className="feed-container">
                <p role="alert">Could not load ideas: {error}</p>
                <Link to="/categories">Back to categories</Link>
            </main>
        );
    }

    return (
        <main className="feed-container">
            <header className="feed-header">
                <div className="header-left">
                    <h1>Ideon</h1>
                    <p>
                        {categoryParam
                            ? `Category ID: ${categoryParam}`
                            : "All ideas"}
                    </p>
                </div>

                <nav>
                    <Link to="/categories">Change category</Link>
                    {" | "}
                    <Link to="/ideas/new">Create an idea</Link>
                </nav>
            </header>

            <section className="feed-list">
                {filteredIdeas.length === 0 && (
                    <p>No ideas found in this category yet.</p>
                )}

                {filteredIdeas.map((idea) => (
                    <article key={idea.id} className="idea-card">
                        <div className="idea-author-row">
                            <div>
                                <h2>
                                    <Link to={`/ideas/${idea.id}`}>
                                        {idea.title}
                                    </Link>
                                </h2>

                                <p className="post-time">
                                    By {idea.authorUsername}
                                    {" · "}
                                    {idea.categoryName}
                                    {" · "}
                                    {new Date(
                                        idea.createdAt
                                    ).toLocaleDateString()}
                                </p>
                            </div>
                        </div>

                        <p className="idea-body-text">
                            {idea.description}
                        </p>

                        {idea.lookingForBuilder && (
                            <p>
                                <strong>Looking for a Builder</strong>
                            </p>
                        )}

                        <p>
                            <Link to={`/ideas/${idea.id}`}>
                                Open idea and comments →
                            </Link>
                        </p>
                    </article>
                ))}
            </section>
        </main>
    );
}