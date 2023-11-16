using MassTransit;
using Play.Catalog.Contracts;
using Play.Common.Repositories;
using Play.Inventory.Service.Entities;

namespace Play.Inventory.Service.Consumers
{
    public class CatalogItemUpdateConsumer : IConsumer<CatalogItemUpdated>
    {
        private readonly IRepository<CatalogItem> repository;

        public CatalogItemUpdateConsumer(IRepository<CatalogItem> repository)
        {
            this.repository = repository;
        }

        public async Task Consume(ConsumeContext<CatalogItemUpdated> context)
        {
            var message=context.Message;
            var item=await repository.GetAsync(message.ItemId);
            if(item==null)
            {
               item=new CatalogItem{
                Id=message.ItemId,
                Name=message.Name,
                Description=message.Description
            };
            await repository.CreatedAsync(item);
            }
            else{
                item.Name=message.Name;
                item.Description=message.Description;
                await repository.UpdateAsync(item);
            }
            
            
        }
    }
}