import './App.css'
import { Routes, Route } from 'react-router-dom'
import Navbar from "./components/Navbar";
import LoginForm from './components/LoginForm'
import MainPage from './components/MainPage'
import CategorySelect from './components/CategorySelect'
import IdeaDetails from "./components/IdeaDetails";
import CreateIdea from "./components/CreateIdea";
import IncomingRequests from "./components/IncomingRequests";
import ProfilePage from './components/ProfilePage';
import { getCurrentUser } from './services/api';

export default function App() {
  const currentUser = getCurrentUser();

  return (
    <>
      {currentUser && <Navbar />}

      <main className="main-content">
        <Routes>
          <Route path="/" element={<LoginForm />} />
          <Route path="/categories" element={<CategorySelect />} />
          <Route path="/ideas/:id" element={<IdeaDetails />} />
          <Route path="/main" element={<MainPage />} />
          <Route path="/ideas/new" element={<CreateIdea />} />
          <Route path="/collaboration-requests" element={<IncomingRequests />}/>
          <Route path="/profile/:username" element={<ProfilePage />} />
        </Routes>
      </main>
    </>
  );
}