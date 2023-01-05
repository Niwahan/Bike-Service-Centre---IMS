using System.Text.Json;
using NiwaCoursework.Data;

namespace NiwaCoursework.Services
{
    public class ItemRequestService : ItemService
    {
        public static string GetItemsRecordPath()
        {
            return Path.Combine(GetAppDirectoryPath(), "itemsRequest.json");
        }

        public static List<ItemRequest> ReadItemReq()
        {
            string path = GetItemsRecordPath();
            if (!File.Exists(path))
            {
                return new List<ItemRequest>();
            }

            string readJson = File.ReadAllText(path);
            return JsonSerializer.Deserialize<List<ItemRequest>>(readJson);
        }
        private static void SaveAllItemReq(List<ItemRequest> itemsRequest)
        {
            string appDataDirectoryPath = GetAppDirectoryPath();
            if (!Directory.Exists(appDataDirectoryPath))
            {
                Directory.CreateDirectory(appDataDirectoryPath);
            }
            string path = GetItemsRecordPath();
            var json = JsonSerializer.Serialize(itemsRequest);
            File.WriteAllText(path, json);
        }

        public static List<ItemRequest> AddItemReq(string name, int quantity, string requestedBy)
        {
            List<ItemRequest> itemsRequests = ReadItemReq();
            List<Item> items = ReadItem();
            Item item = items.FirstOrDefault(x => x.Name == name);
            if (item.Quantity < quantity || item.Quantity == 0)
            {
                throw new Exception("There is not enough quantity in the inventory currently!");
            }
            itemsRequests.Add(
                new ItemRequest
                {
                    Name = name,
                    Quantity = quantity,
                    RequestedBy = requestedBy,
                }
            );
            SaveAllItemReq(itemsRequests);
            return itemsRequests;
        }
        public static List<ItemRequest> DeleteItemReq(int id)
        {
            List<ItemRequest> itemRequests = ReadItemReq();
            ItemRequest itemRequest = itemRequests.FirstOrDefault(x => x.Id == id);
            if (itemRequest == null)
            {
                throw new Exception("Item Request not found.");
            }
            itemRequests.Remove(itemRequest);
            SaveAllItemReq(itemRequests);
            return itemRequests;
        }
        public static List<ItemRequest> Approve(int id, string name, string approvedBy, DateTime approvedDate, bool isApproved)
        {
            List<ItemRequest> itemRequests = ReadItemReq();
            ItemRequest itemRequest = itemRequests.FirstOrDefault(x => x.Id == id);
            List<Item> items = ReadItem();
            Item item = items.FirstOrDefault(x => x.Name == name);
            if (item == null)
            {
                itemRequests = DeleteItemReq(id);
                throw new Exception("The item has been removed from the inventory!");
            }
            int reducedStock = item.Quantity - itemRequest.Quantity;
            item.Quantity = reducedStock;
            SaveAllItem(items);
            itemRequest.ApprovedBy = approvedBy;
            itemRequest.ApprovedDate = approvedDate;
            itemRequest.IsApproved = isApproved;
            SaveAllItemReq(itemRequests);
            return itemRequests;
        }
        public static void AddOtherItemDetail(string name, DateTime lastApprove, int quantity)
        {
            List<Item> items = ReadItem();
            Item item = items.FirstOrDefault(x => x.Name == name);
            item.LastApprovedDate = lastApprove;
            item.TotalQuantityTaken += quantity;
            SaveAllItem(items);
        }

    }
}
