using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace bbxBE.Application.Commands.cmdImport
{
    public class CustomerMappingParser
    {
        public Dictionary<string, int> customerMap { get; set; }

        public CustomerMappingParser ReCalculateIndexValues()
        {
            var productMapping_temp = new Dictionary<string, int>();
            foreach (var item in this.customerMap)
            {
                productMapping_temp.Add(item.Key, item.Value - 1);
            }

            this.customerMap = productMapping_temp;
            return this;
        }

        public CustomerMappingParser GetProductMapping(ImportCustomerCommand mappingFile)
        {
            string s;
            using (var reader = new StreamReader(mappingFile.ProductFiles[0].OpenReadStream()))
            {
                s = reader.ReadToEnd();
            }

            this.customerMap = JsonConvert.DeserializeObject<Dictionary<string, int>>(s);
            return this;
        }
    }
}