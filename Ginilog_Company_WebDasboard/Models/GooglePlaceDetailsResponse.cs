using Newtonsoft.Json;

namespace Ginilog_Company_WebDasboard.Models
{
    public class GooglePlaceDetailsResponse
    {
        [JsonProperty("result")]
        public GooglePlaceResult? Result { get; set; }
    }

    public class GooglePlaceResult
    {
        [JsonProperty("formatted_address")]
        public string? FormattedAddress { get; set; }

        [JsonProperty("address_components")]
        public List<AddressComponent>? AddressComponents { get; set; }

        [JsonProperty("geometry")]
        public Geometry? Geometry { get; set; }
    }

    public class AddressComponent
    {
        [JsonProperty("long_name")]
        public string? LongName { get; set; }

        [JsonProperty("short_name")]
        public string? ShortName { get; set; }

        [JsonProperty("types")]
        public List<string>? Types { get; set; }
    }

    public class Geometry
    {
        [JsonProperty("location")]
        public LatLng? Location { get; set; }
    }

    public class LatLng
    {
        [JsonProperty("lat")]
        public double Lat { get; set; }

        [JsonProperty("lng")]
        public double Lng { get; set; }
    }

    public class LocationDataDetail
    {
        public string? Address { get; set; }
        public string? Locality { get; set; }
        public string? State { get; set; }
        public string? Country { get; set; }
        public string? Postcode { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }

    }

}
