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
        // Load idea details
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
    // Collaboration request handler
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
        return <main><p>Loading idea...</p></main>;
    }

    if (error) {
        return (
            <main>
                <p role="alert">Could not load this idea: {error}</p>
                <Link to="/categories">Back to categories</Link>
            </main>
        );
    }

    if (!idea) {
        return <main><p>Idea not found.</p></main>;
    }

    return (
        <main>
            <p>
                <Link to="/main">← Back to ideas</Link>
            </p>

            <h1>{idea.title}</h1>

            <p>{idea.description}</p>

            <p>
                Category: {idea.categoryName}
            </p>

            <p>
                Author: {idea.authorUsername}
            </p>

            <p>
                Created: {new Date(idea.createdAt).toLocaleString()}
            </p>

            {idea.lookingForBuilder && (
                <section>
        <h2>Collaboration</h2>

        <p>The author is looking for a builder.</p>

        <button
            type="button"
            onClick={handleCollaborationRequest}
            disabled={collaborationLoading || !getToken()}
        >
            {collaborationLoading
                ? "Sending..."
                : "Request to collaborate"}
        </button>

        {!getToken() && (
            <p>
                <Link to="/">Sign in</Link> to request collaboration.
            </p>
        )}

        {collaborationMessage && (
            <p role="status">{collaborationMessage}</p>
        )}

        {collaborationError && (
            <p role="alert">{collaborationError}</p>
        )}
    </section>
)}
                <p><strong>Looking for a Builder</strong></p>
            

            <hr />
            <section>
    <h2>Rate this idea</h2>

    <p>
        {ratings.length} rating(s)
    </p>

    <form onSubmit={handleRatingSubmit}>
        {[
            ["originality", "Originality"],
            ["feasibility", "Feasibility"],
            ["usefulness", "Usefulness"]
        ].map(([key, label]) => (
            <div key={key}>
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

        {ratingError && <p role="alert">{ratingError}</p>}

        <button
            type="submit"
            disabled={ratingLoading || !getToken()}
        >
            {ratingLoading ? "Saving..." : "Submit rating"}
        </button>
    </form>
</section>

            <section>
                <h2>Comments</h2>

                {commentsLoading && <p>Loading comments...</p>}

                {commentsError && (
                    <p role="alert">{commentsError}</p>
                )}

                {!commentsLoading && comments.length === 0 && (
                    <p>No comments yet.</p>
                )}

                <div>
                    {comments.map((comment) => (
                        <article key={comment.id}>
                            <strong>{comment.authorUsername}</strong>

                            <p>{comment.content}</p>

                            <small>
                                {new Date(comment.createdAt).toLocaleString()}
                            </small>
                        </article>
                    ))}
                </div>

                <form onSubmit={handleCommentSubmit}>
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
                        disabled={commentSubmitting || !getToken()}
                    >
                        {commentSubmitting ? "Posting..." : "Post comment"}
                    </button>

                    {!getToken() && (
                        <p>
                            <Link to="/">Sign in</Link> to write a comment.
                        </p>
                    )}
                </form>
            </section>
        </main>
    );
}