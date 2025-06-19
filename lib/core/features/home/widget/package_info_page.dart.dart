import 'package:ginilog_customer_app/core/components/utils/app_buttons.dart';
import 'package:ginilog_customer_app/core/components/utils/colors.dart';
import 'package:ginilog_customer_app/core/components/utils/helper_functions.dart';
import 'package:ginilog_customer_app/core/components/utils/package_export.dart';
import 'package:ginilog_customer_app/core/components/widgets/app_text.dart';
import 'package:ginilog_customer_app/core/features/home_screen.dart';

class PaymentConfirmationPage extends StatefulWidget {
  const PaymentConfirmationPage({
    super.key,
    required this.totalAmount,
    required this.cardNumber,
    required this.isPackage,
  });

  final String totalAmount;
  final String cardNumber;
  final bool isPackage;

  @override
  // ignore: library_private_types_in_public_api
  _LoginPageState createState() => _LoginPageState();
}

class _LoginPageState extends State<PaymentConfirmationPage> {
  String selectedCarType = "";
  @override
  void initState() {
    super.initState();
  }

  @override
  void dispose() {
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    final key = GlobalKey<ScaffoldMessengerState>();
    double width = MediaQuery.of(context).size.width;
    double height = MediaQuery.of(context).size.height;
    return PopScope(
      canPop: false,
      // ignore: deprecated_member_use
      onPopInvoked: (bool isPop) {
        navigateAndRemoveUntilRoute(
          context,
          HomeScreenPage(imdex: widget.isPackage == true ? 1 : 0),
        );
      },
      child: Container(
        height: height,
        width: width,
        color: Colors.white,
        child: Scaffold(
          key: key,
          backgroundColor: Colors.transparent,
          body: ListView(
            physics: const ScrollPhysics(),
            children: <Widget>[
              Padding(
                padding: const EdgeInsets.only(left: 20, right: 20, top: 60),
                child: Row(
                  children: [
                    IconButton(
                      onPressed: () {
                        navigateAndRemoveUntilRoute(
                          context,
                          const HomeScreenPage(imdex: 1),
                        );
                      },
                      icon: const Icon(
                        Icons.arrow_back_ios,
                        color: AppColors.black,
                        size: 20,
                      ),
                    ),
                    addHorizontalSpacing(20),
                    const AppText(
                      isBody: true,
                      text: "",
                      textAlign: TextAlign.center,
                      fontSize: 79,
                      color: Color.fromRGBO(48, 48, 48, 1),
                      fontStyle: FontStyle.normal,
                      fontWeight: FontWeight.w700,
                    ),
                  ],
                ),
              ),
              Container(
                padding: const EdgeInsets.only(left: 20, right: 20),
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.center,
                  children: [
                    addVerticalSpacing(context, 40),
                    Image.asset(
                      "assets/images/successful.png",
                      height: 65,
                      width: 65,
                      //color: AppColors.black,
                    ),
                    addVerticalSpacing(context, 4),
                    const AppText(
                      isBody: true,
                      text: "Thank you for using our service",
                      textAlign: TextAlign.center,
                      fontSize: 80,
                      color: Color.fromRGBO(48, 48, 48, 1),
                      fontStyle: FontStyle.normal,
                      fontWeight: FontWeight.w500,
                    ),
                    addVerticalSpacing(context, 4),
                    const AppText(
                      isBody: true,
                      text: "Your payment has been successful",
                      textAlign: TextAlign.center,
                      fontSize: 76,
                      color: Color.fromRGBO(48, 48, 48, 1),
                      fontStyle: FontStyle.normal,
                      fontWeight: FontWeight.w400,
                    ),
                    addVerticalSpacing(context, 10),
                    AppButton(
                      onPressed: () async {
                        navigateAndRemoveUntilRoute(
                          context,
                          HomeScreenPage(
                            imdex: widget.isPackage == true ? 1 : 0,
                          ),
                        );
                      },

                      text: "Back to Home",
                      widthPercent: 100,
                      heightPercent: 6,
                      btnColor: AppColors.primary,
                      isLoading: false,
                    ),
                    addVerticalSpacing(context, 4),
                  ],
                ),
              ),
            ],
          ),
        ),
      ),
    );
  }
}
