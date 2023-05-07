// Pego o dado da página de pesquisa(index.html) e envio como parametro para a pagina de links (paginaLinks.html)
// ROTA GET
const params = new URLSearchParams(window.location.search);
const searchQuery = params.get('titleQuery');


// Altera o título da página
document.title = searchQuery;

fetch(`http://localhost:5043/api/Link/GetLinkByTitle/${searchQuery}`)
  .then(response => response.json())
  .then(data => {
    // percorrendo a lista de resultados
    const resultList = document.querySelector('#LinksBuscados');
    resultList.innerHTML = '';
    data.forEach(result => {
        resultList.innerHTML += `
        <li>
            <p class="url">${result.url}</p>
            <a href="${result.url}">${result.titulo}</a>
            <p class="description">${result.descricao}</p>
        </li>
        <br>
      `;
    });
  })
  .catch(error => console.error(error));



