namespace DeleteFromGridFSTest.Data
{

    public class MongoDbConfig
    {
        public required string DatabaseName { get; init; }
        public required string Host { get; init; }
        public int Port { get; init; }
        public string OpovDbConnectionString => $"mongodb://{Host}:{Port}/{DatabaseName}";
    }

}
