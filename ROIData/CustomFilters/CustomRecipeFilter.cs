using ProjectAutomata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ROIData.CustomFilters
{
    public class CustomRecipeFilter
    {
        private WorldEventEffectFilterAmount Amount;
        private bool FromAll;
        private List<Recipe> Recipes = new List<Recipe>();
        private List<ProductDefinition> Products = new List<ProductDefinition>();
        private List<ProductTag> ProductTags = new List<ProductTag>();

        public CustomRecipeFilter(WorldEventEffectFilterAmount amount, bool fromAll)
        {
            Amount = amount;
            FromAll = fromAll;
        }

        public CustomRecipeFilter WithRecipe(Recipe recipe)
        {
            Recipes.Add(recipe);
            return this;
        }

        public CustomRecipeFilter WithRecipes(IEnumerable<Recipe> recipes)
        {
            Recipes.AddRange(recipes);
            return this;
        }

        public CustomRecipeFilter WithProduct(ProductDefinition product)
        {
            Products.Add(product);
            return this;
        }

        public CustomRecipeFilter WithProducts(IEnumerable<ProductDefinition> products)
        {
            Products.AddRange(products);
            return this;
        }

        public CustomRecipeFilter WithProductTag(ProductTag productTag)
        {
            ProductTags.Add(productTag);
            return this;
        }

        public CustomRecipeFilter WithProductTags(IEnumerable<ProductTag> productTags)
        {
            ProductTags.AddRange(productTags);
            return this;
        }

        public WorldEventRecipeFilter Build()
        {
            return new WorldEventRecipeFilter
            {
                amount = Amount,
                fromAll = FromAll,
                fromRecipes = Recipes.ToArray(),
                resultProducts = Products.ToArray(),
                resultProductsTags = ProductTags.ToArray()
            };
        }
    }
}
