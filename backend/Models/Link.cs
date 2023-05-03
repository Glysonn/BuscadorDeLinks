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
        [BsonIgnore]
        public string Id { get {return _id;}}
        
        [BsonElement("titulo")]
        public string Titulo { get; set; }

        [BsonElement("url")]
        public string Url { get; set; }
        
        [BsonElement("descricao")]
        public string Descricao { get; set; }

        // método necessário pois preciso setar o ID quando for fazer update pois sem isso
        // o mongo está criando uma nova objectId.

        // isso tudo pois decidi colocar o Id como readonly, assim não dá pra sobrescrever ele.
        // o Id está como readonly para não aparecer no body da requisição.
        public void setId(string Id)
        {
            this._id = Id;
        }
    }
}