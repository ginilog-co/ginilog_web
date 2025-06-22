// To parse this JSON data, do
//
//     final accomodationReservationResponseModel = accomodationReservationResponseModelFromJson(jsonString);

import 'dart:convert';

List<AccomodationReservationResponseModel>
accomodationReservationResponseModelFromJson(String str) =>
    List<AccomodationReservationResponseModel>.from(
      json
          .decode(str)
          .map((x) => AccomodationReservationResponseModel.fromJson(x)),
    );

String accomodationReservationResponseModelToJson(
  List<AccomodationReservationResponseModel> data,
) => json.encode(List<dynamic>.from(data.map((x) => x.toJson())));

class AccomodationReservationResponseModel {
  final String? id;
  final String? accomodationId;
  final String? adminId;
  final String? accomodationName;
  final String? accomodationType;
  final String? accomodationLocality;
  final String? accomodationState;
  final String? accomodationImage;
  final String? ticketNum;
  final num? roomNumber;
  final num? maximumNoOfGuest;
  final num? roomPrice;
  final String? roomType;
  final List<String>? roomImages;
  final List<String>? roomFeatures;
  final String? qrCode;
  final bool? isBooked;
  final DateTime? updateddAt;
  final DateTime? createdAt;
  final String? checkInTime;
  final String? checkOutTime;
  final String? location;

  AccomodationReservationResponseModel({
    this.id,
    this.accomodationId,
    this.adminId,
    this.accomodationName,
    this.accomodationType,
    this.accomodationLocality,
    this.accomodationState,
    this.accomodationImage,
    this.ticketNum,
    this.roomNumber,
    this.maximumNoOfGuest,
    this.roomPrice,
    this.roomType,
    this.roomImages,
    this.roomFeatures,
    this.qrCode,
    this.isBooked,
    this.updateddAt,
    this.createdAt,
    this.checkInTime,
    this.checkOutTime,
    this.location,
  });

  factory AccomodationReservationResponseModel.fromJson(
    Map<String, dynamic> json,
  ) => AccomodationReservationResponseModel(
    id: json["id"] ?? "",
    accomodationId: json["accomodationId"] ?? "",
    adminId: json["adminId"] ?? "",
    accomodationName: json["accomodationName"] ?? "",
    accomodationType: json["accomodationType"] ?? "",
    accomodationLocality: json["accomodationLocality"] ?? "",
    accomodationState: json["accomodationState"] ?? "",
    accomodationImage: json["accomodationImage"] ?? "",
    ticketNum: json["ticketNum"] ?? "",
    roomNumber: json["roomNumber"] ?? 0,
    maximumNoOfGuest: json["maximumNoOfGuest"] ?? 0,
    roomPrice: json["roomPrice"] ?? 0,
    roomType: json["roomType"] ?? "",
    roomImages:
        json["roomImages"] == null
            ? []
            : List<String>.from(json["roomImages"]!.map((x) => x)),
    roomFeatures:
        json["roomFeatures"] == null
            ? []
            : List<String>.from(json["roomFeatures"]!.map((x) => x)),
    qrCode: json["qrCode"],
    isBooked: json["isBooked"],
    updateddAt:
        json["updateddAt"] == null ? null : DateTime.parse(json["updateddAt"]),
    createdAt:
        json["createdAt"] == null ? null : DateTime.parse(json["createdAt"]),
    checkInTime: json["checkInTime"] ?? "",
    checkOutTime: json["checkOutTime"] ?? "",
    location: json["location"] ?? "",
  );

  Map<String, dynamic> toJson() => {
    "id": id,
    "accomodationId": accomodationId,
    "adminId": adminId,
    "accomodationName": accomodationName,
    "accomodationType": accomodationType,
    "accomodationLocality": accomodationLocality,
    "accomodationState": accomodationState,
    "accomodationImage": accomodationImage,
    "ticketNum": ticketNum,
    "roomNumber": roomNumber,
    "roomPrice": roomPrice,
    "maximumNoOfGuest": maximumNoOfGuest,
    "roomType": roomType,
    "roomImages":
        roomImages == null ? [] : List<dynamic>.from(roomImages!.map((x) => x)),
    "roomFeatures":
        roomFeatures == null
            ? []
            : List<dynamic>.from(roomFeatures!.map((x) => x)),
    "qrCode": qrCode,
    "isBooked": isBooked,
    "updateddAt": updateddAt?.toIso8601String(),
    "createdAt": createdAt?.toIso8601String(),
    "checkInTime": checkInTime,
    "checkOutTime": checkOutTime,
    "location": location,
  };
}
