import 'package:ginilog_customer_app/core/components/utils/colors.dart';
import 'package:ginilog_customer_app/core/components/utils/helper_functions.dart';
import 'package:ginilog_customer_app/core/components/utils/package_export.dart';
import 'package:ginilog_customer_app/core/components/widgets/app_text.dart';
import 'package:ginilog_customer_app/core/components/widgets/rating_star.dart';
import 'package:ginilog_customer_app/core/features/bookings/model/accomodation_response_model.dart';
import 'package:ginilog_customer_app/core/features/bookings/view/accommodation/view_accomodation_detail.dart';

class AccomodationItemWidget extends ConsumerStatefulWidget {
  final AccomodationResponseModel dataModel;
  const AccomodationItemWidget({super.key, required this.dataModel});

  @override
  ConsumerState<AccomodationItemWidget> createState() =>
      _AccomodationItemWidgetState();
}

class _AccomodationItemWidgetState
    extends ConsumerState<AccomodationItemWidget> {
  @override
  Widget build(BuildContext context) {
    final data3 = widget.dataModel;

    return Padding(
      padding: const EdgeInsets.only(left: 15.0, right: 15.0, top: 5.0),
      child: GestureDetector(
        onTap: () {
          navigateToRoute(
              context,
              ViewAccomodationPage(
                reservation: data3,
              ));
        },
        child: Container(
          width: getScreenWidth(context),
          decoration: BoxDecoration(
            color: AppColors.white,
            borderRadius: const BorderRadius.all(
              Radius.circular(10),
            ),
          ),
          child: Padding(
            padding: const EdgeInsets.all(8.0),
            child: Column(
              spacing: 8,
              children: [
                Container(
                  width: getScreenWidth(context),
                  height: 200,
                  decoration: BoxDecoration(
                      borderRadius: const BorderRadius.only(
                        topLeft: Radius.circular(10),
                        topRight: Radius.circular(10),
                      ),
                      image: DecorationImage(
                          fit: BoxFit.fill,
                          image: NetworkImage(data3.accomodationImages ==
                                      null ||
                                  data3.accomodationImages!.isEmpty
                              ? "https://developers.elementor.com/docs/assets/img/elementor-placeholder-image.png"
                              : data3.accomodationImages![0]))),
                ),
                Column(
                  spacing: 5,
                  crossAxisAlignment: CrossAxisAlignment.start,
                  mainAxisAlignment: MainAxisAlignment.start,
                  children: [
                    Row(
                      children: [
                        AppText(
                            isBody: true,
                            text: "${data3.accomodationName}",
                            textAlign: TextAlign.start,
                            fontSize: 35,
                            color: AppColors.black,
                            fontStyle: FontStyle.normal,
                            fontWeight: FontWeight.bold),
                        Spacer(),
                        StarRating(rating: data3.rating!.toDouble()),
                      ],
                    ),
                    AppText(
                        isBody: true,
                        text: "${data3.accomodationDescription}",
                        textAlign: TextAlign.start,
                        fontSize: 35,
                        overflow: TextOverflow.ellipsis,
                        maxLines: 3,
                        color: AppColors.black,
                        fontStyle: FontStyle.normal,
                        fontWeight: FontWeight.w500),
                    AppText(
                        isBody: true,
                        text: "${data3.location}",
                        textAlign: TextAlign.start,
                        fontSize: 35,
                        color: AppColors.black,
                        fontStyle: FontStyle.normal,
                        fontWeight: FontWeight.w500),
                  ],
                ),
              ],
            ),
          ),
        ),
      ),
    );
  }
}
