// To parse this JSON data, do
//
//     final loginErrorModel = loginErrorModelFromJson(jsonString);

import 'dart:convert';

LoginErrorModel loginErrorModelFromJson(String str) =>
    LoginErrorModel.fromJson(json.decode(str));

String loginErrorModelToJson(LoginErrorModel data) =>
    json.encode(data.toJson());

class LoginErrorModel {
  final String message;
  final bool status;

  LoginErrorModel({
    required this.message,
    required this.status,
  });

  factory LoginErrorModel.fromJson(Map<String, dynamic> json) =>
      LoginErrorModel(
        message: json["message"],
        status: json["status"],
      );

  Map<String, dynamic> toJson() => {
        "message": message,
        "status": status,
      };
}
