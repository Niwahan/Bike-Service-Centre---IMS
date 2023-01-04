using System.Text.Json.Serialization;

namespace NiwaCoursework.Data
{
    public class Item
    {
        public string Name { get; set; }
        public int Quantity { get; set; }
        public DateTime AddedDate { get; set; }
        public int TotalQuantityTaken { get; set; }
        public DateTime LastApprovedDate { get; set; }
    }
}
