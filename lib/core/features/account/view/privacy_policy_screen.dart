// ignore_for_file: library_private_types_in_public_api

import 'package:ginilog_customer_app/core/components/utils/colors.dart';
import 'package:ginilog_customer_app/core/components/utils/package_export.dart';
import 'package:ginilog_customer_app/core/components/utils/size_config.dart';
import 'package:ginilog_customer_app/core/components/widgets/app_text.dart';
import 'package:ginilog_customer_app/core/components/widgets/back_icon.dart';

class PrivacyPolicyScreen extends StatefulWidget {
  const PrivacyPolicyScreen({super.key});

  @override
  _LoginPageState createState() => _LoginPageState();
}

class _LoginPageState extends State<PrivacyPolicyScreen> {
  final List<String> personalInformation = [
    "Name",
    "Email address",
    "Phone number",
    "Mailing address",
    "Payment information",
  ];
  final List<String> nonPersonalInformation = [
    "IP address",
    "Browser type and version",
    "Operating system",
    "Device information",
    "Pages visited and actions taken on our website",
    "Time and date of visits",
  ];
  final List<String> usingInformation = [
    "To provide, operate, and maintain our services",
    "To improve, personalize, and expand our services",
    "To process transactions and manage your orders",
    "To communicate with you, including responding to your inquiries and providing customer support",
    "To send you updates, promotional materials, and other information related to our services",
    "To analyze usage patterns and improve our website functionality",
    "To comply with legal obligations and protect the rights and safety of GINILOG and its users",
  ];
  final List<String> shareInformation = [
    "With service providers who assist us in operating our business, such as payment processors, shipping companies, and marketing agencies",
    "With legal authorities or other third parties to comply with legal obligations or protect our rights and interests",
    "In connection with a merger, acquisition, or sale of our business",
    "With your consent or at your direction",
  ];
  final List<String> rightAndChoices = [
    "The right to access and obtain a copy of your personal information",
    "The right to request correction or updating of inaccurate or incomplete information",
    "The right to request deletion of your personal information",
    "The right to object to or restrict processing of your personal information",
    "The right to withdraw your consent to our processing of your personal information",
    "The right to lodge a complaint with a supervisory authority",
  ];

  @override
  void initState() {
    super.initState();
  }

  @override
  void dispose() {
    super.dispose();
  }

  Widget _buildBulletPoint(String text) {
    return Padding(
      padding: const EdgeInsets.symmetric(vertical: 5.0),
      child: RichText(
        text: TextSpan(
          style: TextStyle(
            color: Colors.black,
            fontWeight: FontWeight.w400,
            fontSize: 15.textSize,
            fontFamily: "Inter",
          ),
          children: [TextSpan(text: "• "), TextSpan(text: text)],
        ),
      ),
    );
  }

  @override
  Widget build(BuildContext context) {
    final key = GlobalKey<ScaffoldMessengerState>();
    return Scaffold(
      appBar: buildFlexibleAppBar(
        context: context,

        title: AppText(
          isBody: true,
          text: "Privacy Policy",
          textAlign: TextAlign.start,
          fontSize: 18,
          color: AppColors.black,
          fontStyle: FontStyle.normal,
          fontWeight: FontWeight.w800,
        ),
      ),
      key: key,
      backgroundColor: Colors.white,
      body: SafeArea(
        child: ListView(
          physics: const ScrollPhysics(),
          children: [
            const Padding(
              padding: EdgeInsets.only(left: 10.0, right: 10),
              child: AppText(
                isBody: false,
                text: "Privacy Policy",
                textAlign: TextAlign.start,
                fontSize: 15,
                color: AppColors.black,
                fontStyle: FontStyle.normal,
                fontWeight: FontWeight.bold,
              ),
            ),
            addVerticalSpacing(5),
            const Padding(
              padding: EdgeInsets.only(left: 10.0, right: 10),
              child: AppText(
                isBody: false,
                text: "Introduction",
                textAlign: TextAlign.start,
                fontSize: 15,
                color: AppColors.black,
                fontStyle: FontStyle.normal,
                fontWeight: FontWeight.w800,
              ),
            ),
            const Padding(
              padding: EdgeInsets.only(left: 10.0, right: 10),
              child: AppText(
                isBody: true,
                text:
                    "At GINILOG, we are committed to protecting your privacy and ensuring that your personal information is handled in a safe and responsible manner. This Privacy Policy outlines how we collect, use, disclose, and protect your information in accordance with applicable privacy laws.",
                textAlign: TextAlign.start,
                fontSize: 15,
                color: AppColors.black,
                fontStyle: FontStyle.normal,
                fontWeight: FontWeight.w400,
              ),
            ),
            addVerticalSpacing(5),
            const Padding(
              padding: EdgeInsets.only(left: 10.0, right: 10),
              child: AppText(
                isBody: false,
                text: "Information We Collect",
                textAlign: TextAlign.start,
                fontSize: 15,
                color: AppColors.black,
                fontStyle: FontStyle.normal,
                fontWeight: FontWeight.w800,
              ),
            ),
            const Padding(
              padding: EdgeInsets.only(left: 10.0, right: 10),
              child: AppText(
                isBody: false,
                text: "Personal Information",
                textAlign: TextAlign.start,
                fontSize: 15,
                color: AppColors.black,
                fontStyle: FontStyle.normal,
                fontWeight: FontWeight.w800,
              ),
            ),
            const Padding(
              padding: EdgeInsets.only(left: 10.0, right: 10),
              child: AppText(
                isBody: true,
                text:
                    "When you use our services, we may collect personal information that you provide to us, including but not limited to: ",
                textAlign: TextAlign.start,
                fontSize: 15,
                color: AppColors.black,
                fontStyle: FontStyle.normal,
                fontWeight: FontWeight.w400,
              ),
            ),
            Padding(
              padding: EdgeInsets.only(left: 10.0, right: 10),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children:
                    personalInformation
                        .map((item) => _buildBulletPoint(item))
                        .toList(),
              ),
            ),
            addVerticalSpacing(5),
            const Padding(
              padding: EdgeInsets.only(left: 10.0, right: 10),
              child: AppText(
                isBody: false,
                text: "Non-Personal Information ",
                textAlign: TextAlign.start,
                fontSize: 15,
                color: AppColors.black,
                fontStyle: FontStyle.normal,
                fontWeight: FontWeight.w800,
              ),
            ),
            const Padding(
              padding: EdgeInsets.only(left: 10.0, right: 10),
              child: AppText(
                isBody: true,
                text:
                    "We may also collect non-personal information about your interaction with our services, such as:",
                textAlign: TextAlign.start,
                fontSize: 15,
                color: AppColors.black,
                fontStyle: FontStyle.normal,
                fontWeight: FontWeight.w400,
              ),
            ),
            Padding(
              padding: EdgeInsets.only(left: 10.0, right: 10),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children:
                    nonPersonalInformation
                        .map((item) => _buildBulletPoint(item))
                        .toList(),
              ),
            ),
            addVerticalSpacing(5),
            const Padding(
              padding: EdgeInsets.only(left: 10.0, right: 10),
              child: AppText(
                isBody: false,
                text: "How We Use Your Information",
                textAlign: TextAlign.start,
                fontSize: 15,
                color: AppColors.black,
                fontStyle: FontStyle.normal,
                fontWeight: FontWeight.w800,
              ),
            ),
            const Padding(
              padding: EdgeInsets.only(left: 10.0, right: 10),
              child: AppText(
                isBody: true,
                text:
                    "We use the information we collect for various purposes, including: ",
                textAlign: TextAlign.start,
                fontSize: 15,
                color: AppColors.black,
                fontStyle: FontStyle.normal,
                fontWeight: FontWeight.w400,
              ),
            ),
            Padding(
              padding: EdgeInsets.only(left: 10.0, right: 10),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children:
                    usingInformation
                        .map((item) => _buildBulletPoint(item))
                        .toList(),
              ),
            ),
            addVerticalSpacing(5),
            const Padding(
              padding: EdgeInsets.only(left: 10.0, right: 10),
              child: AppText(
                isBody: false,
                text: "How We Share Your Information ",
                textAlign: TextAlign.start,
                fontSize: 15,
                color: AppColors.black,
                fontStyle: FontStyle.normal,
                fontWeight: FontWeight.w800,
              ),
            ),
            const Padding(
              padding: EdgeInsets.only(left: 10.0, right: 10),
              child: AppText(
                isBody: true,
                text:
                    "We may share your information with third parties in the following circumstances: ",
                textAlign: TextAlign.start,
                fontSize: 15,
                color: AppColors.black,
                fontStyle: FontStyle.normal,
                fontWeight: FontWeight.w400,
              ),
            ),
            Padding(
              padding: EdgeInsets.only(left: 10.0, right: 10),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children:
                    shareInformation
                        .map((item) => _buildBulletPoint(item))
                        .toList(),
              ),
            ),
            addVerticalSpacing(5),
            const Padding(
              padding: EdgeInsets.only(left: 10.0, right: 10),
              child: AppText(
                isBody: false,
                text: "Security of Your Information ",
                textAlign: TextAlign.start,
                fontSize: 15,
                color: AppColors.black,
                fontStyle: FontStyle.normal,
                fontWeight: FontWeight.w800,
              ),
            ),
            const Padding(
              padding: EdgeInsets.only(left: 10.0, right: 10),
              child: AppText(
                isBody: true,
                text:
                    "We implement appropriate technical and organizational measures to protect your personal information against unauthorized access, disclosure, alteration, or destruction. However, no method of transmission over the internet or electronic storage is completely secure, and we cannot guarantee absolute security.  ",
                textAlign: TextAlign.start,
                fontSize: 15,
                color: AppColors.black,
                fontStyle: FontStyle.normal,
                fontWeight: FontWeight.w400,
              ),
            ),
            Padding(
              padding: EdgeInsets.only(left: 10.0, right: 10),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children:
                    rightAndChoices
                        .map((item) => _buildBulletPoint(item))
                        .toList(),
              ),
            ),
            addVerticalSpacing(5),
            const Padding(
              padding: EdgeInsets.only(left: 10.0, right: 10),
              child: AppText(
                isBody: false,
                text: "Third-Party Links ",
                textAlign: TextAlign.start,
                fontSize: 15,
                color: AppColors.black,
                fontStyle: FontStyle.normal,
                fontWeight: FontWeight.w800,
              ),
            ),
            const Padding(
              padding: EdgeInsets.only(left: 10.0, right: 10),
              child: AppText(
                isBody: true,
                text:
                    "Our website may contain links to third-party websites or services that are not owned or controlled by GINILOG. We are not responsible for the privacy practices of these third parties. We encourage you to review their privacy policies before providing any personal information.",
                textAlign: TextAlign.start,
                fontSize: 15,
                color: AppColors.black,
                fontStyle: FontStyle.normal,
                fontWeight: FontWeight.w400,
              ),
            ),
            addVerticalSpacing(5),
            const Padding(
              padding: EdgeInsets.only(left: 10.0, right: 10),
              child: AppText(
                isBody: false,
                text: "Changes to This Privacy Policy ",
                textAlign: TextAlign.start,
                fontSize: 15,
                color: AppColors.black,
                fontStyle: FontStyle.normal,
                fontWeight: FontWeight.w800,
              ),
            ),
            const Padding(
              padding: EdgeInsets.only(left: 10.0, right: 10),
              child: AppText(
                isBody: true,
                text:
                    "We may update this Privacy Policy from time to time to reflect changes in our practices or legal requirements. We will notify you of any significant changes by posting the revised policy on our website and indicating the effective date. Your continued use of our services after the effective date constitutes your acceptance of the updated policy. ",
                textAlign: TextAlign.start,
                fontSize: 15,
                color: AppColors.black,
                fontStyle: FontStyle.normal,
                fontWeight: FontWeight.w400,
              ),
            ),
            addVerticalSpacing(5),
            const Padding(
              padding: EdgeInsets.only(left: 10.0, right: 10),
              child: AppText(
                isBody: false,
                text: "Contact Us",
                textAlign: TextAlign.start,
                fontSize: 15,
                color: AppColors.black,
                fontStyle: FontStyle.normal,
                fontWeight: FontWeight.w800,
              ),
            ),
            const Padding(
              padding: EdgeInsets.only(left: 10.0, right: 10),
              child: AppText(
                isBody: true,
                text:
                    "If you have any questions or concerns about this Privacy Policy or our privacy practices, please contact us. \n\nThank you for choosing GINILOG. We value your trust and are committed to protecting your privacy. ",
                textAlign: TextAlign.start,
                fontSize: 15,
                color: AppColors.black,
                fontStyle: FontStyle.normal,
                fontWeight: FontWeight.w400,
              ),
            ),
            addVerticalSpacing(5),
          ],
        ),
      ),
    );
  }
}
