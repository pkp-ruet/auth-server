﻿namespace AuthServer.Models
{
    public class AuthenticationConfiguration
    {
        public string AccessTokenSecret { get; set; }
        public int AccessTokenExpirationMinute { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }


    }
}
