import { useNavigate } from 'react-router-dom'
import '../styles/CategorySelect.css'

const CATEGORIES = [
  { id: 'programming', name: 'Software & Web Apps'},
  { id: 'gamedev', name: 'Game Development'},
  { id: 'engineering', name: 'Hardware & Engineering'},
  { id: 'art', name: 'Art & Design'}
]

export default function CategorySelect() {
  const navigate = useNavigate()

  const handleSelect = (categoryId) => {
    navigate(`/main?category=${categoryId}`)
  }

  return (
    <div className="category-container">
      <h1>Share your idea with the world!</h1>
      <h2>Please select a category</h2>

      <div className="category-grid">
        {CATEGORIES.map((cat) => (
          <div 
            key={cat.id} 
            className="category-card"
            onClick={() => handleSelect(cat.id)}
          >
            <span className="icon">{cat.icon}</span>
            <h3>{cat.name}</h3>
            <p>{cat.desc}</p>
          </div>
        ))}
      </div>
    </div>
  )
}