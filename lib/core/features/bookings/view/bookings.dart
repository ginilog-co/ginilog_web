import 'package:ginilog_customer_app/core/components/architecture/mvc.dart';
import 'package:ginilog_customer_app/core/components/utils/colors.dart';
import 'package:ginilog_customer_app/core/components/utils/package_export.dart';
import 'package:ginilog_customer_app/core/components/utils/size_config.dart';
import 'package:ginilog_customer_app/core/components/widgets/back_icon.dart';
import 'package:ginilog_customer_app/core/features/bookings/controller/bookings.dart';
import 'package:ginilog_customer_app/core/features/bookings/view/all_booking_list.dart';

class BookingsScreenView
    extends StatelessView<BookingsScreen, BookingsScreenController> {
  const BookingsScreenView(super.state, {super.key});

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: AppColors.white,
      appBar: PreferredSize(
          preferredSize: Size.fromHeight(SizeConfig.heightAdjusted(11)),
          child: Padding(
            padding: EdgeInsets.only(top: SizeConfig.heightAdjusted(10)),
            child: Column(
              children: [
                const GlobalBackButton(
                    backText: 'Bookings', showBackButton: false),
              ],
            ),
          )),
      body: BookingsListTab(
        allAccomodations: controller.allAccomodations,
      ),
    );
  }
}
