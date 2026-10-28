import { useEffect, useMemo, useState } from "react";
import { Link, useSearchParams } from "react-router-dom";
import { getIdeas } from "../services/api";
import "../styles/MainPage.css";
//this is the main page of the application, which displays a list of ideas and allows users to search and sort them. It also handles loading and error states.
export default function MainPage() {
    const [searchParams] = useSearchParams();
    const categoryParam = searchParams.get("category");
    //usestate hooks to manage the state of ideas, search query, sorting option, loading state, and error message.
    const [ideas, setIdeas] = useState([]);
    const [search, setSearch] = useState("");
    const [sortBy, setSortBy] = useState("newest");
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
                    setError(err.message || "Could not load ideas.");
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
    //useMemo hook to compute the visible ideas based on the current search query, selected category, and sorting option. It filters and sorts the ideas accordingly.
    //filter() - The filter() method creates a new array with all elements that pass the test implemented by the provided function. In this case, it filters the ideas based on the search query and selected category.
    const visibleIdeas = useMemo(() => {
        const normalizedSearch = search.trim().toLowerCase();

        let result = ideas.filter((idea) => {
            if (categoryParam) {
                const categoryId = Number(categoryParam);

                if (
                    !Number.isInteger(categoryId) ||
                    idea.categoryId !== categoryId
                ) {
                    return false;
                }
            }

            const searchableText = [
                idea.title,
                idea.description,
                idea.authorUsername,
                idea.categoryName
            ]
                .filter(Boolean)
                .join(" ")
                .toLowerCase();

            return searchableText.includes(normalizedSearch);
        });

        result = [...result];
        //sort() - The sort() method sorts the elements of an array in place and returns the sorted array. In this case, it sorts the filtered ideas based on the selected sorting option (newest, oldest, or title).
        if (sortBy === "newest") {
            result.sort(
                (a, b) =>
                    new Date(b.createdAt).getTime() -
                    new Date(a.createdAt).getTime()
            );
        } else if (sortBy === "oldest") {
            result.sort(
                (a, b) =>
                    new Date(a.createdAt).getTime() -
                    new Date(b.createdAt).getTime()
            );
        } else if (sortBy === "title") {
            result.sort((a, b) =>
                a.title.localeCompare(b.title)
            );
        }

        return result;
    }, [ideas, categoryParam, search, sortBy]);

    if (loading) {
        return (
            <main className="feed-container">
                <p className="status-message">Loading ideas…</p>
            </main>
        );
    }

    if (error) {
        return (
            <main className="feed-container">
                <h1>Ideon</h1>
                <p className="error-message" role="alert">
                    Could not load ideas: {error}
                </p>
                <Link className="btn btn-secondary" to="/categories">
                    Back to categories
                </Link>
            </main>
        );
    }

    return (
        <main className="feed-container">
            <header className="feed-header">
                <div className="header-left">
                    <div>
                        <h1>Ideon</h1>
                        <p className="muted">
                            Discover ideas. Find your people.
                        </p>
                    </div>
                </div>

                <nav className="header-actions">
                    <Link
                        className="btn btn-secondary"
                        to="/categories"
                    >
                        Categories
                    </Link>

                    <Link
                        className="create-idea-btn"
                        to="/ideas/new"
                    >
                        + New idea
                    </Link>
                    <Link
                     className="btn btn-secondary"
                    to="/collaboration-requests"
                    >
                        Requests
                </Link>
                </nav>
            </header>

            <section className="feed-toolbar">
                <div className="field search-field">
                    <label htmlFor="idea-search">Search ideas</label>
                    <input
                        id="idea-search"
                        type="search"
                        value={search}
                        onChange={(event) => setSearch(event.target.value)}
                        placeholder="Title, description, author…"
                    />
                </div>

                <div className="field sort-field">
                    <label htmlFor="idea-sort">Sort by</label>
                    <select
                        id="idea-sort"
                        value={sortBy}
                        onChange={(event) => setSortBy(event.target.value)}
                    >
                        <option value="newest">Newest first</option>
                        <option value="oldest">Oldest first</option>
                        <option value="title">Title A–Z</option>
                    </select>
                </div>
            </section>

            <div className="feed-summary">
                <h2>
                    {categoryParam ? "Ideas in this category" : "Explore ideas"}
                </h2>
                <span className="muted">
                    {visibleIdeas.length}{" "}
                    {visibleIdeas.length === 1 ? "idea" : "ideas"}
                </span>
            </div>

            <section className="feed-list" aria-label="Ideas">
                {visibleIdeas.length === 0 ? (
                    <div className="empty-state surface">
                        <h3>No ideas found</h3>
                        <p className="muted">
                            Try a different search or clear the search field.
                        </p>
                        {search && (
                            <button
                                className="btn btn-secondary"
                                type="button"
                                onClick={() => setSearch("")}
                            >
                                Clear search
                            </button>
                        )}
                    </div>
                ) : (
                    visibleIdeas.map((idea) => (
                        <article key={idea.id} className="idea-card">
                            <div className="idea-author-row">
                                <div className="author-avatar" aria-hidden="true">
                                    {(idea.authorUsername || "?")
                                        .charAt(0)
                                        .toUpperCase()}
                                </div>

                                <div>
                                    <p className="author-name">
                                        {idea.authorUsername || "Unknown author"}
                                    </p>
                                    <p className="post-time">
                                        {idea.categoryName}
                                        {" · "}
                                        {new Date(
                                            idea.createdAt
                                        ).toLocaleDateString()}
                                    </p>
                                </div>
                            </div>

                            <span className="category-badge">
                                {idea.categoryName}
                            </span>

                            <h3>
                                <Link to={`/ideas/${idea.id}`}>
                                    {idea.title}
                                </Link>
                            </h3>

                            <p className="idea-body-text">
                                {idea.description}
                            </p>

                            {idea.lookingForBuilder && (
                                <p className="builder-status">
                                    <span aria-hidden="true">✦</span>
                                    Looking for a collaborator
                                </p>
                            )}

                            <div className="idea-card-footer">
                                <Link
                                    className="idea-open-link"
                                    to={`/ideas/${idea.id}`}
                                >
                                    View idea and discussion →
                                </Link>
                            </div>
                        </article>
                    ))
                )}
            </section>
        </main>
    );
}
