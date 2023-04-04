namespace HabrEntity.Entity
{
    public class Adress
    {
        private string _Country { get; set; }
        private string _City { get; set; }
        private string _Region { get; set; }
        private int _PostalCode { get; set; }
        private int _CountryCode { get; set; }
        private int _CityCode { get; set; }
        private int _RegionCode { get; set; }
        private string _Street { get; set; }
        private int _NumberHouse { get; set; }
        private int _ApartmentNumber { get; set; }
       
        public Adress( string Country, string City, string Region, int PostalCode, int CountryCode, int CityCode, int RegionCode, string Street, int NumberHouse, int ApartmentNumber)
        {
            _Country = Country;
            _City = City;
            _Region = Region;
            _PostalCode = PostalCode;
            _CountryCode = CountryCode;
            _CityCode = CityCode;
            _RegionCode = RegionCode;
            _Street = Street;
            _NumberHouse = NumberHouse;
            _ApartmentNumber = ApartmentNumber;

        }


    }
}