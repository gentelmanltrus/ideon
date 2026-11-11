import React from 'react';
import { render, screen, waitFor } from '@testing-library/react';
import '@testing-library/jest-dom';
import { MemoryRouter, Route, Routes } from 'react-router-dom';
import { beforeEach, vi } from 'vitest';
import IdeaDetails from '../components/IdeaDetails.jsx';

vi.mock('../services/api', () => ({
  createComment: vi.fn(),
  getComments: vi.fn(),
  getIdeaById: vi.fn(),
  getToken: vi.fn(() => null),
  createRating: vi.fn(),
  getRatings: vi.fn(),
  updateMyRating: vi.fn(),
  createCollaborationRequest: vi.fn(),
}));

import { getComments, getIdeaById, getRatings } from '../services/api';

describe('IdeaDetails', () => {
  const idea = {
    id: 1,
    title: 'Solar-powered city bikes',
    description: 'A sustainable transport idea to reduce emissions in dense urban areas.',
    categoryName: 'Sustainability',
    authorUsername: 'Alice Johnson',
    createdAt: '2024-01-01T12:00:00.000Z',
    lookingForBuilder: false,
  };

  beforeEach(() => {
    vi.clearAllMocks();
    getIdeaById.mockResolvedValue(idea);
    getComments.mockResolvedValue([]);
    getRatings.mockResolvedValue([]);
  });

  function renderIdeaDetails() {
    return render(
      <MemoryRouter initialEntries={['/ideas/1']}>
        <Routes>
          <Route path="/ideas/:id" element={<IdeaDetails />} />
        </Routes>
      </MemoryRouter>
    );
  }

  it('renders the idea title and description', () => {
    renderIdeaDetails();

    return waitFor(() => {
      expect(screen.getByRole('heading', { name: /solar-powered city bikes/i })).toBeInTheDocument();
      expect(screen.getByText(/a sustainable transport idea to reduce emissions/i)).toBeInTheDocument();
    });
  });

  it('renders the idea metadata', () => {
    renderIdeaDetails();

    return waitFor(() => {
      expect(screen.getByText('Sustainability')).toBeInTheDocument();
      expect(screen.getByText('Alice Johnson')).toBeInTheDocument();
    });
  });
});
