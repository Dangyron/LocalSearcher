import React from 'react';
import { Link } from 'react-router-dom';
import Spinner from './Spinner';
import { useSearchStore } from '../store/searchStore';

export default function SearchResults() {
    const results = useSearchStore((state) => state.results);
    const loading = useSearchStore((state) => state.loading);
    const error = useSearchStore((state) => state.error);

    if (loading) {
        return <Spinner />;
    }

    if (error) {
        return (
            <div className="error-message">
                <p>{error}</p>
            </div>
        );
    }

    if (results.length === 0) {
        return (
            <div className="no-results">
                <p>No results found. Try a different search term.</p>
            </div>
        );
    }

    return (
        <div className="search-results">
            {results.map((result) => (
                <Link
                    key={result.filePath}
                    to={`/file/${encodeURIComponent(result.filePath)}`}
                    className="result-item"
                >
                    <div className="result-header">
                        <h3 className="file-name">{result.fileName}</h3>
                        <div className="score-badge">
                            {Math.round(result.score * 100)}% match
                        </div>
                    </div>
                    <p className="file-path">{result.filePath}</p>
                </Link>
            ))}
        </div>
    );
};
