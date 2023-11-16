using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Play.Catalog.Contracts;
using Play.Catalog.Service;
using Play.Catalog.Service.Dtos;
using Play.Catalog.Service.Entities;
using Play.Common.Repositories;


namespace Play.Catalog.Controllers
{
    [ApiController]
    [Route("Items")]
    public class ItemsController : ControllerBase
    {
        private readonly IRepository<Item> repo;
        //private static int requestCoutner = 0;
        private readonly IPublishEndpoint publishEndpoint;

        public ItemsController(IRepository<Item> repo, IPublishEndpoint publishEndpoint)
        {
            this.repo = repo;
            this.publishEndpoint = publishEndpoint;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ItemDto>>> GetAsync()
        {

            // requestCoutner++;
            // Console.WriteLine($"Reqeust {requestCoutner}: Starting...");
            // if (requestCoutner <= 2)
            // {
            //     Console.WriteLine($"Reqeust {requestCoutner}: Delaying...");
            //     await Task.Delay(TimeSpan.FromSeconds(10));
            // }
            // if (requestCoutner <= 4)
            // {
            //     Console.WriteLine($"Reqeust {requestCoutner}: 500(Internal Server Error).");
            //     return StatusCode(500);
            // }
            var items = (await repo.GetAllAsync()).Select(item => item.AsDto());
            // Console.WriteLine($"Reqeust {requestCoutner}: 200 (OK)");
            return Ok(items);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ItemDto>> GetAsyncById(Guid id)
        {

            var item = await repo.GetAsync(id);
            if (item == null)
                return NotFound();
            return item.AsDto();
        }
        [HttpPost]
        public async Task<ActionResult<ItemDto>> PostAsync(CreateItemDto createItemDto)
        {
            var item = new Item()
            {
                Name = createItemDto.Name,
                Description = createItemDto.Description,
                Price = createItemDto.Price,
                CreatedDate = DateTimeOffset.UtcNow
            };
            await repo.CreatedAsync(item);
            await publishEndpoint.Publish(new CatalogItemCreated(item.Id, item.Name, item.Description));

            return CreatedAtAction(nameof(GetAsyncById), new { id = item.Id }, item);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAsync(Guid id, UpdateItemDto updateItemDto)
        {
            var item = await repo.GetAsync(id);
            if (item == null)
            {
                return NotFound();
            }

            item.Name = updateItemDto.Name;
            item.Description = updateItemDto.Description;
            item.Price = updateItemDto.Price;

            await repo.UpdateAsync(item);
            await publishEndpoint.Publish(new CatalogItemUpdated(item.Id, item.Name, item.Description));
            return NoContent();


        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            var item = await repo.GetAsync(id);
            if (item == null)
            {
                return NotFound();
            }
            await repo.RemoveAsync(item.Id);
            await publishEndpoint.Publish(new CatalogItemDeleted(item.Id));
            return NoContent();
        }
    }
}