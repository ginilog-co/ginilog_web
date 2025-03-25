// ignore_for_file: library_private_types_in_public_api

import 'package:ginilog_customer_app/core/components/utils/colors.dart';
import 'package:ginilog_customer_app/core/components/utils/helper_functions.dart';
import 'package:ginilog_customer_app/core/components/utils/package_export.dart';
import 'package:ginilog_customer_app/core/components/utils/size_config.dart';
import 'package:ginilog_customer_app/core/components/widgets/app_text.dart';
import 'package:ginilog_customer_app/core/components/widgets/back_icon.dart';

class AboutUsScreen extends StatefulWidget {
  const AboutUsScreen({
    super.key,
  });

  @override
  _LoginPageState createState() => _LoginPageState();
}

class _LoginPageState extends State<AboutUsScreen> {
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
            child: GlobalBackButton(backText: "About Us", showBackButton: true),
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
                  text: "About  Us",
                  textAlign: TextAlign.start,
                  fontSize: 33,
                  color: AppColors.black,
                  fontStyle: FontStyle.normal,
                  fontWeight: FontWeight.bold),
            ),
            addVerticalSpacing(context, 3),
            const Padding(
              padding: EdgeInsets.only(left: 10.0, right: 10),
              child: AppText(
                  isBody: true,
                  text:
                      "At GINILOG, we believe travel and logistics shouldn’t be a source of stress ‘they should be seamless, efficient and tailored to your unique needs. Founded by a team of innovative minds, who recognized the growing complexity in coordinating both personal and professional journeys, shipments and booking  accommodations. We are creating a solution a one-stop-platform that simplifies everything from planning your dream vacation  to managing complex  logistics. We’re driven by a passion for connecting people and goods and we’re committed to delivering exceptional experiences every step of the way, while connecting every dots across every end. ",
                  textAlign: TextAlign.start,
                  fontSize: 33,
                  color: AppColors.black,
                  fontStyle: FontStyle.normal,
                  fontWeight: FontWeight.w400),
            ),
          ],
        ),
      ),
    );
  }
}
