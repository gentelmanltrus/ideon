import { useState } from 'react'
import { useSearchParams, Link } from 'react-router-dom'
import '../styles/MainPage.css'

const INITIAL_IDEAS = [
  {
    id: 1,
    category: 'programming',
    author: 'Alex R',
    avatar: 'https://api.dicebear.com/7.x/bottts/svg?seed=Alex',
    timestamp: '2 hours ago',
    body: 'Imagine a website where you can share ideas and others can try and make them!',
    comments: [
      { id: 101, author: 'Sarah T', text: 'Seems fun to do!' }
    ]
  }
]

export default function MainFeed() {
  const [searchParams] = useSearchParams()
  const activeCategory = searchParams.get('category') || 'all'

  const [ideas, setIdeas] = useState(INITIAL_IDEAS)

  const [newBody, setNewBody] = useState('')
  const [commentInputs, setCommentInputs] = useState({})

  // if no category selected, defaults to all
  const filteredIdeas = activeCategory === 'all' ? ideas: ideas.filter(item => item.category === activeCategory)

  const handlePostSubmit = (e) => {
    e.preventDefault()
    if (!newBody.trim()) return

    const newIdeaObj = {
      id: Date.now(),
      category: activeCategory,
      author: 'You',
      avatar: 'https://api.dicebear.com/7.x/bottts/svg?seed=You',
      timestamp: 'Just now',
      body: newBody,
      comments: []
    }

    setIdeas([newIdeaObj, ...ideas])
    setNewBody('')
  }

  const handleAddComment = (ideaId) => {
    const text = commentInputs[ideaId]
    if (!text || !text.trim()) return

    setIdeas(ideas.map(idea => {
      if (idea.id === ideaId) {
        return {
          ...idea,
          comments: [...idea.comments, { id: Date.now(), author: 'You', text }]
        }
      }
      return idea
    }))

    setCommentInputs({ ...commentInputs, [ideaId]: '' })
  }

  return (
    <div className="feed-container">
      <header className="feed-header">
        <div className="header-left">
          <h2>Ideon</h2>
          <span className="category-badge">{activeCategory.toUpperCase()}</span>
        </div>
        <Link to="/categories" className="change-category-btn">Change Category</Link>
      </header>

      <div className="post-box-card">
        <div className="post-box-header">
          <img src="https://api.dicebear.com/7.x/bottts/svg?seed=You" alt="Avatar" className="user-avatar" />
        </div>
        <textarea
          placeholder="Write your idea..."
          value={newBody}
          onChange={(e) => setNewBody(e.target.value)}
          rows="3"
        />
        <div className="post-box-actions">
          <button onClick={handlePostSubmit} className="post-btn">Share Idea</button>
        </div>
      </div>

      <div className="feed-list">
        {filteredIdeas.map((idea) => (
          <article key={idea.id} className="idea-card">
            <div className="idea-author-row">
              <img src={idea.avatar} alt={idea.author} className="author-avatar" />
              <div>
                <h3 className="author-name">{idea.author}</h3>
                <span className="post-time">{idea.timestamp} • <span className="post-cat">{idea.category}</span></span>
              </div>
            </div>

            <div className="idea-content">
              <p className="idea-body-text">{idea.body}</p>
            </div>

            <div className="comments-section">
              <div className="comment-input-row">
                <input 
                  type="text" 
                  placeholder="Evaluate or give advice on this idea..."
                  value={commentInputs[idea.id] || ''}
                  onChange={(e) => setCommentInputs({ ...commentInputs, [idea.id]: e.target.value })}
                />
                <button onClick={() => handleAddComment(idea.id)}>Comment</button>
              </div>

              {idea.comments.length > 0 && (
                <div className="comments-list">
                  {idea.comments.map(comment => (
                    <div key={comment.id} className="comment-item">
                      <strong>@{comment.author}: </strong>
                      <span>{comment.text}</span>
                    </div>
                  ))}
                </div>
              )}
            </div>
          </article>
        ))}
      </div>
    </div>
  )
}