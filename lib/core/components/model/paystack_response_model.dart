// To parse this JSON data, do
//
//     final paystackResponseData = paystackResponseDataFromJson(jsonString);

import 'dart:convert';

PaystackResponseData paystackResponseDataFromJson(String str) =>
    PaystackResponseData.fromJson(json.decode(str));

String paystackResponseDataToJson(PaystackResponseData data) =>
    json.encode(data.toJson());

class PaystackResponseData {
  final bool? status;
  final String? message;
  final Data? data;

  PaystackResponseData({
    this.status,
    this.message,
    this.data,
  });

  factory PaystackResponseData.fromJson(Map<String, dynamic> json) =>
      PaystackResponseData(
        status: json["status"],
        message: json["message"],
        data: json["data"] == null ? null : Data.fromJson(json["data"]),
      );

  Map<String, dynamic> toJson() => {
        "status": status,
        "message": message,
        "data": data?.toJson(),
      };
}

class Data {
  final String? authorizationUrl;
  final String? accessCode;
  final String? reference;

  Data({
    this.authorizationUrl,
    this.accessCode,
    this.reference,
  });

  factory Data.fromJson(Map<String, dynamic> json) => Data(
        authorizationUrl: json["authorization_url"],
        accessCode: json["access_code"],
        reference: json["reference"],
      );

  Map<String, dynamic> toJson() => {
        "authorization_url": authorizationUrl,
        "access_code": accessCode,
        "reference": reference,
      };
}
