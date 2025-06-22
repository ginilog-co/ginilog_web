// To parse this JSON data, do
//
//     final advertResponseModel = advertResponseModelFromJson(jsonString);

import 'dart:convert';

List<AdvertResponseModel> advertResponseModelFromJson(String str) =>
    List<AdvertResponseModel>.from(
      json.decode(str).map((x) => AdvertResponseModel.fromJson(x)),
    );

String advertResponseModelToJson(List<AdvertResponseModel> data) =>
    json.encode(List<dynamic>.from(data.map((x) => x.toJson())));

class AdvertResponseModel {
  final String? id;
  final String? adminId;
  final String? advertItemId;
  final String? advertImage;
  final String? advertName;
  final String? advertType;
  final String? advertItemDescription;
  final num? advertItemCost;
  final String? transRef;
  final bool? transStatus;
  final num? advertDays4;
  final DateTime? expiredAt;
  final DateTime? createdAt;

  AdvertResponseModel({
    this.id,
    this.adminId,
    this.advertItemId,
    this.advertImage,
    this.advertName,
    this.advertType,
    this.advertItemDescription,
    this.advertItemCost,
    this.transRef,
    this.transStatus,
    this.advertDays4,
    this.expiredAt,
    this.createdAt,
  });

  AdvertResponseModel copyWith({
    String? id,
    String? adminId,
    String? advertItemId,
    String? advertImage,
    String? advertName,
    String? advertType,
    String? advertItemDescription,
    num? advertItemCost,
    String? transRef,
    bool? transStatus,
    num? advertDays4,
    DateTime? expiredAt,
    DateTime? createdAt,
  }) => AdvertResponseModel(
    id: id ?? this.id,
    adminId: adminId ?? this.adminId,
    advertItemId: advertItemId ?? this.advertItemId,
    advertImage: advertImage ?? this.advertImage,
    advertName: advertName ?? this.advertName,
    advertType: advertType ?? this.advertType,
    advertItemDescription: advertItemDescription ?? this.advertItemDescription,
    advertItemCost: advertItemCost ?? this.advertItemCost,
    transRef: transRef ?? this.transRef,
    transStatus: transStatus ?? this.transStatus,
    advertDays4: advertDays4 ?? this.advertDays4,
    expiredAt: expiredAt ?? this.expiredAt,
    createdAt: createdAt ?? this.createdAt,
  );

  factory AdvertResponseModel.fromJson(
    Map<String, dynamic> json,
  ) => AdvertResponseModel(
    id: json["id"] ?? "",
    adminId: json["adminId"] ?? "",
    advertItemId: json["advertItemId"] ?? "",
    advertImage: json["advertImage"] ?? "",
    advertName: json["advertName"] ?? "",
    advertType: json["advertType"] ?? "",
    advertItemDescription: json["advertItemDescription"] ?? "",
    advertItemCost: json["advertItemCost"] ?? 0,
    transRef: json["transRef"] ?? "",
    transStatus: json["transStatus"] ?? false,
    advertDays4: json["advertDays4"] ?? 0,
    expiredAt:
        json["expiredAt"] == null ? null : DateTime.parse(json["expiredAt"]),
    createdAt:
        json["createdAt"] == null ? null : DateTime.parse(json["createdAt"]),
  );

  Map<String, dynamic> toJson() => {
    "id": id,
    "adminId": adminId,
    "advertItemId": advertItemId,
    "advertImage": advertImage,
    "advertName": advertName,
    "advertType": advertType,
    "advertItemDescription": advertItemDescription,
    "advertItemCost": advertItemCost,
    "transRef": transRef,
    "transStatus": transStatus,
    "advertDays4": advertDays4,
    "expiredAt": expiredAt?.toIso8601String(),
    "createdAt": createdAt?.toIso8601String(),
  };
}
