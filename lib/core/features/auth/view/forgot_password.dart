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
      appBar: PreferredSize(
        preferredSize: Size.fromHeight(SizeConfig.heightAdjusted(16)),
        child: Padding(
          padding: EdgeInsets.only(top: SizeConfig.heightAdjusted(10)),
          child: const GlobalBackButton(backText: "", showBackButton: true),
        ),
      ),
      body: SingleChildScrollView(
        child: SafeArea(
          child: Form(
            key: controller.formKey,
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Padding(
                  padding: const EdgeInsets.all(18.0),
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      const SizedBox(height: 12),
                      const AppText(
                        isBody: false,
                        text: "Forgot Password",
                        textAlign: TextAlign.center,
                        fontSize: 88,
                        color: AppColors.black,
                        fontStyle: FontStyle.normal,
                        fontWeight: FontWeight.bold,
                      ),
                      const AppText(
                        isBody: true,
                        text: "Enter your email address",
                        textAlign: TextAlign.start,
                        fontSize: 78,
                        color: AppColors.black,
                        fontStyle: FontStyle.normal,
                        fontWeight: FontWeight.w400,
                      ),
                      const SizedBox(height: 40),
                      const AppText(
                        isBody: false,
                        text: "Email Address",
                        textAlign: TextAlign.start,
                        fontSize: 75,
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
                      addVerticalSpacing(context, 5),
                      controller.isEmailChanged.isNotEmpty
                          ? AppButton(
                            text: "Send OTP",
                            onPressed: () {
                              controller.onSubmit();
                            },
                            widthPercent: 100,
                            heightPercent: 6,
                            btnColor: AppColors.primary,
                            isLoading: controller.isLoading,
                          )
                          : AppButton(
                            text: "Send OTP",
                            onPressed: () {},
                            widthPercent: 100,
                            heightPercent: 6,
                            btnColor: AppColors.grey,
                            isLoading: controller.isLoading,
                          ),
                      addVerticalSpacing(context, 5),
                      Row(
                        mainAxisAlignment: MainAxisAlignment.center,
                        children: [
                          const AppText(
                            isBody: true,
                            text: "Remember password? Back to  ",
                            textAlign: TextAlign.center,
                            fontSize: 55,
                            color: AppColors.black,
                            fontStyle: FontStyle.normal,
                            fontWeight: FontWeight.w500,
                          ),
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
                              fontWeight: FontWeight.w700,
                            ),
                          ),
                        ],
                      ),
                    ],
                  ),
                ),
              ],
            ),
          ),
        ),
      ),
    );
  }
}
