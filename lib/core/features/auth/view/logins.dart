import 'package:flutter/gestures.dart';
import 'package:ginilog_customer_app/core/components/utils/package_export.dart';
import '../../../components/architecture/mvc.dart';
import '../../../components/utils/app_buttons.dart';
import '../../../components/utils/colors.dart';
import '../../../components/utils/helper_functions.dart';
import '../../../components/widgets/app_text.dart';
import '../../../components/widgets/input.dart';
import '../controller/forgot_password.dart';
import '../controller/login_controller.dart';
import '../controller/user_controller.dart';
import 'dart:io' show Platform;

class LoginScreensView
    extends StatelessView<LoginScreens, LoginScreensController> {
  const LoginScreensView(super.state, {super.key});

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: AppColors.white,
      appBar: AppBar(
        backgroundColor: AppColors.white,
        surfaceTintColor: AppColors.white,
        elevation: 0,
        automaticallyImplyLeading: false,
      ),
      body: Container(
        height: getScreenHeight(context),
        width: getScreenWidth(context),
        color: AppColors.white,
        child: SingleChildScrollView(
          child: Form(
            key: controller.formKey,
            child: Padding(
              padding: const EdgeInsets.only(left: 14, right: 14, top: 20),
              child: Column(
                mainAxisAlignment: MainAxisAlignment.start,
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    mainAxisAlignment: MainAxisAlignment.start,
                    children: [
                      const AppText(
                          isBody: false,
                          text: "Welcome Back!",
                          textAlign: TextAlign.start,
                          fontSize: 100,
                          color: AppColors.black,
                          fontStyle: FontStyle.normal,
                          fontWeight: FontWeight.bold),
                      addVerticalSpacing(context, 1),
                      const AppText(
                          isBody: true,
                          text: "Fill in your email and password to continue",
                          textAlign: TextAlign.start,
                          fontSize: 80,
                          color: AppColors.black,
                          fontStyle: FontStyle.normal,
                          fontWeight: FontWeight.w500),
                    ],
                  ),
                  addVerticalSpacing(context, 15),
                  const AppText(
                      isBody: true,
                      text: "Email",
                      textAlign: TextAlign.start,
                      fontSize: 80,
                      color: AppColors.black,
                      fontStyle: FontStyle.normal,
                      fontWeight: FontWeight.w500),
                  GlobalTextField(
                    fieldName: 'Email',
                    keyBoardType: TextInputType.emailAddress,
                    obscureText: false,
                    textController: controller.emailController,
                    onChanged: (String? value) {
                      controller.emailOnChanged(value!);
                    },
                  ),
                  addVerticalSpacing(context, 5),
                  const AppText(
                      isBody: true,
                      text: "Password",
                      textAlign: TextAlign.start,
                      fontSize: 80,
                      color: AppColors.black,
                      fontStyle: FontStyle.normal,
                      fontWeight: FontWeight.w500),
                  GlobalTextField(
                    fieldName: 'Password',
                    obscureText: true,
                    isEyeVisible: true,
                    isNotePad: false,
                    keyBoardType: TextInputType.name,
                    textController: controller.passwordController,
                    onChanged: (String? value) {
                      controller.passwordOnChanged(value!);
                    },
                  ),
                  addVerticalSpacing(context, 3),
                  Row(
                    mainAxisAlignment: MainAxisAlignment.end,
                    children: [
                      GestureDetector(
                        onTap: () {
                          navigateToRoute(
                              context, const ForgotPasswordScreen());
                        },
                        child: const AppText(
                            isBody: true,
                            text: "Forgot Password?",
                            textAlign: TextAlign.end,
                            fontSize: 50,
                            color: AppColors.blue,
                            fontStyle: FontStyle.normal,
                            fontWeight: FontWeight.w700),
                      )
                    ],
                  ),
                  addVerticalSpacing(context, 4),
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
                                fontSize: fontSized(context, 65)),
                            text: "I Agree to the ",
                            children: <TextSpan>[
                              TextSpan(
                                text: "Terms & Conditions",
                                style: TextStyle(
                                    fontWeight: FontWeight.w500,
                                    fontFamily: "Mulish",
                                    color: AppColors.primary,
                                    fontSize: fontSized(context, 65)),
                                recognizer: TapGestureRecognizer()
                                  ..onTap = () {
                                    controller.urlString(
                                        "https://ginilog.com/Home/TermsOfService");
                                  },
                              ),
                              TextSpan(
                                text: " & ",
                                style: TextStyle(
                                    fontWeight: FontWeight.w500,
                                    fontFamily: "Mulish",
                                    fontSize: fontSized(context, 65)),
                              ),
                              TextSpan(
                                text: "Privacy Policy",
                                style: TextStyle(
                                    fontWeight: FontWeight.w500,
                                    fontFamily: "Mulish",
                                    color: AppColors.primary,
                                    fontSize: fontSized(context, 65)),
                                recognizer: TapGestureRecognizer()
                                  ..onTap = () {
                                    controller.urlString(
                                        "https://ginilog.com/Home/Privacy");
                                  },
                              ),
                            ],
                          ),
                        ),
                      ),
                    ],
                  ),
                  addVerticalSpacing(context, 20),
                  controller.isEmailChanged.isEmpty ||
                          controller.isPasswordChanged.isEmpty ||
                          controller.isChecked == false
                      ? appButton("LOG IN", getScreenWidth(context), () {},
                          AppColors.grey, controller.isLoading)
                      : appButton("LOG IN", getScreenWidth(context), () {
                          controller.loginUser();
                        }, AppColors.primary, controller.isLoading),
                  addVerticalSpacing(context, 5),
                  Row(
                    mainAxisAlignment: MainAxisAlignment.center,
                    children: [
                      const AppText(
                          isBody: true,
                          text: "Don't have an account? ",
                          textAlign: TextAlign.center,
                          fontSize: 55,
                          color: AppColors.black,
                          fontStyle: FontStyle.normal,
                          fontWeight: FontWeight.w500),
                      GestureDetector(
                        onTap: () {
                          navigateToRoute(context, const RegisterScreen());
                        },
                        child: const AppText(
                            isBody: false,
                            text: "Sign Up",
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
                        )
                ],
              ),
            ),
          ),
        ),
      ),
    );
  }
}
