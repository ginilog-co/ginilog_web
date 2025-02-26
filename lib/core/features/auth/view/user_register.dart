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
                    crossAxisAlignment: CrossAxisAlignment.start,
                    mainAxisAlignment: MainAxisAlignment.start,
                    children: [
                      Text(
                        "Create an Account",
                        style: TextStyle(
                            fontSize: fontSized(context, 107),
                            color: AppColors.black,
                            fontWeight: FontWeight.bold,
                            fontFamily: "Inter"),
                      ),
                      Text(
                        "Complete the sign up process to get started",
                        style: TextStyle(
                            fontSize: fontSized(context, 65),
                            color: AppColors.black,
                            fontWeight: FontWeight.w400,
                            fontFamily: "Montserrat"),
                      ),
                    ],
                  ),
                  addVerticalSpacing(context, 5),
                  Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Text(
                        "First Name",
                        style: TextStyle(
                            fontSize: fontSized(context, 80),
                            color: AppColors.black,
                            fontWeight: FontWeight.w200,
                            fontFamily: "Inter"),
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
                    ],
                  ),
                  addVerticalSpacing(context, 5),
                  Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Text(
                        "Last Name",
                        style: TextStyle(
                            fontSize: fontSized(context, 80),
                            color: AppColors.black,
                            fontWeight: FontWeight.w200,
                            fontFamily: "Inter"),
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
                    ],
                  ),
                  addVerticalSpacing(context, 5),
                  Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Text(
                        "Email Address",
                        style: TextStyle(
                            fontSize: fontSized(context, 80),
                            color: AppColors.black,
                            fontWeight: FontWeight.w200,
                            fontFamily: "Inter"),
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
                    ],
                  ),
                  addVerticalSpacing(context, 5),
                  Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Text(
                        "Phone Number",
                        style: TextStyle(
                            fontSize: fontSized(context, 80),
                            color: AppColors.black,
                            fontWeight: FontWeight.w200,
                            fontFamily: "Inter"),
                      ),
                      GlobalTextField(
                        fieldName: 'Phone Number',
                        keyBoardType: TextInputType.phone,
                        obscureText: false,
                        maxLength: 10,
                        textController: controller.phoneNo,
                        onChanged: (String? value) {
                          controller.phoneNoChanged(value!);
                        },
                      ),
                    ],
                  ),
                  addVerticalSpacing(context, 5),
                  Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Text(
                        "Password",
                        style: TextStyle(
                            fontSize: fontSized(context, 80),
                            color: AppColors.black,
                            fontWeight: FontWeight.w200,
                            fontFamily: "Inter"),
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
                    ],
                  ),
                  addVerticalSpacing(context, 5),
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
                                fontFamily: "Montserrat",
                                color: AppColors.black,
                                fontSize: fontSized(context, 65)),
                            text: "I Agree to the ",
                            children: <TextSpan>[
                              TextSpan(
                                text: "Terms & Conditions",
                                style: TextStyle(
                                    fontWeight: FontWeight.w500,
                                    fontFamily: "Montserrat",
                                    color: AppColors.primary,
                                    fontSize: fontSized(context, 65)),
                                recognizer: TapGestureRecognizer()
                                  ..onTap = () {
                                    controller.urlString(
                                        "https://bringmygas.com/Home/TermsOfService");
                                  },
                              ),
                              TextSpan(
                                text: " & ",
                                style: TextStyle(
                                    fontWeight: FontWeight.w500,
                                    fontFamily: "Montserrat",
                                    fontSize: fontSized(context, 65)),
                              ),
                              TextSpan(
                                text: "Privacy Policy",
                                style: TextStyle(
                                    fontWeight: FontWeight.w500,
                                    fontFamily: "Montserrat",
                                    color: AppColors.primary,
                                    fontSize: fontSized(context, 65)),
                                recognizer: TapGestureRecognizer()
                                  ..onTap = () {
                                    controller.urlString(
                                        "https://bringmygas.com/Home/Privacy");
                                  },
                              ),
                            ],
                          ),
                        ),
                      ),
                    ],
                  ),
                  addVerticalSpacing(context, 5),
                  controller.isEmailChanged.isEmpty ||
                          controller.isPasswordChanged.isEmpty ||
                          controller.isFirstNameChanged.isEmpty ||
                          controller.isLastNameChanged.isEmpty ||
                          controller.isChecked == false
                      ? appButton("Sign Up", getScreenWidth(context), () {},
                          AppColors.grey, controller.isLoading)
                      : appButton("Sign Up", getScreenWidth(context), () {
                          controller.userRegister();
                        }, AppColors.primary, controller.isLoading),
                  addVerticalSpacing(context, 5),
                  Row(
                    mainAxisAlignment: MainAxisAlignment.center,
                    children: [
                      const AppText(
                          isBody: true,
                          text: "Already have an account? ",
                          textAlign: TextAlign.center,
                          fontSize: 55,
                          color: AppColors.black,
                          fontStyle: FontStyle.normal,
                          fontWeight: FontWeight.w500),
                      GestureDetector(
                        onTap: () {
                          navigateToRoute(context, const LoginScreens());
                        },
                        child: const AppText(
                            isBody: false,
                            text: "Sign In",
                            textAlign: TextAlign.center,
                            fontSize: 78,
                            color: AppColors.blue,
                            fontStyle: FontStyle.normal,
                            fontWeight: FontWeight.w700),
                      )
                    ],
                  ),
                  addVerticalSpacing(context, 5),
                  const Row(
                    mainAxisAlignment: MainAxisAlignment.center,
                    children: [
                      Expanded(
                        child: Divider(
                          color: Color.fromRGBO(218, 218, 218, 1),
                          thickness: 1,
                        ),
                      ),
                      SizedBox(
                        width: 10,
                      ),
                      AppText(
                          isBody: true,
                          text: "Or",
                          textAlign: TextAlign.center,
                          fontSize: 30,
                          color: AppColors.black,
                          fontStyle: FontStyle.normal,
                          fontWeight: FontWeight.w600),
                      SizedBox(
                        width: 10,
                      ),
                      Expanded(
                        child: Divider(
                          color: Color.fromRGBO(218, 218, 218, 1),
                          thickness: 1,
                        ),
                      ),
                    ],
                  ),
                  addVerticalSpacing(context, 5),
                  Platform.isIOS
                      ? Row(
                          mainAxisAlignment: MainAxisAlignment.center,
                          crossAxisAlignment: CrossAxisAlignment.center,
                          spacing: 5,
                          children: [
                            IconButton(
                              onPressed: () async {
                                //   controller.google();
                              },
                              icon: SvgPicture.asset(
                                'assets/svgs/google.svg',
                                height: 30,
                                width: 30,
                              ),
                            ),
                            IconButton(
                              onPressed: () async {
                                //   controller.google();
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
                              //   controller.google();
                            },
                            icon: SvgPicture.asset(
                              'assets/svgs/google.svg',
                              height: 30,
                              width: 30,
                            ),
                          ),
                        ),
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
