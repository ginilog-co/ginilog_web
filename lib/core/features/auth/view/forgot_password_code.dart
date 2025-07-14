import 'package:ginilog_customer_app/core/components/utils/size_config.dart';
import 'package:ginilog_customer_app/core/components/widgets/back_icon.dart';

import '../../../components/architecture/mvc.dart';
import '../../../components/utils/app_buttons.dart';
import '../../../components/utils/colors.dart';
import '../../../components/utils/package_export.dart';
import '../../../components/widgets/app_text.dart';
import '../../../components/widgets/input.dart';
import '../controller/forgot_password_code.dart';

class ForgotPasswordCodeScreenView
    extends
        StatelessView<ForgotPasswordCodeScreen, ForgotPasswordCodeController> {
  const ForgotPasswordCodeScreenView(super.state, {super.key});

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
                mainAxisAlignment: MainAxisAlignment.start,
                children: [
                  const AppText(
                    isBody: false,
                    text: "Create New Password",
                    textAlign: TextAlign.start,
                    fontSize: 25,
                    color: AppColors.black,
                    fontStyle: FontStyle.normal,
                    fontWeight: FontWeight.bold,
                  ),
                  SizedBox(height: 1.heightAdjusted),
                  const AppText(
                    isBody: true,
                    text:
                        "Please enter the your new password if you forget it, then click on forget password",
                    textAlign: TextAlign.start,
                    fontSize: 18,
                    color: AppColors.black,
                    fontStyle: FontStyle.normal,
                    fontWeight: FontWeight.w400,
                  ),
                  addVerticalSpacing(2),
                  const AppText(
                    isBody: false,
                    text: "OTP",
                    textAlign: TextAlign.start,
                    fontSize: 25,
                    color: AppColors.black,
                    fontStyle: FontStyle.normal,
                    fontWeight: FontWeight.w400,
                  ),
                  GlobalTextField(
                    fieldName: 'OTP Code',
                    keyBoardType: TextInputType.number,
                    obscureText: false,
                    maxLength: 5,
                    textController: controller.codeController,
                    onChanged: (String? value) {
                      controller.otpOnChanged(value!);
                    },
                  ),
                  addVerticalSpacing(2),
                  const AppText(
                    isBody: false,
                    text: "New Password",
                    textAlign: TextAlign.start,
                    fontSize: 25,
                    color: AppColors.black,
                    fontStyle: FontStyle.normal,
                    fontWeight: FontWeight.w400,
                  ),
                  GlobalTextField(
                    fieldName: 'New Password',
                    keyBoardType: TextInputType.text,
                    obscureText: true,
                    isEyeVisible: true,
                    textController: controller.newPasswordController,
                    onChanged: (String? value) {
                      controller.newPasswordOnChanged(value!);
                    },
                  ),
                  addVerticalSpacing(2),
                  const AppText(
                    isBody: false,
                    text: "Confirm Password",
                    textAlign: TextAlign.start,
                    fontSize: 25,
                    color: AppColors.black,
                    fontStyle: FontStyle.normal,
                    fontWeight: FontWeight.w400,
                  ),
                  GlobalTextField(
                    fieldName: 'Confirm Password',
                    keyBoardType: TextInputType.text,
                    obscureText: true,
                    isEyeVisible: true,
                    confirmPasswordMatch:
                        controller.newPasswordController.text, // Compare here
                    textController: controller.confirmPasswordController,
                    onChanged: (String? value) {
                      controller.confirmPasswordOnChanged(value!);
                    },
                  ),
                  addVerticalSpacing(2),
                  controller.isOTPChanged.isEmpty ||
                          controller.isNewPasswordChanged.isEmpty ||
                          controller.isConfirmPasswordChanged.isEmpty ||
                          controller.isConfirmPasswordChanged !=
                              controller.isNewPasswordChanged
                      ? AppButton(
                        text: "Update Password",
                        onPressed: () {},
                        widthPercent: 100,
                        heightPercent: 6,
                        btnColor: AppColors.grey,
                        fontSize: 20,
                        // safeArea: true,
                        isLoading: false,
                      )
                      : AppButton(
                        text: "Update Password",
                        onPressed: () {
                          controller.onSubmit();
                        },
                        widthPercent: 100,
                        heightPercent: 6,
                        fontSize: 20,
                        isLoading: controller.isLoading,
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
