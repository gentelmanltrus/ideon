import { useState } from 'react'
import './App.css'
import { validateLoginForm } from './components/validation'

function App() {
  const [formData, setFormData] = useState({ username: '', password: '' })
  const [errors, setErrors] = useState({})

  const handleChange = (e) => {
    const { name, value } = e.target
    setFormData({ ...formData, [name]: value })

    if (errors[name]) {
      setErrors({ ...errors, [name]: '' })
    }
  }

  const handleSubmit = (e) => {
    e.preventDefault()
    const validationErrors = validateLoginForm(formData)

    if (Object.keys(validationErrors).length > 0) {
      setErrors(validationErrors)
    }
    else {
      console.log('Form submitted successfully:', formData)
    }
  }

  return (
    <div className="AppContainer">
      <header>
        <h1>Ideon</h1>
      </header>
      <main>
        <p className="tagline">Make your ideas a reality</p>
        <form onSubmit={handleSubmit}>
          <div>
            <input
              id="username"
              name="username"
              type="text"
              placeholder="Username"
              value={formData.username}
              onChange={handleChange}
            />
            {errors.username && <span className="error">{errors.username}</span>}
          </div>

          <div>
            <input
              id="password"
              name="password"
              type="password"
              placeholder="Password"
              value={formData.password}
              onChange={handleChange}
            />
            {errors.password && <span className="error">{errors.password}</span>}
          </div>

          <button type="submit">Submit</button>
        </form>
      </main>
    </div>
  )
}

export default App