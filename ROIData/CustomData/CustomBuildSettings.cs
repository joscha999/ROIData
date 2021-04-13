using ProjectAutomata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ROIData.CustomData
{
    public class CustomBuildSettings
    {
        private IList<Building> Buildings = new List<Building>();
        private IList<ProductDefinition> Products = new List<ProductDefinition>();
        private IList<Recipe> Recipes = new List<Recipe>();
        private IList<Vehicle> Vehicles = new List<Vehicle>();

        public CustomBuildSettings WithBuilding(Building building)
        {
            Buildings.Add(building);
            return this;
        }

        public CustomBuildSettings WithBuildings(IEnumerable<Building> buildings)
        {
            Buildings.AddRange(buildings);
            return this;
        }

        public CustomBuildSettings WithProduct(ProductDefinition product)
        {
            Products.Add(product);
            return this;
        }

        public CustomBuildSettings WithProducts(IEnumerable<ProductDefinition> products)
        {
            Products.AddRange(products);
            return this;
        }

        public CustomBuildSettings WithRecipe(Recipe recipe)
        {
            Recipes.Add(recipe);
            return this;
        }

        public CustomBuildSettings WithRecipes(IEnumerable<Recipe> recipes)
        {
            Recipes.AddRange(recipes);
            return this;
        }

        public CustomBuildSettings WithVehicle(Vehicle vehicle)
        {
            Vehicles.Add(vehicle);
            return this;
        }

        public CustomBuildSettings WithVehicles(IEnumerable<Vehicle> vehicles)
        {
            Vehicles.AddRange(vehicles);
            return this;
        }

        public WorldEventEffectBuildSettings Build()
        {
            return new WorldEventEffectBuildSettings {
                buildings = Buildings,
                products = Products,
                recipes = Recipes,
                vehicles = Vehicles
            };
        }
    }
}
