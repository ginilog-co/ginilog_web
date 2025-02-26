import 'package:ginilog_customer_app/core/components/utils/colors.dart';
import 'package:ginilog_customer_app/core/components/utils/helper_functions.dart';
import 'package:ginilog_customer_app/core/components/utils/package_export.dart';
import 'package:ginilog_customer_app/core/components/widgets/app_text.dart';
import 'package:ginilog_customer_app/core/features/bookings/model/airline_response_model.dart';
import 'package:ginilog_customer_app/core/features/bookings/widget/airline_item_widget.dart';

class AllAirlineListTab extends ConsumerStatefulWidget {
  final List<AirlineResponseModel> allAirline;
  const AllAirlineListTab({super.key, required this.allAirline});

  @override
  ConsumerState<AllAirlineListTab> createState() => _AllAirlineListTabState();
}

class _AllAirlineListTabState extends ConsumerState<AllAirlineListTab> {
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
              widget.allAirline.isEmpty
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
                              fontSize: 78,
                              color: AppColors.black,
                              fontStyle: FontStyle.normal,
                              fontWeight: FontWeight.bold),
                          const AppText(
                              isBody: true,
                              text: "You don't have any flight at the moment",
                              textAlign: TextAlign.center,
                              fontSize: 70,
                              color: AppColors.black,
                              fontStyle: FontStyle.normal,
                              fontWeight: FontWeight.normal),
                        ],
                      ),
                    )
                  : ListView.builder(
                      itemCount: widget.allAirline.length,
                      shrinkWrap: true,
                      physics: const NeverScrollableScrollPhysics(),
                      itemBuilder: (context, index) => AirlineItemWidget(
                            dataModel: widget.allAirline[index],
                          ))
            ],
          ),
        ));
  }
}
