import { useNavigate } from 'react-router-dom'
import { useState } from 'react'
import { validateLoginForm } from './validation'

export default function LoginForm() {
  const [formData, setFormData] = useState({ username: '', password: '' })
  const [isRegistering, setIsRegistering] = useState(false) 
  const [errors, setErrors] = useState({})
  const [serverError, setServerError] = useState('')
  const navigate = useNavigate()

  const handleChange = (e) => {
    const { name, value } = e.target
    setFormData({ ...formData, [name]: value })

    if (errors[name]) {
      setErrors({ ...errors, [name]: '' })
    }
  }

  const handleSubmit = async (e) => {
    e.preventDefault()
    setServerError('')
    const validationErrors = validateLoginForm(formData)

    if (Object.keys(validationErrors).length > 0) {
      setErrors(validationErrors)
      return
    }

    //  Dynamically pick endpoint based on mode
    const endpoint = isRegistering 
      ? 'http://localhost:5104/api/auth/register' 
      : 'http://localhost:5104/api/auth/login'

try {
      const response = await fetch(endpoint, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(formData)
      })

      // Parses JSON or handles text errors
      const isJson = response.headers.get('content-type')?.includes('application/json')
      const data = isJson ? await response.json() : null

      if (!response.ok) {
        throw new Error(data?.message || `Server error (${response.status}). Check your dotnet console.`)
      }

      // Saves user info in localStorage
      localStorage.setItem('ideon_user', JSON.stringify(data))
      localStorage.setItem('isAuthenticated', 'true')

      navigate('/categories')
    } catch (err) {
      setServerError(err.message || 'Failed to authenticate.')
    }
  }

  return (
    <div className="AppContainer">
      <header>
        <h1>Ideon</h1>
      </header>
      <main>
        <p className="tagline">
          {isRegistering ? 'Create your new account' : 'Make your ideas a reality'}
        </p>
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

          {serverError && <p className="error" role="alert">{serverError}</p>}

          <button type="submit" className="button">
            {isRegistering ? 'Sign Up' : 'Log In'}
          </button>
        </form>

        <div style={{ marginTop: '1rem', textAlign: 'center' }}>
          <button 
            type="button" 
            className="link-button" 
            onClick={() => {
              setIsRegistering(!isRegistering)
              setServerError('')
            }}
            style={{ background: 'none', border: 'none', color: '#0070f3', cursor: 'pointer'}}
          >
            {isRegistering ? 'Already have an account? Log In' : "Don't have an account? Sign Up"}
          </button>
        </div>
      </main>
    </div>
  )
}