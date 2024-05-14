using Integration.Common;
using Integration.Backend;

namespace Integration.Service;

public sealed class ItemIntegrationService
{
    //This is a dependency that is normally fulfilled externally.
    private ItemOperationBackend ItemIntegrationBackend { get; set; } = new();

	// singleton service
	private ControlDuplicateService ControlDuplicateService { get; set; } = new();

	// This is called externally and can be called multithreaded, in parallel.
	// More than one item with the same content should not be saved. However,
	// calling this with different contents at the same time is OK, and should
	// be allowed for performance reasons.
	public Result SaveItem(string itemContent)
    {
        lock(itemContent)
        {
            if(ControlDuplicateService.IsExistContent(itemContent))
            {
				return new Result(false, $"Duplicate item received with content {itemContent}.");
			}
            else
            {
                ControlDuplicateService.AddContent(itemContent);
            }
        }

        // Check the backend to see if the content is already saved.
        if (ItemIntegrationBackend.FindItemsWithContent(itemContent).Count != 0)
        {
            return new Result(false, $"Duplicate item received with content {itemContent}.");
        }

        var item = ItemIntegrationBackend.SaveItem(itemContent);

        lock (itemContent) 
        {
            ControlDuplicateService.RemoveContent(itemContent);
		}

        return new Result(true, $"Item with content {itemContent} saved with id {item.Id}");
    }

    public List<Item> GetAllItems()
    {
        return ItemIntegrationBackend.GetAllItems();
    }
}