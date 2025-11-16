namespace Api.Models
{
    public class User
    {
        public enum GenderType
        {
            Male,
            Female,
            Other,
        }

        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public Personal PersonalInformation { get; set; } = new Personal();

        public class Personal
        {
            public string Firstname { get; set; } = string.Empty;
            public string Surname { get; set; } = string.Empty;
            public string Lastname { get; set; } = string.Empty;
            public GenderType Gender { get; set; }
            public string Country { get; set; } = string.Empty;
            public string CountryCode { get; set; } = string.Empty;
            public string Province { get; set; } = string.Empty;
            public string City { get; set; } = string.Empty;
            public string Street { get; set; } = string.Empty;
            public int HouseNumber { get; set; }
            public string HouseNumberAddition { get; set; } = string.Empty;
            public string PostalCode { get; set; } = string.Empty;
        }
    }
}
