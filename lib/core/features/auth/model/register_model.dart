// To parse this JSON data, do
//
//     final registerModel = registerModelFromJson(jsonString);

import 'dart:convert';

RegisterModel registerModelFromJson(String str) =>
    RegisterModel.fromJson(json.decode(str));

String registerModelToJson(RegisterModel data) => json.encode(data.toJson());

class RegisterModel {
  final String lastName;
  final String firstName;
  final String email;
  final String password;
  final String phoneNo;

  RegisterModel({
    required this.lastName,
    required this.firstName,
    required this.email,
    required this.password,
    required this.phoneNo,
  });

  factory RegisterModel.fromJson(Map<String, dynamic> json) => RegisterModel(
        lastName: json["lastName"],
        firstName: json["firstName"],
        email: json["email"],
        password: json["password"],
        phoneNo: json["phoneNo"],
      );

  Map<String, dynamic> toJson() => {
        "lastName": lastName,
        "firstName": firstName,
        "email": email,
        "password": password,
        "phoneNo": phoneNo,
      };
}
