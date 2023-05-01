using MongoDB.Bson.Serialization.Attributes;
namespace backend.Models
{
    [BsonIgnoreExtraElements]
    public class Link
    {
        [BsonElement("titulo")]
        public string Titulo { get; set; }

        [BsonElement("url")]
        public string Url { get; set; }
        
        [BsonElement("descricao")]
        public string Descricao { get; set; }
    }
}