// To parse this JSON data, do
//
//     final userDeliveryAddress = userDeliveryAddressFromJson(jsonString);

import 'dart:convert';

List<UserDeliveryAddress> userDeliveryAddressFromJson(String str) =>
    List<UserDeliveryAddress>.from(
        json.decode(str).map((x) => UserDeliveryAddress.fromJson(x)));

String userDeliveryAddressToJson(List<UserDeliveryAddress> data) =>
    json.encode(List<dynamic>.from(data.map((x) => x.toJson())));

class UserDeliveryAddress {
  final String? id;
  final String? address;
  final String? userName;
  final String? phoneNo;
  final String? addressPostCodes;
  final String? city;
  final String? state;
  final String? houseNo;
  final double? latitude;
  final double? longitude;
  final String? usersDataModelTableId;
  final DateTime? createdAt;

  UserDeliveryAddress({
    this.id,
    this.address,
    this.userName,
    this.phoneNo,
    this.addressPostCodes,
    this.city,
    this.state,
    this.houseNo,
    this.latitude,
    this.longitude,
    this.usersDataModelTableId,
    this.createdAt,
  });

  factory UserDeliveryAddress.fromJson(Map<String, dynamic> json) =>
      UserDeliveryAddress(
        id: json["id"],
        address: json["address"],
        userName: json["userName"],
        phoneNo: json["phoneNo"],
        addressPostCodes: json["addressPostCodes"],
        city: json["city"],
        state: json["state"],
        houseNo: json["houseNo"],
        latitude: json["latitude"]?.toDouble(),
        longitude: json["longitude"]?.toDouble(),
        usersDataModelTableId: json["usersDataModelTableId"],
        createdAt: json["createdAt"] == null
            ? null
            : DateTime.parse(json["createdAt"]),
      );

  Map<String, dynamic> toJson() => {
        "id": id,
        "address": address,
        "userName": userName,
        "phoneNo": phoneNo,
        "addressPostCodes": addressPostCodes,
        "city": city,
        "state": state,
        "houseNo": houseNo,
        "latitude": latitude,
        "longitude": longitude,
        "usersDataModelTableId": usersDataModelTableId,
        "createdAt": createdAt?.toIso8601String(),
      };
}

class Region {
  final String name;
  final String nativeName;
  final String code;
  final List<String> lgas;

  const Region({
    required this.name,
    required this.nativeName,
    required this.code,
    required this.lgas,
  });

  factory Region.fromJson(Map<String, dynamic> json) => Region(
        name: json['name'],
        code: json['code'],
        nativeName: json['native'],
        lgas: List<String>.from(json['lgas']),
      );

  @override
  bool operator ==(Object other) =>
      identical(this, other) ||
      other is Region &&
          runtimeType == other.runtimeType &&
          name == other.name &&
          nativeName == other.nativeName &&
          code == other.code &&
          lgas == other.lgas;

  @override
  int get hashCode =>
      name.hashCode ^ nativeName.hashCode ^ code.hashCode ^ lgas.hashCode;
}
