// To parse this JSON data, do
//
//     final accomodationResponseModel = accomodationResponseModelFromJson(jsonString);

import 'dart:convert';

List<AccomodationResponseModel> accomodationResponseModelFromJson(String str) =>
    List<AccomodationResponseModel>.from(
        json.decode(str).map((x) => AccomodationResponseModel.fromJson(x)));

String accomodationResponseModelToJson(List<AccomodationResponseModel> data) =>
    json.encode(List<dynamic>.from(data.map((x) => x.toJson())));

class AccomodationResponseModel {
  final String? id;
  final String? adminId;
  final String? accomodationName;
  final String? accomodationLogo;
  final String? accomodationEmail;
  final String? accomodationDescription;
  final String? accomodationType;
  final String? checkInTime;
  final String? checkOutTime;
  final String? accomodationWebsite;
  final String? accomodationPhoneNo;
  final String? location;
  final String? state;
  final String? country;
  final String? locality;
  final String? postcode;
  final num? latitude;
  final num? longitude;
  final num? bookingAmount;
  final num? rating;
  final num? noOfRooms;
  final String? accomodationAdvertType;
  final bool? available;
  final AccomodationDay? accomodationMonday;
  final AccomodationDay? accomodationTuesday;
  final AccomodationDay? accomodationWednesday;
  final AccomodationDay? accomodationThursday;
  final AccomodationDay? accomodationFriday;
  final AccomodationDay? accomodationSaturday;
  final AccomodationDay? accomodationSunday;
  final List<String>? accomodationImages;
  final List<String>? accomodationFacilities;
  final List<AccomodationReviewModel>? accomodationReviewModels;
  final DateTime? createdAt;

  AccomodationResponseModel({
    this.id,
    this.adminId,
    this.accomodationName,
    this.accomodationLogo,
    this.accomodationEmail,
    this.accomodationDescription,
    this.accomodationType,
    this.checkInTime,
    this.checkOutTime,
    this.accomodationWebsite,
    this.accomodationPhoneNo,
    this.location,
    this.state,
    this.country,
    this.locality,
    this.postcode,
    this.latitude,
    this.longitude,
    this.bookingAmount,
    this.rating,
    this.noOfRooms,
    this.accomodationAdvertType,
    this.available,
    this.accomodationMonday,
    this.accomodationTuesday,
    this.accomodationWednesday,
    this.accomodationThursday,
    this.accomodationFriday,
    this.accomodationSaturday,
    this.accomodationSunday,
    this.accomodationImages,
    this.accomodationFacilities,
    this.accomodationReviewModels,
    this.createdAt,
  });

  factory AccomodationResponseModel.fromJson(Map<String, dynamic> json) =>
      AccomodationResponseModel(
        id: json["id"] ?? "",
        adminId: json["adminId"] ?? "",
        accomodationName: json["accomodationName"] ?? "",
        accomodationLogo: json["accomodationLogo"] ?? "",
        accomodationEmail: json["accomodationEmail"] ?? "",
        accomodationDescription: json["accomodationDescription"] ?? "",
        accomodationType: json["accomodationType"] ?? "",
        checkInTime: json["checkInTime"] ?? "",
        checkOutTime: json["checkOutTime"] ?? "",
        accomodationWebsite: json["accomodationWebsite"] ?? "",
        accomodationPhoneNo: json["accomodationPhoneNo"] ?? "",
        location: json["location"] ?? "",
        state: json["state"] ?? "",
        country: json["country"] ?? "",
        locality: json["locality"] ?? "",
        postcode: json["postcode"] ?? "",
        latitude: json["latitude"] ?? 0,
        longitude: json["longitude"] ?? 0,
        bookingAmount: json["bookingAmount"] ?? 0,
        rating: json["rating"] ?? 0,
        noOfRooms: json["noOfRooms"] ?? "",
        accomodationAdvertType: json["accomodationAdvertType"] ?? "",
        available: json["available"] ?? false,
        accomodationMonday: json["accomodationMonday"] == null
            ? null
            : AccomodationDay.fromJson(json["accomodationMonday"]),
        accomodationTuesday: json["accomodationTuesday"] == null
            ? null
            : AccomodationDay.fromJson(json["accomodationTuesday"]),
        accomodationWednesday: json["accomodationWednesday"] == null
            ? null
            : AccomodationDay.fromJson(json["accomodationWednesday"]),
        accomodationThursday: json["accomodationThursday"] == null
            ? null
            : AccomodationDay.fromJson(json["accomodationThursday"]),
        accomodationFriday: json["accomodationFriday"] == null
            ? null
            : AccomodationDay.fromJson(json["accomodationFriday"]),
        accomodationSaturday: json["accomodationSaturday"] == null
            ? null
            : AccomodationDay.fromJson(json["accomodationSaturday"]),
        accomodationSunday: json["accomodationSunday"] == null
            ? null
            : AccomodationDay.fromJson(json["accomodationSunday"]),
        accomodationImages: json["accomodationImages"] == null
            ? []
            : List<String>.from(json["accomodationImages"]!.map((x) => x)),
        accomodationFacilities: json["accomodationFacilities"] == null
            ? []
            : List<String>.from(json["accomodationFacilities"]!.map((x) => x)),
        accomodationReviewModels: json["accomodationReviewModels"] == null
            ? []
            : List<AccomodationReviewModel>.from(
                json["accomodationReviewModels"]!
                    .map((x) => AccomodationReviewModel.fromJson(x))),
        createdAt: json["createdAt"] == null
            ? null
            : DateTime.parse(json["createdAt"]),
      );

  Map<String, dynamic> toJson() => {
        "id": id,
        "adminId": adminId,
        "accomodationName": accomodationName,
        "accomodationLogo": accomodationLogo,
        "accomodationEmail": accomodationEmail,
        "accomodationDescription": accomodationDescription,
        "accomodationType": accomodationType,
        "checkInTime": checkInTime,
        "checkOutTime": checkOutTime,
        "accomodationWebsite": accomodationWebsite,
        "accomodationPhoneNo": accomodationPhoneNo,
        "location": location,
        "state": state,
        "country": country,
        "locality": locality,
        "postcode": postcode,
        "latitude": latitude,
        "longitude": longitude,
        "bookingAmount": bookingAmount,
        "rating": rating,
        "noOfRooms": noOfRooms,
        "accomodationAdvertType": accomodationAdvertType,
        "available": available,
        "accomodationMonday": accomodationMonday?.toJson(),
        "accomodationTuesday": accomodationTuesday?.toJson(),
        "accomodationWednesday": accomodationWednesday?.toJson(),
        "accomodationThursday": accomodationThursday?.toJson(),
        "accomodationFriday": accomodationFriday?.toJson(),
        "accomodationSaturday": accomodationSaturday?.toJson(),
        "accomodationSunday": accomodationSunday?.toJson(),
        "accomodationImages": accomodationImages == null
            ? []
            : List<dynamic>.from(accomodationImages!.map((x) => x)),
        "accomodationFacilities": accomodationFacilities == null
            ? []
            : List<dynamic>.from(accomodationFacilities!.map((x) => x)),
        "accomodationReviewModels": accomodationReviewModels == null
            ? []
            : List<dynamic>.from(
                accomodationReviewModels!.map((x) => x.toJson())),
        "createdAt": createdAt?.toIso8601String(),
      };
}

class AccomodationDay {
  final String? id;
  final String? hourStart;
  final String? hourEnd;
  final bool? isClosed;
  final String? accomodationDataModelId;
  final String? airlineDataModelId;

  AccomodationDay({
    this.id,
    this.hourStart,
    this.hourEnd,
    this.isClosed,
    this.accomodationDataModelId,
    this.airlineDataModelId,
  });

  factory AccomodationDay.fromJson(Map<String, dynamic> json) =>
      AccomodationDay(
        id: json["id"] ?? "",
        hourStart: json["hourStart"] ?? "",
        hourEnd: json["hourEnd"] ?? "",
        isClosed: json["isClosed"] ?? false,
        accomodationDataModelId: json["accomodationDataModelId"] ?? "",
      );

  Map<String, dynamic> toJson() => {
        "id": id,
        "hourStart": hourStart,
        "hourEnd": hourEnd,
        "isClosed": isClosed,
        "accomodationDataModelId": accomodationDataModelId,
      };
}

class AccomodationReviewModel {
  final String? id;
  final String? userName;
  final String? profileImage;
  final String? reviewMessage;
  final num? ratingNum;
  final String? userId;
  final String? accomodationDataTableId;
  final DateTime? createdAt;

  AccomodationReviewModel({
    this.id,
    this.userName,
    this.profileImage,
    this.reviewMessage,
    this.ratingNum,
    this.userId,
    this.accomodationDataTableId,
    this.createdAt,
  });

  factory AccomodationReviewModel.fromJson(Map<String, dynamic> json) =>
      AccomodationReviewModel(
        id: json["id"] ?? "",
        userName: json["userName"] ?? "",
        profileImage: json["profileImage"] ?? "",
        reviewMessage: json["reviewMessage"],
        ratingNum: json["ratingNum"] ?? 0,
        userId: json["userId"] ?? "",
        accomodationDataTableId: json["accomodationDataTableId"] ?? "",
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
        "accomodationDataTableId": accomodationDataTableId,
        "createdAt": createdAt?.toIso8601String(),
      };
}
