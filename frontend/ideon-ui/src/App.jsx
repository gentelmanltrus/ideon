import './App.css'
import { Routes, Route } from 'react-router-dom'
import LoginForm from './components/LoginForm'
import MainPage from './components/MainPage'
import CategorySelect from './components/CategorySelect'

function App() {
  return (
    <Routes>
      <Route path="/" element={<LoginForm />} />
      <Route path="/categories" element={<CategorySelect />} />
      <Route path="/main" element={<MainPage />} />
    </Routes>
  )
}

export default App