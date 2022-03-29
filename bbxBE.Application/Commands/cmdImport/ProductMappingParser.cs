using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

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

        public ProductMappingParser GetProductMapping(ImportProductCommand mappingFile)
        {
            string s;
            using (var reader = new StreamReader(mappingFile.ProductFiles[0].OpenReadStream()))
            {
                s = reader.ReadToEnd();
            }
            
            this.productMap = JsonConvert.DeserializeObject<Dictionary<string, int>>(s);
            return this;
        }
    }
}