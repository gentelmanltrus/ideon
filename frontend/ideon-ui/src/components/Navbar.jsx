import { Link, useNavigate } from "react-router-dom";
import { getCurrentUser, clearAuth } from "../services/api";
import "../styles/Navbar.css";

export default function Navbar() {
  const user = getCurrentUser();
  const navigate = useNavigate();

  const handleLogout = () => {
    clearAuth(); // 
    navigate("/"); 
    window.location.reload(); 
  };

  if (!user) return null; 

  return (
    <nav className="navbar">
      <div className="navbar-brand">
        <Link to="/main">Ideon</Link>
      </div>
      <div className="navbar-links">
        <Link to={`/profile/${user.username}`}>
          My Profile ({user.username})
        </Link>
        <button onClick={handleLogout} className="logout-btn">
          Logout
        </button>
      </div>
    </nav>
  );
}