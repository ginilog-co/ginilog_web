// ignore_for_file: use_build_context_synchronously

import 'package:ginilog_customer_app/core/components/helpers/globals.dart';
import 'package:ginilog_customer_app/core/components/state/connectivity_state.dart';
import 'package:ginilog_customer_app/core/components/utils/constants.dart';
import 'package:ginilog_customer_app/core/components/widgets/custom_snackbar.dart';
import 'package:ginilog_customer_app/core/features/auth/controller/confirm_email.dart';

import 'package:ginilog_customer_app/core/features/home_screen.dart';

import '../services/auth_service.dart';
import '../../../components/utils/helper_functions.dart';
import '../../../components/utils/package_export.dart';

import '../view/logins.dart';

class LoginScreens extends ConsumerStatefulWidget {
  const LoginScreens({super.key});

  @override
  ConsumerState<LoginScreens> createState() => LoginScreensController();
}

class LoginScreensController extends ConsumerState<LoginScreens> {
  final GlobalKey<FormState> formKey = GlobalKey<FormState>();
  bool obscureText = true;
  late TextEditingController emailController;
  late TextEditingController passwordController;
  String? email = "";
  String? password = "";
  bool saveButtonPressed = false;
  int currentIndex = 0;
  bool switchvalue = false;
  bool isLoading = false;
  String isCompleted = "";
  // late Login login;
  @override
  void initState() {
    super.initState();
    emailController = TextEditingController();
    passwordController = TextEditingController();
  }

  @override
  void dispose() {
    super.dispose();
    emailController.dispose();
    passwordController.dispose();
  }

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

  String isEmailChanged = "";
  String isPasswordChanged = "";

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

  loginUser() async {
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
          final res = await AuthService().userLogin(
              email: emailController.text.trim(),
              password: passwordController.text.trim(),
              cxt: context);
          await globals.init();
          await AuthService().updateDeviceToken();

          if (res.statusCode == 200 || res.statusCode == 201) {
            if (globals.isEmailVerified == false) {
              setState(() {
                isLoading = false;
              });
              ScaffoldMessenger.of(context).showSnackBar(
                  const SnackBar(content: Text("New code sent to your email")));
              navigateAndRemoveUntilRoute(
                  context,
                  ConfirmEmailAddressScreen(
                    email: emailController.text,
                    fromLogin: false,
                    password: passwordController.text,
                  ));
            } else {
              setState(() {
                isLoading = false;
              });

              navigateAndReplaceRoute(
                  context,
                  const HomeScreenPage(
                    imdex: 0,
                  ));
            }
          } else {
            setState(() {
              isLoading = false;
            });
            showCustomSnackbar(context,
                title: "Invalid User",
                content: res.body,
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

  google() async {
    var connectivityStatusProvider = ref.read(connectivityStatusProviders);

    if (connectivityStatusProvider == ConnectivityStatus.isConnected) {
      if (isChecked == true) {
        final res = await AuthService().signInWithGoogle();
        if (res.statusCode == 200 || res.statusCode == 201) {
          await globals.init();
          await AuthService().updateDeviceToken();
          navigateAndReplaceRoute(
              context,
              const HomeScreenPage(
                imdex: 0,
              ));
        } else {
          showCustomSnackbar(context,
              title: "Authentication",
              content: res.body,
              type: SnackbarType.error,
              isTopPosition: false);
        }
      } else {
        showCustomSnackbar(context,
            title: "Terms of Service",
            content: "Please accept terms & condition",
            type: SnackbarType.error,
            isTopPosition: false);
      }
    } else {
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

  apple() async {
    var connectivityStatusProvider = ref.read(connectivityStatusProviders);

    if (connectivityStatusProvider == ConnectivityStatus.isConnected) {
      if (isChecked == true) {
        final res = await AuthService().signInWithApple();
        if (res.statusCode == 200 || res.statusCode == 201) {
          await globals.init();
          await AuthService().updateDeviceToken();
          navigateAndReplaceRoute(
              context,
              const HomeScreenPage(
                imdex: 0,
              ));
        } else {
          showCustomSnackbar(context,
              title: "Authentication",
              content: res.body,
              type: SnackbarType.error,
              isTopPosition: false);
        }
      } else {
        showCustomSnackbar(context,
            title: "Terms of Service",
            content: "Please accept terms & condition",
            type: SnackbarType.error,
            isTopPosition: false);
      }
    } else {
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

  final GoogleSignIn _googleSignIn = GoogleSignIn(
    scopes: ['email', 'profile'],
  );
  google1() async {
    await _googleSignIn.signOut();
  }

  @override
  Widget build(BuildContext context) {
    return LoginScreensView(this);
  }
}
