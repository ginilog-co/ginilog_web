import 'package:country_code_picker/country_code_picker.dart';
import 'package:ginilog_customer_app/core/components/utils/size_config.dart';
import 'package:ginilog_customer_app/core/components/widgets/back_icon.dart';
import 'package:flutter/gestures.dart';

import '../../../components/architecture/mvc.dart';
import '../../../components/utils/app_buttons.dart';
import '../../../components/utils/colors.dart';
import '../../../components/utils/helper_functions.dart';
import '../../../components/utils/package_export.dart';
import '../../../components/widgets/app_text.dart';
import '../../../components/widgets/input.dart';
import '../controller/login_controller.dart';
import '../controller/user_controller.dart';
import 'dart:io' show Platform;

class RegisterScreenView
    extends StatelessView<RegisterScreen, RegisterScreenController> {
  const RegisterScreenView(super.state, {super.key});

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: AppColors.white,
      appBar: buildFlexibleAppBar(context: context),
      body: SafeArea(
        child: SingleChildScrollView(
          child: Form(
            key: controller.formKey,
            child: Padding(
              padding: EdgeInsets.symmetric(horizontal: 4.widthAdjusted),
              child: Column(
                mainAxisAlignment: MainAxisAlignment.start,
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    mainAxisAlignment: MainAxisAlignment.start,
                    children: [
                      Text(
                        "Create an Account",
                        style: TextStyle(
                          fontSize: 20.textSize,
                          color: AppColors.black,
                          fontWeight: FontWeight.bold,
                          fontFamily: "Inter",
                        ),
                      ),
                      Text(
                        "Complete the sign up process to get started",
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
                  Text(
                    "First Name",
                    style: TextStyle(
                      fontSize: 20.textSize,
                      color: AppColors.black,
                      fontWeight: FontWeight.w200,
                      fontFamily: "Inter",
                    ),
                  ),
                  GlobalTextField(
                    fieldName: 'First Name',
                    keyBoardType: TextInputType.name,
                    obscureText: false,
                    textController: controller.fistNameTEC,
                    onChanged: (String? value) {
                      controller.firstNameOnChanged(value!);
                    },
                  ),
                  addVerticalSpacing(1),
                  Text(
                    "Last Name",
                    style: TextStyle(
                      fontSize: 20.textSize,
                      color: AppColors.black,
                      fontWeight: FontWeight.w200,
                      fontFamily: "Inter",
                    ),
                  ),
                  GlobalTextField(
                    fieldName: 'Last Name',
                    keyBoardType: TextInputType.name,
                    obscureText: false,
                    textController: controller.lastNameTEC,
                    onChanged: (String? value) {
                      controller.lastNameOnChanged(value!);
                    },
                  ),
                  addVerticalSpacing(1),
                  Text(
                    "Email Address",
                    style: TextStyle(
                      fontSize: 20.textSize,
                      color: AppColors.black,
                      fontWeight: FontWeight.w200,
                      fontFamily: "Inter",
                    ),
                  ),
                  GlobalTextField(
                    fieldName: 'Email',
                    keyBoardType: TextInputType.emailAddress,
                    obscureText: false,
                    textController: controller.email,
                    onChanged: (String? value) {
                      controller.emailOnChanged(value!);
                    },
                  ),
                  addVerticalSpacing(1),
                  Row(
                    children: [
                      Container(
                        height: 6.8.heightAdjusted,
                        decoration: BoxDecoration(
                          border: Border(
                            top: BorderSide(color: AppColors.grey2, width: 0.5),
                            bottom: BorderSide(
                              color: AppColors.grey2,
                              width: 0.5,
                            ),
                            left: BorderSide(
                              color: AppColors.grey2,
                              width: 0.5,
                            ),
                            right: BorderSide(
                              color: AppColors.grey2,
                              width: 0.5,
                            ),
                            // left is intentionally omitted
                          ),
                          borderRadius: BorderRadius.only(
                            topRight: Radius.circular(5),
                            topLeft: Radius.circular(5),
                            bottomRight: Radius.circular(5),
                            bottomLeft: Radius.circular(5),
                          ),
                        ),
                        child: CountryCodePicker(
                          showDropDownButton: true,
                          padding: EdgeInsets.all(0),
                          flagWidth: 12,
                          showFlag: false,
                          onChanged: (CountryCode country) {
                            controller.phoneNoCountryCodeChanged(
                              country.dialCode!,
                            );
                          },
                          initialSelection: 'NG', // Default country
                          favorite: [
                            '+1',
                            '+91',
                            '+44',
                          ], // Prioritize commonly used codes
                          showCountryOnly: true,
                          showOnlyCountryWhenClosed: false,
                          alignLeft: false,
                        ),
                      ),
                      addHorizontalSpacing(1),
                      Expanded(
                        child: GlobalTextField(
                          fieldName: 'Phone Number',
                          keyBoardType: TextInputType.phone,
                          obscureText: false,
                          maxLength: 10,
                          textController: controller.phoneNo,
                          onChanged: (String? value) {
                            controller.phoneNoChanged(value!);
                          },
                        ),
                      ),
                    ],
                  ),
                  addVerticalSpacing(1),
                  Text(
                    "Password",
                    style: TextStyle(
                      fontSize: 20.textSize,
                      color: AppColors.black,
                      fontWeight: FontWeight.w200,
                      fontFamily: "Inter",
                    ),
                  ),
                  GlobalTextField(
                    fieldName: 'Password',
                    obscureText: true,
                    isEyeVisible: true,
                    keyBoardType: TextInputType.name,
                    textController: controller.password,
                    onChanged: (String? value) {
                      controller.passwordOnChanged(value!);
                    },
                  ),
                  addVerticalSpacing(3),
                  Row(
                    mainAxisAlignment: MainAxisAlignment.start,
                    crossAxisAlignment: CrossAxisAlignment.center,
                    children: [
                      Checkbox(
                        checkColor: AppColors.white,
                        activeColor: AppColors.primary,
                        value: controller.isChecked,
                        onChanged: (value) {
                          controller.onChanged(value!);
                        },
                      ),
                      Expanded(
                        child: Text.rich(
                          TextSpan(
                            style: TextStyle(
                              fontWeight: FontWeight.w500,
                              fontFamily: "Mulish",
                              color: AppColors.black,
                              fontSize: 15.textSize,
                            ),
                            text: "I Agree to the ",
                            children: <TextSpan>[
                              TextSpan(
                                text: "Terms & Conditions",
                                style: TextStyle(
                                  fontWeight: FontWeight.w500,
                                  fontFamily: "Mulish",
                                  color: AppColors.primary,
                                  fontSize: 18.textSize,
                                ),
                                recognizer:
                                    TapGestureRecognizer()
                                      ..onTap = () {
                                        controller.urlString(
                                          "https://ginilog.com/Home/TermsOfService",
                                        );
                                      },
                              ),
                              TextSpan(
                                text: " & ",
                                style: TextStyle(
                                  fontWeight: FontWeight.w500,
                                  fontFamily: "Mulish",
                                  fontSize: 18.textSize,
                                ),
                              ),
                              TextSpan(
                                text: "Privacy Policy",
                                style: TextStyle(
                                  fontWeight: FontWeight.w500,
                                  fontFamily: "Mulish",
                                  color: AppColors.primary,
                                  fontSize: 18.textSize,
                                ),
                                recognizer:
                                    TapGestureRecognizer()
                                      ..onTap = () {
                                        controller.urlString(
                                          "https://ginilog.com/Home/Privacy",
                                        );
                                      },
                              ),
                            ],
                          ),
                        ),
                      ),
                    ],
                  ),
                  addVerticalSpacing(5),
                  controller.isEmailChanged.isEmpty ||
                          controller.isPasswordChanged.isEmpty ||
                          controller.isFirstNameChanged.isEmpty ||
                          controller.isLastNameChanged.isEmpty ||
                          controller.isChecked == false
                      ? AppButton(
                        text: "Sign Up",
                        onPressed: () {},
                        widthPercent: 100,
                        heightPercent: 6,
                        fontSize: 35,
                        btnColor: AppColors.grey,
                        isLoading: controller.isLoading,
                      )
                      : AppButton(
                        text: "Sign Up",
                        onPressed: () {
                          controller.userRegister();
                        },
                        widthPercent: 100,
                        heightPercent: 6,
                        fontSize: 35,
                        btnColor: AppColors.primary,
                        isLoading: controller.isLoading,
                      ),
                  addVerticalSpacing(5),
                  Row(
                    mainAxisAlignment: MainAxisAlignment.center,
                    children: [
                      const AppText(
                        isBody: true,
                        text: "Already have an account? ",
                        textAlign: TextAlign.center,
                        fontSize: 15,
                        color: AppColors.black,
                        fontStyle: FontStyle.normal,
                        fontWeight: FontWeight.w300,
                      ),
                      GestureDetector(
                        onTap: () {
                          navigateToRoute(context, const LoginScreens());
                        },
                        child: const AppText(
                          isBody: false,
                          text: "Sign In",
                          textAlign: TextAlign.center,
                          fontSize: 20,
                          color: AppColors.blue,
                          fontStyle: FontStyle.normal,
                          fontWeight: FontWeight.w700,
                        ),
                      ),
                    ],
                  ),
                  addVerticalSpacing(5),
                  const Row(
                    mainAxisAlignment: MainAxisAlignment.center,
                    children: [
                      Expanded(
                        child: Divider(
                          color: Color.fromRGBO(218, 218, 218, 1),
                          thickness: 1,
                        ),
                      ),
                      SizedBox(width: 10),
                      AppText(
                        isBody: true,
                        text: "Or",
                        textAlign: TextAlign.center,
                        fontSize: 10,
                        color: AppColors.black,
                        fontStyle: FontStyle.normal,
                        fontWeight: FontWeight.w600,
                      ),
                      SizedBox(width: 10),
                      Expanded(
                        child: Divider(
                          color: Color.fromRGBO(218, 218, 218, 1),
                          thickness: 1,
                        ),
                      ),
                    ],
                  ),
                  addVerticalSpacing(5),
                  Platform.isIOS
                      ? Row(
                        mainAxisAlignment: MainAxisAlignment.center,
                        crossAxisAlignment: CrossAxisAlignment.center,
                        spacing: 5,
                        children: [
                          IconButton(
                            onPressed: () async {
                              controller.google();
                            },
                            icon: SvgPicture.asset(
                              'assets/svgs/google.svg',
                              height: 30,
                              width: 30,
                            ),
                          ),
                          IconButton(
                            onPressed: () async {
                              controller.apple();
                            },
                            icon: SvgPicture.asset(
                              'assets/svgs/apple.svg',
                              height: 30,
                              width: 30,
                            ),
                          ),
                        ],
                      )
                      : Center(
                        child: IconButton(
                          onPressed: () async {
                            controller.google();
                          },
                          icon: SvgPicture.asset(
                            'assets/svgs/google.svg',
                            height: 30,
                            width: 30,
                          ),
                        ),
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
