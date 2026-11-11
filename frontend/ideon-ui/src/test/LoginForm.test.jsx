import { render, screen } from '@testing-library/react';
import LoginForm from '../components/LoginForm';
//login form test file, which checks that the component renders correctly and displays the email and password inputs and a submit button.
describe('LoginForm', () => {
	it('renders email and password inputs and a submit button', () => {
		render(<LoginForm />);

		expect(document.querySelector('input[type="email"]')).toBeInTheDocument();
		expect(document.querySelector('input[type="password"]')).toBeInTheDocument();
		expect(
			screen.getByRole('button', { name: /log in|login|sign in/i }),
		).toBeInTheDocument();
	});
});
