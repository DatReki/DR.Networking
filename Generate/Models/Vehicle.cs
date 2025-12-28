using Bogus;

namespace Generate.Models
{
    public class Vehicle
    {
        public Vehicle()
        {

        }

        public Vehicle(Faker faker)
        {
            Manufacturer = faker.Vehicle.Manufacturer();
            Model = faker.Vehicle.Model();
            Type = faker.Vehicle.Type();
            Fuel = faker.Vehicle.Fuel();
            Vin = faker.Vehicle.Vin();
        }

        public string Manufacturer { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Fuel { get; set; } = string.Empty;
        public string Vin { get; set; } = string.Empty;
    }
}
