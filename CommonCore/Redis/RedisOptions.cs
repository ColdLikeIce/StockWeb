using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonCore.Redis
{
    /// <summary>
    /// Redis config Options
    /// </summary>
    public class RedisOptions
    {
        public string Name { get; set; }

        /// <summary>
        /// Host
        /// </summary>
        public string HostName { get; set; }

        /// <summary>
        /// Port
        /// </summary>
        public string Port { get; set; }

        /// <summary>
        /// Password
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Default Database
        /// </summary>
        public int DefaultDatabase { get; set; }
    }
}