using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using backend.Models;
using backend.Context;
using System.Net;
using MongoDB.Bson;


namespace backend.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class LinkController : ControllerBase
    {
        // aqui é a minha collection do banco de dados
        IMongoCollection<Link> _linksCollection;


        public LinkController(MongoConnection myMongoConnection, IConfiguration configuration)
        {
            var CollectionString = configuration["MongoCollections:BuscadorLinks:Link"];

            // recuperando a collection do banco e atribuíndo à variável _links (o tipo dela é IMongoCollection)
            _linksCollection = myMongoConnection.context.GetCollection<Link>(CollectionString);
        }

        // método para tratar os erros de forma genérica
        private IActionResult HandleError(HttpStatusCode statusCode, string exceptionMessage)
        {
            return StatusCode((int)statusCode, new {
                Error = Enum.GetName(typeof(HttpStatusCode), (int)statusCode),
                StatusCode = statusCode,
                Details = exceptionMessage });
        }

        // ENDPOINTS
        [HttpGet]
        [Route("GetAllLinks")]
        public IActionResult GetAllLinks()
        {
            try
            {
                var links = _linksCollection.AsQueryable().ToList();
                if (links.Count() == 0)
                    return HandleError(HttpStatusCode.NotFound, "Oops! Ainda não há nenhum link cadastrado na nossa base de dados! :(");

                return Ok(links);
            }
            catch (Exception ex)
            {
                return HandleError(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpGet]
        [Route("GetLinkByTitle/{title}")]
        public IActionResult GetLinkByTitle(string title)
        {
            try
            {
                // RegEx para trazer todos que contém a string passada pelo parâmetro. O "i" serve para que seja independente de CaseSensitive
                var filtro = Builders<Link>.Filter.Regex(lk => lk.Titulo, new BsonRegularExpression(title, "i"));
                var linkBanco =_linksCollection.Find(filtro).ToList();
                
                if (linkBanco.Count() == 0 || linkBanco == null)
                    return HandleError(HttpStatusCode.NotFound, $"Perdão, mas não achamos nenhuma página que contenha esse nome ({title})");

                return Ok(linkBanco);
            }
            catch(Exception ex)
            {
                return HandleError(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        [Route("InsertNewLink")]
        public IActionResult InsertNewLink([FromBody] Link newLink)
        {
            try
            {
                // verifica se os campos contém um valor que não seja vazio ou nulo. O MongoDB driver já verifica se é do tipo correto.
                // caso contenha, retorna um status de erro
                if((String.IsNullOrWhiteSpace(newLink.Titulo)) || (String.IsNullOrWhiteSpace(newLink.Url)) || (String.IsNullOrWhiteSpace(newLink.Descricao)))
                    return HandleError(HttpStatusCode.BadRequest, "Oops! Todos os dados precisam ter um valor e não podem ser vazio!");
                
                // verifica se a URL já está cadastrada (é uma regra da aplicação, cada URL só pode ter um cadastro)
                var filtro = Builders<Link>.Filter.Eq(lk => lk.Url, newLink.Url);
                var urlEstaCadastrada = _linksCollection.Find(filtro).Any();

                if (urlEstaCadastrada)
                    return HandleError(HttpStatusCode.BadRequest, "Oops! Essa URL já está cadastrada e não pode ser cadastrada novamente.");

                _linksCollection.InsertOne(newLink);
                return Created("Link cadastrado!", newLink);
            }
            catch(Exception ex)
            {
                return HandleError(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpPut]
        [Route("EditLink/{url}")]
        public IActionResult EditLink(string url, [FromBody] Link newLink)
        {
            try
            {
                var filtro = Builders<Link>.Filter.Eq(lk => lk.Url, url);
                var linkBanco = _linksCollection.Find(filtro).FirstOrDefault();
                
                if (linkBanco == null)
                    return HandleError(HttpStatusCode.NotFound, "Oops! Esse link não está cadastrado! Tente com um endereço válido!");
                
                var novoLink = new Link {
                    Titulo = newLink.Titulo,
                    Url = newLink.Url,
                    Descricao = newLink.Descricao
                };
                novoLink.setId(linkBanco.Id);

                var resultado = _linksCollection.ReplaceOne(filtro, novoLink);

                if (!resultado.IsAcknowledged && resultado.ModifiedCount == 0)
                    return HandleError(HttpStatusCode.InternalServerError, "Não foi possível atualizar o link!");

                return Ok($"Dados atualizados! \n{novoLink.ToJson()}");
            }
            catch (Exception ex)
            {
                return HandleError(HttpStatusCode.InternalServerError, ex.Message);
            }
        }
    }
}