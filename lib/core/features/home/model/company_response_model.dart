// To parse this JSON data, do
//
//     final logisticResponseModel = logisticResponseModelFromJson(jsonString);

import 'dart:convert';

List<LogisticResponseModel> logisticResponseModelFromJson(String str) =>
    List<LogisticResponseModel>.from(
        json.decode(str).map((x) => LogisticResponseModel.fromJson(x)));

String logisticResponseModelToJson(List<LogisticResponseModel> data) =>
    json.encode(List<dynamic>.from(data.map((x) => x.toJson())));

class LogisticResponseModel {
  final String? id;
  final String? adminId;
  final String? companyEmail;
  final String? companyName;
  final String? phoneNumber;
  final String? companyLogo;
  final String? companyRegNo;
  final String? companyInfo;
  final num? rating;
  final num? valueCharge;
  final num? noOfTrucks;
  final num? nofOfBikes;
  final bool? available;
  final String? companyAddress;
  final String? postCodes;
  final String? locality;
  final String? state;
  final num? latitude;
  final num? longitude;
  final String? bankName;
  final String? accountName;
  final String? accountNumber;
  final List<String>? deliveryTypes;
  final List<String>? serviceAreas;
  final List<CompanyReviewModel>? companyReviewModels;
  final DateTime? createdAt;

  LogisticResponseModel({
    this.id,
    this.adminId,
    this.companyEmail,
    this.companyName,
    this.phoneNumber,
    this.companyLogo,
    this.companyRegNo,
    this.companyInfo,
    this.rating,
    this.valueCharge,
    this.noOfTrucks,
    this.nofOfBikes,
    this.available,
    this.companyAddress,
    this.postCodes,
    this.locality,
    this.state,
    this.latitude,
    this.longitude,
    this.bankName,
    this.accountName,
    this.accountNumber,
    this.deliveryTypes,
    this.serviceAreas,
    this.companyReviewModels,
    this.createdAt,
  });

  factory LogisticResponseModel.fromJson(Map<String, dynamic> json) =>
      LogisticResponseModel(
        id: json["id"] ?? "",
        adminId: json["adminId"] ?? "",
        companyEmail: json["companyEmail"] ?? "",
        phoneNumber: json["phoneNumber"] ?? "",
        companyRegNo: json["companyRegNo"] ?? "",
        companyName: json["companyName"] ?? "",
        companyInfo: json["companyInfo"] ?? "",
        rating: json["rating"] ?? 0,
        valueCharge: json["valueCharge"] ?? 0,
        companyLogo: json["companyLogo"] ?? "",
        noOfTrucks: json["noOfTrucks"] ?? 0,
        nofOfBikes: json["nofOfBikes"] ?? 0,
        available: json["available"] ?? false,
        companyAddress: json["companyAddress"] ?? "",
        postCodes: json["postCodes"] ?? "",
        locality: json["locality"],
        state: json["state"] ?? "",
        latitude: json["latitude"] ?? 0,
        longitude: json["longitude"] ?? 0,
        bankName: json["bankName"],
        accountName: json["accountName"],
        accountNumber: json["accountNumber"],
        deliveryTypes: json["deliveryTypes"] == null
            ? []
            : List<String>.from(json["deliveryTypes"]!.map((x) => x)),
        serviceAreas: json["serviceAreas"] == null
            ? []
            : List<String>.from(json["serviceAreas"]!.map((x) => x)),
        companyReviewModels: json["companyReviewModels"] == null
            ? []
            : List<CompanyReviewModel>.from(json["companyReviewModels"]!
                .map((x) => CompanyReviewModel.fromJson(x))),
        createdAt: json["createdAt"] == null
            ? null
            : DateTime.parse(json["createdAt"]),
      );

  Map<String, dynamic> toJson() => {
        "id": id,
        "adminId": adminId,
        "companyEmail": companyEmail,
        "companyName": companyName,
        "phoneNumber": phoneNumber,
        "companyLogo": companyLogo,
        "companyRegNo": companyRegNo,
        "companyInfo": companyInfo,
        "rating": rating,
        "valueCharge": valueCharge,
        "noOfTrucks": noOfTrucks,
        "nofOfBikes": nofOfBikes,
        "available": available,
        "companyAddress": companyAddress,
        "postCodes": postCodes,
        "locality": locality,
        "state": state,
        "latitude": latitude,
        "longitude": longitude,
        "bankName": bankName,
        "accountName": accountName,
        "accountNumber": accountNumber,
        "deliveryTypes": deliveryTypes == null
            ? []
            : List<dynamic>.from(deliveryTypes!.map((x) => x)),
        "serviceAreas": serviceAreas == null
            ? []
            : List<dynamic>.from(serviceAreas!.map((x) => x)),
        "companyReviewModels": companyReviewModels == null
            ? []
            : List<dynamic>.from(companyReviewModels!.map((x) => x.toJson())),
        "createdAt": createdAt?.toIso8601String(),
      };
}

class CompanyReviewModel {
  final String? id;
  final String? userName;
  final String? profileImage;
  final String? reviewMessage;
  final String? userId;
  final String? companyDataModelId;
  final num? ratingNum;
  final String? orderId;
  final DateTime? createdAt;

  CompanyReviewModel({
    this.id,
    this.userName,
    this.profileImage,
    this.reviewMessage,
    this.userId,
    this.companyDataModelId,
    this.ratingNum,
    this.orderId,
    this.createdAt,
  });

  factory CompanyReviewModel.fromJson(Map<String, dynamic> json) =>
      CompanyReviewModel(
        id: json["id"] ?? "",
        userName: json["userName"] ?? "",
        profileImage: json["profileImage"] ?? "",
        reviewMessage: json["reviewMessage"] ?? "",
        userId: json["userId"] ?? "",
        companyDataModelId: json["companyDataModelId"] ?? "",
        ratingNum: json["ratingNum"] ?? 0,
        orderId: json["orderId"] ?? "",
        createdAt: json["createdAt"] == null
            ? null
            : DateTime.parse(json["createdAt"]),
      );

  Map<String, dynamic> toJson() => {
        "id": id,
        "userName": userName,
        "profileImage": profileImage,
        "reviewMessage": reviewMessage,
        "userId": userId,
        "companyDataModelId": companyDataModelId,
        "ratingNum": ratingNum,
        "orderId": orderId,
        "createdAt": createdAt?.toIso8601String(),
      };
}
