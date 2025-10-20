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
                          fontSize: 18.textSize,
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
                      fontSize: 18.textSize,
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
                      fontSize: 18.textSize,
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
                      fontSize: 18.textSize,
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
                  Text(
                    "Phone Number",
                    style: TextStyle(
                      fontSize: 18.textSize,
                      color: AppColors.black,
                      fontWeight: FontWeight.w200,
                      fontFamily: "Inter",
                    ),
                  ),
                  GlobalPhoneTextField(
                    fieldName: 'Phone Number',
                    textController: controller.phoneNo,
                    onChanged: (value) {
                      controller.phoneNoChanged(value!.completeNumber);
                    },
                  ),
                  addVerticalSpacing(1),
                  Text(
                    "Password",
                    style: TextStyle(
                      fontSize: 18.textSize,
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
                        fontSize: 15,
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
                        fontSize: 15,
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
                              height: 10.imageSize,
                            ),
                          ),
                          IconButton(
                            onPressed: () async {
                              controller.apple();
                            },
                            icon: SvgPicture.asset(
                              'assets/svgs/apple.svg',
                              height: 10.imageSize,
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
                            height: 10.imageSize,
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
