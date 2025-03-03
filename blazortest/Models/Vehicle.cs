namespace Aaronbackend
{
    public class Vehicle
    {
            public int Id { get; set; }
            public string LicensePlate { get; set; }
            public string Model { get; set; }
            public string Owner { get; set; }
            public DateTime RegistrationDate { get; set; }

            // New fields for personal information
            public string VehicleName { get; set; }
            public string OwnerAddress { get; set; }
            public string OwnerContactNumber { get; set; }
            public string OwnerEmail { get; set; }

            // New field for price
            public float Price { get; set; }

        }
}
