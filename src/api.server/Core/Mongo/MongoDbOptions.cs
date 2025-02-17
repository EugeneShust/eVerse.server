using MongoDB.Driver.Core.Configuration;
using System;

namespace Core.Mongo
{
    public class MongoDbOptions
    {
        public MongoDbOptions()
        {
            ConnectionString = "";
            DatabaseName = "";
            MinConnectionPoolSize = 0;
            MaxConnectionPoolSize = 1000;
            WaitQueueSize = 500;
            WaitQueueTimeout = TimeSpan.FromSeconds(120);
        }

        public string ConnectionString { get; set; }

        public string DatabaseName { get; set; }

        public int MinConnectionPoolSize { get; set; }

        public int MaxConnectionPoolSize { get; set; }

        public int WaitQueueSize { get; set; }

        public TimeSpan WaitQueueTimeout { get; set; }
    }
}
