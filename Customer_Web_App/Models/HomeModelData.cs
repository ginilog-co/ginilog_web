using Customer_Web_App.Models.BookingsModel;
using Customer_Web_App.Models.LogisticsModel;

namespace Customer_Web_App.Models
{
    public class HomeModelData
    {
        public List<CompanyModelData>? CompanyModelData { get; set; }
        public List<AccomodationDataModel>? AccomodationDataModel { get; set; }
        public List<BookAccomodationReservatioModel>? BookAccomodationReservatioModel { get; set; }

    }
}
