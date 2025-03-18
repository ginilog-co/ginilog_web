// ignore_for_file: use_build_context_synchronously

import 'package:ginilog_customer_app/core/features/auth/view/email_dialog.dart';

import '../../../components/helpers/globals.dart';
import '../services/auth_service.dart';
import '../../../components/utils/constants.dart';
import '../../../components/utils/package_export.dart';
import '../view/forgot_password_code.dart';

class ForgotPasswordCodeScreen extends StatefulWidget {
  const ForgotPasswordCodeScreen({super.key, required this.email});
  final String email;

  @override
  State<ForgotPasswordCodeScreen> createState() =>
      ForgotPasswordCodeController();
}

class ForgotPasswordCodeController extends State<ForgotPasswordCodeScreen> {
  late TextEditingController codeController;
  late TextEditingController newPasswordController;
  late TextEditingController confirmPasswordController;
  late FocusNode codeFocusNode = FocusNode();
  late FocusNode newPasswordFocusNode = FocusNode();
  late FocusNode confirmPasswordFocusNode = FocusNode();
  final GlobalKey<FormState> formKey = GlobalKey<FormState>();
  bool isLoading = false;
  bool saveButtonPressed = false;
  String isOTPChanged = "";
  String isNewPasswordChanged = "";
  String isConfirmPasswordChanged = "";

  otpOnChanged(String value) {
    setState(() {
      isOTPChanged = value;
    });
    printData("identifier", isOTPChanged);
  }

  newPasswordOnChanged(String value) {
    setState(() {
      isNewPasswordChanged = value;
    });
    printData("identifier", isNewPasswordChanged);
  }

  confirmPasswordOnChanged(String value) {
    setState(() {
      isConfirmPasswordChanged = value;
    });
    printData("identifier", isConfirmPasswordChanged);
  }

  onSubmit() async {
    setState(() {
      saveButtonPressed = true;
    });
    FocusScope.of(context).unfocus();
    if (formKey.currentState!.validate()) {
      formKey.currentState!.save();
      setState(() {
        isLoading = true;
      });

      final res = await AuthService().forgotPassword(
          token: codeController.text.trim(),
          password: newPasswordController.text.trim(),
          cxt: context);
      if (res.statusCode == 200 || res.statusCode == 201) {
        setState(() {
          isLoading = false;
        });
        showDialog(
            context: context,
            barrierDismissible: false,
            builder: (context) => const PasswordChangedDialog());
      } else {
        setState(() {
          isLoading = false;
        });
        showDialog(
            context: context,
            barrierDismissible: true,
            builder: (context) => const ErrorEmailSentDialog());
      }
    }

    setState(() {
      isLoading = false;
    });
  }

  bool obscureText = true;
  bool obscureConfirmPassword = true;

  @override
  void initState() {
    codeController = TextEditingController();
    newPasswordController = TextEditingController();
    confirmPasswordController = TextEditingController();
    globals.stopWatchTimer = StopWatchTimer(
      mode: StopWatchMode.countDown,
      onEnded: () {
        setState(() {});
      },
    );
    globals.stopWatchTimer!.setPresetMinuteTime(1);
    globals.stopWatchTimer!.onStartTimer();
    super.initState();
  }

  @override
  void dispose() {
    codeController.dispose();
    newPasswordController.dispose();
    confirmPasswordController.dispose();

    super.dispose();
  }

  resendCode() async {
    if (!globals.stopWatchTimer!.isRunning) {
      setState(() {
        globals.stopWatchTimer!.setPresetMinuteTime(1);
        globals.stopWatchTimer!.onStartTimer();
      });

      final res = await AuthService()
          .reSendPasswordCode(email: widget.email, cxt: context);
      printData("result", res.statusCode);
      showSnack();
      setState(() {
        //  isLoading = false;
      });
    }
  }

  showSnack() {
    return ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(content: Text("New code sent to your email")));
  }

  obscureTextPassword() {
    setState(() {
      obscureText = !obscureText;
    });
  }

  obscureTextConfirmPassword() {
    setState(() {
      obscureConfirmPassword = !obscureConfirmPassword;
    });
  }

  RegExp passValid = RegExp(r"(?=.*\d)(?=.*[a-z])(?=.*[A-Z])");
  double passStrength = 0;
  bool validatePassword(String pass) {
    String password = pass.trim();
    if (password.isEmpty) {
      setState(() {
        passStrength = 0;
      });
    } else if (password.length < 6) {
      passStrength = 1 / 4;
    } else if (password.length < 8) {
      passStrength = 2 / 4;
    } else {
      if (passValid.hasMatch(password)) {
        passStrength = 4 / 4;
        return true;
      } else {
        passStrength = 3 / 4;
        return false;
      }
    }
    return false;
  }

  @override
  Widget build(BuildContext context) {
    return ForgotPasswordCodeScreenView(this);
  }
}
