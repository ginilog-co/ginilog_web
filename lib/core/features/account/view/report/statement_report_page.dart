import 'package:ginilog_customer_app/core/components/architecture/mvc.dart';
import 'package:ginilog_customer_app/core/components/utils/colors.dart';
import 'package:ginilog_customer_app/core/components/utils/package_export.dart';
import 'package:ginilog_customer_app/core/components/utils/size_config.dart';
import 'package:ginilog_customer_app/core/components/widgets/app_text.dart';
import 'package:ginilog_customer_app/core/components/widgets/back_icon.dart';
import 'package:ginilog_customer_app/core/features/account/controller/statement_reports_page.dart';
import 'package:ginilog_customer_app/core/features/account/view/report/bookings_item.dart';
import 'package:ginilog_customer_app/core/features/bookings/state/customer_booked_state.dart';

class StatementReportScreenView
    extends
        StatelessView<StatementReportScreen, StatementReportScreenController> {
  const StatementReportScreenView(super.state, {super.key});

  @override
  Widget build(BuildContext context) {
    final notifier = controller.ref.watch(customerBookedStateProvider.notifier);
    final state = controller.ref.watch(customerBookedStateProvider);

    final isLoading =
        state is CustomerBookedStateLoading && !state.hasLoadedInitially;
    final reservationsList = notifier.allCustomerReservations;
    return Scaffold(
      backgroundColor: AppColors.white,
      appBar: buildFlexibleAppBar(
        context: context,

        title: AppText(
          isBody: true,
          text: "Bookings Report",
          textAlign: TextAlign.start,
          fontSize: 18,
          color: AppColors.black,
          fontStyle: FontStyle.normal,
          fontWeight: FontWeight.w800,
        ),
      ),
      body: SafeArea(
        child: RefreshIndicator(
          onRefresh: () async {
            await notifier.getAllCustomerReservationData(refresh: true);
          },
          child: Builder(
            builder: (context) {
              if (isLoading) {
                return Center(
                  child: CircularProgressIndicator(
                    color: AppColors.primary,
                    strokeWidth: 2.0,
                  ),
                );
              }
              return reservationsList.isEmpty
                  ? Center(
                    child: Column(
                      mainAxisAlignment: MainAxisAlignment.center,
                      crossAxisAlignment: CrossAxisAlignment.center,
                      children: [
                        addVerticalSpacing(15),
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
                          text: "You don't have any Bookings at the moment",
                          textAlign: TextAlign.center,
                          fontSize: 13,
                          color: AppColors.black,
                          fontStyle: FontStyle.normal,
                          fontWeight: FontWeight.normal,
                        ),
                      ],
                    ),
                  )
                  : ListView.builder(
                    itemCount: reservationsList.length,
                    shrinkWrap: true,
                    physics: const NeverScrollableScrollPhysics(),
                    itemBuilder:
                        (context, index) => CustomerItemPage(
                          accomodation: reservationsList[index],
                        ),
                  );
            },
          ),
        ),
      ),
    );
  }
}
