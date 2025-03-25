import 'package:ginilog_customer_app/core/components/utils/colors.dart';
import 'package:ginilog_customer_app/core/components/utils/helper_functions.dart';
import 'package:ginilog_customer_app/core/components/utils/package_export.dart';
import 'package:ginilog_customer_app/core/components/widgets/app_text.dart';
import 'package:ginilog_customer_app/core/features/account/view/report/bookings_item.dart';
import 'package:ginilog_customer_app/core/features/bookings/model/customer_book_response_model.dart';

class AllBookingsHistoryPage extends ConsumerStatefulWidget {
  final List<CustomerBookResponseModel> allAccomodations;
  const AllBookingsHistoryPage({super.key, required this.allAccomodations});

  @override
  ConsumerState<AllBookingsHistoryPage> createState() =>
      _AllBookingsHistoryPageState();
}

class _AllBookingsHistoryPageState
    extends ConsumerState<AllBookingsHistoryPage> {
  @override
  void initState() {
    super.initState();
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
        backgroundColor: AppColors.white,
        body: SingleChildScrollView(
          child: Column(
            children: [
              widget.allAccomodations.isEmpty
                  ? Center(
                      child: Column(
                        mainAxisAlignment: MainAxisAlignment.center,
                        crossAxisAlignment: CrossAxisAlignment.center,
                        children: [
                          addVerticalSpacing(context, 55),
                          const AppText(
                              isBody: false,
                              text: "Nothing to show here",
                              textAlign: TextAlign.start,
                              fontSize: 38,
                              color: AppColors.black,
                              fontStyle: FontStyle.normal,
                              fontWeight: FontWeight.bold),
                          const AppText(
                              isBody: true,
                              text: "You don't have any Bookings at the moment",
                              textAlign: TextAlign.center,
                              fontSize: 30,
                              color: AppColors.black,
                              fontStyle: FontStyle.normal,
                              fontWeight: FontWeight.normal),
                        ],
                      ),
                    )
                  : ListView.builder(
                      itemCount: widget.allAccomodations.length,
                      shrinkWrap: true,
                      physics: const NeverScrollableScrollPhysics(),
                      itemBuilder: (context, index) => CustomerItemPage(
                            accomodation: widget.allAccomodations[index],
                          ))
            ],
          ),
        ));
  }
}
