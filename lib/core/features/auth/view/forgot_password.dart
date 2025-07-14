import 'package:flutter/gestures.dart';
import 'package:ginilog_customer_app/core/components/utils/size_config.dart';
import 'package:ginilog_customer_app/core/components/widgets/back_icon.dart';
import 'package:flutter/material.dart';
import 'package:ginilog_customer_app/core/features/auth/controller/login_controller.dart';

import '../../../components/architecture/mvc.dart';
import '../../../components/utils/app_buttons.dart';
import '../../../components/utils/colors.dart';
import '../../../components/utils/helper_functions.dart';
import '../../../components/widgets/app_text.dart';
import '../../../components/widgets/input.dart';
import '../controller/forgot_password.dart';

class ForgotPasswordView
    extends StatelessView<ForgotPasswordScreen, ForgotPasswordController> {
  const ForgotPasswordView(super.state, {super.key});

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
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  const AppText(
                    isBody: false,
                    text: "Forgot Password",
                    textAlign: TextAlign.center,
                    fontSize: 28,
                    color: AppColors.black,
                    fontStyle: FontStyle.normal,
                    fontWeight: FontWeight.bold,
                  ),
                  const AppText(
                    isBody: true,
                    text: "Enter your email address",
                    textAlign: TextAlign.start,
                    fontSize: 28,
                    color: AppColors.black,
                    fontStyle: FontStyle.normal,
                    fontWeight: FontWeight.w400,
                  ),
                  addVerticalSpacing(2),
                  const AppText(
                    isBody: false,
                    text: "Email Address",
                    textAlign: TextAlign.start,
                    fontSize: 25,
                    color: AppColors.black,
                    fontStyle: FontStyle.normal,
                    fontWeight: FontWeight.w400,
                  ),
                  GlobalTextField(
                    fieldName: 'Email',
                    keyBoardType: TextInputType.emailAddress,
                    obscureText: false,
                    textController: controller.emailController,
                    onChanged: (String? value) {
                      controller.emailOnChanged(value!);
                    },
                  ),
                  addVerticalSpacing(2),
                  controller.isEmailChanged.isNotEmpty
                      ? AppButton(
                        text: "Send OTP",
                        onPressed: () {
                          controller.onSubmit();
                        },
                        widthPercent: 100,
                        heightPercent: 6,
                        fontSize: 20,
                        isLoading: controller.isLoading,
                      )
                      : AppButton(
                        text: "Send OTP",
                        onPressed: () {},
                        widthPercent: 100,
                        heightPercent: 6,
                        fontSize: 20,
                        btnColor: AppColors.grey,
                        isLoading: false,
                      ),
                  addVerticalSpacing(2),

                  Center(
                    child: Text.rich(
                      textAlign: TextAlign.center,
                      TextSpan(
                        style: TextStyle(
                          fontWeight: FontWeight.w500,
                          fontFamily: "Inter",
                          color: AppColors.black,
                          fontSize: 20.textSize,
                        ),
                        text: "Remember password? Back to  ",
                        children: <TextSpan>[
                          TextSpan(
                            text: " Sign In",
                            style: TextStyle(
                              fontWeight: FontWeight.w500,
                              fontFamily: "Mulish",
                              color: AppColors.primary,
                              fontSize: 25.textSize,
                            ),
                            recognizer:
                                TapGestureRecognizer()
                                  ..onTap = () {
                                    navigateToRoute(
                                      context,
                                      const LoginScreens(),
                                    );
                                  },
                          ),
                        ],
                      ),
                    ),
                  ),
                ],
              ),
            ),
          ),
        ),
      ),
    );
  }
}
