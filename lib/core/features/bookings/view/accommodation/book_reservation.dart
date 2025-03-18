import 'package:country_code_picker/country_code_picker.dart';
import 'package:ginilog_customer_app/core/components/architecture/mvc.dart';
import 'package:ginilog_customer_app/core/components/utils/app_buttons.dart';
import 'package:ginilog_customer_app/core/components/utils/colors.dart';
import 'package:ginilog_customer_app/core/components/utils/helper_functions.dart';
import 'package:ginilog_customer_app/core/components/utils/package_export.dart';
import 'package:ginilog_customer_app/core/components/utils/size_config.dart';
import 'package:ginilog_customer_app/core/components/widgets/app_text.dart';
import 'package:ginilog_customer_app/core/components/widgets/back_icon.dart';
import 'package:ginilog_customer_app/core/components/widgets/input.dart';
import 'package:ginilog_customer_app/core/features/bookings/controller/book_reservation.dart';

class BookReservationScreenView extends StatelessView<BookReservationScreen,
    BookReservationScreenController> {
  const BookReservationScreenView(super.state, {super.key});

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: AppColors.white,
      appBar: PreferredSize(
          preferredSize: Size.fromHeight(SizeConfig.heightAdjusted(14)),
          child: Padding(
            padding: EdgeInsets.only(top: SizeConfig.heightAdjusted(10)),
            child: const GlobalBackButton(backText: "", showBackButton: true),
          )),
      body: Container(
        height: getScreenHeight(context),
        width: getScreenWidth(context),
        color: AppColors.white,
        child: SingleChildScrollView(
          child: Form(
            key: controller.formKey,
            child: Padding(
              padding: const EdgeInsets.only(
                left: 14.0,
                right: 14.0,
              ),
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
                            fontSize: fontSized(context, 65),
                            color: AppColors.black,
                            fontWeight: FontWeight.w400,
                            fontFamily: "Mulish"),
                      ),
                    ],
                  ),
                  addVerticalSpacing(context, 5),
                  GlobalTextField(
                    fieldName: 'First Name',
                    keyBoardType: TextInputType.name,
                    obscureText: false,
                    textController: controller.fistNameTec,
                    onChanged: (String? value) {
                      controller.firstNameOnChanged(value!);
                    },
                  ),
                  addVerticalSpacing(context, 5),
                  GlobalTextField(
                    fieldName: 'Last Name',
                    keyBoardType: TextInputType.name,
                    obscureText: false,
                    textController: controller.lastNameTEC,
                    onChanged: (String? value) {
                      controller.lastNameOnChanged(value!);
                    },
                  ),
                  addVerticalSpacing(context, 5),
                  GlobalTextField(
                    fieldName: 'Email',
                    keyBoardType: TextInputType.emailAddress,
                    obscureText: false,
                    textController: controller.email,
                    onChanged: (String? value) {
                      controller.emailOnChanged(value!);
                    },
                  ),
                  addVerticalSpacing(context, 5),
                  SizedBox(
                    height: 50,
                    child: GlobalTextField(
                      prefix: CountryCodePicker(
                        padding: EdgeInsets.all(0),
                        flagWidth: 12,
                        showFlag: true,
                        onChanged: (CountryCode country) {
                          controller
                              .phoneNoCountryCodeChanged(country.dialCode!);
                        },
                        initialSelection: 'US', // Default country
                        favorite: [
                          '+1',
                          '+91',
                          '+44'
                        ], // Prioritize commonly used codes
                        showCountryOnly: true,
                        showOnlyCountryWhenClosed: false,
                        alignLeft: false,
                      ),
                      fieldName: 'Phone Number',
                      keyBoardType: TextInputType.phone,
                      obscureText: false,
                      textController: controller.phoneNo,
                      onChanged: (String? value) {
                        controller.phoneNoChanged(value!);
                      },
                    ),
                  ),
                  addVerticalSpacing(context, 5),
                  GlobalTextField(
                    fieldName: 'Number Of Guest',
                    keyBoardType: TextInputType.number,
                    obscureText: false,
                    textController: controller.numberOfGuest,
                    onChanged: (String? value) {
                      controller.numberOfGuestChanged(value!);
                    },
                  ),
                  addVerticalSpacing(context, 5),
                  GlobalTextField(
                    fieldName: 'Start Date',
                    keyBoardType: TextInputType.text,
                    obscureText: false,
                    readOnly: true,
                    textController: controller.reservationStartDate,
                    onTap: () async {
                      DateTime? initialDate;
                      final selectedDateTime =
                          await controller.pickDateTime(context, initialDate);
                      controller.reservationStartChanged(
                          selectedDateTime.toString(), selectedDateTime);
                    },
                    onChanged: (String? value) {},
                  ),
                  addVerticalSpacing(context, 5),
                  GlobalTextField(
                    fieldName: 'End Date',
                    keyBoardType: TextInputType.text,
                    obscureText: false,
                    readOnly: true,
                    textController: controller.reservationEndDate,
                    onTap: () async {
                      DateTime? initialDate;
                      final selectedDateTime =
                          await controller.pickDateTime(context, initialDate);
                      controller.reservationEndChanged(
                          selectedDateTime.toString(), selectedDateTime);
                    },
                    onChanged: (String? value) {},
                  ),
                  addVerticalSpacing(context, 5),
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
                  addVerticalSpacing(context, 5),
                  controller.isEmailChanged.isEmpty ||
                          controller.isFirstNameChanged.isEmpty ||
                          controller.isPhoneNoChanged.isEmpty ||
                          controller.isLastNameChanged.isEmpty ||
                          controller.isNumberOfGuestChanged.isEmpty ||
                          controller.isReservationStartDateChanged.isEmpty ||
                          controller.isReservationEndDateChanged.isEmpty
                      ? appButton("Book Now", getScreenWidth(context), () {},
                          AppColors.grey, controller.isLoading)
                      : appButton("Book Now", getScreenWidth(context), () {
                          controller.userRegister();
                        }, AppColors.primary, controller.isLoading),
                  addVerticalSpacing(context, 5),
                  addVerticalSpacing(context, 5),
                ],
              ),
            ),
          ),
        ),
      ),
    );
  }
}
