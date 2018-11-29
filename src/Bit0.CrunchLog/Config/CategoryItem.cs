﻿using System;
using Newtonsoft.Json;

namespace Bit0.CrunchLog.Config
{
    public class CategoryItem
    {
        [JsonProperty("title")]
        public String Title { get; set; }
        [JsonProperty("url")]
        public String Url { get; set; }
        [JsonProperty("count")]
        public Int32 Count { get; set; }
    }
}