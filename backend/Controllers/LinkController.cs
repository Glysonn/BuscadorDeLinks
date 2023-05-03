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
    }
}