import 'package:ginilog_customer_app/core/components/utils/constants.dart';
import 'package:ginilog_customer_app/core/components/utils/helper_functions.dart';
import 'package:ginilog_customer_app/core/components/utils/package_export.dart';
import 'package:ginilog_customer_app/core/features/home/widget/package_info_page.dart.dart';

class PaystackPaymentPage extends ConsumerStatefulWidget {
  final String url;
  final bool isPackageOrder;
  final bool isPaystack;
  const PaystackPaymentPage({
    required this.url,
    super.key,
    this.isPackageOrder = false,
    required this.isPaystack,
  });

  @override
  ConsumerState<PaystackPaymentPage> createState() =>
      _PaystackPaymentPageState();
}

class _PaystackPaymentPageState extends ConsumerState<PaystackPaymentPage> {
  late final WebViewController _controller;
  String paymentUrl = "";
  bool isLoading = true;

  @override
  void initState() {
    paymentUrl = widget.url;
    _controller =
        WebViewController()
          ..loadRequest(Uri.parse(widget.url))
          ..setJavaScriptMode(JavaScriptMode.unrestricted)
          ..setNavigationDelegate(
            NavigationDelegate(
              onNavigationRequest: (NavigationRequest request) {
                final uri = Uri.parse(request.url);
                printData("identifier", uri);
                printData("identifier", uri.host);
                if (widget.isPaystack == true) {
                  if (uri.host == "api-data.ginilog.com" &&
                      (uri.path.contains("paystack-redirect") ||
                          uri.path.contains("delete-accomodation"))) {
                    navigateBack(context);
                    return NavigationDecision.prevent;
                  } else {
                    navigateToRoute(
                      context,
                      PaymentConfirmationPage(
                        totalAmount: uri.queryParameters['totalAmount'] ?? "0",
                        cardNumber: "card Number",
                        isPackage: widget.isPackageOrder,
                      ),
                    );
                    // return NavigationDecision.prevent;
                  }
                  return NavigationDecision.navigate;
                } else {
                  if (uri.host == "api-data.ginilog.com" &&
                      uri.path.contains("flutterwave-redirect")) {
                    navigateBack(context);
                    return NavigationDecision.prevent;
                  } else {
                    navigateToRoute(
                      context,
                      PaymentConfirmationPage(
                        totalAmount: uri.queryParameters['totalAmount'] ?? "0",
                        cardNumber: "card Number",
                        isPackage: widget.isPackageOrder,
                      ),
                    );
                    // return NavigationDecision.prevent;
                  }
                  return NavigationDecision.navigate;
                }
              },
            ),
          );
    super.initState();
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: const Text('Paystack Payment')),
      body: WebViewWidget(controller: _controller),
    );
  }
}
