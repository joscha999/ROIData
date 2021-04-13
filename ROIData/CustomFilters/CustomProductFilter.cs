using ProjectAutomata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ROIData.CustomFilters
{
    public class CustomProductFilter
    {
        private WorldEventEffectFilterAmount Amount;
        private bool FromAll;
        private List<DataCategory> DataCategories = new List<DataCategory>();
        private List<ProductDefinition> Products = new List<ProductDefinition>();
        private List<ProductTag> ProductTags = new List<ProductTag>();
        private List<Building> Buildings = new List<Building>();

        public CustomProductFilter(WorldEventEffectFilterAmount amount, bool fromAll)
        {
            Amount = amount;
            FromAll = fromAll;
        }

        public CustomProductFilter WithCategory(DataCategory category)
        {
            DataCategories.Add(category);
            return this;
        }

        public CustomProductFilter WithCategories(IEnumerable<DataCategory> categories)
        {
            DataCategories.AddRange(categories);
            return this;
        }

        public CustomProductFilter WithProduct(ProductDefinition product)
        {
            Products.Add(product);
            return this;
        }

        public CustomProductFilter WithProducts(IEnumerable<ProductDefinition> products)
        {
            Products.AddRange(products);
            return this;
        }

        public CustomProductFilter WithProductTag(ProductTag productTag)
        {
            ProductTags.Add(productTag);
            return this;
        }

        public CustomProductFilter WithProductTags(IEnumerable<ProductTag> productTags)
        {
            ProductTags.AddRange(productTags);
            return this;
        }

        public CustomProductFilter WithProducer(Building building)
        {
            Buildings.Add(building);
            return this;
        }

        public CustomProductFilter WithProducers(IEnumerable<Building> buildings)
        {
            Buildings.AddRange(buildings);
            return this;
        }

        public WorldEventProductFilter Build()
        {
            return new WorldEventProductFilter
            {
                amount = Amount,
                categoryRestrictions = DataCategories.ToArray(),
                fromAll = FromAll,
                fromProducts = Products.ToArray(),
                fromTags = ProductTags.ToArray(),
                producers = Buildings.ToArray()
            };
        }
    }
}
