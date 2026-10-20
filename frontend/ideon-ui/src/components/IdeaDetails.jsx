import { useCallback, useEffect, useState } from "react";
import { Link, useParams } from "react-router-dom";
import {
    createComment,
    getComments,
    getIdeaById,
    getToken
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

    const loadComments = useCallback(async () => {
        setCommentsLoading(true);
        setCommentsError("");

        try {
            const data = await getComments(id);
            setComments(data);
        } catch (err) {
            setCommentsError(err.message);
        } finally {
            setCommentsLoading(false);
        }
    }, [id]);

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
        loadComments();
    }, [loadComments]);

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
                <p><strong>Looking for a Builder</strong></p>
            )}

            <hr />

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