// To parse this JSON data, do
//
//     final registerResponseModel = registerResponseModelFromJson(jsonString);

import 'dart:convert';

RegisterResponseModel registerResponseModelFromJson(String str) =>
    RegisterResponseModel.fromJson(json.decode(str));

String registerResponseModelToJson(RegisterResponseModel data) =>
    json.encode(data.toJson());

class RegisterResponseModel {
  final String? id;
  final String? firstName;
  final String? lastName;
  final String? email;
  final String? phoneNo;
  final String? profilePicture;
  final String? sex;
  final String? referralCode;
  final UserAddress? userAddress;
  final List<DeliveryAddress>? deliveryAddresses;
  final List<DeviceTokenModel>? deviceTokenModels;
  final DateTime? createdAt;

  RegisterResponseModel({
    this.id,
    this.firstName,
    this.lastName,
    this.email,
    this.phoneNo,
    this.profilePicture,
    this.sex,
    this.referralCode,
    this.userAddress,
    this.deliveryAddresses,
    this.deviceTokenModels,
    this.createdAt,
  });

  factory RegisterResponseModel.fromJson(Map<String, dynamic> json) =>
      RegisterResponseModel(
        id: json["id"],
        firstName: json["firstName"],
        lastName: json["lastName"],
        email: json["email"],
        phoneNo: json["phoneNo"],
        profilePicture: json["profilePicture"],
        sex: json["sex"],
        referralCode: json["referralCode"],
        userAddress: json["userAddress"] == null
            ? null
            : UserAddress.fromJson(json["userAddress"]),
        deliveryAddresses: json["deliveryAddresses"] == null
            ? []
            : List<DeliveryAddress>.from(json["deliveryAddresses"]!
                .map((x) => DeliveryAddress.fromJson(x))),
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
        "sex": sex,
        "referralCode": referralCode,
        "userAddress": userAddress?.toJson(),
        "deliveryAddresses": deliveryAddresses == null
            ? []
            : List<dynamic>.from(deliveryAddresses!.map((x) => x.toJson())),
        "deviceTokenModels": deviceTokenModels == null
            ? []
            : List<dynamic>.from(deviceTokenModels!.map((x) => x.toJson())),
        "createdAt": createdAt?.toIso8601String(),
      };
}

class UserAddress {
  final String? id;
  final String? address;
  final String? postCodes;
  final String? city;
  final String? state;
  final num? latitude;
  final num? longitude;
  final String? usersDataModelTableId;

  UserAddress({
    this.id,
    this.address,
    this.postCodes,
    this.city,
    this.state,
    this.latitude,
    this.longitude,
    this.usersDataModelTableId,
  });

  factory UserAddress.fromJson(Map<String, dynamic> json) => UserAddress(
        id: json["id"],
        address: json["address"],
        postCodes: json["postCodes"],
        city: json["city"],
        state: json["state"],
        latitude: json["latitude"],
        longitude: json["longitude"],
        usersDataModelTableId: json["usersDataModelTableId"],
      );

  Map<String, dynamic> toJson() => {
        "id": id,
        "address": address,
        "postCodes": postCodes,
        "city": city,
        "state": state,
        "latitude": latitude,
        "longitude": longitude,
        "usersDataModelTableId": usersDataModelTableId,
      };
}

class DeliveryAddress {
  final String? id;
  final String? userName;
  final String? phoneNo;
  final String? address;
  final String? addressPostCodes;
  final String? city;
  final String? state;
  final String? houseNo;
  final num? latitude;
  final num? longitude;
  final String? usersDataModelTableId;
  final DateTime? createdAt;

  DeliveryAddress({
    this.id,
    this.userName,
    this.phoneNo,
    this.address,
    this.addressPostCodes,
    this.city,
    this.state,
    this.houseNo,
    this.latitude,
    this.longitude,
    this.usersDataModelTableId,
    this.createdAt,
  });

  factory DeliveryAddress.fromJson(Map<String, dynamic> json) =>
      DeliveryAddress(
        id: json["id"],
        userName: json["userName"],
        phoneNo: json["phoneNo"],
        address: json["address"],
        addressPostCodes: json["addressPostCodes"],
        city: json["city"],
        state: json["state"],
        houseNo: json["houseNo"],
        latitude: json["latitude"],
        longitude: json["longitude"],
        usersDataModelTableId: json["usersDataModelTableId"],
        createdAt: json["createdAt"] == null
            ? null
            : DateTime.parse(json["createdAt"]),
      );

  Map<String, dynamic> toJson() => {
        "id": id,
        "userName": userName,
        "phoneNo": phoneNo,
        "address": address,
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
        id: json["id"],
        deviceTokenId: json["deviceTokenId"],
        userId: json["userId"],
        userType: json["userType"],
      );

  Map<String, dynamic> toJson() => {
        "id": id,
        "deviceTokenId": deviceTokenId,
        "userId": userId,
        "userType": userType,
      };
}
