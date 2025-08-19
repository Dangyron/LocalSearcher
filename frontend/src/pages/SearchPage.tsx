import SearchInput from '../components/SearchInput';
import SearchResults from '../components/SearchResults';

export default function SearchPage() {
    return (
        <div className="search-page">
            <SearchInput />
            <SearchResults />
        </div>
    );
};
