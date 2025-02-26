// ignore_for_file: unnecessary_null_comparison, null_check_always_fails, deprecated_member_use
import 'dart:developer';
import 'package_export.dart';

// ignore: constant_identifier_names
enum ViewState { IDLE, BUSY, RETRIEVED, ERROR }

class FormUtils {
  FormUtils._();

  static bool isValidEmail(String email) {
    final emailValid = RegExp(
            r"^[a-zA-Z0-9.a-zA-Z0-9.!#$%&'*+-/=?^_`{|}~]+@[a-zA-Z0-9]+\.[a-zA-Z]+")
        .hasMatch(email);

    return emailValid;
  }

  static bool isValidPhoneNumber(String phoneNumber) {
    final phoneValid = RegExp(r"0[789][01]\d{8}").hasMatch(phoneNumber);

    return phoneValid;
  }

  static bool hasDigits(String text) {
    final hasDigits = text.contains(RegExp(r'[0-9]'));
    return hasDigits;
  }

  static bool hasUppercase(String text) {
    final hasUppercase = text.contains(RegExp(r'[A-Z]'));
    return hasUppercase;
  }

  static bool hasLowercase(String text) {
    final hasLowercase = text.contains(RegExp(r'[a-z]'));
    return hasLowercase;
  }

  static bool hasSpecialCharacters(String text) {
    final hasSpecialCharacters =
        text.contains(RegExp(r'[!@#$%^&*(),.?":{}|<>]'));
    return hasSpecialCharacters;
  }

  static bool hasMinLength(String text, {int minLength = 8}) {
    final hasMinLength = text.length >= minLength;
    return hasMinLength;
  }
}

//
printData(identifier, data) {
  return log('===> $identifier <=== $data');
}

extension CapExtension on String {
  String get inCaps => '${this[0].toUpperCase()}${substring(1)}';
  String get firstCap => "${this[0].toUpperCase()}${substring(1)}";
  String get allInCaps => toUpperCase();
  String get capitalizeFirstofEach =>
      split(" ").map((str) => str.capitalizeFirstofEach).join(" ");
}

class Constants {
  Constants._();

  static double getScreenHeight(BuildContext context) {
    return MediaQuery.of(context).size.height;
  }

  static double getScreenWidth(BuildContext context) {
    return MediaQuery.of(context).size.width;
  }

  static double buttonHeight = 55;

  static const scrollerTransitionTime = 5;

//   printData(identifier, data) {
//   return log('===> $identifier <=== $data');
// }
}

// setToLocalStorage() method will set data to the local storage
Future setToLocalStorage({required String name, dynamic data}) async {
  SharedPreferences prefs = await SharedPreferences.getInstance();
  prefs.setString(name, data);
}

Future setIntToLocalStorage({required String name, dynamic data}) async {
  SharedPreferences prefs = await SharedPreferences.getInstance();
  await prefs.setInt(name, data);
}

Future setDoubleToLocalStorage({required String name, dynamic data}) async {
  SharedPreferences prefs = await SharedPreferences.getInstance();
  await prefs.setDouble(name, data);
}

Future setBoolToLocalStorage({required String name, dynamic data}) async {
  SharedPreferences prefs = await SharedPreferences.getInstance();
  await prefs.setBool(name, data);
}

Future setListToLocalStorage(
    {required String name, required List<String> data}) async {
  SharedPreferences prefs = await SharedPreferences.getInstance();
  await prefs.setStringList(name, data);
}

// getFromLocalStorage() method will get data from the local storage
Future getFromLocalStorage({required String name}) async {
  SharedPreferences prefs = await SharedPreferences.getInstance();
  return prefs.getString(name);
}

Future getIntFromLocalStorage({required String name}) async {
  SharedPreferences prefs = await SharedPreferences.getInstance();
  return prefs.getInt(name);
}

Future getDoubleFromLocalStorage({required String name}) async {
  SharedPreferences prefs = await SharedPreferences.getInstance();
  return prefs.getDouble(name);
}

Future getBoolFromLocalStorage({required String name}) async {
  SharedPreferences prefs = await SharedPreferences.getInstance();
  return prefs.getBool(name);
}

removeFromLocalStorage({required String name}) async {
  SharedPreferences prefs = await SharedPreferences.getInstance();
  prefs.remove(name);
}

void finish(BuildContext context, [Object? result]) {
  if (Navigator.canPop(context)) Navigator.pop(context, result);
}
