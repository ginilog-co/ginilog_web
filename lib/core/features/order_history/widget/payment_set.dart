// ignore_for_file: library_private_types_in_public_api, use_build_context_synchronously

import 'dart:convert';

import 'package:ginilog_customer_app/core/components/model/flutterwave_response_model.dart';
import 'package:ginilog_customer_app/core/components/model/paystack_response_model.dart';
import 'package:ginilog_customer_app/core/components/utils/app_buttons.dart';
import 'package:ginilog_customer_app/core/components/utils/colors.dart';
import 'package:ginilog_customer_app/core/components/utils/helper_functions.dart';
import 'package:ginilog_customer_app/core/components/utils/package_export.dart';
import 'package:ginilog_customer_app/core/components/utils/size_config.dart';
import 'package:ginilog_customer_app/core/components/widgets/app_text.dart';
import 'package:ginilog_customer_app/core/components/widgets/custom_snackbar.dart';
import 'package:ginilog_customer_app/core/components/widgets/payment_page_widget.dart';
import 'package:ginilog_customer_app/core/features/order_history/model/package_orders_model.dart';
import 'package:ginilog_customer_app/core/features/order_history/services/package_order_service.dart';

class PaymentMethodBottomSheet extends StatefulWidget {
  const PaymentMethodBottomSheet({super.key, required this.order});
  final PackageOrderResponseModel order;
  @override
  _PaymentMethodBottomSheetState createState() =>
      _PaymentMethodBottomSheetState();
}

class _PaymentMethodBottomSheetState extends State<PaymentMethodBottomSheet> {
  int selected = 0;
  String paymentMethodUse = "";
  bool isLoading = false;
  void handlePayment() {
    if (selected == 1) {
      handlePaystackPayment();
    } else if (selected == 2) {
      handleFlutterWavePayment();
    } else {
      ScaffoldMessenger.of(
        context,
      ).showSnackBar(SnackBar(content: Text("Please select a payment method")));
    }
  }

  @override
  Widget build(BuildContext context) {
    return Container(
      width: double.infinity,
      padding: EdgeInsets.only(top: 10, left: 8, right: 8),
      height: SizeConfig.heightAdjusted(100) * 0.45,
      decoration: const BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.vertical(top: Radius.circular(20)),
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          /// Header
          Row(
            mainAxisAlignment: MainAxisAlignment.spaceBetween,
            children: [
              AppText(
                isBody: true,
                text: "Choose Payment Method",
                textAlign: TextAlign.start,
                fontSize: 10,
                color: AppColors.black,
                fontStyle: FontStyle.normal,
                fontWeight: FontWeight.w800,
              ),

              addHorizontalSpacing(10),
              GestureDetector(
                onTap: () => Navigator.pop(context),
                child: const SizedBox(
                  height: 30,
                  width: 30,
                  child: Icon(Icons.close),
                ),
              ),
            ],
          ),
          addVerticalSpacing(1.2),
          const Divider(),

          addVerticalSpacing(5.2),

          _buildPaymentOption("Pay with Paystack", 1),
          // SizedBox(height: 15),
          // _buildPaymentOption("Pay with Flutterwave", 2),
          addVerticalSpacing(2.2),
          AppButton(
            text: "Continue",
            onPressed: handlePayment,
            widthPercent: 100,
            heightPercent: 6,
            btnColor: AppColors.primary,
            isLoading: isLoading,
          ),
        ],
      ),
    );
  }

  Widget _buildPaymentOption(String text, int value) {
    return GestureDetector(
      onTap: () {
        setState(() {
          selected = value;
          paymentMethodUse = text;
        });
      },
      child: Container(
        padding: EdgeInsets.symmetric(vertical: 15),
        decoration: BoxDecoration(
          color: selected == value ? AppColors.primary : AppColors.white,
          border: Border.all(color: AppColors.primaryDark),
          borderRadius: const BorderRadius.all(Radius.circular(15)),
        ),
        alignment: Alignment.center,
        child: Text(
          text,
          style: TextStyle(
            fontSize: 16.textSize,
            color: selected == value ? Colors.white : Colors.black,
          ),
        ),
      ),
    );
  }

  // PAYSTACK

  void handlePaystackPayment() async {
    setState(() {
      isLoading = true;
    });

    final response = await PackageOrderService().makePayment(
      orderId: widget.order.id.toString(),
      trnxReference: generateTransactionReference(),
      paymentStatus: true,
      paymentChannel: "Paystack",
      paymentType: "paystack",
    );
    if (response.statusCode == 200 || response.statusCode == 201) {
      setState(() {
        isLoading = false;
      });
      var data = json.decode(response.body);
      var payment = PaystackResponseModel.fromJson(data);
      navigateBack(context);
      navigateToRoute(
        context,
        PaystackPaymentPage(
          url: payment.data!.authorizationUrl!,
          isPaystack: true,
          isPackageOrder: true,
        ),
      );
      setState(() {
        isLoading = false;
      });
    } else {
      setState(() {
        isLoading = false;
      });
      navigateBack(context);
      showCustomSnackbar(
        context,
        title: "Payment Error",
        content: "Payment could not Completed because${response.body}",
        type: SnackbarType.error,
        isTopPosition: false,
      );
    }
  }

  String generateTransactionReference() {
    return DateTime.now().millisecondsSinceEpoch.toString();
  }

  //FLUTTERWAVE
  void handleFlutterWavePayment() async {
    setState(() {
      isLoading = true;
    });

    final response = await PackageOrderService().makePayment(
      orderId: widget.order.id.toString(),
      trnxReference: generateTransactionReference(),
      paymentStatus: true,
      paymentChannel: "Flutterwave",
      paymentType: "flutterwave",
    );
    if (response.statusCode == 200 || response.statusCode == 201) {
      setState(() {
        isLoading = false;
      });
      var data = json.decode(response.body);
      var payment = FlutterwaveResponseModel.fromJson(data);
      navigateBack(context);
      navigateToRoute(
        context,
        PaystackPaymentPage(
          url: payment.data!.link!,
          isPaystack: false,
          isPackageOrder: true,
        ),
      );
      setState(() {
        isLoading = false;
      });
    } else {
      setState(() {
        isLoading = false;
      });
      navigateBack(context);
      showCustomSnackbar(
        context,
        title: "Payment Error",
        content: "Payment could not Completed because${response.body}",
        type: SnackbarType.error,
        isTopPosition: false,
      );
    }
  }
}
