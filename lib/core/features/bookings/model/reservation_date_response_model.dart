// To parse this JSON data, do
//
//     final reservationDataResponseModel = reservationDataResponseModelFromJson(jsonString);

import 'dart:convert';

List<ReservationDataResponseModel> reservationDataResponseModelFromJson(
  String str,
) => List<ReservationDataResponseModel>.from(
  json.decode(str).map((x) => ReservationDataResponseModel.fromJson(x)),
);

String reservationDataResponseModelToJson(
  List<ReservationDataResponseModel> data,
) => json.encode(List<dynamic>.from(data.map((x) => x.toJson())));

class ReservationDataResponseModel {
  final DateTime? reservationStartDate;
  final DateTime? reservationEndDate;

  ReservationDataResponseModel({
    this.reservationStartDate,
    this.reservationEndDate,
  });

  ReservationDataResponseModel copyWith({
    DateTime? reservationStartDate,
    DateTime? reservationEndDate,
  }) => ReservationDataResponseModel(
    reservationStartDate: reservationStartDate ?? this.reservationStartDate,
    reservationEndDate: reservationEndDate ?? this.reservationEndDate,
  );

  factory ReservationDataResponseModel.fromJson(Map<String, dynamic> json) =>
      ReservationDataResponseModel(
        reservationStartDate:
            json["reservationStartDate"] == null
                ? null
                : DateTime.parse(json["reservationStartDate"]),
        reservationEndDate:
            json["reservationEndDate"] == null
                ? null
                : DateTime.parse(json["reservationEndDate"]),
      );

  Map<String, dynamic> toJson() => {
    "reservationStartDate": reservationStartDate?.toIso8601String(),
    "reservationEndDate": reservationEndDate?.toIso8601String(),
  };
}
