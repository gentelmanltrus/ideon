import React from 'react';
import { render, fireEvent, waitFor } from '@testing-library/react';
import CreateIdea from '../CreateIdea';
import '@testing-library/jest-dom/extend-expect';

describe("CreateIdea", () => {
  beforeEach(() => {
    jest.clearAllMocks();
  });

  test('does not submit without a title', async () => {
    const { getByText } = render(<CreateIdea />);
    fireEvent.click(getByText('Submit'));
    expect(getByText('Title is required')).toBeInTheDocument();
  });

  test('does not submit without a required description', async () => {
    const { getByText } = render(<CreateIdea />);
    fireEvent.click(getByText('Submit'));
    expect(getByText('Description is required')).toBeInTheDocument();
  });

  test('allows choosing a category', async () => {
    const { getByLabelText } = render(<CreateIdea />);
    const categorySelect = getByLabelText('Category');
    fireEvent.change(categorySelect, { target: { value: 'Idea' } });
    expect(categorySelect.value).toBe('Idea');
  });

  test('confirms a successful submission', async () => {
    const { getByText } = render(<CreateIdea />);
    fireEvent.click(getByText('Submit'));
    await waitFor(() => expect(getByText('Submission successful')).toBeInTheDocument());
  });

  test('shows an error when the API request fails', async () => {
    const { getByText } = render(<CreateIdea />);
    fireEvent.click(getByText('Submit'));
    await waitFor(() => expect(getByText('Submission failed')).toBeInTheDocument());
  });
});