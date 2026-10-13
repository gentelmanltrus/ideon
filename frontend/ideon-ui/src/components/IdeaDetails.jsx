import { useEffect, useState } from "react";
import { Link, useParams } from "react-router-dom";
import { getIdeaById } from "../services/api";
//summary: This file contains the IdeaDetails component, which is responsible for displaying the details of a specific idea. It uses the useParams hook from react-router-dom to get the idea ID from the URL, and it fetches the idea data from the API using the getIdeaById function. The component manages loading and error states, and it renders the idea details or appropriate messages based on the state.
export default function IdeaDetails() {
    const { id } = useParams();

    const [idea, setIdea] = useState(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState("");

    useEffect(() => {
        let cancelled = false;

        async function loadIdea() {
            setLoading(true);
            setError("");
            setIdea(null);

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

    if (loading) {
        return <p>Loading idea...</p>;
    }

    if (error) {
        return (
            <main>
                <p>Could not load this idea: {error}</p>
                <Link to="/">Back to Home</Link>
            </main>
        );
    }

    if (!idea) {
        return <p>Idea not found.</p>;
    }

    return (
        <main>
            <Link to="/">← Back to Home</Link>

            <h1>{idea.title}</h1>

            <p>{idea.description}</p>

            <p>
                Category: {idea.categoryName}
            </p>

            <p>
                Author: {idea.authorUsername}
            </p>

            <p>
                Created:{" "}
                {new Date(idea.createdAt).toLocaleDateString()}
            </p>

            {idea.lookingForBuilder && (
                <p>Looking for a Builder</p>
            )}
        </main>
    );
}