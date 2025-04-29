using Ginilog_AdminWeb.Models.BookingsModel;
using Ginilog_AdminWeb.Models.InfoModel;
using Ginilog_AdminWeb.Models.LogisticsModel;

namespace Ginilog_AdminWeb.Models
{
    public class HomeModelData
    {
        public List<OrderModelData>? OrderModelData { get; set; }
        public OrderStatistics? OrderStatistics { get; set; }
        public List<CustomerBookedReservation>? CustomerBookedReservation { get; set; }
        public SendMailModel? SendMailModel { get; set; }
    }
}
