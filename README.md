# ideon
Ideon is a web platform designed to help people turn their ideas into reality. Platform where users share ideas for apps, games, and products, while others rate their usefulness, creativity, and feasibility.
## Team Details

* **Team Name:** Idea Architects
* **Team Members:**
  * Mišelė Ralf - misele-ra
  * Arijus Rasymas - oArijus
  * Paulius Radzišauskas - PauliusRd
  * Madina Tanyrbergenova - madinasejsengazykyzy-debug (Team leader)
  * Aleksandr Samonov - gentelmanltrus (Backend)
 
## Features

- Publish ideas and assign them to categories
- Discover and browse ideas
- Evaluate ideas
- Leave comments and suggestions
- Help improve ideas through community feedback
- Request collaboration
- Mark an idea as **looking for a builder**

## Technology Stack

### Frontend

- React
- Vite
- JavaScript

### Backend

- C#
- ASP.NET Core Web API
- .NET 10

### Database

- PostgreSQL
- Entity Framework Core

### Tools

- Git
- GitHub
- Visual Studio Code

## Project Structure

```text
ideon/
├── backend/
│   └── Ideon.API/
├── frontend/
│   └── ideon-ui/
├── docs/
│   ├── roadmap.md
│   └── architecture.md
├── .gitignore
└── README.md
```

## Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Node.js](https://nodejs.org/) and npm
- PostgreSQL

### Running the Backend

1. Navigate to the backend project:

```bash
cd backend/Ideon.API
```

2. Run the API:

```bash
dotnet run
```

The API will be available locally on the port displayed by ASP.NET Core.

OpenAPI documentation is available at:

```text
/openapi/v1.json
```

### Running the Frontend

1. Navigate to the frontend project:

```bash
cd frontend/ideon-ui
```

2. Install dependencies:

```bash
npm install
```

3. Start the development server:

```bash
npm run dev
```

The frontend will normally be available at:

```text
http://localhost:5173
```

## Development Workflow

The project uses a feature-branch and Pull Request workflow.

```text
feature branch
      ↓
    commit
      ↓
     push
      ↓
Pull Request
      ↓
    review
      ↓
    merge
```

Direct pushes to `main` should be avoided.

## Status

The project is currently in the pre alpha version stage - data base, backend api and frontend ui are there, connections between them in progress.

**Roadmap:**

[Roadmap](https://github.com/gentelmanltrus/ideon/blob/main/docs/roadmap.md) 
