// To parse this JSON data, do
//
//     final ridersResponseModel = ridersResponseModelFromJson(jsonString);

import 'dart:convert';

List<RidersResponseModel> ridersResponseModelFromJson(String str) =>
    List<RidersResponseModel>.from(
        json.decode(str).map((x) => RidersResponseModel.fromJson(x)));

String ridersResponseModelToJson(List<RidersResponseModel> data) =>
    json.encode(List<dynamic>.from(data.map((x) => x.toJson())));

class RidersResponseModel {
  final String? id;
  final String? firstName;
  final String? lastName;
  final String? email;
  final String? phoneNo;
  final String? profilePicture;
  final String? idCardUpload;
  final bool? idCardVerification;
  final String? licenseUpload;
  final bool? licenseVerification;
  final bool? allVerification;
  final bool? availability;
  final int? rating;
  final bool? infoCompleted;
  final RiderAddress? riderAddress;
  final RidersBankModels? ridersBankModels;
  final List<RidersReviewModel>? ridersReviewModels;
  final List<DeviceTokenModel>? deviceTokenModels;
  final DateTime? createdAt;

  RidersResponseModel({
    this.id,
    this.firstName,
    this.lastName,
    this.email,
    this.phoneNo,
    this.profilePicture,
    this.idCardUpload,
    this.idCardVerification,
    this.licenseUpload,
    this.licenseVerification,
    this.allVerification,
    this.availability,
    this.rating,
    this.infoCompleted,
    this.riderAddress,
    this.ridersBankModels,
    this.ridersReviewModels,
    this.deviceTokenModels,
    this.createdAt,
  });

  factory RidersResponseModel.fromJson(Map<String, dynamic> json) =>
      RidersResponseModel(
        id: json["id"] ?? "",
        firstName: json["firstName"] ?? "",
        lastName: json["lastName"] ?? "",
        email: json["email"] ?? "",
        phoneNo: json["phoneNo"] ?? "",
        profilePicture: json["profilePicture"] ?? "",
        idCardUpload: json["idCardUpload"] ?? "",
        idCardVerification: json["idCardVerification"] ?? false,
        licenseUpload: json["licenseUpload"] ?? "",
        licenseVerification: json["licenseVerification"] ?? false,
        allVerification: json["allVerification"] ?? false,
        availability: json["availability"] ?? false,
        rating: json["rating"] ?? 1,
        infoCompleted: json["infoCompleted"] ?? "",
        riderAddress: json["riderAddress"] == null
            ? null
            : RiderAddress.fromJson(json["riderAddress"]),
        ridersBankModels: json["ridersBankModels"] == null
            ? null
            : RidersBankModels.fromJson(json["ridersBankModels"]),
        ridersReviewModels: json["ridersReviewModels"] == null
            ? []
            : List<RidersReviewModel>.from(json["ridersReviewModels"]!
                .map((x) => RidersReviewModel.fromJson(x))),
        deviceTokenModels: json["deviceTokenModels"] == null
            ? []
            : List<DeviceTokenModel>.from(json["deviceTokenModels"]!
                .map((x) => DeviceTokenModel.fromJson(x))),
        createdAt: json["createdAt"] == null
            ? null
            : DateTime.parse(json["createdAt"]),
      );

  Map<String, dynamic> toJson() => {
        "id": id,
        "firstName": firstName,
        "lastName": lastName,
        "email": email,
        "phoneNo": phoneNo,
        "profilePicture": profilePicture,
        "idCardUpload": idCardUpload,
        "idCardVerification": idCardVerification,
        "licenseUpload": licenseUpload,
        "licenseVerification": licenseVerification,
        "allVerification": allVerification,
        "availability": availability,
        "rating": rating,
        "infoCompleted": infoCompleted,
        "riderAddress": riderAddress?.toJson(),
        "ridersReviewModels": ridersReviewModels == null
            ? []
            : List<dynamic>.from(ridersReviewModels!.map((x) => x.toJson())),
        "deviceTokenModels": deviceTokenModels == null
            ? []
            : List<dynamic>.from(deviceTokenModels!.map((x) => x.toJson())),
        "createdAt": createdAt?.toIso8601String(),
      };
}

class RiderAddress {
  final String? id;
  final String? address;
  final String? postCodes;
  final String? city;
  final String? state;
  final num? latitude;
  final num? longitude;
  final String? ridersModelDataId;

  RiderAddress({
    this.id,
    this.address,
    this.postCodes,
    this.city,
    this.state,
    this.latitude,
    this.longitude,
    this.ridersModelDataId,
  });

  factory RiderAddress.fromJson(Map<String, dynamic> json) => RiderAddress(
        id: json["id"] ?? "",
        address: json["address"] ?? "",
        postCodes: json["postCodes"] ?? "",
        city: json["city"] ?? "",
        state: json["state"] ?? "",
        latitude: json["latitude"] ?? 1.1,
        longitude: json["longitude"] ?? 1.1,
        ridersModelDataId: json["ridersModelDataId"] ?? "",
      );

  Map<String, dynamic> toJson() => {
        "id": id,
        "address": address,
        "postCodes": postCodes,
        "city": city,
        "state": state,
        "latitude": latitude,
        "longitude": longitude,
        "ridersModelDataId": ridersModelDataId,
      };
}

class RidersBankModels {
  final String? id;
  final String? bankName;
  final String? accountName;
  final String? accountNumber;
  final String? ridersModelDataId;

  RidersBankModels({
    this.id,
    this.bankName,
    this.accountName,
    this.accountNumber,
    this.ridersModelDataId,
  });

  factory RidersBankModels.fromJson(Map<String, dynamic> json) =>
      RidersBankModels(
        id: json["id"] ?? "",
        bankName: json["bankName"] ?? "",
        accountName: json["accountName"] ?? "",
        accountNumber: json["accountNumber"] ?? "",
        ridersModelDataId: json["ridersModelDataId"] ?? "",
      );

  Map<String, dynamic> toJson() => {
        "id": id,
        "bankName": bankName,
        "accountName": accountName,
        "accountNumber": accountNumber,
        "ridersModelDataId": ridersModelDataId,
      };
}

class DeviceTokenModel {
  final String? id;
  final String? deviceTokenId;
  final String? userId;
  final String? userType;

  DeviceTokenModel({
    this.id,
    this.deviceTokenId,
    this.userId,
    this.userType,
  });

  factory DeviceTokenModel.fromJson(Map<String, dynamic> json) =>
      DeviceTokenModel(
        id: json["id"] ?? "",
        deviceTokenId: json["deviceTokenId"] ?? "",
        userId: json["userId"] ?? "",
        userType: json["userType"] ?? "",
      );

  Map<String, dynamic> toJson() => {
        "id": id,
        "deviceTokenId": deviceTokenId,
        "userId": userId,
        "userType": userType,
      };
}

class RidersReviewModel {
  final String? id;
  final String? userName;
  final String? profileImage;
  final String? reviewMessage;
  final String? userId;
  final String? ridersModelDataId;
  final num? ratingNum;
  final String? orderId;
  final DateTime? createdAt;

  RidersReviewModel({
    this.id,
    this.userName,
    this.profileImage,
    this.reviewMessage,
    this.userId,
    this.ridersModelDataId,
    this.ratingNum,
    this.orderId,
    this.createdAt,
  });

  factory RidersReviewModel.fromJson(Map<String, dynamic> json) =>
      RidersReviewModel(
        id: json["id"] ?? "",
        userName: json["userName"] ?? "",
        profileImage: json["profileImage"] ?? "",
        reviewMessage: json["reviewMessage"] ?? "",
        userId: json["userId"] ?? "",
        ridersModelDataId: json["ridersModelDataId"] ?? "",
        ratingNum: json["ratingNum"] ?? 0.0,
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
        "ridersModelDataId": ridersModelDataId,
        "ratingNum": ratingNum,
        "orderId": orderId,
        "createdAt": createdAt?.toIso8601String(),
      };
}
