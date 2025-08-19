import { create } from 'zustand';
import { SearchResult, SearchOptions } from '../types';
import { searchFiles } from '../api/searchApi';

interface SearchState {
    results: SearchResult[];
    loading: boolean;
    error: string | null;
    search: (options: SearchOptions) => Promise<void>;
    reset: () => void;
}

export const useSearchStore = create<SearchState>((set) => ({
    results: [],
    loading: false,
    error: null,

    search: async (options) => {
        if (!options.query.trim()) {
            set({ results: [], loading: false, error: null });
            return;
        }

        set({ loading: true, error: null });

        try {
            const results = await searchFiles(options);
            set({ results, loading: false });
        } catch (error) {
            set({ error: 'Failed to fetch search results', loading: false });
            console.error(error);
        }
    },

    reset: () => {
        set({ results: [], error: null, loading: false });
    }
}));