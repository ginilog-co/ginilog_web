import 'package:ginilog_customer_app/core/components/utils/colors.dart';
import 'package:ginilog_customer_app/core/components/utils/helper_functions.dart';
import 'package:ginilog_customer_app/core/components/utils/package_export.dart';
import 'package:ginilog_customer_app/core/components/widgets/app_text.dart';
import 'package:ginilog_customer_app/core/features/order_history/model/package_orders_model.dart';
import 'package:ginilog_customer_app/core/features/order_history/view/order_items.dart';

class OngoingTab extends ConsumerStatefulWidget {
  final List<PackageOrderResponseModel> ongoingOrder;
  final String userPhone;
  const OngoingTab(
      {super.key, required this.ongoingOrder, required this.userPhone});

  @override
  ConsumerState<OngoingTab> createState() => _OngoingTabState();
}

class _OngoingTabState extends ConsumerState<OngoingTab> {
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
              widget.ongoingOrder.isEmpty
                  ? Center(
                      child: Column(
                        mainAxisAlignment: MainAxisAlignment.center,
                        crossAxisAlignment: CrossAxisAlignment.center,
                        children: [
                          addVerticalSpacing(context, 55),
                          const AppText(
                              isBody: false,
                              text: "Nothing to show here",
                              textAlign: TextAlign.start,
                              fontSize: 78,
                              color: AppColors.black,
                              fontStyle: FontStyle.normal,
                              fontWeight: FontWeight.bold),
                          const AppText(
                              isBody: true,
                              text: "You don't have any order at the moment",
                              textAlign: TextAlign.center,
                              fontSize: 80,
                              color: AppColors.black,
                              fontStyle: FontStyle.normal,
                              fontWeight: FontWeight.normal),
                        ],
                      ),
                    )
                  : ListView.builder(
                      itemCount: widget.ongoingOrder.length,
                      shrinkWrap: true,
                      physics: const NeverScrollableScrollPhysics(),
                      itemBuilder: (context, index) => ActiveOrderItem(
                            order: widget.ongoingOrder[index],
                            userPhone: widget.userPhone,
                          ))
            ],
          ),
        ));
  }
}
