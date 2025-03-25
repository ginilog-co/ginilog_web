// ignore_for_file: library_private_types_in_public_api

import 'package:ginilog_customer_app/core/components/utils/colors.dart';
import 'package:ginilog_customer_app/core/components/utils/helper_functions.dart';
import 'package:ginilog_customer_app/core/components/utils/package_export.dart';
import 'package:ginilog_customer_app/core/components/utils/size_config.dart';
import 'package:ginilog_customer_app/core/components/widgets/app_text.dart';
import 'package:ginilog_customer_app/core/components/widgets/back_icon.dart';
import 'package:ginilog_customer_app/core/features/bookings/model/accomodation_response_model.dart';

class ViewAllReviewPagePage extends ConsumerStatefulWidget {
  const ViewAllReviewPagePage(
      {super.key, required this.reviews, required this.accomodationName});
  final List<AccomodationReviewModel> reviews;
  final String accomodationName;
  @override
  _LoginPageState createState() => _LoginPageState();
}

class _LoginPageState extends ConsumerState<ViewAllReviewPagePage> {
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
    return Scaffold(
      backgroundColor: AppColors.white,
      appBar: PreferredSize(
          preferredSize: Size.fromHeight(SizeConfig.heightAdjusted(16)),
          child: Padding(
            padding: EdgeInsets.only(top: SizeConfig.heightAdjusted(10)),
            child: GlobalBackButton(
                backText: "${widget.accomodationName} Review",
                showBackButton: true),
          )),
      body: SafeArea(
        child: Padding(
          padding: const EdgeInsets.only(left: 18.0, right: 18.0),
          child: Column(
            children: [
              Expanded(
                child: widget.reviews.isNotEmpty
                    ? ListView.builder(
                        shrinkWrap: true,
                        scrollDirection: Axis.vertical,
                        itemCount: widget.reviews.length,
                        itemBuilder: (BuildContext context, int index) {
                          return Column(
                            crossAxisAlignment: CrossAxisAlignment.start,
                            children: [
                              Row(
                                children: [
                                  Container(
                                    margin: const EdgeInsets.all(5),
                                    height: 30.0,
                                    width: 30.0,
                                    decoration: BoxDecoration(
                                      border: Border.all(
                                        color: AppColors.grey,
                                        width: 1,
                                      ),
                                      borderRadius: BorderRadius.circular(50),
                                      //set border radius to 50% of square height and width
                                      image: DecorationImage(
                                        image: NetworkImage(widget
                                            .reviews[index].profileImage
                                            .toString()),
                                        fit: BoxFit
                                            .contain, //change image fill type
                                      ),
                                    ),
                                  ),
                                  addHorizontalSpacing(10),
                                  AppText(
                                      isBody: true,
                                      text: "${widget.reviews[index].userName}",
                                      textAlign: TextAlign.start,
                                      fontSize: 76,
                                      color: AppColors.primaryDark,
                                      fontStyle: FontStyle.normal,
                                      fontWeight: FontWeight.bold),
                                ],
                              ),
                              RatingBar.readOnly(
                                isHalfAllowed: true,
                                alignment: Alignment.centerLeft,
                                halfFilledIcon: Icons.star_half,
                                filledIcon: Icons.star,
                                emptyIcon: Icons.star_border,
                                emptyColor: Colors.yellow,
                                halfFilledColor: Colors.grey,
                                initialRating:
                                    widget.reviews[index].ratingNum!.toDouble(),
                                size: 15,
                              ),
                              addVerticalSpacing(context, 4),
                              AppText(
                                  isBody: true,
                                  text:
                                      "${widget.reviews[index].reviewMessage}",
                                  textAlign: TextAlign.start,
                                  fontSize: 76,
                                  color: AppColors.primaryDark,
                                  fontStyle: FontStyle.normal,
                                  fontWeight: FontWeight.bold),
                              const Divider(
                                thickness: 0.7,
                                color: AppColors.grey,
                              )
                            ],
                          );
                        },
                      )
                    : Center(
                        child: Column(
                          mainAxisAlignment: MainAxisAlignment.center,
                          crossAxisAlignment: CrossAxisAlignment.center,
                          children: [
                            Image.asset(
                              "assets/images/order_icon.png",
                              width: 200,
                              height: 100,
                            ),
                            addVerticalSpacing(context, 5),
                            const AppText(
                                isBody: false,
                                text: "Nothing to show here",
                                textAlign: TextAlign.start,
                                fontSize: 85,
                                color: AppColors.black,
                                fontStyle: FontStyle.normal,
                                fontWeight: FontWeight.bold),
                            const AppText(
                                isBody: true,
                                text:
                                    "There is no reviews of This Accomodation ",
                                textAlign: TextAlign.center,
                                fontSize: 75,
                                color: AppColors.black,
                                fontStyle: FontStyle.normal,
                                fontWeight: FontWeight.normal),
                          ],
                        ),
                      ),
              ),
            ],
          ),
        ),
      ),
    );
  }
}
