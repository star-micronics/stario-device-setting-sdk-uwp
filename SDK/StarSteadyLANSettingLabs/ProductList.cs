using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarSteadyLANSettingLabs
{
    public class ProductList
    {
        public ProductList(StarIOPort.ProductInformation item)
        {
            this.MacAddress = "(" + item.MacAddress + ")";
            this.ModelName = item.Name.StartsWith("BT:") ? item.Name.Remove(0, 3) : item.ModelName;
            this.PortName = item.Name.StartsWith("BT:") ? "BT:" + item.MacAddress : item.Name;
        }

        public string MacAddress { get; set; }
        public string ModelName { get; set; }
        public string PortName { get; set; }
    }
}
