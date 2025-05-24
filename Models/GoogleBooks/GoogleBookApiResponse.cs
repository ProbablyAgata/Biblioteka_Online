using System.Text.Json.Serialization;

namespace BibliotekaOnline.Models.GoogleBooks
{
    public class GoogleBookApiResponse
    {
        [JsonPropertyName("kind")]
        public string Kind { get; set; } = string.Empty;

        [JsonPropertyName("totalItems")]
        public int TotalItems { get; set; }

        [JsonPropertyName("items")]
        public List<GoogleBookItem> Items { get; set; } = new List<GoogleBookItem>();
    }

    public class GoogleBookItem
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("volumeInfo")]
        public VolumeInfo VolumeInfo { get; set; } = new VolumeInfo();

        [JsonPropertyName("accessInfo")]
        public AccessInfo AccessInfo { get; set; } = new AccessInfo();
    }

    public class VolumeInfo
    {
        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;

        [JsonPropertyName("authors")]
        public List<string> Authors { get; set; } = new List<string>();

        [JsonPropertyName("publisher")]
        public string Publisher { get; set; } = string.Empty;

        [JsonPropertyName("publishedDate")]
        public string PublishedDate { get; set; } = string.Empty;

        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;

        [JsonPropertyName("pageCount")]
        public int PageCount { get; set; }

        [JsonPropertyName("categories")]
        public List<string> Categories { get; set; } = new List<string>();

        [JsonPropertyName("imageLinks")]
        public ImageLinks ImageLinks { get; set; } = new ImageLinks();

        [JsonPropertyName("language")]
        public string Language { get; set; } = string.Empty;

        [JsonPropertyName("industryIdentifiers")]
        public List<IndustryIdentifier> IndustryIdentifiers { get; set; } = new List<IndustryIdentifier>();
    }

    public class ImageLinks
    {
        [JsonPropertyName("smallThumbnail")]
        public string SmallThumbnail { get; set; } = string.Empty;

        [JsonPropertyName("thumbnail")]
        public string Thumbnail { get; set; } = string.Empty;
    }

    public class IndustryIdentifier
    {
        [JsonPropertyName("type")]
        public string Type { get; set; } = string.Empty;

        [JsonPropertyName("identifier")]
        public string Identifier { get; set; } = string.Empty;
    }

    public class AccessInfo
    {
        [JsonPropertyName("webReaderLink")]
        public string WebReaderLink { get; set; } = string.Empty;

        [JsonPropertyName("accessViewStatus")]
        public string AccessViewStatus { get; set; } = string.Empty;

        [JsonPropertyName("pdf")]
        public FormatAvailability Pdf { get; set; } = new FormatAvailability();

        [JsonPropertyName("epub")]
        public FormatAvailability Epub { get; set; } = new FormatAvailability();
    }

    public class FormatAvailability
    {
        [JsonPropertyName("isAvailable")]
        public bool IsAvailable { get; set; }

        [JsonPropertyName("acsTokenLink")]
        public string AcsTokenLink { get; set; } = string.Empty;
    }
} 