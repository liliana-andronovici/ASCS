import { StrictMode } from 'react';
import { createRoot } from 'react-dom/client';
import '@fontsource-variable/plus-jakarta-sans';
import './index.css';
import { QueryClientProvider} from "@tanstack/react-query";
import { router } from "./routes/router";
import { RouterProvider } from "react-router-dom";
import { queryClient } from "./lib/queryClient";

createRoot(document.getElementById('root')!).render(
    <StrictMode>
        <QueryClientProvider client={queryClient}>
            <RouterProvider router={router}/>
        </QueryClientProvider>
    </StrictMode>
)
