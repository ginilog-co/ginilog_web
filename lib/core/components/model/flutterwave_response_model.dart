import 'dart:convert';

FlutterwaveResponseModel flutterwaveResponseModelFromJson(String str) =>
    FlutterwaveResponseModel.fromJson(json.decode(str));

String flutterwaveResponseModelToJson(FlutterwaveResponseModel data) =>
    json.encode(data.toJson());

class FlutterwaveResponseModel {
  String? status;
  String? message;
  Data? data;

  FlutterwaveResponseModel({this.status, this.message, this.data});

  factory FlutterwaveResponseModel.fromJson(Map<String, dynamic> json) =>
      FlutterwaveResponseModel(
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
  String? link;

  Data({this.link});

  factory Data.fromJson(Map<String, dynamic> json) => Data(link: json["link"]);

  Map<String, dynamic> toJson() => {"link": link};
}
