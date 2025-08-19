export interface SearchResult {
    fileName: string;
    filePath: string;
    score: number;
}

export interface SearchOptions {
    query: string;
    top?: number;
}
