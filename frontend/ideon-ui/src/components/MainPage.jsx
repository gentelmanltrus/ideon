import { useEffect, useState } from "react";
import { Link, useSearchParams } from "react-router-dom";
import { getPagedIdeas } from "../services/api";
import "../styles/MainPage.css";
//this is the main page of the application, which displays a list of ideas and allows users to search and sort them. It also handles loading and error states.
export default function MainPage() {
    const [searchParams] = useSearchParams();
    const categoryParam = searchParams.get("category");
    //usestate hooks to manage the state of ideas, search query, sorting option, loading state, and error message.
    const [pagedResult, setPagedResult] = useState({
        items: [],
        totalCount: 0,
        totalPages: 0
    });
    const [search, setSearch] = useState("");
    const [sortBy, setSortBy] = useState("newest");
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState("");
    const [pageState, setPageState] = useState({
        categoryParam,
        page: 1
    });
    const page = pageState.categoryParam === categoryParam
        ? pageState.page
        : 1;
    const setPage = (nextPage) => {
        setPageState((currentState) => {
            const currentPage = currentState.categoryParam === categoryParam
                ? currentState.page
                : 1;

            return {
                categoryParam,
                page: typeof nextPage === "function"
                    ? nextPage(currentPage)
                    : nextPage
            };
        });
    };
    const [pageSize] = useState(10);
    useEffect(() => {
        let cancelled = false;

        async function loadIdeas() {
            setLoading(true);
            setError("");

            try {
                const categoryId = categoryParam
                    ? Number(categoryParam)
                    : null;
                const data = await getPagedIdeas(page, pageSize, {
                    search,
                    categoryId,
                    sortBy
                });

                if (!cancelled) {
                    setPagedResult(data);
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
    }, [page, pageSize, search, sortBy, categoryParam]);
    const visibleIdeas = pagedResult.items;

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
                        onChange={(event) => {
                            setSearch(event.target.value);
                            setPage(1);
                        }}
                        placeholder="Title, description, author…"
                    />
                </div>

                <div className="field sort-field">
                    <label htmlFor="idea-sort">Sort by</label>
                    <select
                        id="idea-sort"
                        value={sortBy}
                        onChange={(event) => {
                            setSortBy(event.target.value);
                            setPage(1);
                        }}
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
                    {pagedResult.totalCount}{" "}
                    {pagedResult.totalCount === 1 ? "idea" : "ideas"}
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

                            {/* --- Aggregate Ratings Section --- */}
                            <div className="idea-card-ratings">
                                <div className="rating-pill">
                                    <span>Originality</span>
                                    <strong>
                                        {idea.ratingCount > 0
                                            ? Number(idea.averageOriginality).toFixed(1)
                                            : "-"}
                                    </strong>
                                </div>
                                <div className="rating-pill">
                                    <span>Feasibility</span>
                                    <strong>
                                        {idea.ratingCount > 0
                                            ? Number(idea.averageFeasibility).toFixed(1)
                                            : "-"}
                                    </strong>
                                </div>
                                <div className="rating-pill">
                                    <span>Usefulness</span>
                                    <strong>
                                        {idea.ratingCount > 0
                                            ? Number(idea.averageUsefulness).toFixed(1)
                                            : "-"}
                                    </strong>
                                </div>
                                <span className="rating-count-text">
                                    ({idea.ratingCount || 0} {idea.ratingCount === 1 ? "vote" : "votes"})
                                </span>
                            </div>

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

            {pagedResult.totalPages > 1 && (
                <nav className="pagination" aria-label="Ideas pagination">
                    <button
                        type="button"
                        className="btn btn-secondary"
                        onClick={() => setPage((currentPage) => currentPage - 1)}
                        disabled={page <= 1}
                    >
                        Previous
                    </button>
                    <span className="muted">
                        Page {page} of {pagedResult.totalPages}
                    </span>
                    <button
                        type="button"
                        className="btn btn-secondary"
                        onClick={() => setPage((currentPage) => currentPage + 1)}
                        disabled={page >= pagedResult.totalPages}
                    >
                        Next
                    </button>
                </nav>
            )}
        </main>
    );
}
