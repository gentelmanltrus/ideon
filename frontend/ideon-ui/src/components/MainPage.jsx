import { useEffect, useState } from 'react'
import { useSearchParams, Link } from 'react-router-dom'
import '../styles/MainPage.css'
import { getIdeas } from '../services/api'

// Displays ideas from the backend, filtered by the selected category.
export default function MainPage() {
  const [searchParams] = useSearchParams()
  const activeCategory = searchParams.get('category') || 'all'

  const [ideas, setIdeas] = useState([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState('')
  // Fetch ideas from the backend when the component mounts.
  useEffect(() => {
    async function loadIdeas() {
      try {
        const data = await getIdeas()
        setIdeas(data)
      } catch (err) {
        setError(err.message)
      } finally {
        setLoading(false)
      }
    }

    loadIdeas()
  }, [])
  // Filter ideas based on the selected category. If 'all' is selected, show all ideas; otherwise, filter by the selected category ID.
  const filteredIdeas =
    activeCategory === 'all'
      ? ideas
      : ideas.filter((idea) => idea.categoryId === Number(activeCategory))
  // Render loading, error, or the main page with the list of ideas filtered by the selected category.
  if (loading) {
    return <div className="feed-container">Loading ideas...</div>
  }

  if (error) {
    return <div className="feed-container">Error: {error}</div>
  }

  return (
    <div className="feed-container">
      <header className="feed-header">
        <div className="header-left">
          <h2>Ideon</h2>
          <span className="category-badge">{activeCategory.toUpperCase()}</span>
        </div>
        <div className="header-actions">
          <Link to="/ideas/new" className="create-idea-btn">Create an idea</Link>
          <Link to="/categories" className="change-category-btn">Change Category</Link>
        </div>
      </header>

      <div className="feed-list">
        {filteredIdeas.map((idea) => (
          <Link
            key={idea.id}
            to={`/ideas/${idea.id}`}
            className="idea-card"
          >
            <div className="idea-author-row">
              <img src={idea.avatar} alt={idea.authorUsername} className="author-avatar" />
              <div>
                <h3 className="author-name">{idea.authorUsername}</h3>
                <span className="post-time">{new Date(idea.createdAt).toLocaleDateString()} • <span className="post-cat">{idea.categoryName}</span></span>
              </div>
            </div>

            <div className="idea-content">
              <p className="idea-body-text">{idea.description}</p>
            </div>
          </Link>
        ))}
      </div>
    </div>
  )
}