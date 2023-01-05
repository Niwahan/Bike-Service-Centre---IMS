using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NiwaCoursework.Data;

namespace NiwaCoursework.Services
{
    public class BarChartService : ItemService
    {
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
