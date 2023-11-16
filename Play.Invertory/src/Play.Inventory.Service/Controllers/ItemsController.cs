using Microsoft.AspNetCore.Mvc;
using Play.Common.Repositories;
using Play.Inventory.Service.Clients;
using Play.Inventory.Service.Dtos;
using Play.Inventory.Service.Entities;

namespace Play.Inventory.Service.Controller
{
    [ApiController]
    [Route("items")]
    public class ItemsController : ControllerBase
    {
        private readonly IRepository<InventoryItem> inventoryItemRespository;
        //private readonly CatalogClient catalogClient;
        private readonly IRepository<CatalogItem>catalogItemRepository;

        public ItemsController(IRepository<InventoryItem> inventoryItemRespository
, IRepository<CatalogItem> catalogItemRepository
        //, CatalogClient catalogClient
        )
        {
            this.inventoryItemRespository = inventoryItemRespository;
            this.catalogItemRepository = catalogItemRepository;
            //this.catalogClient = catalogClient;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<InventoryItemDto>>> GetAsync(Guid userId)
        {
            if (userId == Guid.Empty)
                return BadRequest();
           // var catalogItems = await catalogClient.GetCatalogItemsAsync();

            var inventoryItemEntities = (await inventoryItemRespository.GetAllAsync(item => item.UserId == userId));
            var itemIds=inventoryItemEntities.Select(item=>item.CatalogItemId);
            var catalogItemEntities=await catalogItemRepository.GetAllAsync(item=>itemIds.Contains(item.Id));
            var inventoryItemDtos = inventoryItemEntities.Select(inventoryItem =>
            {
                //var catalogItem = catalogItems.Single(catalogITem => catalogITem.Id == inventoryItem.CatalogItemId);
                var catalogItem = catalogItemEntities.Single(catalogITem => catalogITem.Id == inventoryItem.CatalogItemId);
                return inventoryItem.AsDto(catalogItem.Name, catalogItem.Description);
            });

            return Ok(inventoryItemDtos);
        }
        [HttpPost]
        public async Task<ActionResult> PostAsync(GrantItemsDto grantItemsDto)
        {
            var inventoryItem = await inventoryItemRespository.GetAsync(item => item.UserId == grantItemsDto.UserId
            && item.CatalogItemId == grantItemsDto.CatalogItemId);
            if (inventoryItem == null)
            {
                inventoryItem = new InventoryItem
                {
                    CatalogItemId = grantItemsDto.CatalogItemId,
                    UserId = grantItemsDto.UserId,
                    Quantity = grantItemsDto.Quantity,
                    AcquiredDate = DateTimeOffset.UtcNow
                };
                await inventoryItemRespository.CreatedAsync(inventoryItem);
            }
            else
            {
                inventoryItem.Quantity += grantItemsDto.Quantity;
                await inventoryItemRespository.UpdateAsync(inventoryItem);
            }
            return Ok();
        }
    }
}