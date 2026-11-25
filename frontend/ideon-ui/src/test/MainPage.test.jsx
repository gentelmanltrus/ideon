import { render, screen, waitFor } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { beforeEach, describe, expect, it, vi } from "vitest";
import { MemoryRouter } from "react-router-dom";
import MainPage from "../components/MainPage";
import { getPagedIdeas } from "../services/api";

vi.mock("../services/api", () => ({
    getPagedIdeas: vi.fn()
}));
// this test file is for the MainPage component, which displays a list of ideas fetched from an API. The tests check that the component renders correctly and displays the ideas returned by the API.
describe("MainPage", () => {
    beforeEach(() => {
        vi.clearAllMocks();
    });

    it("renders the main page", async () => {
        getPagedIdeas.mockResolvedValue({
            items: [],
            totalCount: 0,
            totalPages: 0
        });

        render(
            <MemoryRouter>
                <MainPage />
            </MemoryRouter>
        );

        expect(
            await screen.findByRole("heading", { name: "Explore ideas" })
        ).toBeInTheDocument();
        expect(screen.getByLabelText("Search ideas")).toBeInTheDocument();
    });

    it("renders ideas returned by the API", async () => {
        getPagedIdeas.mockResolvedValue({
            items: [{
                id: 1,
                title: "Community garden",
                description: "A shared garden for the neighborhood.",
                authorUsername: "alex",
                categoryName: "Community",
                categoryId: 2,
                createdAt: "2026-01-01T12:00:00Z",
                lookingForBuilder: false
            }],
            totalCount: 1,
            totalPages: 1
        });

        render(
            <MemoryRouter>
                <MainPage />
            </MemoryRouter>
        );

        expect(
            await screen.findByRole("link", { name: "Community garden" })
        ).toBeInTheDocument();
        expect(
            screen.getByText("A shared garden for the neighborhood.")
        ).toBeInTheDocument();
    });

    it("shows an empty state when a search has no results", async () => {
        const idea = {
            id: 1,
            title: "Community garden",
            description: "A shared garden for the neighborhood.",
            authorUsername: "alex",
            categoryName: "Community",
            categoryId: 2,
            createdAt: "2026-01-01T12:00:00Z",
            lookingForBuilder: false
        };
        getPagedIdeas.mockImplementation((_page, _pageSize, { search }) =>
            Promise.resolve({
                items: search ? [] : [idea],
                totalCount: search ? 0 : 1,
                totalPages: search ? 0 : 1
            })
        );

        render(
            <MemoryRouter>
                <MainPage />
            </MemoryRouter>
        );

        expect(
            await screen.findByRole("link", { name: "Community garden" })
        ).toBeInTheDocument();

        await userEvent.type(screen.getByLabelText("Search ideas"), "no match");

        expect(
            await screen.findByRole("heading", { name: "No ideas found" })
        ).toBeInTheDocument();
        expect(
            screen.queryByRole("link", { name: "Community garden" })
        ).not.toBeInTheDocument();
        await waitFor(() => {
            expect(getPagedIdeas).toHaveBeenLastCalledWith(1, 10, {
                search: "no match",
                categoryId: null,
                sortBy: "newest"
            });
        });
    });

    it("shows an error when loading ideas fails", async () => {
        getPagedIdeas.mockRejectedValue(new Error("Service unavailable"));

        render(
            <MemoryRouter>
                <MainPage />
            </MemoryRouter>
        );

        expect(await screen.findByRole("alert")).toHaveTextContent(
            "Could not load ideas: Service unavailable"
        );
    });

    it("updates the list when the sort filter changes", async () => {
        const newestIdea = {
            id: 1,
            title: "Newest idea",
            description: "Recently added.",
            authorUsername: "alex",
            categoryName: "Community",
            categoryId: 2,
            createdAt: "2026-01-01T12:00:00Z",
            lookingForBuilder: false
        };
        const oldestIdea = {
            ...newestIdea,
            id: 2,
            title: "Oldest idea",
            description: "Added earlier."
        };
        getPagedIdeas.mockImplementation((_page, _pageSize, { sortBy }) =>
            Promise.resolve({
                items: [sortBy === "newest" ? newestIdea : oldestIdea],
                totalCount: 1,
                totalPages: 1
            })
        );

        render(
            <MemoryRouter>
                <MainPage />
            </MemoryRouter>
        );

        expect(
            await screen.findByRole("link", { name: "Newest idea" })
        ).toBeInTheDocument();

        await userEvent.selectOptions(screen.getByLabelText("Sort by"), "oldest");

        expect(
            await screen.findByRole("link", { name: "Oldest idea" })
        ).toBeInTheDocument();
        expect(
            screen.queryByRole("link", { name: "Newest idea" })
        ).not.toBeInTheDocument();
        expect(getPagedIdeas).toHaveBeenLastCalledWith(1, 10, {
            search: "",
            categoryId: null,
            sortBy: "oldest"
        });
    });

    it("loads the next backend page when pagination advances", async () => {
        const pageOne = Array.from({ length: 10 }, (_, index) => ({
            id: 11 - index,
            title: `Idea ${11 - index}`,
            description: `Description ${11 - index}`,
            authorUsername: "alex",
            categoryName: "Community",
            categoryId: 2,
            createdAt: "2026-01-01T12:00:00Z",
            lookingForBuilder: false
        }));
        const pageTwo = [{
            id: 1,
            title: "Idea 1",
            description: "Description 1",
            authorUsername: "alex",
            categoryName: "Community",
            categoryId: 2,
            createdAt: "2026-01-01T12:00:00Z",
            lookingForBuilder: false
        }];
        getPagedIdeas.mockImplementation((page) => Promise.resolve({
            items: page === 1 ? pageOne : pageTwo,
            totalCount: 11,
            totalPages: 2
        }));

        render(
            <MemoryRouter>
                <MainPage />
            </MemoryRouter>
        );

        expect(await screen.findByRole("link", { name: "Idea 11" })).toBeInTheDocument();
        expect(screen.getByText("11 ideas")).toBeInTheDocument();

        await userEvent.click(await screen.findByRole("button", { name: "Next" }));

        expect(await screen.findByRole("link", { name: "Idea 1" })).toBeInTheDocument();
        expect(getPagedIdeas).toHaveBeenLastCalledWith(2, 10, {
            search: "",
            categoryId: null,
            sortBy: "newest"
        });
        expect(screen.getByText("Page 2 of 2")).toBeInTheDocument();
    });
});