// ignore_for_file: library_private_types_in_public_api, use_build_context_synchronously

import 'dart:convert';

import 'package:ginilog_customer_app/core/components/extension/error_handling.dart';
import 'package:ginilog_customer_app/core/components/helpers/globals.dart';
import 'package:ginilog_customer_app/core/components/utils/app_buttons.dart';
import 'package:ginilog_customer_app/core/components/utils/colors.dart';
import 'package:ginilog_customer_app/core/components/utils/constants.dart';
import 'package:ginilog_customer_app/core/components/utils/helper_functions.dart';
import 'package:ginilog_customer_app/core/components/utils/package_export.dart';
import 'package:ginilog_customer_app/core/components/widgets/app_text.dart';
import 'package:ginilog_customer_app/core/features/account/states/account_provider.dart';
import 'package:ginilog_customer_app/core/features/home/services/home_service.dart';
import 'package:ginilog_customer_app/core/features/home/widget/package_info_page.dart.dart';
import 'package:flutter/foundation.dart';
import 'package:flutter_paystack_max/flutter_paystack_max.dart';

import 'package:http/http.dart' as http;

//Flutterwave
String testKey = "FLWPUBK_TEST-41fc8442444607f9ec28b0a119a4b2d6-X";
String liveKey = "FLWPUBK-54b62cb25ac7f6e70327acd5e086dfc3-X";

// PayStack
String payStackTestKey = "pk_test_6c4465ae9fe18e99f5f898013ef714f727ccc114";
String payStackLiveKey = "pk_live_32106bb22e0ecfe6418d96ad463c38b880eda8c4";

String payStackTestSecretKey =
    "sk_test_7553995f1c8e16443ac75be842e0b55487379b09";
String payStackLiveSecretKey =
    "sk_live_5abaf9269f3671088440b0c7c1c6c5362e26cbbe";

class PaymentMethodWidget extends ConsumerStatefulWidget {
  const PaymentMethodWidget(
      {super.key,
      required this.stationId,
      required this.address,
      required this.phoneNo,
      required this.gasInputkg,
      required this.numberOfCylinders,
      required this.distance,
      required this.prices,
      required this.state,
      required this.city,
      required this.latitude,
      required this.longitude,
      required this.postcodes});

  final String stationId, address, phoneNo;
  final num gasInputkg;
  final int numberOfCylinders;
  final num distance;
  final num prices;
  final String state;
  final String city;
  final num latitude;
  final num longitude;
  final String postcodes;

  @override
  _PaymentMethodWidgetState createState() => _PaymentMethodWidgetState();
}

class _PaymentMethodWidgetState extends ConsumerState<PaymentMethodWidget>
    with WidgetsBindingObserver {
  String paymentMethodUse = "Paystack";
  int selected = 1;
  late AccountNotifier provider;
  bool isLoading = false;
  String? _currentTransactionReference;

  @override
  void initState() {
    super.initState();
    provider = ref.read(accountProvider.notifier);
    provider.getAccount();
    WidgetsBinding.instance.addObserver(this);
  }

  @override
  void dispose() {
    WidgetsBinding.instance.removeObserver(this);
    super.dispose();
  }

  @override
  void didChangeAppLifecycleState(AppLifecycleState state) async {
    if (state == AppLifecycleState.paused) {
      if (kDebugMode) {
        print("App is in the background.");
      }
      if (_currentTransactionReference != null) {
        await saveTransactionDetails(_currentTransactionReference!);
      }
    } else if (state == AppLifecycleState.resumed) {
      if (kDebugMode) {
        print("App is resumed.");
      }
      final pendingTransaction = await getTransactionDetails();
      if (pendingTransaction != null) {
        final isTransactionComplete =
            await verifyTransaction(pendingTransaction);
        if (isTransactionComplete) {
          if (kDebugMode) {
            print("Transaction verified successfully: $pendingTransaction");
          }
          await clearTransactionDetails();
        } else {
          if (kDebugMode) {
            print("Transaction verification failed: $pendingTransaction");
          }
        }
      }
    }
  }

  Future<void> saveTransactionDetails(String reference) async {
    final prefs = await SharedPreferences.getInstance();
    prefs.setString('transaction_reference', reference);
  }

  Future<String?> getTransactionDetails() async {
    final prefs = await SharedPreferences.getInstance();
    return prefs.getString('transaction_reference');
  }

  Future<void> clearTransactionDetails() async {
    final prefs = await SharedPreferences.getInstance();
    prefs.remove('transaction_reference');
  }

  String generateTransactionReference() {
    return DateTime.now().millisecondsSinceEpoch.toString();
  }

  Future<void> sendNotifications(String message) async {
    for (var tokenModel in provider.userData!.deviceTokenModels!) {
      await HomeService().sendNotification(
        title: "Payment Notification",
        body: message,
        notificationType: "Gas Orders",
        deviceToken: tokenModel.deviceTokenId.toString(),
      );
    }
  }

  Future<bool> verifyTransaction(String reference) async {
    try {
      final response = await http.post(
        Uri.parse('https://api.paystack.co/transaction/verify/$reference'),
        headers: {
          'Authorization': 'Bearer $payStackLiveSecretKey',
          'Content-Type': 'application/json',
        },
      );

      if (response.statusCode == 200) {
        final responseBody = jsonDecode(response.body);
        return responseBody['status'] == true &&
            responseBody['data']['status'] == 'success';
      } else {
        logError("Transaction verification failed", response.body);
        return false;
      }
    } catch (e) {
      logError("Error verifying transaction", e);
      return Future.error(handleHttpError(e));
    }
  }

  void logError(String message, [dynamic error]) {
    if (kDebugMode) {
      print("$message: $error");
    }
  }

  Future<void> handleFlutterWavePayment() async {
    final Customer customer = Customer(
      name: globals.userName ?? "Guest",
      phoneNumber: '+234${widget.phoneNo}',
      email: globals.userEmail ?? "guest@example.com",
    );

    final Flutterwave flutterwave = Flutterwave(
      context: context,
      publicKey: liveKey,
      currency: "NGN",
      txRef: generateTransactionReference(),
      amount: widget.prices.toString(),
      redirectUrl: 'https://www.bringmygas.com/',
      customer: customer,
      paymentOptions: "Bank transfer",
      customization: Customization(title: "Gas Payment"),
      isTestMode: false,
    );

    final ChargeResponse response = await flutterwave.charge();

    if (response.status == "completed") {
      setState(() {
        isLoading = true;
      });
      _currentTransactionReference = response.txRef;
      // await PackageOrderService().createOrderWithAddress(
      //     gasStationId: widget.stationId,
      //     gasInputkg: widget.gasInputkg,
      //     numberOfCylinders: widget.numberOfCylinders,
      //     trnxReference: response.txRef!,
      //     paymentStatus: true,
      //     distance: widget.distance,
      //     address: widget.address,
      //     paymentChannel: "Flutterwave",
      //     state: widget.state,
      //     city: widget.city,
      //     postCodes: widget.postcodes,
      //     latitude: widget.latitude,
      //     longitude: widget.longitude);

      await sendNotifications(
          "You have successfully placed an order of ${widget.gasInputkg}Kg gas.");
      setState(() {
        isLoading = false;
      });
      navigateBack(context);
      navigateToRoute(
          context,
          PaymentConfirmationPage(
              totalAmount: widget.prices.toString(),
              cardNumber: "card Number"));
      setState(() {
        isLoading = false;
      });
    } else {
      setState(() {
        isLoading = false;
      });
      navigateBack(context);
      _showMessage("Payment failed: ${response.success}");
    }
  }

  Future<void> handlePaystackPayment() async {
    PaystackFlutter().pay(
      context: context,
      reference: generateTransactionReference(),
      secretKey: payStackLiveSecretKey,
      amount: widget.prices.toInt() * 100,
      email: globals.userEmail.toString(),
      callbackUrl: 'https://callback.com',
      paymentOptions: [
        PaymentOption.bankTransfer,
        PaymentOption.mobileMoney,
        PaymentOption.ussd,
        PaymentOption.bank,
      ],
      currency: Currency.NGN,
      confirmTransaction: true,
      metaData: {
        "product_name": "Gas Payment",
        "product_quantity": widget.numberOfCylinders,
        "product_price": widget.prices
      },
      onSuccess: (paystackCallback) async {
        setState(() {
          isLoading = true;
        });
        _currentTransactionReference = paystackCallback.reference;
        // await PackageOrderService().createOrderWithAddress(
        //     gasStationId: widget.stationId,
        //     gasInputkg: widget.gasInputkg,
        //     numberOfCylinders: widget.numberOfCylinders,
        //     trnxReference: paystackCallback.reference.toString(),
        //     paymentStatus: true,
        //     distance: widget.distance,
        //     address: widget.address,
        //     paymentChannel: "Paystack",
        //     state: widget.state,
        //     city: widget.city,
        //     postCodes: widget.postcodes,
        //     latitude: widget.latitude,
        //     longitude: widget.longitude);

        await sendNotifications(
            "You have successfully placed an order of ${widget.gasInputkg}Kg gas.");
        setState(() {
          isLoading = false;
        });
        navigateBack(context);
        navigateToRoute(
            context,
            PaymentConfirmationPage(
                totalAmount: widget.prices.toString(),
                cardNumber: "card Number"));
        setState(() {
          isLoading = false;
        });
      },
      onCancelled: (paystackCallback) {
        setState(() {
          isLoading = false;
        });
        navigateBack(context);
        _showMessage(
            'Reference: ${paystackCallback.reference} \n Payment cancelled.');
      },
    );
  }

  bool initializingPayment = false;
  void handlePaystackPayment2() async {
    final request = PaystackTransactionRequest(
      reference: generateTransactionReference(),
      secretKey: payStackLiveSecretKey,
      email: globals.userEmail.toString(),
      amount: widget.prices.toDouble() * 100,
      currency: PaystackCurrency.ngn,
      channel: [
        PaystackPaymentChannel.mobileMoney,
        PaystackPaymentChannel.card,
        PaystackPaymentChannel.ussd,
        PaystackPaymentChannel.bankTransfer,
        PaystackPaymentChannel.bank,
        PaystackPaymentChannel.qr,
        PaystackPaymentChannel.eft,
      ],
    );

    setState(() => initializingPayment = true);
    final initializedTransaction =
        await PaymentService.initializeTransaction(request);

    if (!mounted) return;
    setState(() => initializingPayment = false);

    if (!initializedTransaction.status) {
      ScaffoldMessenger.of(context).showSnackBar(SnackBar(
        backgroundColor: Colors.red,
        content: Text(initializedTransaction.message),
      ));

      return;
    }

    await PaymentService.showPaymentModal(
      context,
      transaction: initializedTransaction,
      // Callback URL must match the one specified on your paystack dashboard,
      callbackUrl: 'https://callback.com',
      onClosing: () {
        ScaffoldMessenger.of(context).showSnackBar(SnackBar(
          backgroundColor: Colors.red,
          content: const Text('Payment modal closed'),
        ));
      },
    );

    final response = await PaymentService.verifyTransaction(
      paystackSecretKey: payStackLiveSecretKey,
      initializedTransaction.data?.reference ?? request.reference,
    );
    // if (kDebugMode) {
    //   Logger().i(response.data.status == PaystackTransactionStatus.abandoned);
    // }
    printData("Payment:Test", response.data);
    setState(() {
      isLoading = true;
      initializingPayment = false;
    });
    if (response.data.status == PaystackTransactionStatus.success &&
        response.status == true) {
      // await PackageOrderService().createOrderWithAddress(
      //     gasStationId: widget.stationId,
      //     gasInputkg: widget.gasInputkg,
      //     numberOfCylinders: widget.numberOfCylinders,
      //     trnxReference: response.data.reference.toString(),
      //     paymentStatus: true,
      //     distance: widget.distance,
      //     address: widget.address,
      //     paymentChannel: "Paystack",
      //     state: widget.state,
      //     city: widget.city,
      //     postCodes: widget.postcodes,
      //     latitude: widget.latitude,
      //     longitude: widget.longitude);

      await sendNotifications(
          "You have successfully placed an order of ${widget.gasInputkg}Kg gas.");
      setState(() {
        isLoading = false;
        initializingPayment = false;
      });
      navigateBack(context);
      navigateToRoute(
          context,
          PaymentConfirmationPage(
              totalAmount: widget.prices.toString(),
              cardNumber: "card Number"));
      setState(() {
        isLoading = false;
        initializingPayment = false;
      });
    } else {
      setState(() {
        isLoading = false;
        initializingPayment = false;
      });
      navigateBack(context);
      _showMessage('Reason: ${response.message}');
    }
  }

  _showMessage(String message,
      [Duration duration = const Duration(seconds: 4)]) {
    ScaffoldMessenger.of(context).showSnackBar(SnackBar(
      backgroundColor: AppColors.primaryDark,
      content: Text(message),
      duration: duration,
      action: SnackBarAction(
          label: 'CLOSE',
          onPressed: () =>
              ScaffoldMessenger.of(context).removeCurrentSnackBar()),
    ));
  }

  @override
  Widget build(BuildContext context) {
    return Container(
      height: getScreenHeight(context) / 3,
      padding: const EdgeInsets.all(8),
      alignment: Alignment.center,
      child: !isLoading
          ? Column(
              children: [
                const AppText(
                    isBody: false,
                    text: "Choose your Payment Method",
                    fontSize: 18,
                    color: AppColors.black),
                addVerticalSpacing(context, 35),
                GestureDetector(
                  onTap: () {
                    setState(() {
                      selected = 1;
                      paymentMethodUse = "Flutterwave";
                    });
                  },
                  child: _buildPaymentOption(
                      "Pay with Flutterwave", selected == 1),
                ),
                addVerticalSpacing(context, 34),
                initializingPayment
                    ? const CircularProgressIndicator()
                    : GestureDetector(
                        onTap: () {
                          setState(() {
                            selected = 2;
                            paymentMethodUse = "Paystack";
                          });
                        },
                        child: _buildPaymentOption(
                            "Pay with Paystack", selected == 2),
                      ),
                addVerticalSpacing(context, 35),
                appButton("Continue", getScreenWidth(context), () {
                  if (selected == 1) {
                    handleFlutterWavePayment();
                  } else {
                    handlePaystackPayment2();
                  }
                }, AppColors.primary, false),
              ],
            )
          : const Column(
              mainAxisAlignment: MainAxisAlignment.center,
              children: [
                CircularProgressIndicator(color: AppColors.primary),
                Text("Processing Payment, please wait..."),
              ],
            ),
    );
  }

  Widget _buildPaymentOption(String text, bool isSelected) {
    return Container(
      height: getScreenHeight(context) / 12,
      width: getScreenWidth(context),
      decoration: BoxDecoration(
        color: isSelected ? AppColors.primary : AppColors.white,
        border: Border.all(color: AppColors.primaryDark),
        borderRadius: const BorderRadius.all(Radius.circular(15)),
      ),
      alignment: Alignment.center,
      child: AppText(
        isBody: false,
        text: text,
        fontSize: 16,
        color: isSelected ? AppColors.white : AppColors.black,
      ),
    );
  }
}
