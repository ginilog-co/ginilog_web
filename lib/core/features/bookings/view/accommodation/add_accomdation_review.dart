import 'package:ginilog_customer_app/core/components/architecture/mvc.dart';
import 'package:ginilog_customer_app/core/components/utils/app_buttons.dart';
import 'package:ginilog_customer_app/core/components/utils/colors.dart';
import 'package:ginilog_customer_app/core/components/utils/helper_functions.dart';
import 'package:ginilog_customer_app/core/components/utils/package_export.dart';
import 'package:ginilog_customer_app/core/components/utils/size_config.dart';
import 'package:ginilog_customer_app/core/components/widgets/app_text.dart';
import 'package:ginilog_customer_app/core/components/widgets/back_icon.dart';
import 'package:ginilog_customer_app/core/components/widgets/input.dart';
import 'package:ginilog_customer_app/core/features/bookings/controller/add_review.dart';

class AddAccomodationReviewScreenView extends StatelessView<
    AddAccomodationReviewScreen, AddAccomodationReviewScreenController> {
  const AddAccomodationReviewScreenView(super.state, {super.key});

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: AppColors.white,
      appBar: PreferredSize(
          preferredSize: Size.fromHeight(SizeConfig.heightAdjusted(16)),
          child: Padding(
            padding: EdgeInsets.only(top: SizeConfig.heightAdjusted(10)),
            child: const GlobalBackButton(backText: "", showBackButton: true),
          )),
      body: Container(
        height: getScreenHeight(context),
        width: getScreenWidth(context),
        color: AppColors.white,
        child: SingleChildScrollView(
          child: Form(
            key: controller.formKey,
            child: Padding(
              padding: const EdgeInsets.only(
                left: 14.0,
                right: 14.0,
                top: 10,
              ),
              child: Column(
                mainAxisAlignment: MainAxisAlignment.start,
                children: [
                  Column(
                    crossAxisAlignment: CrossAxisAlignment.center,
                    mainAxisAlignment: MainAxisAlignment.center,
                    children: [
                      Text(
                        "Review Rider",
                        style: TextStyle(
                            fontSize: fontSized(context, 7),
                            color: AppColors.black,
                            fontWeight: FontWeight.bold,
                            fontFamily: "Montserrat"),
                      ),
                    ],
                  ),
                  Center(
                    child: CircleAvatar(
                      radius: 60,
                      backgroundColor: Colors.grey,
                      child: CircleAvatar(
                          backgroundColor: Colors.white,
                          radius: 57,
                          backgroundImage: controller.widget.accomodationLogo
                                  .toString()
                                  .isEmpty
                              ? const NetworkImage(
                                  "https://upload.wikimedia.org/wikipedia/commons/thumb/d/d3/Microsoft_Account.svg/512px-Microsoft_Account.svg.png?20170218203212")
                              // ignore: unnecessary_null_comparison
                              : controller.widget.accomodationLogo == null
                                  ? const NetworkImage(
                                      "https://upload.wikimedia.org/wikipedia/commons/thumb/d/d3/Microsoft_Account.svg/512px-Microsoft_Account.svg.png?20170218203212")
                                  : NetworkImage(controller
                                      .widget.accomodationLogo
                                      .toString())),
                    ),
                  ),
                  Center(
                    child: AppText(
                        isBody: true,
                        text: controller.widget.accomodationName.toString(),
                        textAlign: TextAlign.start,
                        fontSize: 19,
                        color: AppColors.black,
                        fontStyle: FontStyle.normal,
                        fontWeight: FontWeight.bold),
                  ),
                  Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      addVerticalSpacing(context, 15),
                      Center(
                        child: RatingBar(
                          isHalfAllowed: true,
                          halfFilledIcon: Icons.star_half,
                          filledIcon: Icons.star,
                          emptyIcon: Icons.star_border,
                          onRatingChanged: (value) =>
                              controller.reviewControllerOnChanged(value),
                          initialRating: 0,
                          alignment: Alignment.center,
                          size: 50,
                        ),
                      ),
                      addVerticalSpacing(context, 15),
                      Text(
                        "Write Review",
                        style: TextStyle(
                            fontSize: fontSized(context, 12),
                            color: AppColors.black,
                            fontWeight: FontWeight.w700,
                            fontFamily: "Montserrat"),
                      ),
                      GlobalTextField(
                        fieldName: 'Write a Review',
                        keyBoardType: TextInputType.multiline,
                        removeSpace: false,
                        obscureText: false,
                        isNotePad: true,
                        maxLength: 200,
                        textController: controller.reviewController,
                        onChanged: (String? value) {
                          controller.reviewMessageOnChanged(value!);
                        },
                      ),
                      addVerticalSpacing(context, 10),
                      appButton("Add Review", getScreenWidth(context), () {
                        controller.userRegister();
                      }, AppColors.primary, controller.isLoading),
                      addVerticalSpacing(context, 20),
                    ],
                  ),
                ],
              ),
            ),
          ),
        ),
      ),
    );
  }
}
