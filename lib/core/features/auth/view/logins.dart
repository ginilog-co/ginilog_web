import 'package:flutter/gestures.dart';
import 'package:ginilog_customer_app/core/components/helpers/globals.dart';
import 'package:ginilog_customer_app/core/components/utils/package_export.dart';
import 'package:ginilog_customer_app/core/components/utils/size_config.dart';
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
      body: SafeArea(
        child: SingleChildScrollView(
          child: Form(
            key: controller.formKey,
            child: Padding(
              padding: EdgeInsets.symmetric(
                horizontal: 4.widthAdjusted,
                vertical: 4.heightAdjusted,
              ),
              child: Column(
                mainAxisAlignment: MainAxisAlignment.start,
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    mainAxisAlignment: MainAxisAlignment.start,
                    children: [
                      addVerticalSpacing(2),
                      AppText(
                        isBody: false,
                        text:
                            globals.userName!.isEmpty
                                ? "Login"
                                : "Welcome back, ${globals.userName}",
                        textAlign: TextAlign.start,
                        fontSize: 18,
                        color: AppColors.black,
                        fontStyle: FontStyle.normal,
                        fontWeight: FontWeight.bold,
                      ),
                      addVerticalSpacing(1),
                      const AppText(
                        isBody: true,
                        text:
                            "Enter username and password to login to your account",
                        textAlign: TextAlign.start,
                        fontSize: 15,
                        color: AppColors.black,
                        fontStyle: FontStyle.normal,
                        fontWeight: FontWeight.w500,
                      ),
                    ],
                  ),
                  globals.userEmail!.isEmpty
                      ? addVerticalSpacing(2)
                      : SizedBox.shrink(),
                  globals.userEmail!.isEmpty
                      ? const AppText(
                        isBody: true,
                        text: "Email Address",
                        textAlign: TextAlign.start,
                        fontSize: 18,
                        color: AppColors.black,
                        fontStyle: FontStyle.normal,
                        fontWeight: FontWeight.w500,
                      )
                      : SizedBox.shrink(),
                  globals.userEmail!.isEmpty
                      ? GlobalTextField(
                        fieldName: 'Enter Email',
                        suffix: Icon(
                          Icons.email,
                          size: 2.imageSize,
                          color: AppColors.grey,
                        ),
                        keyBoardType: TextInputType.emailAddress,
                        obscureText: false,
                        textController: controller.emailController,
                        onChanged: (String? value) {
                          controller.emailOnChanged(value!);
                        },
                      )
                      : SizedBox.shrink(),
                  addVerticalSpacing(2),
                  const AppText(
                    isBody: true,
                    text: "Enter Password",
                    textAlign: TextAlign.start,
                    fontSize: 18,
                    color: AppColors.black,
                    fontStyle: FontStyle.normal,
                    fontWeight: FontWeight.w500,
                  ),
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
                  addVerticalSpacing(2),

                  Row(
                    mainAxisAlignment: MainAxisAlignment.end,
                    crossAxisAlignment: CrossAxisAlignment.end,
                    children: [
                      TextButton(
                        onPressed: () {
                          navigateToRoute(
                            context,
                            const ForgotPasswordScreen(),
                          );
                        },
                        child: const AppText(
                          isBody: true,
                          text: "Forgot Password?",
                          textAlign: TextAlign.end,
                          fontSize: 18,
                          color: AppColors.primary,
                          fontStyle: FontStyle.normal,
                          fontWeight: FontWeight.w700,
                        ),
                      ),
                    ],
                  ),
                  addVerticalSpacing(2),
                  controller.isEmailChanged.isEmpty ||
                          controller.isPasswordChanged.isEmpty
                      ? AppButton(
                        text: "Proceed",
                        onPressed: () {},
                        widthPercent: 100,
                        heightPercent: 6,
                        fontSize: 15,
                        textColor: AppColors.black,
                        btnColor: AppColors.grey,
                        isLoading: controller.isLoading,
                        borderRadius: 25,
                      )
                      : AppButton(
                        text: "Proceed",
                        onPressed: () {
                          controller.loginUser();
                        },
                        widthPercent: 100,
                        heightPercent: 6,
                        borderRadius: 25,
                        fontSize: 15,
                        btnColor: AppColors.primary,
                        isLoading: controller.isLoading,
                      ),
                  addVerticalSpacing(2),
                  Center(
                    child: Text.rich(
                      textAlign: TextAlign.center,
                      TextSpan(
                        style: TextStyle(
                          fontWeight: FontWeight.w500,
                          fontFamily: "Mulish",
                          color: AppColors.black,
                          fontSize: 15.textSize,
                        ),
                        text: "Don't have an account? ",
                        children: <TextSpan>[
                          TextSpan(
                            text: "Sign Up",
                            style: TextStyle(
                              fontWeight: FontWeight.w500,
                              fontFamily: "Mulish",
                              color: AppColors.primary,
                              fontSize: 20.textSize,
                            ),
                            recognizer:
                                TapGestureRecognizer()
                                  ..onTap = () {
                                    navigateToRoute(
                                      context,
                                      const RegisterScreen(),
                                    );
                                  },
                          ),
                        ],
                      ),
                    ),
                  ),

                  addVerticalSpacing(2),
                  Row(
                    mainAxisAlignment: MainAxisAlignment.center,
                    children: [
                      Expanded(
                        child: Divider(
                          color: Color.fromRGBO(218, 218, 218, 1),
                          thickness: 1,
                        ),
                      ),
                      addHorizontalSpacing(2),
                      AppText(
                        isBody: true,
                        text: "Or",
                        textAlign: TextAlign.center,
                        fontSize: 15,
                        color: AppColors.black,
                        fontStyle: FontStyle.normal,
                        fontWeight: FontWeight.w600,
                      ),
                      addHorizontalSpacing(2),
                      Expanded(
                        child: Divider(
                          color: Color.fromRGBO(218, 218, 218, 1),
                          thickness: 1,
                        ),
                      ),
                    ],
                  ),
                  addVerticalSpacing(2),
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
                  addVerticalSpacing(2),
                ],
              ),
            ),
          ),
        ),
      ),
    );
  }
}
