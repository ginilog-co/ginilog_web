// To parse this JSON data, do
//
//     final airlineResponseModel = airlineResponseModelFromJson(jsonString);

import 'dart:convert';

List<CustomerBookResponseModel> customerBookResponseModelFromJson(String str) =>
    List<CustomerBookResponseModel>.from(
      json.decode(str).map((x) => CustomerBookResponseModel.fromJson(x)),
    );

String customerBookResponseModelToJson(List<CustomerBookResponseModel> data) =>
    json.encode(List<dynamic>.from(data.map((x) => x.toJson())));

class CustomerBookResponseModel {
  final String? id;
  final String? resevationId;
  final String? accomodationId;
  final String? accomodationType;
  final String? accomodationName;
  final String? accomodationImage;
  final String? accomodationLocation;
  final String? userId;
  final num? roomNumber;
  final String? qrCode;
  final String? customerName;
  final String? customerPhoneNumber;
  final String? customerEmail;
  final num? numberOfGuests;
  final String? trnxReference;
  final String? paymentChannel;
  final bool? paymentStatus;
  final String? comment;
  final String? ticketNum;
  final DateTime? reservationStartDate;
  final DateTime? reservationEndDate;
  final num? noOfDays;
  final bool? ticketClosed;
  final num? totalCost;
  final DateTime? updateddAt;
  final DateTime? createdAt;

  CustomerBookResponseModel({
    this.id,
    this.resevationId,
    this.accomodationId,
    this.accomodationType,
    this.accomodationName,
    this.accomodationImage,
    this.accomodationLocation,
    this.userId,
    this.roomNumber,
    this.qrCode,
    this.customerName,
    this.customerPhoneNumber,
    this.customerEmail,
    this.numberOfGuests,
    this.trnxReference,
    this.paymentChannel,
    this.paymentStatus,
    this.comment,
    this.ticketNum,
    this.reservationStartDate,
    this.reservationEndDate,
    this.noOfDays,
    this.ticketClosed,
    this.totalCost,
    this.updateddAt,
    this.createdAt,
  });

  factory CustomerBookResponseModel.fromJson(
    Map<String, dynamic> json,
  ) => CustomerBookResponseModel(
    id: json["id"],
    resevationId: json["resevationId"],
    accomodationId: json["accomodationId"],
    accomodationType: json["accomodationType"],
    accomodationName: json["accomodationName"],
    accomodationImage: json["accomodationImage"],
    accomodationLocation: json["accomodationLocation"],
    userId: json["userId"],
    roomNumber: json["roomNumber"],
    qrCode: json["qrCode"],
    customerName: json["customerName"],
    customerPhoneNumber: json["customerPhoneNumber"],
    customerEmail: json["customerEmail"],
    numberOfGuests: json["numberOfGuests"],
    trnxReference: json["trnxReference"],
    paymentChannel: json["paymentChannel"],
    paymentStatus: json["paymentStatus"],
    comment: json["comment"],
    ticketNum: json["ticketNum"],
    reservationStartDate:
        json["reservationStartDate"] == null
            ? null
            : DateTime.parse(json["reservationStartDate"]),
    reservationEndDate:
        json["reservationEndDate"] == null
            ? null
            : DateTime.parse(json["reservationEndDate"]),
    noOfDays: json["noOfDays"],
    ticketClosed: json["ticketClosed"],
    totalCost: json["totalCost"],
    updateddAt:
        json["updateddAt"] == null ? null : DateTime.parse(json["updateddAt"]),
    createdAt:
        json["createdAt"] == null ? null : DateTime.parse(json["createdAt"]),
  );

  Map<String, dynamic> toJson() => {
    "id": id,
    "resevationId": resevationId,
    "accomodationId": accomodationId,
    "accomodationType": accomodationType,
    "accomodationName": accomodationName,
    "accomodationImage": accomodationImage,
    "accomodationLocation": accomodationLocation,
    "userId": userId,
    "roomNumber": roomNumber,
    "qrCode": qrCode,
    "customerName": customerName,
    "customerPhoneNumber": customerPhoneNumber,
    "customerEmail": customerEmail,
    "numberOfGuests": numberOfGuests,
    "trnxReference": trnxReference,
    "paymentChannel": paymentChannel,
    "paymentStatus": paymentStatus,
    "comment": comment,
    "ticketNum": ticketNum,
    "reservationStartDate": reservationStartDate,
    "reservationEndDate": reservationEndDate,
    "noOfDays": noOfDays,
    "ticketClosed": ticketClosed,
    "totalCost": totalCost,
    "updateddAt": updateddAt?.toIso8601String(),
    "createdAt": createdAt?.toIso8601String(),
  };
}
