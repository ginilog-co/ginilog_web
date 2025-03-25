import 'package:ginilog_customer_app/core/components/architecture/mvc.dart';
import 'package:ginilog_customer_app/core/components/utils/colors.dart';
import 'package:ginilog_customer_app/core/components/utils/package_export.dart';
import 'package:ginilog_customer_app/core/components/utils/size_config.dart';
import 'package:ginilog_customer_app/core/components/widgets/app_text.dart';
import 'package:ginilog_customer_app/core/components/widgets/back_icon.dart';
import 'package:ginilog_customer_app/core/features/account/controller/statement_reports_page.dart';
import 'package:ginilog_customer_app/core/features/account/widget/bookings_widget.dart';
import 'package:ginilog_customer_app/core/features/account/widget/order_history_page.dart';
import 'package:ginilog_customer_app/core/features/order_history/model/package_orders_model.dart';

class StatementReportScreenView extends StatelessView<StatementReportScreen,
    StatementReportScreenController> {
  const StatementReportScreenView(super.state, {super.key});

  @override
  Widget build(BuildContext context) {
    var completedOrder = controller.allOrders.where((element) {
      return (element.orderStatus == OrderClassState.delivered);
    }).toList();
    return Scaffold(
      backgroundColor: AppColors.white,
      appBar: PreferredSize(
          preferredSize: Size.fromHeight(SizeConfig.heightAdjusted(22)),
          child: Padding(
            padding: EdgeInsets.only(top: SizeConfig.heightAdjusted(10)),
            child: Column(
              children: [
                const GlobalBackButton(
                    backText: 'Report', showBackButton: true),
                TabBar(
                    controller: controller.tabController,
                    labelColor: AppColors.primary,
                    labelStyle: TextStyle(
                      color: AppColors.black,
                      fontWeight: FontWeight.bold,
                      fontSize: fontSized(context, 35),
                      fontFamily: "Mulish",
                    ),
                    isScrollable: false,
                    unselectedLabelColor: AppColors.black.withAlpha(100),
                    indicatorColor: AppColors.primary,
                    tabs: const [Tab(text: "Packages"), Tab(text: "Booking")]),
              ],
            ),
          )),
      body: TabBarView(
        physics: NeverScrollableScrollPhysics(),
        controller: controller.tabController,
        children: [
          AllOrderHistoryPage(
            allOrder: completedOrder,
          ),
          AllBookingsHistoryPage(
            allAccomodations: controller.allAccomodations,
          ),
        ],
      ),
    );
  }
}
