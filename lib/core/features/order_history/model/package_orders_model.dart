// To parse this JSON data, do
//
//     final packageOrderResponseModel = packageOrderResponseModelFromJson(jsonString);

import 'dart:convert';

enum OrderClassState { open, picked, booked, inTransit, delivered, cancelled }

List<PackageOrderResponseModel> packageOrderResponseModelFromJson(String str) =>
    List<PackageOrderResponseModel>.from(
        json.decode(str).map((x) => PackageOrderResponseModel.fromJson(x)));

String packageOrderResponseModelToJson(List<PackageOrderResponseModel> data) =>
    json.encode(List<dynamic>.from(data.map((x) => x.toJson())));

class PackageOrderResponseModel {
  final String? id;
  final String? trackingNum;
  final String? itemName;
  final String? itemDescription;
  final String? itemModelNumber;
  final num? itemCost;
  final num? itemQuantity;
  final String? packageType;
  final String? expectedDeliveryTime;
  final OrderClassState? orderStatus;
  final String? userId;
  final String? senderName;
  final String? senderPhoneNo;
  final String? senderEmail;
  final String? senderAddress;
  final String? senderState;
  final String? senderLocality;
  final String? senderPostalCode;
  final num? senderLatitude;
  final num? senderLongitude;
  final String? recieverName;
  final String? recieverPhoneNo;
  final String? recieverEmail;
  final String? recieverAddress;
  final String? recieverState;
  final String? recieverLocality;
  final String? recieverPostalCode;
  final num? recieverLatitude;
  final num? recieverLongitude;
  final String? companyId;
  final String? riderId;
  final String? riderName;
  final String? companyName;
  final String? companyPhoneNo;
  final String? companyEmail;
  final String? companyAddress;
  final num? currentLatitude;
  final num? currentLongitude;
  final String? currentLocation;
  final String? confirmationImage;
  final num? shippingCost;
  final String? trnxReference;
  final String? paymentChannel;
  final bool? paymentStatus;
  final String? qrCode;
  final String? comment;
  final DateTime? createdAt;
  final DateTime? updatedAt;
  final List<String>? packageImageLists;
  final String? riderType;
  final String? shippingType;
  final num? itemWeight;
  final String? senderCountry;
  final String? recieverCountry;
  final num? vatCost;
  final List<OrderDeliveryFlow>? orderDeliveryFlows;

  PackageOrderResponseModel(
      {this.id,
      this.trackingNum,
      this.itemName,
      this.itemDescription,
      this.itemModelNumber,
      this.itemCost,
      this.itemQuantity,
      this.packageType,
      this.expectedDeliveryTime,
      this.orderStatus,
      this.userId,
      this.senderName,
      this.senderPhoneNo,
      this.senderEmail,
      this.senderAddress,
      this.senderState,
      this.senderLocality,
      this.senderPostalCode,
      this.senderLatitude,
      this.senderLongitude,
      this.recieverName,
      this.recieverPhoneNo,
      this.recieverEmail,
      this.recieverAddress,
      this.recieverState,
      this.recieverLocality,
      this.recieverPostalCode,
      this.recieverLatitude,
      this.recieverLongitude,
      this.companyId,
      this.riderId,
      this.riderName,
      this.companyName,
      this.companyPhoneNo,
      this.companyEmail,
      this.companyAddress,
      this.currentLatitude,
      this.currentLongitude,
      this.currentLocation,
      this.confirmationImage,
      this.shippingCost,
      this.trnxReference,
      this.paymentChannel,
      this.paymentStatus,
      this.qrCode,
      this.comment,
      this.createdAt,
      this.updatedAt,
      this.packageImageLists,
      this.riderType,
      this.shippingType,
      this.itemWeight,
      this.recieverCountry,
      this.senderCountry,
      this.vatCost,
      this.orderDeliveryFlows});

  factory PackageOrderResponseModel.fromJson(Map<String, dynamic> json) =>
      PackageOrderResponseModel(
        id: json["id"] ?? "",
        trackingNum: json["trackingNum"] ?? "",
        itemName: json["itemName"] ?? "",
        itemDescription: json["itemDescription"] ?? "",
        itemModelNumber: json["itemModelNumber"] ?? "",
        itemCost: json["itemCost"] ?? 0,
        itemQuantity: json["itemQuantity"] ?? 0,
        packageType: json["packageType"] ?? "",
        expectedDeliveryTime: json["expectedDeliveryTime"] ?? "",
        orderStatus: _parseOrderClassState(json["orderStatus"] ?? "Open"),
        userId: json["userId"] ?? "",
        senderName: json["senderName"] ?? "",
        senderPhoneNo: json["senderPhoneNo"] ?? "",
        senderEmail: json["senderEmail"] ?? "",
        senderAddress: json["senderAddress"] ?? "",
        senderState: json["senderState"] ?? "",
        senderLocality: json["senderLocality"] ?? "",
        senderPostalCode: json["senderPostalCode"] ?? "",
        senderLatitude: json["senderLatitude"] ?? 0,
        senderLongitude: json["senderLongitude"] ?? 0,
        recieverName: json["recieverName"] ?? "",
        recieverPhoneNo: json["recieverPhoneNo"] ?? "",
        recieverEmail: json["recieverEmail"] ?? "",
        recieverAddress: json["recieverAddress"] ?? "",
        recieverState: json["recieverState"] ?? "",
        recieverLocality: json["recieverLocality"] ?? "",
        recieverPostalCode: json["recieverPostalCode"] ?? "",
        recieverLatitude: json["recieverLatitude"] ?? 0,
        recieverLongitude: json["recieverLongitude"] ?? 0,
        companyId: json["companyId"] ?? "",
        riderId: json["riderId"] ?? "",
        riderName: json["riderName"] ?? "",
        companyName: json["companyName"] ?? "",
        companyPhoneNo: json["companyPhoneNo"] ?? "",
        companyEmail: json["companyEmail"] ?? "",
        companyAddress: json["companyAddress"] ?? "",
        currentLatitude: json["currentLatitude"] ?? 0,
        currentLongitude: json["currentLongitude"] ?? 0,
        currentLocation: json["currentLocation"] ?? "",
        confirmationImage: json["confirmationImage"] ?? "",
        shippingCost: json["shippingCost"] ?? 0,
        trnxReference: json["trnxReference"] ?? "",
        paymentChannel: json["paymentChannel"] ?? "",
        paymentStatus: json["paymentStatus"] ?? false,
        qrCode: json["qrCode"] ?? "",
        comment: json["comment"] ?? "",
        createdAt: json["createdAt"] == null
            ? null
            : DateTime.parse(json["createdAt"]),
        updatedAt: json["updatedAt"] == null
            ? null
            : DateTime.parse(json["updatedAt"]),
        packageImageLists: json["packageImageLists"] == null
            ? []
            : List<String>.from(json["packageImageLists"]!.map((x) => x)),
        riderType: json["riderType"] ?? "",
        shippingType: json["shippingType"] ?? "",
        itemWeight: json["itemWeight"] ?? 0,
        senderCountry: json["senderCountry"] ?? "",
        recieverCountry: json["recieverCountry"] ?? "",
        vatCost: json["vatCost"] ?? 0,
        orderDeliveryFlows: json["orderDeliveryFlows"] == null
            ? []
            : List<OrderDeliveryFlow>.from(json["orderDeliveryFlows"]!
                .map((x) => OrderDeliveryFlow.fromJson(x))),
      );

  Map<String, dynamic> toJson() => {
        "id": id,
        "trackingNum": trackingNum,
        "itemName": itemName,
        "itemDescription": itemDescription,
        "itemModelNumber": itemModelNumber,
        "itemCost": itemCost,
        "itemQuantity": itemQuantity,
        "packageType": packageType,
        "expectedDeliveryTime": expectedDeliveryTime,
        "orderStatus": orderStatus!.name,
        "userId": userId,
        "senderName": senderName,
        "senderPhoneNo": senderPhoneNo,
        "senderEmail": senderEmail,
        "senderAddress": senderAddress,
        "senderState": senderState,
        "senderLocality": senderLocality,
        "senderPostalCode": senderPostalCode,
        "senderLatitude": senderLatitude,
        "senderLongitude": senderLongitude,
        "recieverName": recieverName,
        "recieverPhoneNo": recieverPhoneNo,
        "recieverEmail": recieverEmail,
        "recieverAddress": recieverAddress,
        "recieverState": recieverState,
        "recieverLocality": recieverLocality,
        "recieverPostalCode": recieverPostalCode,
        "recieverLatitude": recieverLatitude,
        "recieverLongitude": recieverLongitude,
        "companyId": companyId,
        "riderId": riderId,
        "riderName": riderName,
        "companyName": companyName,
        "companyPhoneNo": companyPhoneNo,
        "companyEmail": companyEmail,
        "companyAddress": companyAddress,
        "currentLatitude": currentLatitude,
        "currentLongitude": currentLongitude,
        "currentLocation": currentLocation,
        "confirmationImage": confirmationImage,
        "shippingCost": shippingCost,
        "trnxReference": trnxReference,
        "paymentChannel": paymentChannel,
        "paymentStatus": paymentStatus,
        "qrCode": qrCode,
        "comment": comment,
        "createdAt": createdAt?.toIso8601String(),
        "updatedAt": updatedAt?.toIso8601String(),
        "packageImageLists": packageImageLists == null
            ? []
            : List<dynamic>.from(packageImageLists!.map((x) => x)),
        "riderType": riderType,
        "shippingType": shippingType,
        "itemWeight": itemWeight,
        "senderCountry": senderCountry,
        "recieverCountry": recieverCountry,
        "vatCost": vatCost,
        "orderDeliveryFlows": orderDeliveryFlows == null
            ? []
            : List<dynamic>.from(orderDeliveryFlows!.map((x) => x.toJson())),
      };
  static OrderClassState _parseOrderClassState(String state) {
    switch (state) {
      case 'Open':
        return OrderClassState.open;
      case 'Picked':
        return OrderClassState.picked;
      case 'Booked':
        return OrderClassState.booked;
      case 'InTransit':
        return OrderClassState.inTransit;
      case 'Delivered':
        return OrderClassState.delivered;
      case 'Cancelled':
        return OrderClassState.cancelled;
      default:
        throw Exception(
            'Unknown order state: $state'); // Or return a default value
    }
  }
}

class OrderDeliveryFlow {
  final String? id;
  final OrderClassState? orderStatus;
  final num? currentLatitude;
  final num? currentLongitude;
  final String? currentLocation;
  final String? orderModelDataId;
  final DateTime? updatedAt;

  OrderDeliveryFlow({
    this.id,
    this.orderStatus,
    this.currentLatitude,
    this.currentLongitude,
    this.currentLocation,
    this.orderModelDataId,
    this.updatedAt,
  });

  factory OrderDeliveryFlow.fromJson(Map<String, dynamic> json) =>
      OrderDeliveryFlow(
        id: json["id"] ?? "",
        orderStatus: _parseOrderClassState(json["orderStatus"] ?? "Open"),
        currentLatitude: json["currentLatitude"] ?? 0,
        currentLongitude: json["currentLongitude"] ?? 0,
        currentLocation: json["currentLocation"] ?? "",
        orderModelDataId: json["orderModelDataId"] ?? "",
        updatedAt: json["updatedAt"] == null
            ? null
            : DateTime.parse(json["updatedAt"]),
      );

  Map<String, dynamic> toJson() => {
        "id": id,
        "orderStatus": orderStatus!.name,
        "currentLatitude": currentLatitude,
        "currentLongitude": currentLongitude,
        "currentLocation": currentLocation,
        "orderModelDataId": orderModelDataId,
        "updatedAt": updatedAt?.toIso8601String(),
      };
  static OrderClassState _parseOrderClassState(String state) {
    switch (state) {
      case 'Open':
        return OrderClassState.open;
      case 'Picked':
        return OrderClassState.picked;
      case 'Booked':
        return OrderClassState.booked;
      case 'InTransit':
        return OrderClassState.inTransit;
      case 'Delivered':
        return OrderClassState.delivered;
      case 'Cancelled':
        return OrderClassState.cancelled;
      default:
        throw Exception(
            'Unknown order state: $state'); // Or return a default value
    }
  }
}
