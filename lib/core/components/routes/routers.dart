import 'package:ginilog_customer_app/core/components/routes/route.dart';
import 'package:ginilog_customer_app/core/features/auth/controller/kyc_verification_controller.dart';
import 'package:flutter/material.dart';
import '../../features/auth/controller/login_controller.dart';
import '../../features/auth/controller/onboarding_controller.dart';
import '../../features/auth/controller/user_controller.dart';
import '../../features/home_screen.dart';
import '../helpers/globals.dart';

Route<dynamic> generateRoute(RouteSettings settings) {
  switch (settings.name) {
    case RootRoutes.onboard:
      return MaterialPageRoute(builder: (context) => const OnboardingScreen());
    case RootRoutes.login:
      return MaterialPageRoute(builder: (context) => const LoginScreens());
    case RootRoutes.createAccount:
      return MaterialPageRoute(builder: (context) => const RegisterScreen());
    case RootRoutes.tab:
      return MaterialPageRoute(
          builder: (context) => const HomeScreenPage(
                imdex: 0,
              ));
    case RootRoutes.kycVerification:
      return MaterialPageRoute(
          builder: (context) => const KYCVerificationScreen());

    default:
      {
        return _errorRoute();
      }
  }
}

Route<dynamic> _errorRoute() {
  return MaterialPageRoute(builder: (context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('ERROR'),
        centerTitle: true,
      ),
      body: const Center(
        child: Text('Page not found!'),
      ),
    );
  });
}

Future<String> initialRoute() async {
  return globals.isViewed != 0
      ? RootRoutes.onboard
      : globals.userId!.isEmpty
          ? RootRoutes.login
          : RootRoutes.tab;
}
