// This component displays a list of incoming collaboration requests for the current user. It allows the user to accept or reject each request and handles loading and error states.
import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import {
    getIncomingCollaborationRequests,
    updateCollaborationRequestStatus
} from "../services/api";
import "../styles/IncomingRequests.css";

export default function IncomingRequests() {
    const [requests, setRequests] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState("");
    const [actionError, setActionError] = useState("");
    const [busyId, setBusyId] = useState(null);

    useEffect(() => {
    let cancelled = false;

    async function loadRequests() {
        try {
            const data = await getIncomingCollaborationRequests();

            if (!cancelled) {
                setRequests(data);
                setError("");
            }
        } catch (err) {
            if (!cancelled) {
                setError(err.message || "Could not load requests.");
            }
        } finally {
            if (!cancelled) {
                setLoading(false);
            }
        }
    }

    loadRequests();

    return () => {
        cancelled = true;
    };
}, []);

    async function handleDecision(requestId, status) {
        setBusyId(requestId);
        setActionError("");

        try {
            const updatedRequest =
                await updateCollaborationRequestStatus(
                    requestId,
                    status
                );

            setRequests((currentRequests) =>
                currentRequests.map((request) =>
                    request.id === requestId
                        ? updatedRequest
                        : request
                )
            );
        } catch (err) {
            setActionError(
                err.message || "Could not update the request."
            );
        } finally {
            setBusyId(null);
        }
    }

    return (
        <main className="requests-page">
            <header className="requests-header">
                <div>
                    <p className="requests-eyebrow">YOUR IDEAS</p>
                    <h1>Collaboration requests</h1>
                    <p className="muted">
                        People who want to help bring your ideas to life.
                    </p>
                </div>

                <Link className="btn btn-secondary" to="/main">
                    Back to ideas
                </Link>
            </header>

            {actionError && (
                <p className="requests-error" role="alert">
                    {actionError}
                </p>
            )}

            {loading ? (
                <p className="status-message">Loading requests…</p>
            ) : error ? (
                <div className="requests-empty">
                    <p className="error-message" role="alert">
                        {error}
                    </p>
                    <button onClick={() => window.location.reload()}>Try again</button>
                </div>
            ) : requests.length === 0 ? (
                <div className="requests-empty surface">
                    <h2>No requests yet</h2>
                    <p className="muted">
                        When someone wants to collaborate on one of your
                        ideas, their request will appear here.
                    </p>
                    <Link className="btn btn-primary" to="/ideas/new">
                        Create an idea
                    </Link>
                </div>
            ) : (
                <section className="requests-list">
                    {requests.map((request) => {
                        const isPending = request.status === "Pending";
                        const isBusy = busyId === request.id;

                        return (
                            <article
                                className="request-card surface"
                                key={request.id}
                            >
                                <div className="request-card-top">
                                    <div>
                                        <p className="muted request-label">
                                            COLLABORATION ON
                                        </p>
                                        <h2>{request.ideaTitle}</h2>
                                    </div>

                                    <span
                                        className={`request-status status-${request.status.toLowerCase()}`}
                                    >
                                        {request.status}
                                    </span>
                                </div>

                                <p>
                                    <strong>{request.username}</strong>
                                    {" wants to collaborate on this idea."}
                                </p>

                                <p className="muted request-date">
                                    Received{" "}
                                    {new Date(
                                        request.createdAt
                                    ).toLocaleDateString()}
                                </p>

                                <div className="request-card-footer">
                                    <Link
                                        to={`/ideas/${request.ideaId}`}
                                        className="idea-open-link"
                                    >
                                        View idea →
                                    </Link>

                                    {isPending && (
                                        <div className="request-actions">
                                            <button
                                                className="btn btn-secondary"
                                                type="button"
                                                disabled={isBusy}
                                                onClick={() =>
                                                    handleDecision(
                                                        request.id,
                                                        "Rejected"
                                                    )
                                                }
                                            >
                                                Reject
                                            </button>

                                            <button
                                                className="btn btn-primary"
                                                type="button"
                                                disabled={isBusy}
                                                onClick={() =>
                                                    handleDecision(
                                                        request.id,
                                                        "Accepted"
                                                    )
                                                }
                                            >
                                                {isBusy
                                                    ? "Saving…"
                                                    : "Accept"}
                                            </button>
                                        </div>
                                    )}
                                </div>
                            </article>
                        );
                    })}
                </section>
            )}
        </main>
    );
}
