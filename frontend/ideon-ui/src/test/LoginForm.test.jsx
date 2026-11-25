import { render, screen } from '@testing-library/react';
import { MemoryRouter } from 'react-router-dom';
import LoginForm from '../components/LoginForm';
//login form test file, which checks that the component renders correctly and displays the email and password inputs and a submit button.
describe('LoginForm', () => {
	it('renders email and password inputs and a submit button', () => {
		render(
			<MemoryRouter>
				<LoginForm />
			</MemoryRouter>,
		);

		expect(screen.getByLabelText(/email/i)).toBeInTheDocument();
		expect(screen.getByLabelText(/password/i)).toBeInTheDocument();
		expect(
			screen.getByRole('button', { name: /log in|login|sign in/i }),
		).toBeInTheDocument();
	});
});
