import './App.css'

function App() {
  return (
    <div className="appContainer">
      <header>
        <h1>Ideon</h1>
      </header>
      <main>
        <p className="tagLine">Make your ideas a reality</p>

        <form action="/" method="GET">
          <div className="UsernameInput">
            <label htmlFor="name"></label>
            <input id="name" name="name" type="text" placeholder="Username" />
          </div>
          <div>
            <label htmlFor="password"> </label>
            <input id="password" name="password" type="password" placeholder="Password" />
          </div>
          <button type = "submit" > Submit </button>
        </form>
      </main>
    </div>
  )
}

export default App