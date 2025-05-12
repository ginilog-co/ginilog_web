import 'dart:convert';

PaystackResponseModel paystackResponseModelFromJson(String str) =>
    PaystackResponseModel.fromJson(json.decode(str));

String paystackResponseModelToJson(PaystackResponseModel data) =>
    json.encode(data.toJson());

class PaystackResponseModel {
  final bool? status;
  final String? message;
  final Data? data;

  PaystackResponseModel({this.status, this.message, this.data});

  factory PaystackResponseModel.fromJson(Map<String, dynamic> json) =>
      PaystackResponseModel(
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

  Data({this.authorizationUrl, this.accessCode, this.reference});

  factory Data.fromJson(Map<String, dynamic> json) => Data(
    authorizationUrl: json["authorizationUrl"],
    accessCode: json["accessCode"],
    reference: json["reference"],
  );

  Map<String, dynamic> toJson() => {
    "authorizationUrl": authorizationUrl,
    "accessCode": accessCode,
    "reference": reference,
  };
}
