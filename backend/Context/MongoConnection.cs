using MongoDB.Driver;

namespace backend.Context
{
    public class MongoConnection
    {
        public IMongoDatabase context;

        public MongoConnection (IConfiguration myConfig)
        {
            try
            {
                var MyMongoClient = new MongoClient(myConfig["ConnectionStrings:MyMongoConnection"]);
                context = MyMongoClient.GetDatabase("BuscadorLinks");
            }
            catch (Exception ex)
            {
                throw new MongoException($"Não foi possível conectar ao MongoDB: {ex.Message}");
            }
        }
    }
}