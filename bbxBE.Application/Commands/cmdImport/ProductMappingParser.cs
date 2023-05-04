using Newtonsoft.Json;
using System.Collections.Generic;

namespace bbxBE.Application.Commands.cmdImport
{
    public class ProductMappingParser
    {
        public Dictionary<string, int> productMap { get; set; }

        public ProductMappingParser ReCalculateIndexValues()
        {
            var productMapping_temp = new Dictionary<string, int>();
            foreach (var item in this.productMap)
            {
                productMapping_temp.Add(item.Key, item.Value - 1);
            }

            this.productMap = productMapping_temp;
            return this;
        }

        public ProductMappingParser GetProductMapping(string mapFileContent)
        {
            this.productMap = JsonConvert.DeserializeObject<Dictionary<string, int>>(mapFileContent);
            return this;
        }

    }
}