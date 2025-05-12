// ignore_for_file: deprecated_member_use

import 'package:ginilog_customer_app/core/features/home_screen.dart';
import 'package:flutter/cupertino.dart';
import 'package:flutter/material.dart';
import 'dart:convert';
import 'dart:typed_data';

import '../routes/routers.dart';
import '../widgets/responsive.dart';
import 'colors.dart';

Image imageFromBase64String(String base64String) {
  return Image.memory(base64Decode(base64String), width: 40, height: 40);
}

Uint8List dataFromBase64String(String base64String) {
  return base64Decode(base64String);
}

String base64String(Uint8List data) {
  return base64Encode(data);
}

double getScreenHeight(BuildContext context) {
  return MediaQuery.of(context).size.height;
}

double getScreenWidth(BuildContext context) {
  return MediaQuery.of(context).size.width;
}

Widget addSeperator(Color color, double height) {
  return Divider(color: color, height: height);
}

Widget addSeperatorDarkMode(bool darkMode, double height) {
  return Divider(color: darkMode ? Colors.grey : Colors.grey, height: height);
}

Widget addVerticalSpacing(BuildContext context, double height) {
  var heightEd = 14.0;
  if (Responsive.isDesktop(context)) {
    final width2 = (MediaQuery.of(context).size.height / 6) * (height / 100);
    heightEd = width2;
  } else if (Responsive.isTablet(context)) {
    final width2 = (MediaQuery.of(context).size.height / 4) * (height / 100);
    heightEd = width2;
  } else {
    final width2 = (MediaQuery.of(context).size.height / 2) * (height / 100);
    heightEd = width2;
  }
  return SizedBox(height: heightEd);
}

Widget addHorizontalSpacing(double width) {
  return SizedBox(width: width);
}

double getPercentageHeight(BuildContext context, double height) {
  var heightPercentage = (height / 100) * getScreenHeight(context);
  return heightPercentage;
}

double getPercentageWidth(BuildContext context, double width) {
  var widthPercentage = (width / 100) * getScreenWidth(context);
  return widthPercentage;
}

double buttonHeight = 50;

String convertToTitleCase(String? text) {
  if (text == null) {
    return "Not Assigned";
  }

  text = text.toLowerCase();

  if (text.length <= 1) {
    return text.toUpperCase();
  }

  // Split string into multiple words
  final List<String> words = text.split(' ');

  // Capitalize first letter of each words
  final capitalizedWords = words.map((word) {
    if (word.trim().isNotEmpty) {
      final String firstLetter = word.trim().substring(0, 1).toUpperCase();
      final String remainingLetters = word.trim().substring(1);

      return '$firstLetter$remainingLetters';
    }
    return '';
  });

  // Join/Merge all words back to one String
  return capitalizedWords.join(' ');
}

String getNairaSymbol() {
  return "\u{20A6}";
}

void dismissKeyboard() {
  FocusManager.instance.primaryFocus?.unfocus();
}

void popSheet(BuildContext context) {
  Navigator.of(context).pop();
}

void navigateBack(BuildContext context) {
  Navigator.pop(context);
}

void navigateToRoute(BuildContext context, dynamic routeClass) {
  Navigator.push(context, CupertinoPageRoute(builder: (context) => routeClass));
}

void navigateToTabRoute(BuildContext context, dynamic routeClass) {
  navKey.currentState!.push(
    CupertinoPageRoute(builder: (context) => routeClass),
  );
}

void navigateAndReplaceRoute(BuildContext context, dynamic routeClass) {
  Navigator.pushReplacement(
    context,
    CupertinoPageRoute(builder: (context) => routeClass),
  );
}

void navigateTabAndReplaceRoute(BuildContext context, dynamic routeClass) {
  navKey.currentState!.pushReplacement(
    CupertinoPageRoute(builder: (context) => routeClass),
  );
}

void navigateAndRemoveUntilRoute(BuildContext context, dynamic routeClass) {
  Navigator.pushAndRemoveUntil(
    context,
    CupertinoPageRoute(builder: (context) => routeClass),
    (route) => false,
  );
}

void navigateTabAndRemoveUntilRoute(BuildContext context, dynamic routeClass) {
  navKey.currentState!.pushAndRemoveUntil(
    CupertinoPageRoute(builder: (context) => routeClass),
    (route) => false,
  );
}

navPush(BuildContext context, String route) {
  Navigator.push(context, generateRoute(RouteSettings(name: route)));
}

navTabPush(BuildContext context, String route) {
  navKey.currentState!.push(generateRoute(RouteSettings(name: route)));
}

displayBottomSheet(context, Widget bottomSheet) {
  showModalBottomSheet(
    context: context,
    isScrollControlled: true,
    backgroundColor: Colors.white,
    shape: const RoundedRectangleBorder(
      borderRadius: BorderRadius.only(
        topLeft: Radius.circular(30.0),
        topRight: Radius.circular(30.0),
      ),
    ),
    builder:
        (context) => Padding(
          padding: MediaQuery.of(context).viewInsets,
          child: GestureDetector(onTap: dismissKeyboard, child: bottomSheet),
        ),
  );
}

class MyBehavior extends ScrollBehavior {
  @override
  Widget buildOverscrollIndicator(
    BuildContext context,
    Widget child,
    ScrollableDetails details,
  ) {
    return child;
  }
}

class KeyboardUtils {
  static bool isKeyboardShowing() {
    // ignore: unnecessary_null_comparison
    if (WidgetsBinding.instance != null) {
      return WidgetsBinding.instance.window.viewInsets.bottom > 0;
    } else {
      return false;
    }
  }

  static closeKeyboard(BuildContext context) {
    FocusScopeNode currentFocus = FocusScope.of(context);
    if (!currentFocus.hasPrimaryFocus) {
      currentFocus.unfocus();
    }
  }
}

class DashedLineVerticalPainter extends CustomPainter {
  @override
  void paint(Canvas canvas, Size size) {
    double dashHeight = 10, dashSpace = 3, startY = 0;
    final paint =
        Paint()
          ..color = AppColors.green
          ..strokeWidth = size.width * 2;
    while (startY < size.height) {
      canvas.drawLine(Offset(0, startY), Offset(0, startY + dashHeight), paint);
      startY += dashHeight + dashSpace;
    }
  }

  @override
  bool shouldRepaint(CustomPainter oldDelegate) => false;
}
