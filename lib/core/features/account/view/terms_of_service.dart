// ignore_for_file: library_private_types_in_public_api

import 'package:ginilog_customer_app/core/components/utils/colors.dart';
import 'package:ginilog_customer_app/core/components/utils/helper_functions.dart';
import 'package:ginilog_customer_app/core/components/utils/package_export.dart';
import 'package:ginilog_customer_app/core/components/utils/size_config.dart';
import 'package:ginilog_customer_app/core/components/widgets/app_text.dart';
import 'package:ginilog_customer_app/core/components/widgets/back_icon.dart';

class TermsOfServiceScreen extends StatefulWidget {
  const TermsOfServiceScreen({
    super.key,
  });

  @override
  _LoginPageState createState() => _LoginPageState();
}

class _LoginPageState extends State<TermsOfServiceScreen> {
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
          preferredSize: Size.fromHeight(SizeConfig.heightAdjusted(12)),
          child: Padding(
            padding: EdgeInsets.only(top: SizeConfig.heightAdjusted(10)),
            child: GlobalBackButton(
                backText: "Terms of Service", showBackButton: true),
          )),
      key: key,
      backgroundColor: Colors.white,
      body: Container(
        height: MediaQuery.of(context).size.height,
        width: MediaQuery.of(context).size.width,
        color: AppColors.white,
        child: ListView(
          physics: const ScrollPhysics(),
          children: [
            const Padding(
              padding: EdgeInsets.only(left: 10.0, right: 10),
              child: AppText(
                  isBody: false,
                  text: "Terms of Services",
                  textAlign: TextAlign.start,
                  fontSize: 33,
                  color: AppColors.black,
                  fontStyle: FontStyle.normal,
                  fontWeight: FontWeight.bold),
            ),
            addVerticalSpacing(context, 5),
            const Padding(
              padding: EdgeInsets.only(left: 10.0, right: 10),
              child: AppText(
                  isBody: false,
                  text: "Acceptance of Terms",
                  textAlign: TextAlign.start,
                  fontSize: 33,
                  color: AppColors.black,
                  fontStyle: FontStyle.normal,
                  fontWeight: FontWeight.w800),
            ),
            const Padding(
              padding: EdgeInsets.only(left: 10.0, right: 10),
              child: AppText(
                  isBody: true,
                  text:
                      "By accessing and using the services provided by GINILOG, you agree to comply with and be bound by the terms and conditions outlined in this document. If you do not agree with any part of these terms, you must discontinue use of our services immediately.",
                  textAlign: TextAlign.start,
                  fontSize: 33,
                  color: AppColors.black,
                  fontStyle: FontStyle.normal,
                  fontWeight: FontWeight.w400),
            ),
            addVerticalSpacing(context, 5),
            const Padding(
              padding: EdgeInsets.only(left: 10.0, right: 10),
              child: AppText(
                  isBody: false,
                  text: "Services Provided",
                  textAlign: TextAlign.start,
                  fontSize: 33,
                  color: AppColors.black,
                  fontStyle: FontStyle.normal,
                  fontWeight: FontWeight.w800),
            ),
            const Padding(
              padding: EdgeInsets.only(left: 10.0, right: 10),
              child: AppText(
                  isBody: true,
                  text:
                      "GINILOG offers various digital services including but not limited to data management, cloud storage, application hosting, and analytics. These services are subject to the terms and conditions detailed herein.",
                  textAlign: TextAlign.start,
                  fontSize: 33,
                  color: AppColors.black,
                  fontStyle: FontStyle.normal,
                  fontWeight: FontWeight.w400),
            ),
            addVerticalSpacing(context, 5),
            const Padding(
              padding: EdgeInsets.only(left: 10.0, right: 10),
              child: AppText(
                  isBody: false,
                  text: "User Responsibilities",
                  textAlign: TextAlign.start,
                  fontSize: 33,
                  color: AppColors.black,
                  fontStyle: FontStyle.normal,
                  fontWeight: FontWeight.w800),
            ),
            const Padding(
              padding: EdgeInsets.only(left: 10.0, right: 10),
              child: AppText(
                  isBody: true,
                  text:
                      "Users must provide accurate and complete information during the registration process.\n\nUsers are responsible for maintaining the confidentiality of their login credentials and are accountable for all activities under their account\n\nUsers must comply with all applicable laws and regulations while using GINILOG services.",
                  textAlign: TextAlign.start,
                  fontSize: 33,
                  color: AppColors.black,
                  fontStyle: FontStyle.normal,
                  fontWeight: FontWeight.w400),
            ),
            addVerticalSpacing(context, 5),
            const Padding(
              padding: EdgeInsets.only(left: 10.0, right: 10),
              child: AppText(
                  isBody: false,
                  text: "User Responsibilities",
                  textAlign: TextAlign.start,
                  fontSize: 33,
                  color: AppColors.black,
                  fontStyle: FontStyle.normal,
                  fontWeight: FontWeight.w800),
            ),
            const Padding(
              padding: EdgeInsets.only(left: 10.0, right: 10),
              child: AppText(
                  isBody: true,
                  text:
                      "Users must provide accurate and complete information during the registration process.\n\nUsers are responsible for maintaining the confidentiality of their login credentials and are accountable for all activities under their account\n\nUsers must comply with all applicable laws and regulations while using GINILOG services.",
                  textAlign: TextAlign.start,
                  fontSize: 33,
                  color: AppColors.black,
                  fontStyle: FontStyle.normal,
                  fontWeight: FontWeight.w400),
            ),
            addVerticalSpacing(context, 5),
            const Padding(
              padding: EdgeInsets.only(left: 10.0, right: 10),
              child: AppText(
                  isBody: false,
                  text: "Prohibited Activities ",
                  textAlign: TextAlign.start,
                  fontSize: 33,
                  color: AppColors.black,
                  fontStyle: FontStyle.normal,
                  fontWeight: FontWeight.w800),
            ),
            const Padding(
              padding: EdgeInsets.only(left: 10.0, right: 10),
              child: AppText(
                  isBody: true,
                  text:
                      "Users are prohibited from engaging in any of the following activities:",
                  textAlign: TextAlign.start,
                  fontSize: 33,
                  color: AppColors.black,
                  fontStyle: FontStyle.normal,
                  fontWeight: FontWeight.w400),
            ),
            const Padding(
              padding: EdgeInsets.only(left: 10.0, right: 10),
              child: AppText(
                  isBody: true,
                  text:
                      "Using GINILOG services for any unlawful purposes.\n\nUploading or transmitting harmful, offensive, or illegal content.\n\nAttempting to disrupt or interfere with the security or functionality of GINILOG services.",
                  textAlign: TextAlign.start,
                  fontSize: 33,
                  color: AppColors.black,
                  fontStyle: FontStyle.normal,
                  fontWeight: FontWeight.w400),
            ),
            addVerticalSpacing(context, 5),
            const Padding(
              padding: EdgeInsets.only(left: 10.0, right: 10),
              child: AppText(
                  isBody: false,
                  text: "Intellectual Property",
                  textAlign: TextAlign.start,
                  fontSize: 33,
                  color: AppColors.black,
                  fontStyle: FontStyle.normal,
                  fontWeight: FontWeight.w800),
            ),
            const Padding(
              padding: EdgeInsets.only(left: 10.0, right: 10),
              child: AppText(
                  isBody: true,
                  text:
                      "All content, trademarks, and data on GINILOG's platform are the intellectual property of GINILOG or its licensors. Unauthorized use of such intellectual property is strictly prohibited. ",
                  textAlign: TextAlign.start,
                  fontSize: 33,
                  color: AppColors.black,
                  fontStyle: FontStyle.normal,
                  fontWeight: FontWeight.w400),
            ),
            addVerticalSpacing(context, 5),
            const Padding(
              padding: EdgeInsets.only(left: 10.0, right: 10),
              child: AppText(
                  isBody: false,
                  text: "Privacy Policy",
                  textAlign: TextAlign.start,
                  fontSize: 33,
                  color: AppColors.black,
                  fontStyle: FontStyle.normal,
                  fontWeight: FontWeight.w800),
            ),
            const Padding(
              padding: EdgeInsets.only(left: 10.0, right: 10),
              child: AppText(
                  isBody: true,
                  text:
                      "Your use of GINILOG services is also governed by our Privacy Policy. By using our services, you consent to the collection, use, and sharing of your information as described in the Privacy Policy. ",
                  textAlign: TextAlign.start,
                  fontSize: 33,
                  color: AppColors.black,
                  fontStyle: FontStyle.normal,
                  fontWeight: FontWeight.w400),
            ),
            addVerticalSpacing(context, 5),
            const Padding(
              padding: EdgeInsets.only(left: 10.0, right: 10),
              child: AppText(
                  isBody: false,
                  text: "Limitation of Liability",
                  textAlign: TextAlign.start,
                  fontSize: 33,
                  color: AppColors.black,
                  fontStyle: FontStyle.normal,
                  fontWeight: FontWeight.w800),
            ),
            const Padding(
              padding: EdgeInsets.only(left: 10.0, right: 10),
              child: AppText(
                  isBody: true,
                  text:
                      "To the fullest extent permitted by law, GINILOG shall not be liable for any damages arising out of or in connection with the use of our services, including but not limited to direct, indirect, incidental, or consequential damages.",
                  textAlign: TextAlign.start,
                  fontSize: 33,
                  color: AppColors.black,
                  fontStyle: FontStyle.normal,
                  fontWeight: FontWeight.w400),
            ),
            addVerticalSpacing(context, 5),
            const Padding(
              padding: EdgeInsets.only(left: 10.0, right: 10),
              child: AppText(
                  isBody: false,
                  text: "Termination",
                  textAlign: TextAlign.start,
                  fontSize: 33,
                  color: AppColors.black,
                  fontStyle: FontStyle.normal,
                  fontWeight: FontWeight.w800),
            ),
            const Padding(
              padding: EdgeInsets.only(left: 10.0, right: 10),
              child: AppText(
                  isBody: true,
                  text:
                      "GINILOG reserves the right to terminate or suspend your access to our services at any time, without prior notice, for conduct that we believe violates these Terms of Services or is otherwise harmful to GINILOG or other users.",
                  textAlign: TextAlign.start,
                  fontSize: 33,
                  color: AppColors.black,
                  fontStyle: FontStyle.normal,
                  fontWeight: FontWeight.w400),
            ),
            addVerticalSpacing(context, 5),
            const Padding(
              padding: EdgeInsets.only(left: 10.0, right: 10),
              child: AppText(
                  isBody: false,
                  text: "Changes to Terms of Services ",
                  textAlign: TextAlign.start,
                  fontSize: 33,
                  color: AppColors.black,
                  fontStyle: FontStyle.normal,
                  fontWeight: FontWeight.w800),
            ),
            const Padding(
              padding: EdgeInsets.only(left: 10.0, right: 10),
              child: AppText(
                  isBody: true,
                  text:
                      "We may update these Terms of Services from time to time. We will notify you of any significant changes by posting the revised terms on our website and indicating the effective date. Your continued use of our services after the effective date constitutes your acceptance of the updated terms.",
                  textAlign: TextAlign.start,
                  fontSize: 33,
                  color: AppColors.black,
                  fontStyle: FontStyle.normal,
                  fontWeight: FontWeight.w400),
            ),
            addVerticalSpacing(context, 5),
            const Padding(
              padding: EdgeInsets.only(left: 10.0, right: 10),
              child: AppText(
                  isBody: false,
                  text: "Contact Information",
                  textAlign: TextAlign.start,
                  fontSize: 33,
                  color: AppColors.black,
                  fontStyle: FontStyle.normal,
                  fontWeight: FontWeight.w800),
            ),
            const Padding(
              padding: EdgeInsets.only(left: 10.0, right: 10),
              child: AppText(
                  isBody: true,
                  text:
                      "If you have any questions or concerns regarding these Terms of Services, please contact us using the information provided on our website.\n\nThank you for choosing GINILOG. We appreciate your trust and are committed to providing you with the best service possible.",
                  textAlign: TextAlign.start,
                  fontSize: 33,
                  color: AppColors.black,
                  fontStyle: FontStyle.normal,
                  fontWeight: FontWeight.w400),
            ),
            addVerticalSpacing(context, 5),
          ],
        ),
      ),
    );
  }
}
