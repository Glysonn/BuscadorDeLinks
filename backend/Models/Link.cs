using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
namespace backend.Models
{
    [BsonIgnoreExtraElements]
    public class Link
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)] 
        private string _id { get; set;}

        // propriedade readonly do _id
        public string Id { get {return _id;}}
        

        [BsonElement("titulo")]
        public string Titulo { get; set; }

        [BsonElement("url")]
        public string Url { get; set; }
        
        [BsonElement("descricao")]
        public string Descricao { get; set; }
    }
}