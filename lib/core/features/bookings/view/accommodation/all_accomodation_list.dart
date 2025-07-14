import 'package:ginilog_customer_app/core/components/utils/colors.dart';
import 'package:ginilog_customer_app/core/components/utils/package_export.dart';
import 'package:ginilog_customer_app/core/components/utils/size_config.dart';
import 'package:ginilog_customer_app/core/components/widgets/app_text.dart';
import 'package:ginilog_customer_app/core/features/bookings/model/accomodation_response_model.dart';
import 'package:ginilog_customer_app/core/features/bookings/widget/accomodation_item_widget.dart';

class AllAccomodationListTab extends ConsumerStatefulWidget {
  final List<AccomodationResponseModel> allAccomodation;
  const AllAccomodationListTab({super.key, required this.allAccomodation});

  @override
  ConsumerState<AllAccomodationListTab> createState() =>
      _AllAccomodationListTabState();
}

class _AllAccomodationListTabState
    extends ConsumerState<AllAccomodationListTab> {
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
            widget.allAccomodation.isEmpty
                ? Center(
                  child: Column(
                    mainAxisAlignment: MainAxisAlignment.center,
                    crossAxisAlignment: CrossAxisAlignment.center,
                    children: [
                      addVerticalSpacing(5),
                      const AppText(
                        isBody: false,
                        text: "Nothing to show here",
                        textAlign: TextAlign.start,
                        fontSize: 18,
                        color: AppColors.black,
                        fontStyle: FontStyle.normal,
                        fontWeight: FontWeight.bold,
                      ),
                      const AppText(
                        isBody: true,
                        text: "You don't have any Accomodation at the moment",
                        textAlign: TextAlign.center,
                        fontSize: 70,
                        color: AppColors.black,
                        fontStyle: FontStyle.normal,
                        fontWeight: FontWeight.normal,
                      ),
                    ],
                  ),
                )
                : ListView.builder(
                  itemCount: widget.allAccomodation.length,
                  shrinkWrap: true,
                  physics: const NeverScrollableScrollPhysics(),
                  itemBuilder:
                      (context, index) => AccomodationItemWidget(
                        dataModel: widget.allAccomodation[index],
                      ),
                ),
          ],
        ),
      ),
    );
  }
}
