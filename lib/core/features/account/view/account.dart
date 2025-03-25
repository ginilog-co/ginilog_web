// ignore_for_file: public_member_api_docs, sort_constructors_first
// ignore_for_file: library_private_types_in_public_api, use_build_context_synchronously

import 'package:ginilog_customer_app/core/components/helpers/globals.dart';
import 'package:ginilog_customer_app/core/components/utils/app_buttons.dart';
import 'package:ginilog_customer_app/core/components/utils/helper_functions.dart';
import 'package:ginilog_customer_app/core/components/utils/size_config.dart';
import 'package:ginilog_customer_app/core/components/widgets/back_icon.dart';
import 'package:ginilog_customer_app/core/components/widgets/list_tile_widget.dart';
import 'package:ginilog_customer_app/core/features/account/controller/statement_reports_page.dart';
import 'package:ginilog_customer_app/core/features/account/view/about_us.dart';
import 'package:ginilog_customer_app/core/features/account/view/feed_back.dart';
import 'package:ginilog_customer_app/core/features/account/view/personal_detail.dart';
import 'package:ginilog_customer_app/core/features/account/view/privacy_policy_screen.dart';
import 'package:ginilog_customer_app/core/features/account/view/terms_of_service.dart';
import 'package:ginilog_customer_app/core/features/auth/controller/login_controller.dart';
import 'package:ginilog_customer_app/core/features/bookings/state/booking_state.dart';
import 'package:ginilog_customer_app/core/features/account/view/notification_page.dart';
import 'package:ginilog_customer_app/core/features/order_history/states/order_state.dart';

import '../../../components/utils/colors.dart';
import '../../../components/utils/package_export.dart';
import '../../../components/widgets/app_text.dart';
import '../states/account_provider.dart';

class AccountPage extends ConsumerStatefulWidget {
  const AccountPage({
    super.key,
  });

  @override
  _LoginPageState createState() => _LoginPageState();
}

class _LoginPageState extends ConsumerState<AccountPage> {
  String selectedCarType = "";

  late AccountNotifier provider;
  @override
  void initState() {
    super.initState();
    globals.updateHomeLoaded();
    provider = ref.read(accountProvider.notifier);
    provider.getAccount();
    provider.userData;
    final orderProvider = ref.read(packageOrderProvider.notifier);
    final booking = ref.read(bookingProvider.notifier);
    orderProvider.getAllPackageOrderData();
    booking.getAllCustomerBookData();
  }

  @override
  void dispose() {
    super.dispose();
  }

  urlString(String? url) async {
    final link = Uri.parse(url!);
    if (await canLaunchUrl(link)) {
      await launchUrl(link);
    } else {
      throw 'Could not launch $url';
    }
  }

  @override
  Widget build(BuildContext context) {
    final key = GlobalKey<ScaffoldMessengerState>();
    final accountProviderd = ref.read(accountProvider.notifier);
    accountProviderd.getAccount();
    var user = accountProviderd.userData;

    return Container(
        height: MediaQuery.of(context).size.height,
        width: MediaQuery.of(context).size.width,
        color: Colors.white,
        child: Scaffold(
          key: key,
          backgroundColor: AppColors.white,
          appBar: PreferredSize(
              preferredSize: Size.fromHeight(SizeConfig.heightAdjusted(12)),
              child: Padding(
                padding: EdgeInsets.only(top: SizeConfig.heightAdjusted(10)),
                child: const GlobalBackButton(
                    backText: 'My Profile', showBackButton: false),
              )),
          body: SafeArea(
            child: ListView(
              children: [
                Column(crossAxisAlignment: CrossAxisAlignment.start, children: [
                  Padding(
                    padding: const EdgeInsets.only(left: 8.0, right: 8.0),
                    child: Row(
                      spacing: 10,
                      children: [
                        CircleAvatar(
                            backgroundColor: Colors.white,
                            radius: 37,
                            backgroundImage: user!.profilePicture
                                    .toString()
                                    .isEmpty
                                ? const NetworkImage(
                                    "https://upload.wikimedia.org/wikipedia/commons/thumb/d/d3/Microsoft_Account.svg/512px-Microsoft_Account.svg.png?20170218203212")
                                // ignore: unnecessary_null_comparison
                                : user.profilePicture == null
                                    ? const NetworkImage(
                                        "https://upload.wikimedia.org/wikipedia/commons/thumb/d/d3/Microsoft_Account.svg/512px-Microsoft_Account.svg.png?20170218203212")
                                    : NetworkImage(
                                        user.profilePicture.toString())),
                        Column(
                          mainAxisAlignment: MainAxisAlignment.start,
                          crossAxisAlignment: CrossAxisAlignment.start,
                          children: [
                            AppText(
                                isBody: true,
                                text: "${user.firstName} ${user.lastName}",
                                textAlign: TextAlign.start,
                                fontSize: 33,
                                color: AppColors.black,
                                fontStyle: FontStyle.normal,
                                fontWeight: FontWeight.bold),
                            addVerticalSpacing(context, 1),
                            AppText(
                                isBody: true,
                                text: user.email.toString(),
                                textAlign: TextAlign.start,
                                fontSize: 34,
                                color: AppColors.black,
                                fontStyle: FontStyle.normal,
                                fontWeight: FontWeight.w400),
                          ],
                        )
                      ],
                    ),
                  ),
                  addVerticalSpacing(context, 4),
                  Card(
                    color: AppColors.white,
                    shape: RoundedRectangleBorder(),
                    child: CustomListTile(
                      title: "Edit Profile",
                      subtitle: "Update profile details",
                      imageUrl:
                          "assets/images/profile_icon.png", // Replace with actual image
                      onTap: () {
                        navigateToRoute(context, const AccountDetailsPage());
                      },
                    ),
                  ),
                  Card(
                    color: AppColors.white,
                    shape: RoundedRectangleBorder(),
                    child: CustomListTile(
                      title: "Statements & Reports",
                      subtitle:
                          "Download transaction details, orders, deliveries",
                      imageUrl:
                          "assets/images/reports.png", // Replace with actual image
                      onTap: () {
                        navigateToRoute(context, const StatementReportScreen());
                      },
                    ),
                  ),
                  Card(
                    color: AppColors.white,
                    shape: RoundedRectangleBorder(),
                    child: CustomListTile(
                      title: "Notification Settings",
                      subtitle: "mute, unmute, set location & tracking setting",
                      imageUrl:
                          "assets/images/notification.png", // Replace with actual image
                      onTap: () {
                        navigateToRoute(context, const NotificationPage());
                      },
                    ),
                  ),
                  // Card(
                  //   color: AppColors.white,
                  //   shape: RoundedRectangleBorder(),
                  //   child: CustomListTile(
                  //     title: "Card & Bank account settings",
                  //     subtitle: "change cards, delete card details",
                  //     imageUrl:
                  //         "assets/images/wallet_icon.png", // Replace with actual image
                  //     onTap: () {
                  //       //  navigateToRoute(context, const NotificationPage());
                  //     },
                  //   ),
                  // ),
                  Card(
                    color: AppColors.white,
                    shape: RoundedRectangleBorder(),
                    child: CustomListTile(
                      title: "Contact Us",
                      subtitle: "Send us a message or call us",
                      imageUrl:
                          "assets/images/contact_us.png", // Replace with actual image
                      onTap: () {
                        navigateToRoute(context, const FeedbackPage());
                      },
                    ),
                  ),
                  Card(
                    color: AppColors.white,
                    shape: RoundedRectangleBorder(),
                    child: CustomListTile(
                      title: "About Us",
                      subtitle: "know more about us,",
                      imageUrl:
                          "assets/images/about_us.png", // Replace with actual image
                      onTap: () {
                        navigateToRoute(context, AboutUsScreen());
                      },
                    ),
                  ),
                  Card(
                    color: AppColors.white,
                    shape: RoundedRectangleBorder(),
                    child: CustomListTile(
                      title: "Terms of Service",
                      subtitle: "know more about us, terms and conditions",
                      imageUrl:
                          "assets/images/about_us.png", // Replace with actual image
                      onTap: () {
                        navigateToRoute(context, const TermsOfServiceScreen());
                      },
                    ),
                  ),
                  Card(
                    color: AppColors.white,
                    shape: RoundedRectangleBorder(),
                    child: CustomListTile(
                      title: "Privacy Policy",
                      subtitle: "Read our Privacy Policy",
                      imageUrl:
                          "assets/images/about_us.png", // Replace with actual image
                      onTap: () {
                        navigateToRoute(context, const PrivacyPolicyScreen());
                      },
                    ),
                  ),
                  Card(
                    color: AppColors.white,
                    shape: RoundedRectangleBorder(),
                    child: CustomListTile(
                      title: "Log Out",
                      subtitle: "Log out of the App",
                      imageUrl:
                          "assets/images/logout_icon.png", // Replace with actual image
                      onTap: () {
                        showDialog(
                            context: context,
                            barrierDismissible: true,
                            builder: (context) => Dialog(
                                  shape: const RoundedRectangleBorder(),
                                  elevation: 0,
                                  backgroundColor: Colors.transparent,
                                  child: Container(
                                    width: double.infinity,
                                    decoration: BoxDecoration(
                                      color: AppColors.white,
                                      borderRadius: BorderRadius.circular(30),
                                    ),
                                    child: Padding(
                                      padding: const EdgeInsets.symmetric(
                                          horizontal: 20, vertical: 20),
                                      child: Column(
                                        mainAxisAlignment:
                                            MainAxisAlignment.center,
                                        mainAxisSize: MainAxisSize.min,
                                        children: [
                                          addVerticalSpacing(context, 4),
                                          Align(
                                            alignment: Alignment.centerRight,
                                            child: IconButton(
                                                onPressed: () {
                                                  Navigator.of(context)
                                                      .pop(false);
                                                },
                                                icon: Icon(
                                                  Icons.close,
                                                  color: AppColors.primary,
                                                )),
                                          ),
                                          Image.asset(
                                            "assets/images/logout_icon.png",
                                            height: 100,
                                            width: 100,
                                          ),
                                          addVerticalSpacing(context, 4),
                                          Text('Do You want To Log out?',
                                              style: TextStyle(
                                                  color: AppColors.black,
                                                  fontWeight: FontWeight.bold,
                                                  fontFamily: "Inter",
                                                  fontSize:
                                                      fontSized(context, 35))),
                                          addVerticalSpacing(context, 10),
                                          appButton("Log Out",
                                              getScreenWidth(context), () {
                                            accountProviderd.handleSignOut();
                                            navigateAndRemoveUntilRoute(
                                                context, const LoginScreens());
                                          }, AppColors.primary, false),
                                          addVerticalSpacing(context, 4),
                                        ],
                                      ),
                                    ),
                                  ),
                                ));
                      },
                    ),
                  ),
                ]),
              ],
            ),
          ),
        ));
  }
}
