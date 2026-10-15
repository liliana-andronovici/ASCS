import { QueryClient } from "@tanstack/react-query";

export const queryClient = new QueryClient({
    defaultOptions: {
        queries: {
            staleTime: 30_000,
            retry:1, //Revisit this setting when more detailed retry behavior is needed.
        },
    },
});