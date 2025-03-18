// ignore_for_file: library_private_types_in_public_api, use_build_context_synchronously

import 'package:ginilog_customer_app/core/components/helpers/endpoints.dart';
import 'package:ginilog_customer_app/core/components/helpers/globals.dart';
import 'package:ginilog_customer_app/core/components/utils/app_buttons.dart';
import 'package:ginilog_customer_app/core/components/utils/colors.dart';
import 'package:ginilog_customer_app/core/components/utils/constants.dart';
import 'package:ginilog_customer_app/core/components/utils/helper_functions.dart';
import 'package:ginilog_customer_app/core/components/utils/package_export.dart';
import 'package:ginilog_customer_app/core/components/widgets/app_text.dart';
import 'package:ginilog_customer_app/core/components/widgets/custom_snackbar.dart';
import 'package:ginilog_customer_app/core/features/home/widget/package_info_page.dart.dart';
import 'package:ginilog_customer_app/core/features/order_history/model/package_orders_model.dart';
import 'package:ginilog_customer_app/core/features/order_history/services/package_order_service.dart';
import 'package:paystack_for_flutter/paystack_for_flutter.dart';

class PaymentMethodBottomSheet extends StatefulWidget {
  const PaymentMethodBottomSheet({
    super.key,
    required this.order,
  });
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
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(content: Text("Please select a payment method")),
      );
    }
  }

  @override
  Widget build(BuildContext context) {
    return Container(
      width: double.infinity,
      padding: EdgeInsets.only(top: 10, left: 8, right: 8),
      height: getScreenHeight(context) * 0.45,
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
                  fontSize: 70,
                  color: AppColors.black,
                  fontStyle: FontStyle.normal,
                  fontWeight: FontWeight.w800),
              addHorizontalSpacing(10),
              const Divider(),
              addHorizontalSpacing(10),
              GestureDetector(
                onTap: () => Navigator.pop(context),
                child: const SizedBox(
                    height: 30, width: 30, child: Icon(Icons.close)),
              )
            ],
          ),
          addVerticalSpacing(context, 1.2),
          const Divider(),

          addVerticalSpacing(context, 5.2),

          _buildPaymentOption("Pay with Paystack", 1),
          SizedBox(height: 15),
          _buildPaymentOption("Pay with Flutterwave", 2),
          addVerticalSpacing(context, 10.2),
          appButton("Continue", getScreenWidth(context), handlePayment,
              AppColors.primary, isLoading),
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
            fontSize: 16,
            color: selected == value ? Colors.white : Colors.black,
          ),
        ),
      ),
    );
  }

// PAYSTACK

// FLUTTERWAVE

  void handlePaystackPayment() {
    PaystackFlutter().pay(
      context: context,
      secretKey: Endpoints
          .paystackSecretKey, // Your Paystack secret key gotten from your Paystack dashboard.
      amount: ((widget.order.shippingCost! + widget.order.vatCost!.toDouble()) *
          100), // The amount to be charged in the smallest currency unit. If amount is 600, multiply by 100(600*100)
      email: globals.userEmail ??
          widget.order.senderEmail!, // The customer's email address.
      callbackUrl:
          'https://callback.com', // The URL to which Paystack will redirect the user after the transaction.
      showProgressBar:
          true, // If true, it shows progress bar to inform user an action is in progress when getting checkout link from Paystack.
      paymentOptions: [
        PaymentOption.card,
        PaymentOption.bankTransfer,
        PaymentOption.mobileMoney,
        PaymentOption.ussd,
        PaymentOption.eft,
        PaymentOption.bank
      ],
      currency: Currency.NGN,
      metaData: {
        "product_name": "Nike Sneakers",
        "product_quantity": 3,
        "product_price": 24000
      }, // Additional metadata to be associated with the transaction
      onSuccess: (paystackCallback) async {
        setState(() {
          isLoading = true;
        });
        final response2 = await PackageOrderService().makePayment(
          orderId: widget.order.id.toString(),
          trnxReference: paystackCallback.reference,
          paymentStatus: true,
          paymentChannel: "Paystack",
        );
        if (response2.statusCode == 200 || response2.statusCode == 201) {
// await sendNotifications(
          //     "You have successfully placed an order of ${widget}Kg gas.");
          setState(() {
            isLoading = false;
          });
          navigateToRoute(
              context,
              PaymentConfirmationPage(
                  isPackage: true,
                  totalAmount: widget.order.shippingCost.toString(),
                  cardNumber: "card Number"));
          setState(() {
            isLoading = false;
          });
        } else {
          setState(() {
            isLoading = false;
          });
          showCustomSnackbar(context,
              title: "Order Error",
              content: "Order could not Completed because${response2.body}",
              type: SnackbarType.error,
              isTopPosition: false);
        }
        // generateOrderNumber();
      }, // A callback function to be called when the payment is successful.
      onCancelled: (paystackCallback) {
        setState(() {
          isLoading = false;
        });
        showCustomSnackbar(context,
            title: "Payment Error",
            content: "The payment could not be process",
            type: SnackbarType.error,
            isTopPosition: false);
      }, // A callback function to be called when the payment is canceled.
    );
  }

  String generateTransactionReference() {
    return DateTime.now().millisecondsSinceEpoch.toString();
  }

  Future<void> handleFlutterWavePayment() async {
    final Customer customer = Customer(
      name: "${widget.order.senderName}",
      phoneNumber: "${widget.order.senderPhoneNo}",
      email: globals.userEmail ?? "guest@example.com",
    );

    final Flutterwave flutterwave = Flutterwave(
      context: context,
      publicKey: Endpoints.flutterWaveKey,
      currency: "NGN",
      txRef: generateTransactionReference(),
      amount: (widget.order.shippingCost! + widget.order.vatCost!).toString(),
      redirectUrl: 'https://www.ginilog.com/',
      customer: customer,
      paymentOptions: "Bank transfer",
      customization: Customization(title: "Gas Payment"),
      isTestMode: false,
    );

    final ChargeResponse response = await flutterwave.charge();
    printData("Payment Response", response);
    if (response.status == "successful") {
      setState(() {
        isLoading = true;
      });
      final response2 = await PackageOrderService().makePayment(
        orderId: widget.order.id.toString(),
        trnxReference: response.txRef!,
        paymentStatus: true,
        paymentChannel: "Flutterwave",
      );
      if (response2.statusCode == 200 || response2.statusCode == 201) {
// await sendNotifications(
        //     "You have successfully placed an order of ${widget}Kg gas.");
        setState(() {
          isLoading = false;
        });
        navigateToRoute(
            context,
            PaymentConfirmationPage(
                isPackage: true,
                totalAmount: widget.order.shippingCost.toString(),
                cardNumber: "card Number"));
        setState(() {
          isLoading = false;
        });
      } else {
        setState(() {
          isLoading = false;
        });
        showCustomSnackbar(context,
            title: "Order Error",
            content: "Order could not Completed because${response2.body}",
            type: SnackbarType.error,
            isTopPosition: false);
      }
    } else {
      setState(() {
        isLoading = false;
      });
      showCustomSnackbar(context,
          title: "Payment Error",
          content: "The payment could not be process",
          type: SnackbarType.error,
          isTopPosition: false);
    }
  }
}
