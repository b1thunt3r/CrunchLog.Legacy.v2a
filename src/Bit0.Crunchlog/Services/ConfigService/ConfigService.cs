using Bit0.CrunchLog.Helpers;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Bit0.CrunchLog.Sdk.Services.ConfigService
{
    public class ConfigService
    {
        private readonly ILogger<ConfigService> _logger;
        //private JsonNode _rawConfig;

        //public JsonNode RawConfig { get => _rawConfig ?? throw new NullReferenceException("Config file has not been parsed yet."); }

        public ConfigService(ILogger<ConfigService> logger)
        {
            _logger = logger;
        }

        public void ParseFile(FileInfo configFile)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            //_rawConfig = JsonObject.Parse(configFile.OpenRead());
            var json = JsonSerializer.Deserialize<CrunchConfig>(configFile.OpenRead(), CrunchConfigJsonContext.Default.CrunchConfig);
        }
    }

    [JsonSourceGenerationOptions(
     WriteIndented = true,
     PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
    [JsonSerializable(typeof(CrunchConfig))]
    public partial class CrunchConfigJsonContext : JsonSerializerContext
    { }

    public class CrunchConfig
    {
        [JsonPropertyName("title")]
        public String Title { get; set; }
        public String SubTitle { get; set; }
        public String Description { get; set; }
        public String Theme { get; set; }
        public String BaseUrl { get; set; }
        public String LanguageCode { get; set; }
        public Copyright Copyright { get; set; }
        public IEnumerable<String> Tags { get; set; }
        public IDictionary<String, CategoryInfo> Categories { get; set; }
        public String Permalink { get; set; } = @"/:year/:month/:slug";
        public SiteImage Logo { get; set; }
        public SiteImage DefaultImage { get; set; }
        public String DefaultCategory { get; set; }
        public Pagination Pagination { get; set; }
        public IDictionary<String, Author> Authors { get; set; }
        public IDictionary<String, IEnumerable<MenuItem>> Menu { get; set; }
        public IDictionary<String, String> Paths { get; set; }
        public IDictionary<String, String> Manifest { get; set; }
        public IDictionary<String, Favicon> Icons { get; set; }
        public IDictionary<String, String> Social { get; set; }
        public IEnumerable<String> Robots { get; set; }

        [JsonExtensionData]
        public Dictionary<string, JsonElement> AdditionalData { get; set; }
    }

    public class Copyright
    {
        public Int32 StartYear { get; set; }
        public String Owner { get; set; }
    }

    public interface IMenuItem
    {
        public String Title { get; set; }
        public String Url { get; set; }
    }

    public class CategoryInfo : IMenuItem
    {
        public String Title { get; set; }
        public String Color { get; set; }
        public String Permalink { get; set; }
        public SiteImage Image { get; set; }
        public Boolean ShowInMainMenu { get; set; }
        public String Description { get; set; }
        public String Url { get => Permalink; set => Permalink = value; }
    }

    public class SiteImage
    {
        [JsonPropertyName("src")]
        public String Url { get; set; }
        public String Size => $"{Width}x{Height}";
        public String Type { get; set; }
        public Int32 Height { get; set; }
        public Int32 Width { get; set; }
        public String Placeholder { get; set; }
    }

    public class Pagination
    {
        private const Int32 _defaultPazeSize = 6;
        private Int32 _pageSize = _defaultPazeSize;

        public Int32 PageSize
        {
            get => _pageSize > 0 ? _pageSize : _defaultPazeSize;
            set => _pageSize = value;
        }
    }

    public class Author
    {
        public String Name { get; set; }
        public String Alias { get; set; }
        public String Email { get; set; }
        public String HomePage { get; set; }
        public String Description { get; set; }
        public IDictionary<String, String> Social { get; set; }

        [JsonPropertyName("url")]
        [JsonIgnore]
        public String Permalink => String.Format(StaticKeys.ByPathFormat, Alias);

        public override String ToString()
        {
            return $"{Name} ({Alias})";
        }
    }

    public class MenuItem : IMenuItem
    {
        public String Title { get; set; }
        public String Url { get; set; }
        public Int32 Order { get; set; }
    }

    public static class StaticPaths
    {
        public const String ConfigFile = "crunch.json";
        public const String Content = "Content";
        public const String Themes = "Themes";
        public const String Plugins = "Plugins";
        public const String Images = "Content/Images";
        public const String Assets = "Content/Assets";
        public const String Output = "_site";
    }

    public class Favicon
    {
        [JsonPropertyName("src")]
        public String Url { get; set; }
        public String Size { get; set; }
        public String Type { get; set; }
    }
}
