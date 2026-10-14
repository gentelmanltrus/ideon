import { useState } from 'react';

// This component is used to create a new idea. It takes an onSubmit prop, which is a function that will be called when the form is submitted. The form has fields for the title, description, category ID, and a checkbox for whether the idea is looking for a builder. When the form is submitted, the onSubmit function is called with the current state of the idea.
export default function CreateIdea({ onSubmit }) {
	const [idea, setIdea] = useState({
		title: '',
		description: '',
		categoryId: '',
		lookingForBuilder: false,
	});

	function handleChange(event) {
		const { name, value, checked, type } = event.target;
		setIdea((current) => ({
			...current,
			[name]: type === 'checkbox' ? checked : value,
		}));
	}

	function handleSubmit(event) {
		event.preventDefault();
		onSubmit?.(idea);
	}

	return (
		<form onSubmit={handleSubmit}>
			<div>
				<label htmlFor="idea-title">Title</label>
				<input
					id="idea-title"
					name="title"
					type="text"
					value={idea.title}
					onChange={handleChange}
					required
				/>
			</div>

			<div>
				<label htmlFor="idea-description">Description</label>
				<textarea
					id="idea-description"
					name="description"
					value={idea.description}
					onChange={handleChange}
					required
				/>
			</div>

			<div>
				<label htmlFor="idea-category-id">Category ID</label>
				<input
					id="idea-category-id"
					name="categoryId"
					type="text"
					value={idea.categoryId}
					onChange={handleChange}
					required
				/>
			</div>

			<div>
				<label htmlFor="idea-looking-for-builder">Looking for a builder</label>
				<input
					id="idea-looking-for-builder"
					name="lookingForBuilder"
					type="checkbox"
					checked={idea.lookingForBuilder}
					onChange={handleChange}
				/>
			</div>

			<button type="submit">Create idea</button>
		</form>
	);
}
