import React from 'react';
import { render, screen, waitFor } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
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

import { createComment, getComments, getIdeaById, getRatings, getToken } from '../services/api';

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
    getToken.mockReturnValue(null);
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

  it('loads and renders comments', async () => {
    getComments.mockResolvedValue([{
      id: 10,
      content: 'A very promising idea.',
      authorUsername: 'Bob',
      createdAt: '2024-01-02T12:00:00.000Z',
    }]);

    renderIdeaDetails();

    expect(await screen.findByText('A very promising idea.')).toBeInTheDocument();
    expect(screen.getByText('Bob')).toBeInTheDocument();
    expect(getComments).toHaveBeenCalledWith('1');
  });

  it('renders the empty comments state', async () => {
    renderIdeaDetails();

    expect(await screen.findByText('No comments yet.')).toBeInTheDocument();
  });

  it('sends a comment and adds it to the list', async () => {
    getToken.mockReturnValue('test-token');
    createComment.mockResolvedValue({
      id: 11,
      content: 'My new comment',
      authorUsername: 'Current User',
      createdAt: '2024-01-03T12:00:00.000Z',
    });

    renderIdeaDetails();
    await userEvent.type(await screen.findByLabelText('Write a comment'), 'My new comment');
    await userEvent.click(screen.getByRole('button', { name: 'Post comment' }));

    expect(await screen.findByText('My new comment')).toBeInTheDocument();
    expect(createComment).toHaveBeenCalledWith('1', 'My new comment');
  });

  it('shows an error when sending a comment fails', async () => {
    getToken.mockReturnValue('test-token');
    createComment.mockRejectedValue(new Error('Could not post comment'));

    renderIdeaDetails();
    await userEvent.type(await screen.findByLabelText('Write a comment'), 'Comment');
    await userEvent.click(screen.getByRole('button', { name: 'Post comment' }));

    expect(await screen.findByRole('alert')).toHaveTextContent('Could not post comment');
  });

  it('shows an error for a nonexistent idea ID', async () => {
    getIdeaById.mockRejectedValue(new Error('Idea not found'));

    renderIdeaDetails();

    expect(await screen.findByRole('alert')).toHaveTextContent(
      'Could not load this idea: Idea not found'
    );
  });

  it('loads and displays ratings', async () => {
    getRatings.mockResolvedValue([
      { id: 1, originality: 4, feasibility: 4, usefulness: 5 },
      { id: 2, originality: 3, feasibility: 5, usefulness: 4 },
    ]);

    renderIdeaDetails();

    expect(await screen.findByText('2 rating(s)')).toBeInTheDocument();
    expect(getRatings).toHaveBeenCalledWith('1');
  });
});
