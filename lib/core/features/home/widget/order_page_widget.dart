import 'package:ginilog_customer_app/core/components/utils/colors.dart';
import 'package:ginilog_customer_app/core/components/utils/helper_functions.dart';
import 'package:ginilog_customer_app/core/components/utils/package_export.dart';
import 'package:ginilog_customer_app/core/components/widgets/app_text.dart';
import 'package:ginilog_customer_app/core/features/order_history/model/package_orders_model.dart';
import 'package:ginilog_customer_app/core/features/order_history/view/order_items.dart';

class OrderPageWidget extends ConsumerStatefulWidget {
  final List<PackageOrderResponseModel> allOrder;
  final String userPhone;
  const OrderPageWidget(
      {super.key, required this.allOrder, required this.userPhone});

  @override
  ConsumerState<OrderPageWidget> createState() => _OrderPageWidgetState();
}

class _OrderPageWidgetState extends ConsumerState<OrderPageWidget> {
  List<PackageOrderResponseModel> allOrder = [];
  @override
  void initState() {
    allOrder = widget.allOrder;
    super.initState();
  }

  List<PackageOrderResponseModel> orderBar(
      List<PackageOrderResponseModel> data) {
    var pending = data.where((element) {
      return (element.orderStatus == OrderClassState.open ||
          element.orderStatus == OrderClassState.accepted ||
          element.orderStatus == OrderClassState.picked ||
          element.orderStatus == OrderClassState.ongoing ||
          element.orderStatus == OrderClassState.delivered);
    }).toList();
    return pending;
  }

  @override
  Widget build(BuildContext context) {
    return Column(
      children: [
        orderBar(allOrder).isEmpty
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
            : Column(
                mainAxisAlignment: MainAxisAlignment.start,
                crossAxisAlignment: CrossAxisAlignment.start,
                children: List.generate(orderBar(allOrder).length, (index) {
                  return ActiveOrderItem(
                    order: orderBar(allOrder)[index],
                    userPhone: widget.userPhone,
                  );
                }))
      ],
    );
  }
}
