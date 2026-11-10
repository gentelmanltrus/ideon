import React, { useEffect, useState } from "react";
import { useParams, Link } from "react-router-dom";
import { getUserProfile } from "../services/api";
import "../styles/ProfilePage.css";

export default function ProfilePage() {
    const { username } = useParams();
    const [profile, setProfile] = useState(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);

    useEffect(() => {
        getUserProfile(username)
            .then((data) => {
                setProfile(data);
                setLoading(false);
            })
            .catch((err) => {
                setError(err.message);
                setLoading(false);
            });
    }, [username]);

    if (loading) return <div className="profile-status">Loading profile...</div>;
    if (error) return <div className="profile-status profile-error">{error}</div>;

    return (
        <div className="profile-container">
            <div className="profile-header">
                <div className="author-avatar profile-main-avatar" aria-hidden="true">
                    {(profile.username || "?")
                        .charAt(0)
                        .toUpperCase()}
                </div>
                <div className="profile-info">
                    <h1 className="profile-username">{profile.username}</h1>
                    <p className="profile-bio">{profile.bio}</p>
                </div>
            </div>

            {/* Created ideas */}
            <h2 className="profile-ideas-title">
                Ideas Created by {profile.username} ({profile.ideas.length})
            </h2>

            <div className="profile-ideas-grid">
                {profile.ideas.length > 0 ? (
                    profile.ideas.map((idea) => (
                        <div key={idea.id} className="idea-card">
                            <Link
                                to={`/ideas/${idea.id}`}
                                className="idea-title-link"
                            >
                                {idea.title}
                            </Link>
                            <p className="idea-description">{idea.description}</p>
                            <div className="idea-meta">
                                <span className="idea-category-badge">
                                    {idea.categoryName}
                                </span>
                                <span className="idea-date">
                                    Posted on {new Date(idea.createdAt).toLocaleDateString()}
                                </span>
                            </div>
                        </div>
                    ))
                ) : (
                    <div className="profile-empty-state">
                        This user hasn't posted any ideas yet.
                    </div>
                )}
            </div>
        </div>
    );
}