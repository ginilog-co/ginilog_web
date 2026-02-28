namespace Customer_Web_App.Models.WalletModel
{
    public class PayoutStatistics
    {
        public int TotalPayout { get; set; }
        public double TotalAmountPayout { get; set; }
        public int TotalRiderPayout { get; set; }
        public double TotalRiderAmountPayout { get; set; }
        public int TotalLogisticsPayout { get; set; }
        public double TotalLogisticslAmountPayout { get; set; }
        public int TodaysPayout { get; set; }
        public double TodaysAmountPayout { get; set; }
        public int TodaysLogisticsPayout { get; set; }
        public double TodayLogisticsAmountPayout { get; set; }
        public int TodaysRiderPayout { get; set; }
        public double TodayRiderAmountPayout { get; set; }

    }
}
