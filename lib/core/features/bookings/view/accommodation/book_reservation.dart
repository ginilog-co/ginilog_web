import 'package:country_code_picker/country_code_picker.dart';
import 'package:ginilog_customer_app/core/components/architecture/mvc.dart';
import 'package:ginilog_customer_app/core/components/utils/app_buttons.dart';
import 'package:ginilog_customer_app/core/components/utils/colors.dart';
import 'package:ginilog_customer_app/core/components/utils/package_export.dart';
import 'package:ginilog_customer_app/core/components/utils/size_config.dart';
import 'package:ginilog_customer_app/core/components/widgets/app_text.dart';
import 'package:ginilog_customer_app/core/components/widgets/back_icon.dart';
import 'package:ginilog_customer_app/core/components/widgets/input.dart';
import 'package:ginilog_customer_app/core/features/bookings/controller/book_reservation.dart';
import 'package:ginilog_customer_app/core/features/bookings/widget/date_select_widget.dart';

class BookReservationScreenView
    extends
        StatelessView<BookReservationScreen, BookReservationScreenController> {
  const BookReservationScreenView(super.state, {super.key});

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: AppColors.white,
      appBar: buildFlexibleAppBar(
        context: context,

        title: AppText(
          isBody: true,
          text: "",
          textAlign: TextAlign.start,
          fontSize: 18,
          color: AppColors.black,
          fontStyle: FontStyle.normal,
          fontWeight: FontWeight.w800,
        ),
      ),
      body: SafeArea(
        child: SingleChildScrollView(
          child: Form(
            key: controller.formKey,
            child: Padding(
              padding: const EdgeInsets.only(left: 14.0, right: 14.0),
              child: Column(
                mainAxisAlignment: MainAxisAlignment.start,
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Column(
                    crossAxisAlignment: CrossAxisAlignment.center,
                    mainAxisAlignment: MainAxisAlignment.center,
                    children: [
                      Text(
                        "Complete the form below and book the accomodation",
                        style: TextStyle(
                          fontSize: 15.textSize,
                          color: AppColors.black,
                          fontWeight: FontWeight.w400,
                          fontFamily: "Mulish",
                        ),
                      ),
                    ],
                  ),
                  addVerticalSpacing(5),
                  GlobalTextField(
                    fieldName: 'First Name',
                    keyBoardType: TextInputType.name,
                    obscureText: false,
                    textController: controller.fistNameTec,
                    onChanged: (String? value) {
                      controller.firstNameOnChanged(value!);
                    },
                  ),
                  addVerticalSpacing(2),
                  GlobalTextField(
                    fieldName: 'Last Name',
                    keyBoardType: TextInputType.name,
                    obscureText: false,
                    textController: controller.lastNameTEC,
                    onChanged: (String? value) {
                      controller.lastNameOnChanged(value!);
                    },
                  ),
                  addVerticalSpacing(2),
                  GlobalTextField(
                    fieldName: 'Email',
                    keyBoardType: TextInputType.emailAddress,
                    obscureText: false,
                    textController: controller.email,
                    onChanged: (String? value) {
                      controller.emailOnChanged(value!);
                    },
                  ),
                  addVerticalSpacing(2),
                  GlobalPhoneTextField(
                    fieldName: 'Phone Number',
                    textController: controller.phoneNo,
                    onChanged: (value) {
                      controller.phoneNoChanged(value!.completeNumber);
                    },
                  ),

                  addVerticalSpacing(2),
                  GlobalTextField(
                    fieldName: 'Number Of Guest',
                    keyBoardType: TextInputType.number,
                    obscureText: false,
                    textController: controller.numberOfGuest,
                    onChanged: (String? value) {
                      controller.numberOfGuestChanged(value!);
                    },
                  ),
                  addVerticalSpacing(2),
                  GlobalTextField(
                    fieldName: 'Start Date',
                    keyBoardType: TextInputType.text,
                    obscureText: false,
                    readOnly: true,
                    textController: controller.reservationStartDate,
                    onTap: () async {
                      showDialog(
                        context: context,
                        builder:
                            (context) => BookingDateSelect(
                              bookedDates: controller.bookedDates,
                              onDateSelected: (start, end) {
                                controller.reservationStartChanged(
                                  start.toString(),
                                  start,
                                );
                                controller.reservationEndChanged(
                                  end.toString(),
                                  end,
                                );
                              },
                            ),
                      );

                      //  controller.openCalendar(context, true);
                      // DateTime? initialDate;
                      // final selectedDateTime = await controller.pickDateTime(
                      //   context,
                      //   initialDate,
                      // );
                    },
                    onChanged: (String? value) {},
                  ),
                  addVerticalSpacing(2),
                  GlobalTextField(
                    fieldName: 'End Date',
                    keyBoardType: TextInputType.text,
                    obscureText: false,
                    readOnly: true,
                    textController: controller.reservationEndDate,
                    onTap: () async {
                      showDialog(
                        context: context,
                        builder:
                            (context) => BookingDateSelect(
                              bookedDates: controller.bookedDates,
                              onDateSelected: (start, end) {
                                controller.reservationStartChanged(
                                  start.toString(),
                                  start,
                                );
                                controller.reservationEndChanged(
                                  end.toString(),
                                  end,
                                );
                              },
                            ),
                      );
                      // DateTime? initialDate;
                      // final selectedDateTime = await controller.pickDateTime(
                      //   context,
                      //   initialDate,
                      // );
                      // controller.reservationEndChanged(
                      //   selectedDateTime.toString(),
                      //   selectedDateTime,
                      // );
                    },
                    onChanged: (String? value) {},
                  ),
                  addVerticalSpacing(2),
                  GlobalTextField(
                    fieldName: 'Comments',
                    keyBoardType: TextInputType.multiline,
                    isOptional: true,
                    removeSpace: false,
                    obscureText: false,
                    isNotePad: true,
                    maxLength: 200,
                    textController: controller.comment,
                    onChanged: (String? value) {
                      controller.commentOnChanged(value!);
                    },
                  ),
                  addVerticalSpacing(3),
                  controller.isEmailChanged.isEmpty ||
                          controller.isFirstNameChanged.isEmpty ||
                          controller.isPhoneNoChanged.isEmpty ||
                          controller.isLastNameChanged.isEmpty ||
                          controller.isNumberOfGuestChanged.isEmpty ||
                          controller.isReservationStartDateChanged.isEmpty ||
                          controller.isReservationEndDateChanged.isEmpty
                      ? AppButton(
                        text: "Book Now",
                        onPressed: () {},
                        widthPercent: 100,
                        heightPercent: 6,
                        btnColor: AppColors.grey,
                        isLoading: controller.isLoading,
                      )
                      : AppButton(
                        text: "Book Now",
                        onPressed: () {
                          controller.userRegister();
                        },
                        widthPercent: 100,
                        heightPercent: 6,
                        btnColor: AppColors.primary,
                        isLoading: controller.isLoading,
                      ),
                  addVerticalSpacing(5),
                ],
              ),
            ),
          ),
        ),
      ),
    );
  }
}
