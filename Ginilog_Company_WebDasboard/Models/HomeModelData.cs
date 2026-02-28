using Ginilog_Company_WebDasboard.Models.BookingsModel;
using Ginilog_Company_WebDasboard.Models.InfoModel;
using Ginilog_Company_WebDasboard.Models.LogisticsModel;

namespace Ginilog_Company_WebDasboard.Models
{
    public class HomeModelData
    {
        public List<OrderModelData>? OrderModelData { get; set; }
        public OrderStatistics? OrderStatistics { get; set; }
        public List<CustomerBookedReservation>? CustomerBookedReservation { get; set; }
        public SendMailModel? SendMailModel { get; set; }
    }
}
