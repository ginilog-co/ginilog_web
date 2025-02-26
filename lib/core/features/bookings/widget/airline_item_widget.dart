import 'package:ginilog_customer_app/core/components/utils/colors.dart';
import 'package:ginilog_customer_app/core/components/utils/helper_functions.dart';
import 'package:ginilog_customer_app/core/components/utils/package_export.dart';
import 'package:ginilog_customer_app/core/components/widgets/app_text.dart';
import 'package:ginilog_customer_app/core/components/widgets/rating_star.dart';
import 'package:ginilog_customer_app/core/features/bookings/model/airline_response_model.dart';

class AirlineItemWidget extends ConsumerStatefulWidget {
  final AirlineResponseModel dataModel;
  const AirlineItemWidget({super.key, required this.dataModel});

  @override
  ConsumerState<AirlineItemWidget> createState() => _AirlineItemWidgetState();
}

class _AirlineItemWidgetState extends ConsumerState<AirlineItemWidget> {
  @override
  Widget build(BuildContext context) {
    final data3 = widget.dataModel;

    return Padding(
      padding: const EdgeInsets.only(left: 15.0, right: 15.0, top: 5.0),
      child: GestureDetector(
        onTap: () {
          // navigateToRoute(
          //     context,
          //     dataModelDetailsPage(
          //       dataModel: data3,
          //     ));
        },
        child: Container(
          width: getScreenWidth(context),
          decoration: BoxDecoration(
            color: AppColors.white,
            borderRadius: const BorderRadius.all(
              Radius.circular(10),
            ),
            border: Border.all(
                color: AppColors.primaryDark.withAlpha(54), width: 1.5),
          ),
          child: Padding(
            padding: const EdgeInsets.all(8.0),
            child: Row(
              spacing: 8,
              crossAxisAlignment: CrossAxisAlignment.center,
              mainAxisAlignment: MainAxisAlignment.start,
              children: [
                Image.network(
                    data3.airlineLogo == null || data3.airlineLogo!.isEmpty
                        ? "https://developers.elementor.com/docs/assets/img/elementor-placeholder-image.png"
                        : "${data3.airlineLogo}",
                    width: 80,
                    height: 80),
                Expanded(
                  child: Column(
                    spacing: 5,
                    crossAxisAlignment: CrossAxisAlignment.start,
                    mainAxisAlignment: MainAxisAlignment.start,
                    children: [
                      AppText(
                          isBody: true,
                          text: "${data3.airlineName}",
                          textAlign: TextAlign.start,
                          fontSize: 75,
                          color: AppColors.black,
                          fontStyle: FontStyle.normal,
                          fontWeight: FontWeight.bold),
                      AppText(
                          isBody: true,
                          text: "${data3.airlineInfo}",
                          textAlign: TextAlign.start,
                          fontSize: 55,
                          overflow: TextOverflow.ellipsis,
                          maxLines: 3,
                          color: AppColors.black,
                          fontStyle: FontStyle.normal,
                          fontWeight: FontWeight.w500),
                      AppText(
                          isBody: true,
                          text: "${data3.airlineType}",
                          textAlign: TextAlign.start,
                          fontSize: 75,
                          color: AppColors.black,
                          fontStyle: FontStyle.normal,
                          fontWeight: FontWeight.w500),
                      Align(
                          alignment: Alignment.bottomRight,
                          child: StarRating(rating: 4.5)),
                    ],
                  ),
                ),
                Icon(
                  Icons.arrow_forward_ios,
                  color: AppColors.grey2,
                  size: 30,
                )
              ],
            ),
          ),
        ),
      ),
    );
  }
}
