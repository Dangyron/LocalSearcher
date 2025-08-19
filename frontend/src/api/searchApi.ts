import axios from 'axios';
import { SearchOptions, SearchResult } from '../types';
import { apiBaseUrl } from '../config/env';

export const searchFiles = async (options: SearchOptions): Promise<SearchResult[]> => {
    try {
        const response = await axios.get<SearchResult[]>(`${apiBaseUrl}/search`, {
            params: options
        });
        return response.data;
    } catch (error) {
        console.error('Search error:', error);
        return [];
    }
};