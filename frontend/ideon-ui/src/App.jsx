import './App.css'
import { Routes, Route } from 'react-router-dom'
import LoginForm from './components/LoginForm'
import MainPage from './components/MainPage'
import CategorySelect from './components/CategorySelect'
import IdeaDetails from "./components/IdeaDetails";
import CreateIdea from "./components/CreateIdea";
function App() {
  return (
    <Routes>
      <Route path="/" element={<LoginForm />} />
      <Route path="/categories" element={<CategorySelect />} />
      <Route path="/ideas/:id" element={<IdeaDetails />} />
      <Route path="/main" element={<MainPage />} />
      <Route path="/ideas/new" element={<CreateIdea />} />
    </Routes>
  )
}

export default App