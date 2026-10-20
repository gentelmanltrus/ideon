import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { login, register, saveAuth } from "../services/api";
// The LoginForm component provides a user interface for signing in or registering an account. It manages form state, handles form submission, and interacts with the backend API for authentication.
export default function LoginForm() {
    const navigate = useNavigate();

    const [isRegister, setIsRegister] = useState(false);
    const [username, setUsername] = useState("");
    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");
    const [error, setError] = useState("");
    const [loading, setLoading] = useState(false);

    async function handleSubmit(event) {
        event.preventDefault();
        setError("");

        if (!username.trim() || !password) {
            setError("Username and password are required.");
            return;
        }

        if (isRegister && !email.trim()) {
            setError("Email is required.");
            return;
        }

        setLoading(true);

        try {
            let auth;

            if (isRegister) {
                auth = await register({
                    username: username.trim(),
                    email: email.trim(),
                    password
                });
            } else {
                auth = await login({
                    usernameOrEmail: username.trim(),
                    password
                });
            }

            saveAuth(auth);
            navigate("/categories");
        } catch (err) {
            setError(err.message || "Authentication failed.");
        } finally {
            setLoading(false);
        }
    }

    return (
        <div className="AppContainer">
            <header>
                <h1>Ideon</h1>
            </header>

            <main>
                <p className="tagline">Make your ideas a reality</p>

                <h2>{isRegister ? "Create an account" : "Sign in"}</h2>

                <form onSubmit={handleSubmit}>
                    <div>
                        <label htmlFor="username">
                            {isRegister ? "Username" : "Username or email"}
                        </label>

                        <input
                            id="username"
                            name="username"
                            type="text"
                            autoComplete="username"
                            value={username}
                            onChange={(e) => setUsername(e.target.value)}
                            required
                        />
                    </div>

                    {isRegister && (
                        <div>
                            <label htmlFor="email">Email</label>

                            <input
                                id="email"
                                name="email"
                                type="email"
                                autoComplete="email"
                                value={email}
                                onChange={(e) => setEmail(e.target.value)}
                                required
                            />
                        </div>
                    )}

                    <div>
                        <label htmlFor="password">Password</label>

                        <input
                            id="password"
                            name="password"
                            type="password"
                            autoComplete={
                                isRegister ? "new-password" : "current-password"
                            }
                            value={password}
                            onChange={(e) => setPassword(e.target.value)}
                            minLength={isRegister ? 8 : undefined}
                            required
                        />
                    </div>

                    {error && <p role="alert">{error}</p>}

                    <button type="submit" disabled={loading}>
                        {loading
                            ? "Please wait..."
                            : isRegister
                                ? "Register"
                                : "Sign in"}
                    </button>
                </form>

                <p>
                    {isRegister
                        ? "Already have an account?"
                        : "Don't have an account?"}{" "}

                    <button
                        type="button"
                        onClick={() => {
                            setIsRegister(!isRegister);
                            setError("");
                        }}
                    >
                        {isRegister ? "Sign in" : "Register"}
                    </button>
                </p>
            </main>
        </div>
    );
}