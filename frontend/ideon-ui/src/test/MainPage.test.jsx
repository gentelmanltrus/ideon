import { render, screen } from "@testing-library/react";
import { beforeEach, describe, expect, it, vi } from "vitest";
import { MemoryRouter } from "react-router-dom";
import MainPage from "../components/MainPage";
import { getIdeas } from "../services/api";

vi.mock("../services/api", () => ({
    getIdeas: vi.fn()
}));
// this test file is for the MainPage component, which displays a list of ideas fetched from an API. The tests check that the component renders correctly and displays the ideas returned by the API.
describe("MainPage", () => {
    beforeEach(() => {
        vi.clearAllMocks();
    });

    it("renders the main page", async () => {
        getIdeas.mockResolvedValue([]);

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
        getIdeas.mockResolvedValue([
            {
                id: 1,
                title: "Community garden",
                description: "A shared garden for the neighborhood.",
                authorUsername: "alex",
                categoryName: "Community",
                categoryId: 2,
                createdAt: "2026-01-01T12:00:00Z",
                lookingForBuilder: false
            }
        ]);

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
});