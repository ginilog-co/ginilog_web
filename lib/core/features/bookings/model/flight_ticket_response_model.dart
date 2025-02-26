// To parse this JSON data, do
//
//     final flightTicketResponseModel = flightTicketResponseModelFromJson(jsonString);

import 'dart:convert';

List<FlightTicketResponseModel> flightTicketResponseModelFromJson(String str) =>
    List<FlightTicketResponseModel>.from(
        json.decode(str).map((x) => FlightTicketResponseModel.fromJson(x)));

String flightTicketResponseModelToJson(List<FlightTicketResponseModel> data) =>
    json.encode(List<dynamic>.from(data.map((x) => x.toJson())));

class FlightTicketResponseModel {
  final String? id;
  final String? airlineId;
  final String? adminId;
  final String? airlineName;
  final String? operatedBy;
  final String? flightSpeed;
  final String? ticketNum;
  final String? dapatureTime;
  final String? availabeTimeInterval;
  final String? dapature;
  final String? destination;
  final String? ticketType;
  final List<String>? stopPlaces;
  final num? bigLuggageKg;
  final num? smallLuggageKg;
  final num? stops;
  final bool? available;
  final bool? isReturn;
  final num? ticketPrice;
  final DateTime? createdAt;

  FlightTicketResponseModel({
    this.id,
    this.airlineId,
    this.adminId,
    this.airlineName,
    this.operatedBy,
    this.flightSpeed,
    this.ticketNum,
    this.dapatureTime,
    this.availabeTimeInterval,
    this.dapature,
    this.destination,
    this.ticketType,
    this.stopPlaces,
    this.bigLuggageKg,
    this.smallLuggageKg,
    this.stops,
    this.available,
    this.isReturn,
    this.ticketPrice,
    this.createdAt,
  });

  factory FlightTicketResponseModel.fromJson(Map<String, dynamic> json) =>
      FlightTicketResponseModel(
        id: json["id"],
        airlineId: json["airlineId"],
        adminId: json["adminId"],
        airlineName: json["airlineName"],
        operatedBy: json["operatedBy"],
        flightSpeed: json["flightSpeed"],
        ticketNum: json["ticketNum"],
        dapatureTime: json["dapatureTime"],
        availabeTimeInterval: json["availabeTimeInterval"],
        dapature: json["dapature"],
        destination: json["destination"],
        ticketType: json["ticketType"],
        stopPlaces: json["stopPlaces"] == null
            ? []
            : List<String>.from(json["stopPlaces"]!.map((x) => x)),
        bigLuggageKg: json["bigLuggageKg"],
        smallLuggageKg: json["smallLuggageKg"],
        stops: json["stops"],
        available: json["available"],
        isReturn: json["isReturn"],
        ticketPrice: json["ticketPrice"],
        createdAt: json["createdAt"] == null
            ? null
            : DateTime.parse(json["createdAt"]),
      );

  Map<String, dynamic> toJson() => {
        "id": id,
        "airlineId": airlineId,
        "adminId": adminId,
        "airlineName": airlineName,
        "operatedBy": operatedBy,
        "flightSpeed": flightSpeed,
        "ticketNum": ticketNum,
        "dapatureTime": dapatureTime,
        "availabeTimeInterval": availabeTimeInterval,
        "dapature": dapature,
        "destination": destination,
        "ticketType": ticketType,
        "stopPlaces": stopPlaces == null
            ? []
            : List<dynamic>.from(stopPlaces!.map((x) => x)),
        "bigLuggageKg": bigLuggageKg,
        "smallLuggageKg": smallLuggageKg,
        "stops": stops,
        "available": available,
        "isReturn": isReturn,
        "ticketPrice": ticketPrice,
        "createdAt": createdAt?.toIso8601String(),
      };
}
