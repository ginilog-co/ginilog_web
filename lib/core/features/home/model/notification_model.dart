// To parse this JSON data, do
//
//     final notificationResponseModel = notificationResponseModelFromJson(jsonString);

import 'dart:convert';

List<NotificationResponseModel> notificationResponseModelFromJson(String str) =>
    List<NotificationResponseModel>.from(
        json.decode(str).map((x) => NotificationResponseModel.fromJson(x)));

String notificationResponseModelToJson(List<NotificationResponseModel> data) =>
    json.encode(List<dynamic>.from(data.map((x) => x.toJson())));

class NotificationResponseModel {
  final String? id;
  final String? userId;
  final String? title;
  final String? body;
  final String? deviceToken;
  final String? notificationType;
  final String? imageUrl;
  final DateTime? createdAt;
  final DateTime? updatedAt;

  NotificationResponseModel({
    this.id,
    this.userId,
    this.title,
    this.body,
    this.deviceToken,
    this.notificationType,
    this.imageUrl,
    this.createdAt,
    this.updatedAt,
  });

  factory NotificationResponseModel.fromJson(Map<String, dynamic> json) =>
      NotificationResponseModel(
        id: json["id"] ?? "",
        userId: json["userId"] ?? "",
        title: json["title"] ?? "",
        body: json["body"] ?? "",
        deviceToken: json["deviceToken"] ?? "",
        notificationType: json["notificationType"] ?? "",
        imageUrl: json["imageUrl"] ?? "",
        createdAt: json["createdAt"] == null
            ? null
            : DateTime.parse(json["createdAt"]),
        updatedAt: json["updatedAt"] == null
            ? null
            : DateTime.parse(json["updatedAt"]),
      );

  Map<String, dynamic> toJson() => {
        "id": id,
        "userId": userId,
        "title": title,
        "body": body,
        "deviceToken": deviceToken,
        "notificationType": notificationType,
        "imageUrl": imageUrl,
        "createdAt": createdAt?.toIso8601String(),
        "updatedAt": updatedAt?.toIso8601String(),
      };
}
