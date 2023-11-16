using Play.Inventory.Service.Dtos;

namespace Play.Inventory.Service.Clients
{
    public class CatalogClient
    {
        private readonly HttpClient httpClient;

        public CatalogClient(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }
        public async Task<IReadOnlyCollection<CatelogItemDto>> GetCatalogItemsAsync()
        {
            var items=await httpClient.GetFromJsonAsync<IReadOnlyCollection<CatelogItemDto>>("/items");
            return items;
        }
    }
}