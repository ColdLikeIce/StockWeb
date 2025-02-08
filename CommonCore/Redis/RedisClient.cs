using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonCore.Redis
{
    public class RedisClient
    {
        public void ReadFromConfiguration(IConfiguration configuration)
        {
            ConfigurationManager = configuration;
        }

        public IConfiguration ConfigurationManager { get; private set; }

        private static readonly Lazy<RedisClient> _defaultClient = new(() => new RedisClient());

        public static RedisClient Default
        {
            get { return _defaultClient.Value; }
        }
    }
}