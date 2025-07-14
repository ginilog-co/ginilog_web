import 'package:ginilog_customer_app/core/components/utils/app_buttons.dart';
import 'package:ginilog_customer_app/core/components/utils/colors.dart';
import 'package:ginilog_customer_app/core/components/utils/money_formatter.dart';
import 'package:ginilog_customer_app/core/components/utils/package_export.dart';
import 'package:ginilog_customer_app/core/components/utils/size_config.dart';
import 'package:ginilog_customer_app/core/components/widgets/app_text.dart';
import 'package:ginilog_customer_app/core/components/widgets/back_icon.dart';
import 'package:ginilog_customer_app/core/features/bookings/widget/booking_payment.dart';

class ConfirmAccomodationBookings extends ConsumerStatefulWidget {
  final String reservationId;
  final String reservationName;
  final String reservationAddress;
  final String customerName;
  final String customerPhoneNumber;
  final String customerEmail;
  final int numberOfGuests;
  final String comment;
  final String reservationStartDate;
  final String reservationEndDate;
  final int noOfDays;
  final double amount;

  const ConfirmAccomodationBookings({
    super.key,
    required this.amount,
    required this.reservationId,
    required this.reservationName,
    required this.reservationAddress,
    required this.customerName,
    required this.customerEmail,
    required this.customerPhoneNumber,
    required this.comment,
    required this.noOfDays,
    required this.numberOfGuests,
    required this.reservationEndDate,
    required this.reservationStartDate,
  });

  @override
  ConsumerState<ConfirmAccomodationBookings> createState() =>
      _ConfirmAccomodationBookingsState();
}

class _ConfirmAccomodationBookingsState
    extends ConsumerState<ConfirmAccomodationBookings> {
  bool isLoading = false;
  int selected = 0;
  String paymentMethodUse = "";
  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: AppColors.white,
      appBar: PreferredSize(
        preferredSize: Size.fromHeight(SizeConfig.heightAdjusted(16)),
        child: Padding(
          padding: EdgeInsets.only(top: SizeConfig.heightAdjusted(10)),
          child: GlobalBackButton(
            backText: 'Accomodation Payment',
            showBackButton: true,
            buttonElements: [],
          ),
        ),
      ),
      body: SingleChildScrollView(
        child: Padding(
          padding: const EdgeInsets.only(left: 15.0, right: 15.0, top: 0.0),
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            mainAxisAlignment: MainAxisAlignment.start,
            children: [
              const AppText(
                isBody: true,
                text: "Bookings Summary",
                textAlign: TextAlign.start,
                fontSize: 35,
                color: AppColors.primary,
                fontStyle: FontStyle.normal,
                maxLines: 1,
                fontWeight: FontWeight.bold,
              ),
              Divider(),
              AppText(
                isBody: true,
                text: "Accomodation Details",
                textAlign: TextAlign.start,
                fontSize: 35,
                color: AppColors.black,
                fontStyle: FontStyle.normal,
                maxLines: 1,
                fontWeight: FontWeight.bold,
              ),
              AppText(
                isBody: true,
                text: widget.reservationName,
                textAlign: TextAlign.start,
                fontSize: 35,
                color: AppColors.black,
                fontStyle: FontStyle.normal,
                maxLines: 1,
                fontWeight: FontWeight.w400,
              ),
              AppText(
                isBody: true,
                text: widget.reservationAddress,
                textAlign: TextAlign.start,
                fontSize: 35,
                color: AppColors.black,
                fontStyle: FontStyle.normal,
                maxLines: 1,
                fontWeight: FontWeight.w400,
              ),
              addVerticalSpacing(5),
              AppText(
                isBody: true,
                text: "Customer Details",
                textAlign: TextAlign.start,
                fontSize: 35,
                color: AppColors.black,
                fontStyle: FontStyle.normal,
                maxLines: 1,
                fontWeight: FontWeight.bold,
              ),
              AppText(
                isBody: true,
                text: widget.customerName,
                textAlign: TextAlign.start,
                fontSize: 35,
                color: AppColors.black,
                fontStyle: FontStyle.normal,
                maxLines: 1,
                fontWeight: FontWeight.w400,
              ),
              AppText(
                isBody: true,
                text: widget.customerEmail,
                textAlign: TextAlign.start,
                fontSize: 35,
                color: AppColors.black,
                fontStyle: FontStyle.normal,
                maxLines: 1,
                fontWeight: FontWeight.w400,
              ),
              AppText(
                isBody: true,
                text: widget.customerPhoneNumber,
                textAlign: TextAlign.start,
                fontSize: 35,
                color: AppColors.black,
                fontStyle: FontStyle.normal,
                maxLines: 1,
                fontWeight: FontWeight.w400,
              ),
              addVerticalSpacing(5),
              AppText(
                isBody: true,
                text: "Order Details",
                textAlign: TextAlign.start,
                fontSize: 35,
                color: AppColors.black,
                fontStyle: FontStyle.normal,
                maxLines: 1,
                fontWeight: FontWeight.bold,
              ),
              Row(
                children: [
                  AppText(
                    isBody: true,
                    text: "Number of Guest",
                    textAlign: TextAlign.start,
                    fontSize: 35,
                    color: AppColors.black,
                    fontStyle: FontStyle.normal,
                    maxLines: 1,
                    fontWeight: FontWeight.w400,
                  ),
                  Spacer(),
                  AppText(
                    isBody: true,
                    text: "${widget.numberOfGuests}",
                    textAlign: TextAlign.start,
                    fontSize: 35,
                    color: AppColors.warning,
                    fontStyle: FontStyle.normal,
                    maxLines: 1,
                    fontWeight: FontWeight.w800,
                  ),
                ],
              ),
              Row(
                children: [
                  AppText(
                    isBody: true,
                    text: "Duration",
                    textAlign: TextAlign.start,
                    fontSize: 35,
                    color: AppColors.black,
                    fontStyle: FontStyle.normal,
                    maxLines: 1,
                    fontWeight: FontWeight.w400,
                  ),
                  Spacer(),
                  AppText(
                    isBody: true,
                    text: "${widget.noOfDays} Days",
                    textAlign: TextAlign.start,
                    fontSize: 35,
                    color: AppColors.warning,
                    fontStyle: FontStyle.normal,
                    maxLines: 1,
                    fontWeight: FontWeight.w800,
                  ),
                ],
              ),
              Row(
                children: [
                  AppText(
                    isBody: true,
                    text: "Check In Date",
                    textAlign: TextAlign.start,
                    fontSize: 35,
                    color: AppColors.black,
                    fontStyle: FontStyle.normal,
                    maxLines: 1,
                    fontWeight: FontWeight.w400,
                  ),
                  Spacer(),
                  AppText(
                    isBody: true,
                    text: widget.reservationStartDate,
                    textAlign: TextAlign.start,
                    fontSize: 35,
                    color: AppColors.warning,
                    fontStyle: FontStyle.normal,
                    maxLines: 1,
                    fontWeight: FontWeight.w800,
                  ),
                ],
              ),
              Row(
                children: [
                  AppText(
                    isBody: true,
                    text: "Check Out Date",
                    textAlign: TextAlign.start,
                    fontSize: 35,
                    color: AppColors.black,
                    fontStyle: FontStyle.normal,
                    maxLines: 1,
                    fontWeight: FontWeight.w400,
                  ),
                  Spacer(),
                  AppText(
                    isBody: true,
                    text: widget.reservationEndDate,
                    textAlign: TextAlign.start,
                    fontSize: 35,
                    color: AppColors.warning,
                    fontStyle: FontStyle.normal,
                    maxLines: 1,
                    fontWeight: FontWeight.w800,
                  ),
                ],
              ),
              addVerticalSpacing(5),
              Divider(),
              addVerticalSpacing(5),
              AppText(
                isBody: true,
                text: "Charges",
                textAlign: TextAlign.start,
                fontSize: 35,
                color: AppColors.black,
                fontStyle: FontStyle.normal,
                maxLines: 1,
                fontWeight: FontWeight.bold,
              ),
              Row(
                children: [
                  AppText(
                    isBody: true,
                    text: "Booking Price",
                    textAlign: TextAlign.start,
                    fontSize: 35,
                    color: AppColors.black,
                    fontStyle: FontStyle.normal,
                    maxLines: 1,
                    fontWeight: FontWeight.w400,
                  ),
                  Spacer(),
                  AppText(
                    isBody: true,
                    text: moneyFormat(context, (widget.amount).toDouble()),
                    textAlign: TextAlign.start,
                    fontSize: 35,
                    color: AppColors.warning,
                    fontStyle: FontStyle.normal,
                    maxLines: 1,
                    fontWeight: FontWeight.w800,
                  ),
                ],
              ),
              Divider(),
              Row(
                children: [
                  AppText(
                    isBody: true,
                    text: "Total Cost",
                    textAlign: TextAlign.start,
                    fontSize: 35,
                    color: AppColors.black,
                    fontStyle: FontStyle.normal,
                    maxLines: 1,
                    fontWeight: FontWeight.w400,
                  ),
                  Spacer(),
                  AppText(
                    isBody: true,
                    text: moneyFormat(
                      context,
                      (widget.amount * widget.noOfDays).toDouble(),
                    ),
                    textAlign: TextAlign.start,
                    fontSize: 35,
                    color: AppColors.warning,
                    fontStyle: FontStyle.normal,
                    maxLines: 1,
                    fontWeight: FontWeight.w800,
                  ),
                ],
              ),
              addVerticalSpacing(10),
              Align(
                alignment: Alignment.bottomRight,
                child: AppButton(
                  text: "Make Payment",
                  onPressed: () async {
                    showModalBottomSheet(
                      context: context,
                      isScrollControlled: true,
                      shape: RoundedRectangleBorder(
                        borderRadius: BorderRadius.vertical(
                          top: Radius.circular(20),
                        ),
                      ),
                      builder:
                          (context) => BookingPaymentBottomSheet(
                            amount: widget.amount,
                            reservationId: widget.reservationId,
                            customerName: widget.customerName,
                            customerEmail: widget.customerEmail,
                            customerPhoneNumber: widget.customerPhoneNumber,
                            comment: widget.comment,
                            noOfDays: widget.noOfDays,
                            numberOfGuests: widget.numberOfGuests,
                            reservationEndDate: widget.reservationEndDate,
                            reservationStartDate: widget.reservationStartDate,
                          ),
                    );
                  },
                  widthPercent: 50,
                  heightPercent: 6,
                  btnColor: AppColors.primary,
                  isLoading: isLoading,
                ),
              ),
            ],
          ),
        ),
      ),
    );
  }
}
