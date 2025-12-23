using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SaiPublicity.Models
{
    public class RecaptchaVerifyResponse
    {
        [JsonPropertyName("success")]
        public bool success { get; set; }

        [JsonPropertyName("challenge_ts")]
        public DateTime challenge_ts { get; set; }

        [JsonPropertyName("hostname")]
        public string hostname { get; set; }

        [JsonPropertyName("error-codes")]
        public List<string> ErrorCodes { get; set; }
    }
}
