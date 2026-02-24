namespace Customer_Web_App.Models.LogisticsModel
{
    public class OrderStatistics
    {
        public double TotalRevenue { get; set; }
        public int TotalOrders { get; set; }
        public int TodaysOrder { get; set; }
        public int CompletedOrders { get; set; }
        public int PendingOrders { get; set; }
        public int OpenOrders { get; set; }
        public int RejectedOrders { get; set; }
        public double TotalAmountOrders { get; set; }
        public double TodayAmountOrders { get; set; }
        public double CompletedAmountOrders { get; set; }
        public double PendingAmountOrders { get; set; }
        public double OpenAmountOrders { get; set; }
        public double RejectedAmountOrders { get; set; }

    }
}
