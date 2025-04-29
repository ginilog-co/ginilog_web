namespace Genilog_WebApi.Model.LogisticsModel
{
    public class OrderStatistics
    {
        public double TotalRevenue { get; set; }
        public int TotalOrders { get; set; }
        public int TodaysOrder { get; set; }
        public int CompletedOrders { get; set; }
        public int PendingOrders { get; set; }
        public int OpenOrders { get; set; }

    }
}
