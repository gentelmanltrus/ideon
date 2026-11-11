import React from 'react';
import { render, screen } from '@testing-library/react';
import '@testing-library/jest-dom';

let IdeaDetails;
// Try to import the IdeaDetails component from the expected paths
try {
  const module = require('../components/IdeaDetails');
  IdeaDetails = module.default || module.IdeaDetails;
} catch (error) {
  try {
    const module = require('../components/IdeaDetails/IdeaDetails');
    IdeaDetails = module.default || module.IdeaDetails;
  } catch (secondError) {
    throw new Error(
      'IdeaDetails component was not found. Expected it to be exported from ../components/IdeaDetails or ../components/IdeaDetails/IdeaDetails.'
    );
  }
}
//test file for the IdeaDetails component, which checks that the component renders correctly and displays the idea title, description, and metadata.
describe('IdeaDetails', () => {
  const idea = {
    id: 1,
    title: 'Solar-powered city bikes',
    description: 'A sustainable transport idea to reduce emissions in dense urban areas.',
    category: 'Sustainability',
    status: 'In review',
    author: 'Alice Johnson',
    likes: 42,
  };

  it('renders the idea title and description', () => {
    render(<IdeaDetails idea={idea} />);

    expect(
      screen.getByRole('heading', { name: /solar-powered city bikes/i })
    ).toBeInTheDocument();
    expect(
      screen.getByText(/a sustainable transport idea to reduce emissions/i)
    ).toBeInTheDocument();
  });

  it('renders the idea metadata', () => {
    render(<IdeaDetails idea={idea} />);

    expect(screen.getByText(/sustainability/i)).toBeInTheDocument();
    expect(screen.getByText(/in review/i)).toBeInTheDocument();
    expect(screen.getByText(/alice johnson/i)).toBeInTheDocument();
  });
});
