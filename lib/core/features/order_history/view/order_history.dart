import 'package:ginilog_customer_app/core/components/architecture/mvc.dart';
import 'package:ginilog_customer_app/core/components/utils/colors.dart';
import 'package:ginilog_customer_app/core/components/utils/package_export.dart';
import 'package:ginilog_customer_app/core/components/utils/size_config.dart';
import 'package:ginilog_customer_app/core/components/widgets/back_icon.dart';
import 'package:ginilog_customer_app/core/features/order_history/controller/order_history.dart';
import 'package:ginilog_customer_app/core/features/order_history/model/package_orders_model.dart';
import 'package:ginilog_customer_app/core/features/order_history/widget/all_order_list.dart';
import 'package:ginilog_customer_app/core/features/order_history/widget/completed_tab.dart';
import 'package:ginilog_customer_app/core/features/order_history/widget/ongoing_tab.dart';

class OrderHistoryScreenView
    extends StatelessView<OrderHistoryScreen, OrderHistoryScreenController> {
  const OrderHistoryScreenView(super.state, {super.key});

  @override
  Widget build(BuildContext context) {
    var pending = controller.allOrders.where((element) {
      return (element.orderStatus == OrderClassState.open ||
          element.orderStatus == OrderClassState.accepted);
    }).toList();
    var ongoingOrder = controller.allOrders.where((element) {
      return (element.orderStatus == OrderClassState.picked ||
          element.orderStatus == OrderClassState.ongoing ||
          element.orderStatus == OrderClassState.delivered);
    }).toList();
    var completedOrder = controller.allOrders.where((element) {
      return (element.orderStatus == OrderClassState.completed ||
          element.orderStatus == OrderClassState.closed);
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
                    backText: 'My Orders', showBackButton: false),
                Row(
                  mainAxisAlignment: MainAxisAlignment.spaceEvenly,
                  children: List.generate(4, (index) {
                    return ElevatedButton(
                      onPressed: () {
                        controller.selectTabb(index);
                      },
                      style: ElevatedButton.styleFrom(
                        elevation: 0,
                        backgroundColor: controller.tabController.index == index
                            ? Colors.red
                            : Colors.white,
                        foregroundColor: controller.tabController.index == index
                            ? Colors.white
                            : AppColors.grey2,
                        padding:
                            EdgeInsets.symmetric(horizontal: 20, vertical: 10),
                        shape: RoundedRectangleBorder(
                          borderRadius: BorderRadius.circular(8),
                        ),
                      ),
                      child: Text(
                          ['All', 'Pending', 'In-transit', 'Completed'][index]),
                    );
                  }),
                ),
              ],
            ),
          )),
      body: TabBarView(
        controller: controller.tabController,
        children: [
          AllOrderListTab(
            allOrder: controller.allOrders,
            userPhone: controller.userPhone,
          ),
          OngoingTab(
            ongoingOrder: pending,
            userPhone: controller.userPhone,
          ),
          OngoingTab(
            ongoingOrder: ongoingOrder,
            userPhone: controller.userPhone,
          ),
          CompletedTab(
            completedOrder: completedOrder,
            userPhone: controller.userPhone,
          )
        ],
      ),
    );
  }
}
