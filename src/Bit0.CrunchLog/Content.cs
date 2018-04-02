﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Bit0.CrunchLog.Config;
using Bit0.CrunchLog.Extensions;
using Bit0.CrunchLog.ViewModels;
using Markdig;
using Newtonsoft.Json;

namespace Bit0.CrunchLog
{
    public class Content
    {
        private String _mdFile;
        private String _slug;

        public Content()
        { }

        public Content(FileInfo metaFile, String permaLink)
        {
            MetaFile = metaFile;
            PermaLink = permaLink;
        }

        [JsonProperty("content")]
        public String MarkdownFile
        {
            get
            {
                // if content file is not defined
                if (String.IsNullOrWhiteSpace(_mdFile))
                {
                    var dir = MetaFile?.Directory;

                    // find post.md
                    if (dir?.GetFiles("post.md", SearchOption.TopDirectoryOnly)
                            .SingleOrDefault() != null)
                    {
                        return "post.md";
                    }

                    // find md
                    if (dir?.GetFiles("md", SearchOption.TopDirectoryOnly)
                            .SingleOrDefault() != null)
                    {
                        return "md";
                    }

                    // find <dirName>.md
                    if (dir?.GetFiles($"{dir.Name}.md", SearchOption.TopDirectoryOnly)
                            .SingleOrDefault() != null)
                    {
                        return $"{dir.Name}.md";
                    }

                    // fnd <jsonFileName>.md
                    if (dir?.GetFiles($"{MetaFile.Name.Replace(".json", "")}.md", SearchOption.TopDirectoryOnly)
                            .SingleOrDefault() != null)
                    {
                        return $"{MetaFile.Name.Replace(".json", "")}.md";
                    }

                    throw new FileNotFoundException($"Could not find file for {MetaFile?.Name}");
                }

                return _mdFile;
            }
            set => _mdFile = value;
        }

        [JsonProperty("title")]
        public String Title { get; set; }

        [JsonProperty("layout")]
        public String Layout { get; set; } = Layouts.Post;

        [JsonProperty("slug")]
        public String Slug
        {
            get
            {
                if (String.IsNullOrWhiteSpace(_slug))
                {
                    return MetaFile?.Directory?.Name;
                }

                return _slug;
            }

            set => _slug = value;
        }

        [JsonProperty("date")]
        public DateTime Date { get; set; } = DateTime.UtcNow;

        [JsonProperty("tags")]
        public IEnumerable<String> Tags { get; set; }

        [JsonProperty("categories")]
        public IEnumerable<String> Categories { get; set; }

        [JsonProperty("published")]
        public Boolean Published { get; set; }

        [JsonProperty("intro")]
        public String Intro { get; set; }

        [JsonProperty("permaLink")]
        public String PermaLink { get; set; }

        [JsonProperty("author")]
        //[JsonConverter(typeof(AuthorConvertor))]
        public String AuthorKey { get; set; }

        [JsonIgnore]
        public Author Author { get; set; }

        [JsonIgnore]
        public FileInfo MetaFile { get; set; }

        [JsonIgnore]
        public FileInfo ContentFile => new FileInfo(MetaFile?.Directory?.CombinePath(MarkdownFile));

        [JsonIgnore]
        public String Text
        {
            get
            {
                var pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
                return Markdown.ToHtml(ContentFile.GetText(), pipeline);
            }
        }

        [JsonIgnore]
        public Content Parent => null;

        [JsonIgnore]
        public IEnumerable<Content> Children => null;

        [JsonIgnore]
        public PostViewModel Post { get; private set; }

        public override String ToString()
        {
            return PermaLink;
        }

        public void Fix(CrunchConfig config)
        {

            // fix permalink
            PermaLink = PermaLink
                .Replace(":year", Date.ToString("yyyy"))
                .Replace(":month", Date.ToString("MM"))
                .Replace(":day", Date.ToString("dd"))
                .Replace(":slug", Slug);

            // fix author
            if (!String.IsNullOrWhiteSpace(AuthorKey)
                && config.Authors.ContainsKey(AuthorKey))
            {
                Author = config.Authors[AuthorKey];
            }
            else
            {
                var author = config.Authors.FirstOrDefault();

                AuthorKey = author.Key;
                Author = author.Value;
            }
        }
    }

    public static class Layouts
    {
        public const String Post = "post";
        public const String Page = "page";
    }
}
