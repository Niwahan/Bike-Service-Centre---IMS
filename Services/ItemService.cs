using System.Data;
using System.Reflection;
using System.Text.Json;
using NiwaCoursework.Data;

namespace NiwaCoursework.Services
{
    public class ItemService : UserServices
    {
        public static string GetItemsPath()
        {
            return Path.Combine(GetAppDirectoryPath(), "items.json");
        }

        public static List<Item> ReadItem()
        {
            string path = GetItemsPath();
            if (!File.Exists(path))
            {
                return new List<Item>();
            }

            string readJson = File.ReadAllText(path);
            return JsonSerializer.Deserialize<List<Item>>(readJson);
        }
        public static void SaveAllItem(List<Item> items)
        {
            string appDataDirectoryPath = GetAppDirectoryPath();
            if (!Directory.Exists(appDataDirectoryPath))
            {
                Directory.CreateDirectory(appDataDirectoryPath);
            }
            string path = GetItemsPath();
            var json = JsonSerializer.Serialize(items);
            File.WriteAllText(path, json);
        }

        public static List<Item> AddItem(string name, int quantity, DateTime addedDate)
        {
            List<Item> items = ReadItem();
            bool itemExists = items.Any(x => x.Name == name);

            if (itemExists)
            {
                throw new Exception("There is a item with the same name!");
            }
            items.Add(
                new Item
                {
                    Name = name,
                    Quantity = quantity,
                    AddedDate = addedDate
                }
            );
            SaveAllItem(items);
            return items;
        }
        public static List<Item> UpdateItem(string name, int quantity)
        {
            List<Item> items = ReadItem();
            Item item = items.FirstOrDefault(x => x.Name == name);
            if (item == null)
            {
                throw new Exception("Item not found.");
            }
            item.Quantity += quantity;
            SaveAllItem(items);
            return items;
        }
        public static List<Item> DeleteItem(string name)
        {
            List<Item> items = ReadItem();
            Item item = items.FirstOrDefault(x => x.Name == name);

            if (item == null)
            {
                throw new Exception("Item not found.");
            }
            items.Remove(item);
            SaveAllItem(items);
            return items;
        }
    }
}