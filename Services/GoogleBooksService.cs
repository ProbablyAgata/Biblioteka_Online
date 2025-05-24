using System.Text.Json;
using BibliotekaOnline.Models.GoogleBooks;

namespace BibliotekaOnline.Services
{
    public class GoogleBooksService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly string _baseUrl = "https://www.googleapis.com/books/v1/volumes";

        public GoogleBooksService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<GoogleBookApiResponse> SearchBooksAsync(string query, int maxResults = 10)
        {
            // Get API key from configuration if available
            string apiKey = _configuration["GoogleBooks:ApiKey"] ?? string.Empty;
            string apiKeyParam = string.IsNullOrEmpty(apiKey) ? string.Empty : $"&key={apiKey}";
            
            string url = $"{_baseUrl}?q={Uri.EscapeDataString(query)}&maxResults={maxResults}{apiKeyParam}";
            
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<GoogleBookApiResponse>(content);
            
            return result ?? new GoogleBookApiResponse();
        }

        public async Task<GoogleBookItem?> GetBookByIdAsync(string id)
        {
            // Get API key from configuration if available
            string apiKey = _configuration["GoogleBooks:ApiKey"] ?? string.Empty;
            string apiKeyParam = string.IsNullOrEmpty(apiKey) ? string.Empty : $"?key={apiKey}";
            
            string url = $"{_baseUrl}/{id}{apiKeyParam}";
            
            var response = await _httpClient.GetAsync(url);
            
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }
            
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<GoogleBookItem>(content);
            
            return result;
        }

        public async Task<GoogleBookApiResponse> SearchBooksByIsbnAsync(string isbn, int maxResults = 10)
        {
            return await SearchBooksAsync($"isbn:{isbn}", maxResults);
        }

        public async Task<GoogleBookApiResponse> SearchBooksByTitleAsync(string title, int maxResults = 10)
        {
            return await SearchBooksAsync($"intitle:{title}", maxResults);
        }

        public async Task<GoogleBookApiResponse> SearchBooksByAuthorAsync(string author, int maxResults = 10)
        {
            return await SearchBooksAsync($"inauthor:{author}", maxResults);
        }
    }
} 