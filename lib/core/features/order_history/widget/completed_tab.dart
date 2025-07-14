import 'package:ginilog_customer_app/core/components/utils/colors.dart';
import 'package:ginilog_customer_app/core/components/utils/package_export.dart';
import 'package:ginilog_customer_app/core/components/utils/size_config.dart';
import 'package:ginilog_customer_app/core/components/widgets/app_text.dart';
import 'package:ginilog_customer_app/core/features/order_history/model/package_orders_model.dart';
import 'package:ginilog_customer_app/core/features/order_history/view/order_items.dart';

class CompletedTab extends ConsumerStatefulWidget {
  final List<PackageOrderResponseModel> completedOrder;
  final String userPhone;
  const CompletedTab({
    super.key,
    required this.completedOrder,
    required this.userPhone,
  });

  @override
  ConsumerState<CompletedTab> createState() => _CompletedTabState();
}

class _CompletedTabState extends ConsumerState<CompletedTab> {
  @override
  void initState() {
    super.initState();
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: AppColors.white,
      body: SingleChildScrollView(
        child: Column(
          children: [
            widget.completedOrder.isEmpty
                ? Center(
                  child: Column(
                    mainAxisAlignment: MainAxisAlignment.center,
                    crossAxisAlignment: CrossAxisAlignment.center,
                    children: [
                      addVerticalSpacing(25),
                      const AppText(
                        isBody: false,
                        text: "Nothing to show here",
                        textAlign: TextAlign.start,
                        fontSize: 18,
                        color: AppColors.black,
                        fontStyle: FontStyle.normal,
                        fontWeight: FontWeight.bold,
                      ),
                      const AppText(
                        isBody: true,
                        text: "You don't have any order at the moment",
                        textAlign: TextAlign.center,
                        fontSize: 18,
                        color: AppColors.black,
                        fontStyle: FontStyle.normal,
                        fontWeight: FontWeight.normal,
                      ),
                    ],
                  ),
                )
                : ListView.builder(
                  itemCount: widget.completedOrder.length,
                  shrinkWrap: true,
                  physics: const NeverScrollableScrollPhysics(),
                  itemBuilder:
                      (context, index) => ActiveOrderItem(
                        order: widget.completedOrder[index],
                        userPhone: widget.userPhone,
                      ),
                ),
          ],
        ),
      ),
    );
  }
}
