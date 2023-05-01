using MongoDB.Driver;

namespace backend.Context
{
    public class MongoConnection
    {
        public IMongoDatabase context;

        public MongoConnection (IConfiguration myConfig)
        {
            var ConnectionString = myConfig["ConnectionStrings:MyMongoDBConnection"];
            var DataBaseString = myConfig["MongoDatabases:BuscadorLinks"];

            try
            {
                var MyMongoClient = new MongoClient(ConnectionString);
                context = MyMongoClient.GetDatabase(DataBaseString);
            }
            catch (Exception ex)
            {
                throw new MongoException($"Não foi possível conectar ao MongoDB: {ex.Message}");
            }
        }
    }
}