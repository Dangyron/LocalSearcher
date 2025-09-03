namespace LocalSearcher.Api.Models;

// Source: https://en.wikipedia.org/wiki/Tf%E2%80%93idf
public record TfIdfIndex(
    Dictionary<string, Document> Documents,
    Dictionary<string, int> DocumentFrequency);
    
public record Document(
    Dictionary<string, int> TermFrequency,
    int TermsCountWithinDocument,
    DateTime LastModified);