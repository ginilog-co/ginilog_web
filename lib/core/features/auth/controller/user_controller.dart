// ignore_for_file: use_build_context_synchronously

import 'package:ginilog_customer_app/core/components/state/connectivity_state.dart';
import 'package:ginilog_customer_app/core/components/utils/constants.dart';
import 'package:ginilog_customer_app/core/components/widgets/custom_snackbar.dart';
import 'package:ginilog_customer_app/core/features/auth/controller/confirm_email.dart';

import '../model/register_model.dart';
import '../services/auth_service.dart';
import '../../../components/utils/helper_functions.dart';
import '../../../components/utils/package_export.dart';
import '../view/user_register.dart';

class RegisterScreen extends ConsumerStatefulWidget {
  const RegisterScreen({super.key});

  @override
  ConsumerState<RegisterScreen> createState() => RegisterScreenController();
}

class RegisterScreenController extends ConsumerState<RegisterScreen> {
  final GlobalKey<FormState> formKey = GlobalKey<FormState>();
  bool obscureText = true;
  bool obscureConfirmPassword = true;
  late TextEditingController email;
  late TextEditingController fistNameTEC;
  late TextEditingController lastNameTEC;
  late TextEditingController password;
  late TextEditingController confirmPassword;
  late TextEditingController phoneNo;
  final focusEmail = FocusNode();
  final focusPassword = FocusNode();
  late FocusNode firstNameFocusNode = FocusNode();
  late FocusNode lastNameFocusNode = FocusNode();
  final focusConfirmPassword = FocusNode();
  final focusPhoneNo = FocusNode();


  bool saveButtonPressed = false;
  bool isLoading = false;
  // late Login login;

  @override
  void initState() {
    super.initState();
    email = TextEditingController();
    fistNameTEC = TextEditingController();
    lastNameTEC = TextEditingController();
    password = TextEditingController();
    confirmPassword = TextEditingController();
    phoneNo = TextEditingController();
  }

  @override
  void dispose() {
    super.dispose();
    email.dispose();
    fistNameTEC.dispose();
    lastNameTEC.dispose();
    password.dispose();
    confirmPassword.dispose();
    phoneNo.dispose();
  }

  RegisterModel? userRegisterModel;
  bool isChecked = false;
  urlString(String? url) async {
    final link = Uri.parse(url!);
    if (await canLaunchUrl(link)) {
      await launchUrl(link);
    } else {
      throw 'Could not launch $url';
    }
  }

  onChanged(bool? value) {
    setState(() {
      isChecked = value!;
    });
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

  String isEmailChanged = "";
  String isPasswordChanged = "";
  String isConfirmPasswordChanged = "";
  String isFirstNameChanged = "";
  String isLastNameChanged = "";
  String isPhoneNoChanged = "";

  emailOnChanged(String value) {
    setState(() {
      isEmailChanged = value;
    });
    printData("identifier", isEmailChanged);
  }

  passwordOnChanged(String value) {
    setState(() {
      isPasswordChanged = value;
    });
    printData("identifier", isPasswordChanged);
  }

  confirmPasswordOnChanged(String value) {
    setState(() {
      isConfirmPasswordChanged = value;
    });
    printData("identifier", isConfirmPasswordChanged);
  }

  firstNameOnChanged(String value) {
    setState(() {
      isFirstNameChanged = value;
    });
    printData("identifier", isFirstNameChanged);
  }

  lastNameOnChanged(String value) {
    setState(() {
      isLastNameChanged = value;
    });
    printData("identifier", isLastNameChanged);
  }

  phoneNoChanged(String value) {
    setState(() {
      isPhoneNoChanged = value;
    });
    printData("identifier", isPhoneNoChanged);
  }

  userRegister() async {
    var connectivityStatusProvider = ref.read(connectivityStatusProviders);
    setState(() {
      saveButtonPressed = true;
    });
    FocusScope.of(context).unfocus();
    if (formKey.currentState!.validate()) {
      formKey.currentState!.save();
      setState(() {
        isLoading = true;
      });

      if (connectivityStatusProvider == ConnectivityStatus.isConnected) {
        if (isChecked == true) {
          userRegisterModel = RegisterModel(
              firstName: fistNameTEC.text.trim(),
              lastName: lastNameTEC.text.trim(),
              email: email.text.trim(),
              password: password.text.trim(),
              phoneNo: phoneNo.text.trim());
          bool isValid = await AuthService.register(
              registerModel: userRegisterModel, cxt: context);
          if (isValid) {
            setState(() {
              isLoading = false;
            });
            navigateToRoute(
                context,
                ConfirmEmailAddressScreen(
                  email: email.text.trim(),
                  fromLogin: false,
                  password: password.text.trim(),
                ));
          } else {
            setState(() {
              isLoading = false;
            });
            showCustomSnackbar(context,
                title: "User Exist",
                content: "Email Does Not Exist",
                type: SnackbarType.error,
                isTopPosition: false);
          }
        } else {
          setState(() {
            isLoading = false;
          });
          showCustomSnackbar(context,
              title: "Terms of Service",
              content: "Please accept terms & condition",
              type: SnackbarType.error,
              isTopPosition: false);
        }
      } else {
        setState(() {
          isLoading = false;
        });
        showCustomSnackbar(context,
            title: "Network Connection",
            content: "No Internet Connection",
            type: SnackbarType.error,
            isTopPosition: false);
      }
      setState(() {
        isLoading = false;
      });
    }
  }

  @override
  Widget build(BuildContext context) {
    return RegisterScreenView(this);
  }
}
