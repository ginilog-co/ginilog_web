import 'package:ginilog_customer_app/core/components/utils/colors.dart';
import 'package:ginilog_customer_app/core/components/utils/helper_functions.dart';
import 'package:ginilog_customer_app/core/components/utils/package_export.dart';
import 'package:ginilog_customer_app/core/components/widgets/app_text.dart';
import 'package:ginilog_customer_app/core/features/account/view/report/order_items_page.dart';
import 'package:ginilog_customer_app/core/features/order_history/model/package_orders_model.dart';

class AllOrderHistoryPage extends ConsumerStatefulWidget {
  final List<PackageOrderResponseModel> allOrder;
  const AllOrderHistoryPage({super.key, required this.allOrder});

  @override
  ConsumerState<AllOrderHistoryPage> createState() =>
      _AllOrderHistoryPageState();
}

class _AllOrderHistoryPageState extends ConsumerState<AllOrderHistoryPage> {
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
              widget.allOrder.isEmpty
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
                              fontSize: 70,
                              color: AppColors.black,
                              fontStyle: FontStyle.normal,
                              fontWeight: FontWeight.normal),
                        ],
                      ),
                    )
                  : ListView.builder(
                      itemCount: widget.allOrder.length,
                      shrinkWrap: true,
                      physics: const NeverScrollableScrollPhysics(),
                      itemBuilder: (context, index) => OrderItemPage(
                            order: widget.allOrder[index],
                          ))
            ],
          ),
        ));
  }
}
