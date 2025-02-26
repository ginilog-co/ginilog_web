// To parse this JSON data, do
//
//     final airlineResponseModel = airlineResponseModelFromJson(jsonString);

import 'dart:convert';

List<AirlineResponseModel> airlineResponseModelFromJson(String str) =>
    List<AirlineResponseModel>.from(
        json.decode(str).map((x) => AirlineResponseModel.fromJson(x)));

String airlineResponseModelToJson(List<AirlineResponseModel> data) =>
    json.encode(List<dynamic>.from(data.map((x) => x.toJson())));

class AirlineResponseModel {
  final String? id;
  final String? adminId;
  final String? airlineName;
  final String? airlineLogo;
  final String? airlineEmail;
  final String? airlineInfo;
  final String? airlineType;
  final String? airlinePhoneNo;
  final String? airlineWebsite;
  final String? state;
  final String? country;
  final String? locality;
  final int? bookingAmount;
  final int? rating;
  final bool? available;
  final List<AirlineImage>? airlineImages;
  final List<AirCraftList>? airCraftList;
  final List<AirLineServiceLocation>? airLineServiceLocations;
  final List<AirlinePayment>? airlinePayments;
  final List<AirlineReviewModel>? airlineReviewModels;
  final DateTime? createdAt;

  AirlineResponseModel({
    this.id,
    this.adminId,
    this.airlineName,
    this.airlineLogo,
    this.airlineEmail,
    this.airlineInfo,
    this.airlineType,
    this.airlinePhoneNo,
    this.airlineWebsite,
    this.state,
    this.country,
    this.locality,
    this.bookingAmount,
    this.rating,
    this.available,
    this.airlineImages,
    this.airCraftList,
    this.airLineServiceLocations,
    this.airlinePayments,
    this.airlineReviewModels,
    this.createdAt,
  });

  factory AirlineResponseModel.fromJson(Map<String, dynamic> json) =>
      AirlineResponseModel(
        id: json["id"],
        adminId: json["adminId"],
        airlineName: json["airlineName"],
        airlineLogo: json["airlineLogo"],
        airlineEmail: json["airlineEmail"],
        airlineInfo: json["airlineInfo"],
        airlineType: json["airlineType"],
        airlinePhoneNo: json["airlinePhoneNo"],
        airlineWebsite: json["airlineWebsite"],
        state: json["state"],
        country: json["country"],
        locality: json["locality"],
        bookingAmount: json["bookingAmount"],
        rating: json["rating"],
        available: json["available"],
        airlineImages: json["airlineImages"] == null
            ? []
            : List<AirlineImage>.from(
                json["airlineImages"]!.map((x) => AirlineImage.fromJson(x))),
        airCraftList: json["airCraftList"] == null
            ? []
            : List<AirCraftList>.from(
                json["airCraftList"]!.map((x) => AirCraftList.fromJson(x))),
        airLineServiceLocations: json["airLineServiceLocations"] == null
            ? []
            : List<AirLineServiceLocation>.from(json["airLineServiceLocations"]!
                .map((x) => AirLineServiceLocation.fromJson(x))),
        airlinePayments: json["airlinePayments"] == null
            ? []
            : List<AirlinePayment>.from(json["airlinePayments"]!
                .map((x) => AirlinePayment.fromJson(x))),
        airlineReviewModels: json["airlineReviewModels"] == null
            ? []
            : List<AirlineReviewModel>.from(json["airlineReviewModels"]!
                .map((x) => AirlineReviewModel.fromJson(x))),
        createdAt: json["createdAt"] == null
            ? null
            : DateTime.parse(json["createdAt"]),
      );

  Map<String, dynamic> toJson() => {
        "id": id,
        "adminId": adminId,
        "airlineName": airlineName,
        "airlineLogo": airlineLogo,
        "airlineEmail": airlineEmail,
        "airlineInfo": airlineInfo,
        "airlineType": airlineType,
        "airlinePhoneNo": airlinePhoneNo,
        "airlineWebsite": airlineWebsite,
        "state": state,
        "country": country,
        "locality": locality,
        "bookingAmount": bookingAmount,
        "rating": rating,
        "available": available,
        "airlineImages": airlineImages == null
            ? []
            : List<dynamic>.from(airlineImages!.map((x) => x.toJson())),
        "airCraftList": airCraftList == null
            ? []
            : List<dynamic>.from(airCraftList!.map((x) => x.toJson())),
        "airLineServiceLocations": airLineServiceLocations == null
            ? []
            : List<dynamic>.from(
                airLineServiceLocations!.map((x) => x.toJson())),
        "airlinePayments": airlinePayments == null
            ? []
            : List<dynamic>.from(airlinePayments!.map((x) => x.toJson())),
        "airlineReviewModels": airlineReviewModels == null
            ? []
            : List<dynamic>.from(airlineReviewModels!.map((x) => x.toJson())),
        "createdAt": createdAt?.toIso8601String(),
      };
}

class AirCraftList {
  final String? id;
  final String? model;
  final String? manufacturer;
  final int? capacity;
  final String? airlineDataModelId;

  AirCraftList({
    this.id,
    this.model,
    this.manufacturer,
    this.capacity,
    this.airlineDataModelId,
  });

  factory AirCraftList.fromJson(Map<String, dynamic> json) => AirCraftList(
        id: json["id"],
        model: json["model"],
        manufacturer: json["manufacturer"],
        capacity: json["capacity"],
        airlineDataModelId: json["airlineDataModelId"],
      );

  Map<String, dynamic> toJson() => {
        "id": id,
        "model": model,
        "manufacturer": manufacturer,
        "capacity": capacity,
        "airlineDataModelId": airlineDataModelId,
      };
}

class AirLineServiceLocation {
  final String? id;
  final String? name;
  final String? code;
  final String? city;
  final String? country;
  final String? airlineDataModelId;

  AirLineServiceLocation({
    this.id,
    this.name,
    this.code,
    this.city,
    this.country,
    this.airlineDataModelId,
  });

  factory AirLineServiceLocation.fromJson(Map<String, dynamic> json) =>
      AirLineServiceLocation(
        id: json["id"],
        name: json["name"],
        code: json["code"],
        city: json["city"],
        country: json["country"],
        airlineDataModelId: json["airlineDataModelId"],
      );

  Map<String, dynamic> toJson() => {
        "id": id,
        "name": name,
        "code": code,
        "city": city,
        "country": country,
        "airlineDataModelId": airlineDataModelId,
      };
}

class AirlineImage {
  final String? id;
  final String? imagePath;
  final String? airlineDataModelId;

  AirlineImage({
    this.id,
    this.imagePath,
    this.airlineDataModelId,
  });

  factory AirlineImage.fromJson(Map<String, dynamic> json) => AirlineImage(
        id: json["id"],
        imagePath: json["imagePath"],
        airlineDataModelId: json["airlineDataModelId"],
      );

  Map<String, dynamic> toJson() => {
        "id": id,
        "imagePath": imagePath,
        "airlineDataModelId": airlineDataModelId,
      };
}

class AirlinePayment {
  final String? id;
  final String? titles;
  final int? amount;
  final String? airlineDataModelId;

  AirlinePayment({
    this.id,
    this.titles,
    this.amount,
    this.airlineDataModelId,
  });

  factory AirlinePayment.fromJson(Map<String, dynamic> json) => AirlinePayment(
        id: json["id"],
        titles: json["titles"],
        amount: json["amount"],
        airlineDataModelId: json["airlineDataModelId"],
      );

  Map<String, dynamic> toJson() => {
        "id": id,
        "titles": titles,
        "amount": amount,
        "airlineDataModelId": airlineDataModelId,
      };
}

class AirlineReviewModel {
  final String? id;
  final String? userName;
  final String? profileImage;
  final String? reviewMessage;
  final int? ratingNum;
  final String? userId;
  final String? airlineDataModelId;
  final DateTime? createdAt;

  AirlineReviewModel({
    this.id,
    this.userName,
    this.profileImage,
    this.reviewMessage,
    this.ratingNum,
    this.userId,
    this.airlineDataModelId,
    this.createdAt,
  });

  factory AirlineReviewModel.fromJson(Map<String, dynamic> json) =>
      AirlineReviewModel(
        id: json["id"],
        userName: json["userName"],
        profileImage: json["profileImage"],
        reviewMessage: json["reviewMessage"],
        ratingNum: json["ratingNum"],
        userId: json["userId"],
        airlineDataModelId: json["airlineDataModelId"],
        createdAt: json["createdAt"] == null
            ? null
            : DateTime.parse(json["createdAt"]),
      );

  Map<String, dynamic> toJson() => {
        "id": id,
        "userName": userName,
        "profileImage": profileImage,
        "reviewMessage": reviewMessage,
        "ratingNum": ratingNum,
        "userId": userId,
        "airlineDataModelId": airlineDataModelId,
        "createdAt": createdAt?.toIso8601String(),
      };
}
