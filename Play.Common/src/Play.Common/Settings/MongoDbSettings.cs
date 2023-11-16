namespace Play.Common.Settings
{
    public class MongoDbSettings
    {
        public string Host { get; set; }    
        public string Port { get; init; }  
        public string ConnectionString=>$"mongodb://{Host}:{Port}";
    }
}