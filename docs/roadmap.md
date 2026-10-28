# Ideon Roadmap
## Alpha Version
* **Features:**
  * Simple UI
  * Post creation with templsates ("Imagine an app where...", "Imagine a game in which...").
  * Categories for ideas (Games, Apps, Art, etc.)
  * Commenting on posts

## Beta Version
* **Features:**
  * Making the UI better.
  * User signup & basic profiles
  * Idea status toggles ("Looking for a Builder", "In Progress")
  * Builder outreach & collaboration requests

## Final Release
* **Features:**
  * Showcase hub for built/finished projects
  * Activity feed & post follow notifications
  * Idea rating system
  * Basic moderation & spam protection


## 1. Project Setup

* [x] Create GitHub repository
* [x] Clone repository locally
* [x] Initialize ASP.NET Core Web API
* [x] Initialize React + Vite frontend
* [x] Configure Git workflow
* [x] Add project documentation
* [x] Configure branch protection
* [x] Configure CI

## 2. Database

* [x] Design database schema
* [x] Configure PostgreSQL
* [x] Add Entity Framework Core
* [x] Create database context
* [x] Create initial migrations
* [x] Create database tables
* [x] Seed basic categories

## 3. Backend

* [x] Create User entity
* [x] Create Idea entity
* [x] Create Category entity
* [x] Create Rating entity
* [x] Create Comment entity
* [x] Create CollaborationRequest entity
* [x] Configure entity relationships
* [x] Implement DTOs
* [x] Implement services
* [ ] Implement repositories where needed
* [ ] Implement REST API endpoints
* [ ] Add validation
* [ ] Add error handling
* [ ] Add authentication and authorization
* [ ] Add backend tests

## 4. Frontend

* [ ] Create application layout
* [ ] Create navigation
* [x] Create home page
* [ ] Create ideas list
* [x] Create idea details page
* [ ] Create idea creation form
* [ ] Create authentication pages
* [ ] Create comments UI
* [ ] Create rating UI
* [ ] Create collaboration UI
* [ ] Connect frontend to backend API
* [ ] Add frontend validation
* [ ] Add frontend tests

## 5. Integration

* [x] Connect React frontend with ASP.NET Core API
* [x] Connect API with PostgreSQL
* [ ] Test complete user flows
* [ ] Fix integration issues
* [ ] Improve error handling

## 6. Deployment

* [ ] Configure production environment
* [ ] Configure environment variables
* [ ] Deploy backend
* [ ] Deploy frontend
* [ ] Configure production database
* [ ] Test deployed application

## 7. Finalization

* [ ] Improve UI/UX
* [ ] Add documentation
* [ ] Add API documentation
* [ ] Add screenshots
* [ ] Perform final testing
* [ ] Prepare project presentation

## Collaboration requests
* Users can submit a collaboration request for an idea that is looking for a builder.
* Users cannot request collaboration on their own ideas.
* Duplicate requests for the same idea are rejected.
* Idea authors can view incoming requests.
* Idea authors can accept or reject pending requests.
* Only the author of the associated idea can change a request status.
* Processed requests cannot be changed again.

* Idea discovery
* Ideas are loaded from the backend API.
* Users can filter ideas by category.
* Users can search by title, description, author, and category.
* Users can sort ideas by creation date or title.
* Empty, loading, and error states are displayed.
