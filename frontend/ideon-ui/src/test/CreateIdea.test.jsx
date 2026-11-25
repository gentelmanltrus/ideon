import { fireEvent, render, screen, waitFor } from "@testing-library/react";
import { MemoryRouter } from "react-router-dom";
import { vi } from "vitest";
import CreateIdea from "../components/CreateIdea";

const mockCategories = [
    { id: 1, name: "Technology" },
    { id: 2, name: "Education" },
];

const mockResponse = (body, ok = true, status = 200) => ({
    ok,
    status,
    text: vi.fn().mockResolvedValue(JSON.stringify(body)),
    json: vi.fn().mockResolvedValue(body),
});

const renderForm = () =>
    render(
        <MemoryRouter>
            <CreateIdea />
        </MemoryRouter>,
    );

const waitForForm = async () => {
    await screen.findByLabelText(/title/i);

    await waitFor(() => {
        const category = screen.getByLabelText(/category/i);
        expect(category.options.length).toBeGreaterThan(1);
    });
};

const submitForm = () =>
    fireEvent.click(screen.getByRole("button", { name: /create|submit/i }));

describe("CreateIdea", () => {
    beforeEach(() => {
        vi.clearAllMocks();
        globalThis.fetch = vi
            .fn()
            .mockResolvedValue(mockResponse(mockCategories));
    });

    it("does not submit without a title", async () => {
        renderForm();
        await waitForForm();

        fireEvent.change(screen.getByLabelText(/description/i), {
            target: { value: "An idea description" },
        });
        submitForm();

        expect(await screen.findByText(/title|required/i)).toBeInTheDocument();
        expect(globalThis.fetch).toHaveBeenCalledTimes(1);
    });

    it("does not submit without a required description", async () => {
        renderForm();
        await waitForForm();

        fireEvent.change(screen.getByLabelText(/title/i), {
            target: { value: "A new idea" },
        });
        submitForm();

        expect(
            await screen.findByText(/description|required/i),
        ).toBeInTheDocument();
        expect(globalThis.fetch).toHaveBeenCalledTimes(1);
    });

    it("allows choosing a category", async () => {
        renderForm();
        await waitForForm();

        const category = screen.getByLabelText(/category/i);
        const options = Array.from(category.options).filter(
            (option) => option.value !== "",
        );

        fireEvent.change(category, {
            target: { value: options[0].value },
        });

        expect(category).toHaveValue(options[0].value);
    });

    it("confirms a successful submission", async () => {
        globalThis.fetch = vi
            .fn()
            .mockResolvedValueOnce(mockResponse(mockCategories))
            .mockResolvedValueOnce(
                mockResponse({ id: 1, title: "A new idea" }),
            );

        renderForm();
        await waitForForm();

        fireEvent.change(screen.getByLabelText(/title/i), {
            target: { value: "A new idea" },
        });
        fireEvent.change(screen.getByLabelText(/description/i), {
            target: { value: "An idea description" },
        });

        const category = screen.getByLabelText(/category/i);
        const option = Array.from(category.options).find(
            (item) => item.value !== "",
        );

        fireEvent.change(category, {
            target: { value: option.value },
        });
        submitForm();

        await waitFor(() => {
            expect(globalThis.fetch).toHaveBeenCalledTimes(2);
        });

        const [, submitOptions] = globalThis.fetch.mock.calls[1];

        expect(submitOptions.method).toBe("POST");
        expect(submitOptions.body).toContain("A new idea");
        expect(screen.queryByRole("alert")).not.toBeInTheDocument();
    });

    it("shows an error when the API request fails", async () => {
        globalThis.fetch = vi
            .fn()
            .mockResolvedValueOnce(mockResponse(mockCategories))
            .mockResolvedValueOnce(
                mockResponse(
                    { message: "Request failed" },
                    false,
                    400,
                ),
            );

        renderForm();
        await waitForForm();

        fireEvent.change(screen.getByLabelText(/title/i), {
            target: { value: "A new idea" },
        });
        fireEvent.change(screen.getByLabelText(/description/i), {
            target: { value: "An idea description" },
        });

        const category = screen.getByLabelText(/category/i);
        const option = Array.from(category.options).find(
            (item) => item.value !== "",
        );

        fireEvent.change(category, {
            target: { value: option.value },
        });
        submitForm();

        expect(
            await screen.findByText(/request failed|error/i),
        ).toBeInTheDocument();
    });
});