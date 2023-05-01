using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using backend.Models;
using backend.Context;

namespace backend.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class LinkController : ControllerBase
    {
        // aqui é a minha collection do banco de dados
        IMongoCollection<Link> _links;

        public LinkController(MongoConnection myMongoConnection, IConfiguration configuration)
        {
            var CollectionString = configuration["MongoCollections:BuscadorLinks:Link"];

            // recuperando a collection do banco e atribuíndo à variável _links (o tipo dela é IMongoCollection)
            _links = myMongoConnection.context.GetCollection<Link>(CollectionString);
        }

        // rota get somente para testar a API
        [HttpGet]
        public IActionResult rotaTeste()
        {
            // somente para testar se a conexão está tudo funcionando 
            var links = _links.AsQueryable().ToList();
            return Ok(links);
        }
    }
}