import { useState, useEffect } from "react";
import { Link, useParams } from "react-router-dom";
import {
    createComment,
    getComments,
    getIdeaById,
    getToken
} from "../services/api";
import { createRating, getRatings, updateMyRating } from "../services/api";
import {
    createCollaborationRequest
} from "../services/api";
import "../styles/IdeaDetails.css";

//summary: This file contains the IdeaDetails component, which displays detailed information about a specific idea, including its title, description, category, author, creation date, and comments. It uses React hooks to manage state and side effects. The component fetches the idea details and comments from the API when it mounts and provides a form for users to submit new comments. Error handling is implemented to provide feedback to the user in case of issues during fetching or submission.
export default function IdeaDetails() {
    const { id } = useParams();

    const [idea, setIdea] = useState(null);
    const [comments, setComments] = useState([]);
    const [commentText, setCommentText] = useState("");

    const [loading, setLoading] = useState(true);
    const [commentsLoading, setCommentsLoading] = useState(true);
    const [commentSubmitting, setCommentSubmitting] = useState(false);

    const [error, setError] = useState("");
    const [commentsError, setCommentsError] = useState("");

    // Collaboration request state
    const [collaborationError, setCollaborationError] = useState("");
    const [collaborationMessage, setCollaborationMessage] = useState("");
    const [collaborationLoading, setCollaborationLoading] = useState(false);

    const [ratings, setRatings] = useState([]);
    const [ratingValues, setRatingValues] = useState({
        originality: 3,
        feasibility: 3,
        usefulness: 3
    });
    const [ratingError, setRatingError] = useState("");
    const [ratingLoading, setRatingLoading] = useState(false);

    useEffect(() => {
        let cancelled = false;
        async function loadIdea() {
            setLoading(true);
            setError("");

            try {
                const data = await getIdeaById(id);

                if (!cancelled) {
                    setIdea(data);
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

        loadIdea();

        return () => {
            cancelled = true;
        };
    }, [id]);

    useEffect(() => {
        let cancelled = false;

        async function loadComments() {
            try {
                const data = await getComments(id);

                if (!cancelled) {
                    setComments(data);
                    setCommentsError("");
                }
            } catch (err) {
                if (!cancelled) {
                    setCommentsError(err.message);
                }
            } finally {
                if (!cancelled) {
                    setCommentsLoading(false);
                }
            }
        }

        loadComments();

        return () => {
            cancelled = true;
        };
    }, [id]);

    useEffect(() => {
        let cancelled = false;

        getRatings(id)
            .then((data) => {
                if (!cancelled) {
                    setRatings(data);
                }
            })
            .catch((err) => {
                if (!cancelled) {
                    setRatingError(err.message);
                }
            });

        return () => {
            cancelled = true;
        };
    }, [id]);

    async function handleCommentSubmit(event) {
        event.preventDefault();
        setCommentsError("");

        const content = commentText.trim();

        if (!content) {
            setCommentsError("Comment cannot be empty.");
            return;
        }

        if (!getToken()) {
            setCommentsError("Please sign in to write a comment.");
            return;
        }

        setCommentSubmitting(true);

        try {
            const created = await createComment(id, content);

            setComments((current) => [...current, created]);
            setCommentText("");
        } catch (err) {
            setCommentsError(err.message);
        } finally {
            setCommentSubmitting(false);
        }
    }

    async function handleCollaborationRequest() {
        setCollaborationError("");
        setCollaborationMessage("");

        if (!getToken()) {
            setCollaborationError("Please sign in first.");
            return;
        }

        setCollaborationLoading(true);

        try {
            await createCollaborationRequest(id);

            setCollaborationMessage(
                "Your collaboration request has been sent."
            );
        } catch (err) {
            setCollaborationError(err.message);
        } finally {
            setCollaborationLoading(false);
        }
    }

    async function handleRatingSubmit(event) {
        event.preventDefault();
        setRatingError("");

        if (!getToken()) {
            setRatingError("Please sign in to rate an idea.");
            return;
        }

        const payload = {
            originality: Number(ratingValues.originality),
            feasibility: Number(ratingValues.feasibility),
            usefulness: Number(ratingValues.usefulness)
        };

        setRatingLoading(true);

        try {
            let saved;

            try {
                saved = await createRating(id, payload);
            } catch (err) {
                if (err.message.includes("already rated")) {
                    saved = await updateMyRating(id, payload);
                } else {
                    throw err;
                }
            }

            setRatings((current) => {
                const others = current.filter((r) => r.id !== saved.id);
                return [...others, saved];
            });
        } catch (err) {
            setRatingError(err.message);
        } finally {
            setRatingLoading(false);
        }
    }

    if (loading) {
        return <main className="idea-details-container"><p className="loading-state">Loading idea...</p></main>;
    }

    if (error) {
        return (
            <main className="idea-details-container">
                <p className="error-message" role="alert">Could not load this idea: {error}</p>
                <Link to="/categories" className="idea-details-back-link">Back to categories</Link>
            </main>
        );
    }

    if (!idea) {
        return <main className="idea-details-container"><p className="loading-state">Idea not found.</p></main>;
    }

    return (
        <main className="idea-details-container">
            <Link to="/main" className="idea-details-back-link">← Back to ideas</Link>

            <h1 className="idea-details-title">{idea.title}</h1>

            <div className="idea-details-meta">
                <div>Category: <span>{idea.categoryName}</span></div>
                <div>Author: <span>{idea.authorUsername}</span></div>
                <div>Created: <span>{new Date(idea.createdAt).toLocaleString()}</span></div>
            </div>

            <p className="idea-details-description">{idea.description}</p>

            {idea.lookingForBuilder && (
                <section className="idea-details-section">
                    <h2>Collaboration</h2>
                    <div className="collaboration-card">
                        <p style={{ marginTop: 0 }}>The author is looking for a builder.</p>

                        <button
                            type="button"
                            className="idea-details-action-btn"
                            onClick={handleCollaborationRequest}
                            disabled={collaborationLoading || !getToken()}
                        >
                            {collaborationLoading
                                ? "Sending..."
                                : "Request to collaborate"}
                        </button>

                        {!getToken() && (
                            <p style={{ marginTop: "12px", marginBottom: 0 }}>
                                <Link to="/" style={{ color: "var(--color-primary)", fontWeight: "650" }}>Sign in</Link> to request collaboration.
                            </p>
                        )}

                        {collaborationMessage && (
                            <p className="status-message" role="status">{collaborationMessage}</p>
                        )}

                        {collaborationError && (
                            <p className="error-message" role="alert">{collaborationError}</p>
                        )}
                    </div>
                </section>
            )}

            <section className="idea-details-section">
                <h2>Rate this idea</h2>
                <div className="rating-card">
                    <p style={{ marginTop: 0, color: "var(--color-muted)", fontWeight: "600" }}>
                        {ratings.length} rating(s)
                    </p>

                    <form onSubmit={handleRatingSubmit}>
                        <div className="rating-sliders-grid">
                            {[
                                ["originality", "Originality"],
                                ["feasibility", "Feasibility"],
                                ["usefulness", "Usefulness"]
                            ].map(([key, label]) => (
                                <div key={key} className="rating-slider-group">
                                    <label htmlFor={`rating-${key}`}>
                                        {label}: {ratingValues[key]}
                                    </label>

                                    <input
                                        id={`rating-${key}`}
                                        type="range"
                                        min="1"
                                        max="5"
                                        step="1"
                                        value={ratingValues[key]}
                                        onChange={(e) => setRatingValues((current) => ({
                                            ...current,
                                            [key]: Number(e.target.value)
                                        }))}
                                    />
                                </div>
                            ))}
                        </div>

                        {ratingError && <p className="error-message" role="alert">{ratingError}</p>}

                        <button
                            type="submit"
                            className="idea-details-action-btn"
                            disabled={ratingLoading || !getToken()}
                        >
                            {ratingLoading ? "Saving..." : "Submit rating"}
                        </button>
                    </form>
                </div>
            </section>

            <section className="idea-details-section">
                <h2>Comments</h2>
                <div className="comments-card">
                    {commentsLoading && <p style={{ color: "var(--color-muted)" }}>Loading comments...</p>}

                    {commentsError && (
                        <p className="error-message" role="alert">{commentsError}</p>
                    )}

                    {!commentsLoading && comments.length === 0 && (
                        <p style={{ color: "var(--color-muted)", marginTop: 0 }}>No comments yet.</p>
                    )}

                    <div className="comments-list">
                        {comments.map((comment) => (
                            <article key={comment.id} className="comment-item">
                                <div className="comment-header">
                                    <span className="comment-author">{comment.authorUsername}</span>
                                    <span className="comment-time">
                                        {new Date(comment.createdAt).toLocaleString()}
                                    </span>
                                </div>
                                <p className="comment-text">{comment.content}</p>
                            </article>
                        ))}
                    </div>

                    <form onSubmit={handleCommentSubmit} className="comment-form">
                        <label htmlFor="comment">Write a comment</label>

                        <textarea
                            id="comment"
                            value={commentText}
                            onChange={(e) => setCommentText(e.target.value)}
                            maxLength={1000}
                            rows={3}
                            required
                        />

                        <button
                            type="submit"
                            className="idea-details-action-btn"
                            disabled={commentSubmitting || !getToken()}
                        >
                            {commentSubmitting ? "Posting..." : "Post comment"}
                        </button>

                        {!getToken() && (
                            <p style={{ margin: "4px 0 0", color: "var(--color-muted)" }}>
                                <Link to="/" style={{ color: "var(--color-primary)", fontWeight: "650" }}>Sign in</Link> to write a comment.
                            </p>
                        )}
                    </form>
                </div>
            </section>
        </main>
    );
}