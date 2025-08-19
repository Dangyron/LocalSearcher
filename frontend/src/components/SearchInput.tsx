import React, { useEffect, useState } from 'react';
import { useDebounce } from '../hooks/useDebounce';
import { useSearchStore } from '../store/searchStore';

export default function SearchInput() {
    const [query, setQuery] = useState('');
    const debouncedQuery = useDebounce(query, 300);
    const search = useSearchStore((state) => state.search);
    const reset = useSearchStore((state) => state.reset);

    useEffect(() => {
        if (debouncedQuery) {
            search({ query: debouncedQuery, });
        } else {
            reset();
        }
    }, [debouncedQuery, search, reset]);

    return (
        <div className="search-input-container">
            <input
                type="text"
                value={query}
                onChange={(e) => setQuery(e.target.value)}
                placeholder="Search documents..."
                className="search-input"
            />
        </div>
    );
};
