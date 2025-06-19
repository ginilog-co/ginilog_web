import 'package:ginilog_customer_app/core/components/utils/app_buttons.dart';
import 'package:ginilog_customer_app/core/components/utils/colors.dart';
import 'package:ginilog_customer_app/core/components/utils/helper_functions.dart';
import 'package:ginilog_customer_app/core/components/widgets/app_text.dart';
import 'package:ginilog_customer_app/core/features/auth/controller/login_controller.dart';

import '../../../components/utils/package_export.dart';

class EmailSentDialog extends StatefulWidget {
  const EmailSentDialog({super.key});

  @override
  State<EmailSentDialog> createState() => _EmailSentDialogState();
}

class _EmailSentDialogState extends State<EmailSentDialog> {
  @override
  Widget build(BuildContext context) {
    // ignore: deprecated_member_use
    return WillPopScope(
      onWillPop: () async => false,
      child: Dialog(
        shape: const RoundedRectangleBorder(),
        elevation: 0,
        backgroundColor: Colors.transparent,
        child: Container(
          width: double.infinity,
          decoration: BoxDecoration(
            color: AppColors.white,
            borderRadius: BorderRadius.circular(30),
          ),
          child: Padding(
            padding: const EdgeInsets.symmetric(horizontal: 20, vertical: 20),
            child: Column(
              mainAxisAlignment: MainAxisAlignment.center,
              mainAxisSize: MainAxisSize.min,
              children: [
                addVerticalSpacing(context, 4),
                Image.asset(
                  "assets/images/good_big.png",
                  height: 100,
                  width: 100,
                ),
                addVerticalSpacing(context, 10),
                Text(
                  'Verified!',
                  style: TextStyle(
                    color: AppColors.black,
                    fontWeight: FontWeight.bold,
                    fontFamily: "Inter",
                    fontSize: fontSized(context, 85),
                  ),
                ),
                Text(
                  'You have successfully verified your account',
                  textAlign: TextAlign.center,
                  style: TextStyle(
                    color: AppColors.black,
                    fontWeight: FontWeight.w500,
                    fontFamily: "Inter",
                    fontSize: fontSized(context, 95),
                  ),
                ),
                addVerticalSpacing(context, 10),
                AppButton(
                  text: "Go to Login",
                  onPressed: () {
                    navigateAndReplaceRoute(context, const LoginScreens());
                  },
                  widthPercent: 70,
                  heightPercent: 5,
                  btnColor: AppColors.primary,
                  isLoading: false,
                ),
                addVerticalSpacing(context, 10),
              ],
            ),
          ),
        ),
      ),
    );
  }
}

class ErrorEmailSentDialog extends StatefulWidget {
  const ErrorEmailSentDialog({super.key});

  @override
  State<ErrorEmailSentDialog> createState() => _ErrorEmailSentDialogState();
}

class _ErrorEmailSentDialogState extends State<ErrorEmailSentDialog> {
  @override
  Widget build(BuildContext context) {
    // ignore: deprecated_member_use
    return WillPopScope(
      onWillPop: () async => false,
      child: Dialog(
        shape: const RoundedRectangleBorder(),
        elevation: 0,
        backgroundColor: Colors.transparent,
        child: Container(
          width: double.infinity,
          decoration: BoxDecoration(
            color: AppColors.white,
            borderRadius: BorderRadius.circular(30),
          ),
          child: Padding(
            padding: const EdgeInsets.symmetric(horizontal: 20, vertical: 20),
            child: Column(
              mainAxisAlignment: MainAxisAlignment.center,
              mainAxisSize: MainAxisSize.min,
              children: [
                addVerticalSpacing(context, 4),
                Image.asset(
                  "assets/images/declined.png",
                  height: 100,
                  width: 100,
                ),
                addVerticalSpacing(context, 4),
                Text(
                  'Opps!',
                  style: TextStyle(
                    color: AppColors.black,
                    fontWeight: FontWeight.bold,
                    fontFamily: "Inter",
                    fontSize: fontSized(context, 85),
                  ),
                ),
                Text(
                  'Something went wrong, please try again',
                  textAlign: TextAlign.center,
                  style: TextStyle(
                    color: AppColors.black,
                    fontWeight: FontWeight.w500,
                    fontFamily: "Inter",
                    fontSize: fontSized(context, 85),
                  ),
                ),
                addVerticalSpacing(context, 10),
                AppButton(
                  text: "Retry",
                  onPressed: () {
                    Navigator.pop(context);
                  },
                  widthPercent: 70,
                  heightPercent: 5,
                  btnColor: AppColors.primary,
                  isLoading: false,
                ),
                addVerticalSpacing(context, 4),
              ],
            ),
          ),
        ),
      ),
    );
  }
}

class PasswordChangedDialog extends StatefulWidget {
  const PasswordChangedDialog({super.key});

  @override
  State<PasswordChangedDialog> createState() => _PasswordChangedDialogState();
}

class _PasswordChangedDialogState extends State<PasswordChangedDialog> {
  @override
  Widget build(BuildContext context) {
    // ignore: deprecated_member_use
    return WillPopScope(
      onWillPop: () async => false,
      child: Dialog(
        shape: const RoundedRectangleBorder(),
        elevation: 0,
        backgroundColor: Colors.transparent,
        child: Container(
          width: double.infinity,
          decoration: BoxDecoration(
            color: AppColors.white,
            borderRadius: BorderRadius.circular(30),
          ),
          child: Padding(
            padding: const EdgeInsets.symmetric(horizontal: 20, vertical: 20),
            child: Column(
              mainAxisAlignment: MainAxisAlignment.center,
              mainAxisSize: MainAxisSize.min,
              children: [
                addVerticalSpacing(context, 4),
                Image.asset(
                  "assets/images/good_big.png",
                  height: 100,
                  width: 100,
                ),
                addVerticalSpacing(context, 4),
                Text(
                  'Password Change!',
                  style: TextStyle(
                    color: AppColors.black,
                    fontWeight: FontWeight.bold,
                    fontFamily: "Inter",
                    fontSize: fontSized(context, 85),
                  ),
                ),
                Text(
                  'You have successfully Change your password',
                  textAlign: TextAlign.center,
                  style: TextStyle(
                    color: AppColors.black,
                    fontWeight: FontWeight.w500,
                    fontFamily: "Inter",
                    fontSize: fontSized(context, 85),
                  ),
                ),
                addVerticalSpacing(context, 10),
                AppButton(
                  text: "Go to Login",
                  onPressed: () {
                    navigateAndReplaceRoute(context, const LoginScreens());
                  },
                  widthPercent: 100,
                  heightPercent: 6,
                  btnColor: AppColors.primary,
                  isLoading: false,
                ),
                addVerticalSpacing(context, 4),
              ],
            ),
          ),
        ),
      ),
    );
  }
}
