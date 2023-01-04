using System.Text.Json;

namespace NiwaCoursework.Data
{
    public class ItemRequestService : ItemService
    {
        public static string GetItemsRecordPath()
        {
            return Path.Combine(GetAppDirectoryPath(), "itemsRequest.json");
        }
        // For Item requests
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
        public static bool CheckTime()
        {
            bool isOpen;
            string[] workingDays = { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday" };
            TimeOnly currentTime = TimeOnly.FromDateTime(DateTime.Now);
            DateTime currentFullTime = DateTime.Now;
            string weekday = currentFullTime.DayOfWeek.ToString();
            TimeOnly openTime = TimeOnly.Parse("9:00 AM");
            TimeOnly closeTime = TimeOnly.Parse("4:00 PM");
            if (currentTime.CompareTo(openTime) >= 0 && currentTime.CompareTo(closeTime) <= 0 && workingDays.Contains(weekday))
            {
                isOpen = true;
            }
            else
            {
                isOpen = false;
            }
            return isOpen;
        }
        public static List<double> GetChartData()
        {
            List<Item> items = ReadItem();
            List<double> quantity = new();
            foreach (var item in items)
            {
               
                if (item.TotalQuantityTaken > 0)
                {
                    quantity.Add(item.TotalQuantityTaken);
                }

            }

            return quantity;
        }
        public static List<string> GetChartLabel()
        {
            List<Item> items = ReadItem();
            List<string> name = new();
            foreach (var item in items)
            {
                if (item.TotalQuantityTaken > 0)
                {
                    name.Add(item.Name);
                }
            }
            return name;
        }
    }
}
