// ignore_for_file: public_member_api_docs, sort_constructors_first
// ignore_for_file: library_private_types_in_public_api, use_build_context_synchronously

import 'package:ginilog_customer_app/core/components/utils/size_config.dart';
import 'package:ginilog_customer_app/core/components/widgets/back_icon.dart';

import '../../../components/utils/colors.dart';
import '../../../components/utils/package_export.dart';
import '../../../components/widgets/app_text.dart';

class HelpAndSupportPage extends StatefulWidget {
  const HelpAndSupportPage({super.key});

  @override
  _LoginPageState createState() => _LoginPageState();
}

class _LoginPageState extends State<HelpAndSupportPage> {
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
    return Scaffold(
      appBar: PreferredSize(
        preferredSize: Size.fromHeight(SizeConfig.heightAdjusted(16)),
        child: Padding(
          padding: EdgeInsets.only(top: SizeConfig.heightAdjusted(10)),
          child: const Column(
            children: [
              GlobalBackButton(
                backText: 'Help and Support',
                showBackButton: true,
              ),
            ],
          ),
        ),
      ),
      key: key,
      backgroundColor: Colors.white,
      body: Container(
        height: MediaQuery.of(context).size.height,
        width: MediaQuery.of(context).size.width,
        color: AppColors.white,
        child: ListView(
          physics: const ScrollPhysics(),
          children: [
            addVerticalSpacing(20),
            const Padding(
              padding: EdgeInsets.only(left: 10.0, right: 10),
              child: AppText(
                isBody: false,
                text: "FAQ",
                textAlign: TextAlign.start,
                fontSize: 13,
                color: AppColors.black,
                fontStyle: FontStyle.normal,
                fontWeight: FontWeight.w600,
              ),
            ),
            addVerticalSpacing(20),
            ExpandableNotifier(
              child: Padding(
                padding: const EdgeInsets.all(0),
                child: Container(
                  color: AppColors.white,
                  child: Column(
                    children: <Widget>[
                      ScrollOnExpand(
                        scrollOnExpand: true,
                        scrollOnCollapse: false,
                        child: ExpandablePanel(
                          theme: const ExpandableThemeData(
                            headerAlignment:
                                ExpandablePanelHeaderAlignment.center,
                            tapBodyToCollapse: true,
                          ),
                          header: const Padding(
                            padding: EdgeInsets.all(10),
                            child: AppText(
                              isBody: false,
                              text: "How does BMG Work?",
                              textAlign: TextAlign.start,
                              fontSize: 15,
                              color: AppColors.black,
                              fontStyle: FontStyle.normal,
                              fontWeight: FontWeight.w700,
                            ),
                          ),
                          collapsed: Text(
                            "All you need to do is create an account",
                            softWrap: true,
                            maxLines: 2,
                            overflow: TextOverflow.ellipsis,
                            style: TextStyle(
                              color: AppColors.black,
                              fontSize: 15.textSize,
                              fontWeight: FontWeight.w700,
                              fontFamily: "Mulish",
                            ),
                          ),
                          expanded: Column(
                            crossAxisAlignment: CrossAxisAlignment.start,
                            children: <Widget>[
                              for (var _ in Iterable.generate(1))
                                Padding(
                                  padding: const EdgeInsets.only(bottom: 10),
                                  child: Text(
                                    "All you need to do is create an account, then input your details and you are good to go.",
                                    softWrap: true,
                                    overflow: TextOverflow.fade,
                                    style: TextStyle(
                                      color: AppColors.black,
                                      fontSize: 15.textSize,
                                      fontWeight: FontWeight.w700,
                                      fontFamily: "Mulish",
                                    ),
                                  ),
                                ),
                            ],
                          ),
                          builder: (_, collapsed, expanded) {
                            return Padding(
                              padding: const EdgeInsets.only(
                                left: 10,
                                right: 10,
                                bottom: 10,
                              ),
                              child: Expandable(
                                collapsed: collapsed,
                                expanded: expanded,
                                theme: const ExpandableThemeData(
                                  crossFadePoint: 0,
                                ),
                              ),
                            );
                          },
                        ),
                      ),
                    ],
                  ),
                ),
              ),
            ),
            addVerticalSpacing(20),
            ExpandableNotifier(
              child: Padding(
                padding: const EdgeInsets.all(0),
                child: Container(
                  color: AppColors.white,
                  child: Column(
                    children: <Widget>[
                      ScrollOnExpand(
                        scrollOnExpand: true,
                        scrollOnCollapse: false,
                        child: ExpandablePanel(
                          theme: const ExpandableThemeData(
                            headerAlignment:
                                ExpandablePanelHeaderAlignment.center,
                            tapBodyToCollapse: true,
                          ),
                          header: const Padding(
                            padding: EdgeInsets.all(10),
                            child: AppText(
                              isBody: false,
                              text:
                                  "How do i select the right logistics company?",
                              textAlign: TextAlign.start,
                              fontSize: 15,
                              color: AppColors.black,
                              fontStyle: FontStyle.normal,
                              fontWeight: FontWeight.w700,
                            ),
                          ),
                          collapsed: Text(
                            "Logistics Companys are Listed in the App, you can select the logistics company you want, we have verified, you can select any logistics company you want.",
                            softWrap: true,
                            maxLines: 2,
                            overflow: TextOverflow.ellipsis,
                            style: TextStyle(
                              color: AppColors.black,
                              fontSize: 15.textSize,
                              fontWeight: FontWeight.w700,
                              fontFamily: "Mulish",
                            ),
                          ),
                          expanded: Column(
                            crossAxisAlignment: CrossAxisAlignment.start,
                            children: <Widget>[
                              for (var _ in Iterable.generate(1))
                                Padding(
                                  padding: const EdgeInsets.only(bottom: 10),
                                  child: Text(
                                    "Logistics Companys are Listed in the App, you can select the logistics company you want, we have verified, you can select any logistics company you want.",
                                    softWrap: true,
                                    overflow: TextOverflow.fade,
                                    style: TextStyle(
                                      color: AppColors.black,
                                      fontWeight: FontWeight.w700,
                                      fontSize: 15.textSize,
                                      fontFamily: "Mulish",
                                    ),
                                  ),
                                ),
                            ],
                          ),
                          builder: (_, collapsed, expanded) {
                            return Padding(
                              padding: const EdgeInsets.only(
                                left: 10,
                                right: 10,
                                bottom: 10,
                              ),
                              child: Expandable(
                                collapsed: collapsed,
                                expanded: expanded,
                                theme: const ExpandableThemeData(
                                  crossFadePoint: 0,
                                ),
                              ),
                            );
                          },
                        ),
                      ),
                    ],
                  ),
                ),
              ),
            ),
            addVerticalSpacing(20),
            ExpandableNotifier(
              child: Padding(
                padding: const EdgeInsets.all(0),
                child: Container(
                  color: AppColors.white,
                  child: Column(
                    children: <Widget>[
                      ScrollOnExpand(
                        scrollOnExpand: true,
                        scrollOnCollapse: false,
                        child: ExpandablePanel(
                          theme: const ExpandableThemeData(
                            headerAlignment:
                                ExpandablePanelHeaderAlignment.center,
                            tapBodyToCollapse: true,
                          ),
                          header: const Padding(
                            padding: EdgeInsets.all(10),
                            child: AppText(
                              isBody: false,
                              text:
                                  "How long will it take my gas to be delivered?",
                              textAlign: TextAlign.start,
                              fontSize: 15,
                              color: AppColors.black,
                              fontStyle: FontStyle.normal,
                              fontWeight: FontWeight.w700,
                            ),
                          ),
                          collapsed: Text(
                            "It depends on the distance from your house",
                            softWrap: true,
                            maxLines: 2,
                            overflow: TextOverflow.ellipsis,
                            style: TextStyle(
                              color: AppColors.black,
                              fontSize: 15.textSize,
                              fontWeight: FontWeight.w700,
                              fontFamily: "Mulish",
                            ),
                          ),
                          expanded: Column(
                            crossAxisAlignment: CrossAxisAlignment.start,
                            children: <Widget>[
                              for (var _ in Iterable.generate(1))
                                Padding(
                                  padding: const EdgeInsets.only(bottom: 10),
                                  child: Text(
                                    "It depends on the distance from your house to the logistics company . The closer the selected location to you , the faster the delivery",
                                    softWrap: true,
                                    overflow: TextOverflow.fade,
                                    style: TextStyle(
                                      color: AppColors.black,
                                      fontSize: 15.textSize,
                                      fontWeight: FontWeight.w700,
                                      fontFamily: "Mulish",
                                    ),
                                  ),
                                ),
                            ],
                          ),
                          builder: (_, collapsed, expanded) {
                            return Padding(
                              padding: const EdgeInsets.only(
                                left: 10,
                                right: 10,
                                bottom: 10,
                              ),
                              child: Expandable(
                                collapsed: collapsed,
                                expanded: expanded,
                                theme: const ExpandableThemeData(
                                  crossFadePoint: 0,
                                ),
                              ),
                            );
                          },
                        ),
                      ),
                    ],
                  ),
                ),
              ),
            ),
            addVerticalSpacing(20),
            ExpandableNotifier(
              child: Padding(
                padding: const EdgeInsets.all(0),
                child: Container(
                  color: AppColors.white,
                  child: Column(
                    children: <Widget>[
                      ScrollOnExpand(
                        scrollOnExpand: true,
                        scrollOnCollapse: false,
                        child: ExpandablePanel(
                          theme: const ExpandableThemeData(
                            headerAlignment:
                                ExpandablePanelHeaderAlignment.center,
                            tapBodyToCollapse: true,
                          ),
                          header: const Padding(
                            padding: EdgeInsets.all(10),
                            child: AppText(
                              isBody: false,
                              text:
                                  "How can I trust that my gas is complete as requested?",
                              textAlign: TextAlign.start,
                              fontSize: 15,
                              color: AppColors.black,
                              fontStyle: FontStyle.normal,
                              fontWeight: FontWeight.w700,
                            ),
                          ),
                          collapsed: Text(
                            "Our rider must return the receipt of purchase",
                            softWrap: true,
                            maxLines: 2,
                            overflow: TextOverflow.ellipsis,
                            style: TextStyle(
                              color: AppColors.black,
                              fontSize: 15.textSize,
                              fontWeight: FontWeight.w700,
                              fontFamily: "Mulish",
                            ),
                          ),
                          expanded: Column(
                            crossAxisAlignment: CrossAxisAlignment.start,
                            children: <Widget>[
                              for (var _ in Iterable.generate(1))
                                Padding(
                                  padding: const EdgeInsets.only(bottom: 10),
                                  child: Text(
                                    "Our rider must return the receipt of purchase or a video recording for you to verify",
                                    softWrap: true,
                                    overflow: TextOverflow.fade,
                                    style: TextStyle(
                                      color: AppColors.black,
                                      fontSize: 15.textSize,
                                      fontWeight: FontWeight.w700,
                                      fontFamily: "Mulish",
                                    ),
                                  ),
                                ),
                            ],
                          ),
                          builder: (_, collapsed, expanded) {
                            return Padding(
                              padding: const EdgeInsets.only(
                                left: 10,
                                right: 10,
                                bottom: 10,
                              ),
                              child: Expandable(
                                collapsed: collapsed,
                                expanded: expanded,
                                theme: const ExpandableThemeData(
                                  crossFadePoint: 0,
                                ),
                              ),
                            );
                          },
                        ),
                      ),
                    ],
                  ),
                ),
              ),
            ),
            addVerticalSpacing(20),
            ExpandableNotifier(
              child: Padding(
                padding: const EdgeInsets.all(0),
                child: Container(
                  color: AppColors.white,
                  child: Column(
                    children: <Widget>[
                      ScrollOnExpand(
                        scrollOnExpand: true,
                        scrollOnCollapse: false,
                        child: ExpandablePanel(
                          theme: const ExpandableThemeData(
                            headerAlignment:
                                ExpandablePanelHeaderAlignment.center,
                            tapBodyToCollapse: true,
                          ),
                          header: const Padding(
                            padding: EdgeInsets.all(10),
                            child: AppText(
                              isBody: false,
                              text: "How can I make a payment for my order?",
                              textAlign: TextAlign.start,
                              fontSize: 15,
                              color: AppColors.black,
                              fontStyle: FontStyle.normal,
                              fontWeight: FontWeight.w700,
                            ),
                          ),
                          collapsed: Text(
                            "You can either make a payment by bank transfer or",
                            softWrap: true,
                            maxLines: 2,
                            overflow: TextOverflow.ellipsis,
                            style: TextStyle(
                              color: AppColors.black,
                              fontSize: 15.textSize,
                              fontWeight: FontWeight.w700,
                              fontFamily: "Mulish",
                            ),
                          ),
                          expanded: Column(
                            crossAxisAlignment: CrossAxisAlignment.start,
                            children: <Widget>[
                              for (var _ in Iterable.generate(1))
                                Padding(
                                  padding: const EdgeInsets.only(bottom: 10),
                                  child: Text(
                                    "You can either make a payment by bank transfer or by  card in the app via our secured payment gateway",
                                    softWrap: true,
                                    overflow: TextOverflow.fade,
                                    style: TextStyle(
                                      color: AppColors.black,
                                      fontSize: 15.textSize,
                                      fontWeight: FontWeight.w700,
                                      fontFamily: "Mulish",
                                    ),
                                  ),
                                ),
                            ],
                          ),
                          builder: (_, collapsed, expanded) {
                            return Padding(
                              padding: const EdgeInsets.only(
                                left: 10,
                                right: 10,
                                bottom: 10,
                              ),
                              child: Expandable(
                                collapsed: collapsed,
                                expanded: expanded,
                                theme: const ExpandableThemeData(
                                  crossFadePoint: 0,
                                ),
                              ),
                            );
                          },
                        ),
                      ),
                    ],
                  ),
                ),
              ),
            ),
            addVerticalSpacing(20),
            ExpandableNotifier(
              child: Padding(
                padding: const EdgeInsets.all(0),
                child: Container(
                  color: AppColors.white,
                  child: Column(
                    children: <Widget>[
                      ScrollOnExpand(
                        scrollOnExpand: true,
                        scrollOnCollapse: false,
                        child: ExpandablePanel(
                          theme: const ExpandableThemeData(
                            headerAlignment:
                                ExpandablePanelHeaderAlignment.center,
                            tapBodyToCollapse: true,
                          ),
                          header: const Padding(
                            padding: EdgeInsets.all(10),
                            child: AppText(
                              isBody: false,
                              text: "How can I make an order?",
                              textAlign: TextAlign.start,
                              fontSize: 15,
                              color: AppColors.black,
                              fontStyle: FontStyle.normal,
                              fontWeight: FontWeight.w700,
                            ),
                          ),
                          collapsed: Text(
                            "Ensure you have selected the correct location ",
                            softWrap: true,
                            maxLines: 2,
                            overflow: TextOverflow.ellipsis,
                            style: TextStyle(
                              color: AppColors.black,
                              fontSize: 15.textSize,
                              fontWeight: FontWeight.w700,
                              fontFamily: "Mulish",
                            ),
                          ),
                          expanded: Column(
                            crossAxisAlignment: CrossAxisAlignment.start,
                            children: <Widget>[
                              for (var _ in Iterable.generate(1))
                                Padding(
                                  padding: const EdgeInsets.only(bottom: 10),
                                  child: Text(
                                    "Ensure you have selected the correct location in the app . If you still can’t find your preferred logistics company, it means they have not been registered on our app. Please encourage them to contact us to be added on our app",
                                    softWrap: true,
                                    overflow: TextOverflow.fade,
                                    style: TextStyle(
                                      color: AppColors.black,
                                      fontSize: 15.textSize,
                                      fontWeight: FontWeight.w700,
                                      fontFamily: "Mulish",
                                    ),
                                  ),
                                ),
                            ],
                          ),
                          builder: (_, collapsed, expanded) {
                            return Padding(
                              padding: const EdgeInsets.only(
                                left: 10,
                                right: 10,
                                bottom: 10,
                              ),
                              child: Expandable(
                                collapsed: collapsed,
                                expanded: expanded,
                                theme: const ExpandableThemeData(
                                  crossFadePoint: 0,
                                ),
                              ),
                            );
                          },
                        ),
                      ),
                    ],
                  ),
                ),
              ),
            ),
            addVerticalSpacing(20),
            ExpandableNotifier(
              child: Padding(
                padding: const EdgeInsets.all(0),
                child: Container(
                  color: AppColors.white,
                  child: Column(
                    children: <Widget>[
                      ScrollOnExpand(
                        scrollOnExpand: true,
                        scrollOnCollapse: false,
                        child: ExpandablePanel(
                          theme: const ExpandableThemeData(
                            headerAlignment:
                                ExpandablePanelHeaderAlignment.center,
                            tapBodyToCollapse: true,
                          ),
                          header: const Padding(
                            padding: EdgeInsets.all(10),
                            child: AppText(
                              isBody: false,
                              text: "How can I make a complaint about a rider?",
                              textAlign: TextAlign.start,
                              fontSize: 15,
                              color: AppColors.black,
                              fontStyle: FontStyle.normal,
                              fontWeight: FontWeight.w700,
                            ),
                          ),
                          collapsed: Text(
                            "You can report the rider on the app in",
                            softWrap: true,
                            maxLines: 2,
                            overflow: TextOverflow.ellipsis,
                            style: TextStyle(
                              color: AppColors.black,
                              fontSize: 15.textSize,
                              fontWeight: FontWeight.w700,
                              fontFamily: "Mulish",
                            ),
                          ),
                          expanded: Column(
                            crossAxisAlignment: CrossAxisAlignment.start,
                            children: <Widget>[
                              for (var _ in Iterable.generate(1))
                                Padding(
                                  padding: const EdgeInsets.only(bottom: 10),
                                  child: Text(
                                    "You can report the rider on the app in the review section  or you can contact our office directly on our phone lines",
                                    softWrap: true,
                                    overflow: TextOverflow.fade,
                                    style: TextStyle(
                                      color: AppColors.black,
                                      fontSize: 15.textSize,
                                      fontWeight: FontWeight.w700,
                                      fontFamily: "Mulish",
                                    ),
                                  ),
                                ),
                            ],
                          ),
                          builder: (_, collapsed, expanded) {
                            return Padding(
                              padding: const EdgeInsets.only(
                                left: 10,
                                right: 10,
                                bottom: 10,
                              ),
                              child: Expandable(
                                collapsed: collapsed,
                                expanded: expanded,
                                theme: const ExpandableThemeData(
                                  crossFadePoint: 0,
                                ),
                              ),
                            );
                          },
                        ),
                      ),
                    ],
                  ),
                ),
              ),
            ),
            addVerticalSpacing(20),
            ExpandableNotifier(
              child: Padding(
                padding: const EdgeInsets.all(0),
                child: Container(
                  color: AppColors.white,
                  child: Column(
                    children: <Widget>[
                      ScrollOnExpand(
                        scrollOnExpand: true,
                        scrollOnCollapse: false,
                        child: ExpandablePanel(
                          theme: const ExpandableThemeData(
                            headerAlignment:
                                ExpandablePanelHeaderAlignment.center,
                            tapBodyToCollapse: true,
                          ),
                          header: const Padding(
                            padding: EdgeInsets.all(10),
                            child: AppText(
                              isBody: false,
                              text: "How can I track my order?",
                              textAlign: TextAlign.start,
                              fontSize: 15,
                              color: AppColors.black,
                              fontStyle: FontStyle.normal,
                              fontWeight: FontWeight.w700,
                            ),
                          ),
                          collapsed: Text(
                            "You can track your order on the track",
                            softWrap: true,
                            maxLines: 2,
                            overflow: TextOverflow.ellipsis,
                            style: TextStyle(
                              color: AppColors.black,
                              fontSize: 15.textSize,
                              fontWeight: FontWeight.w700,
                              fontFamily: "Mulish",
                            ),
                          ),
                          expanded: Column(
                            crossAxisAlignment: CrossAxisAlignment.start,
                            children: <Widget>[
                              for (var _ in Iterable.generate(1))
                                Padding(
                                  padding: const EdgeInsets.only(bottom: 10),
                                  child: Text(
                                    "You can track your order on the track order section in the app",
                                    softWrap: true,
                                    overflow: TextOverflow.fade,
                                    style: TextStyle(
                                      color: AppColors.black,
                                      fontSize: 15.textSize,
                                      fontWeight: FontWeight.w700,
                                      fontFamily: "Mulish",
                                    ),
                                  ),
                                ),
                            ],
                          ),
                          builder: (_, collapsed, expanded) {
                            return Padding(
                              padding: const EdgeInsets.only(
                                left: 10,
                                right: 10,
                                bottom: 10,
                              ),
                              child: Expandable(
                                collapsed: collapsed,
                                expanded: expanded,
                                theme: const ExpandableThemeData(
                                  crossFadePoint: 0,
                                ),
                              ),
                            );
                          },
                        ),
                      ),
                    ],
                  ),
                ),
              ),
            ),
          ],
        ),
      ),
    );
  }
}
