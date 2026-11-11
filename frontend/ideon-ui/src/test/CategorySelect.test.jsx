import { fireEvent, render, screen } from "@testing-library/react";
import { describe, expect, it, vi } from "vitest";
import CategorySelect from "../components/CategorySelect";
//category select test file, which checks that the component renders correctly and displays the available categories and calls the change handler when a category is selected.
describe("CategorySelect", () => {
	const categories = [
		{ id: "books", name: "Books" },
		{ id: "music", name: "Music" },
	];

	it("renders the available categories", () => {
		render(
			<CategorySelect
				categories={categories}
				selectedCategory=""
				onCategoryChange={vi.fn()}
			/>,
		);

		expect(screen.getByRole("combobox")).toBeTruthy();
		expect(screen.getByRole("option", { name: "Books" })).toBeTruthy();
		expect(screen.getByRole("option", { name: "Music" })).toBeTruthy();
	});

	it("calls the change handler when a category is selected", () => {
		const onCategoryChange = vi.fn();
		render(
			<CategorySelect
				categories={categories}
				selectedCategory=""
				onCategoryChange={onCategoryChange}
			/>,
		);

		fireEvent.change(screen.getByRole("combobox"), {
			target: { value: "music" },
		});

		expect(onCategoryChange).toHaveBeenCalledWith("music");
	});
});
