import { useEffect, useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import { getCategories } from "../services/api";
import "../styles/CategorySelect.css";

//category select page, where the user can select a category to view ideas in that category
export default function CategorySelect() {
  const navigate = useNavigate();

  const [categories, setCategories] = useState([]);//state for categories, loading and error
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  //fetch categories from the backend and set the state accordingly
  useEffect(() => {
    let cancelled = false;

    async function loadCategories() {
      try {
        const data = await getCategories();

        if (!cancelled) {
          setCategories(data);
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

    loadCategories();

    return () => {
      cancelled = true;
    };
  }, []);

  //render loading, error or the category select page
  if (loading) return <p>Loading categories...</p>;
  if (error) return <p role="alert">Could not load categories: {error}</p>;

  //render the category select page with a grid of categories, each category is a button that navigates to the main page with the selected category as a query parameter
  return (
    <main className="category-container">
      <h1>Share your idea with the world!</h1>
      <h2>Please select a category</h2>

      <div className="category-grid">
        {categories.map((category) => (
          <button
            type="button"
            key={category.id}
            className="category-card"
            onClick={() =>
              navigate(`/main?category=${category.id}`)
            }
          >
            <h3>{category.name}</h3>
          </button>
        ))}
      </div>

      <p>
        <Link to="/main">View all ideas</Link>
      </p>
      <p>
        <Link to="/ideas/new">Create an idea</Link>
      </p>
    </main>
  );
}