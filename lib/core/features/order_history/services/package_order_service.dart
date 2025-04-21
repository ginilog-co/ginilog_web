import 'dart:convert';

import 'package:ginilog_customer_app/core/components/extension/error_handling.dart';
import 'package:ginilog_customer_app/core/components/helpers/endpoints.dart';
import 'package:ginilog_customer_app/core/components/helpers/globals.dart';
import 'package:ginilog_customer_app/core/components/utils/constants.dart';
import 'package:ginilog_customer_app/core/features/order_history/model/package_orders_model.dart';
import 'package:http/http.dart' as http;

class PackageOrderService {
  Future<http.Response> createOrderWithAddress({
    required String companyId,
    required String itemName,
    required String itemDescription,
    required String itemModelNumber,
    required num itemCost,
    required num itemWeight,
    required int itemQuantity,
    required String packageType,
    required String senderName,
    required String senderPhoneNo,
    required String senderEmail,
    required String senderAddress,
    required String senderState,
    required String senderCountry,
    required String senderLocality,
    required String senderPostalCode,
    required num senderLatitude,
    required num senderLongitude,
    required String recieverName,
    required String recieverPhoneNo,
    required String recieverEmail,
    required String recieverAddress,
    required String recieverState,
    required String recieverCountry,
    required String recieverLocality,
    required String recieverPostalCode,
    required num recieverLatitude,
    required num recieverLongitude,
    required List<String> packageImageLists,
    required String riderType,
    required String shippingType,
  }) async {
    try {
      var stingUrl = Uri.parse("${Endpoints.baseUrl}Logistics/package-orders");
      Map<String, String> headers = {
        'Content-Type': 'application/json',
        'Accept': 'application/json',
        'Authorization': 'Bearer ${globals.token}',
        "companyId": companyId
      };
      final msg = jsonEncode(
        {
          "itemName": itemName,
          "itemDescription": itemDescription,
          "itemModelNumber": itemModelNumber,
          "itemCost": itemCost,
          "itemWeight": itemWeight,
          "itemQuantity": itemQuantity,
          "packageType": packageType,
          "senderName": senderName,
          "senderPhoneNo": senderPhoneNo,
          "senderEmail": senderEmail,
          "senderAddress": senderAddress,
          "senderState": senderState,
          "senderCountry": senderCountry,
          "senderLocality": senderLocality,
          "senderPostalCode": senderPostalCode,
          "senderLatitude": senderLatitude,
          "senderLongitude": senderLongitude,
          "recieverName": recieverName,
          "recieverPhoneNo": recieverPhoneNo,
          "recieverEmail": recieverEmail,
          "recieverAddress": recieverAddress,
          "recieverState": recieverState,
          "recieverCountry": recieverCountry,
          "recieverLocality": recieverLocality,
          "recieverPostalCode": recieverPostalCode,
          "recieverLatitude": recieverLatitude,
          "recieverLongitude": recieverLongitude,
          "packageImageLists": packageImageLists,
          "riderType": riderType,
          "shippingType": shippingType
        },
      );
      http.Response response =
          await http.post(stingUrl, body: msg, headers: headers);
      printData("dataResponse Status Code", response.statusCode);
      printData("dataResponse", response.body);
      if (response.statusCode == 200 || response.statusCode == 201) {
        printData("successRegister", response.body);

        return response;
      } else {
        printData("Error", response.body);
        return response;
      }
    } catch (e) {
      printData('Error', e.toString());
      return Future.error(handleHttpError(e));
    }
  }

  Future<http.Response> makePayment({
    required String orderId,
    required bool paymentStatus,
    required String paymentChannel,
    required String trnxReference,
  }) async {
    try {
      var stingUrl =
          Uri.parse("${Endpoints.baseUrl}Logistics/package-orders/$orderId");
      Map<String, String> headers = {
        'Content-Type': 'application/json',
        'Accept': 'application/json',
        'Authorization': 'Bearer ${globals.token}'
      };
      final msg = jsonEncode(
        {
          "paymentStatus": paymentStatus,
          "paymentChannel": paymentChannel,
          "trnxReference": trnxReference
        },
      );
      http.Response response =
          await http.put(stingUrl, body: msg, headers: headers);
      printData("dataResponse Status Code", response.statusCode);
      printData("dataResponse", response.body);
      printData("orderId", orderId);
      printData("paymentStatus", paymentStatus);
      printData("paymentChannel", paymentChannel);
      printData("trnxReference", trnxReference);
      if (response.statusCode == 200 || response.statusCode == 201) {
        printData("successRegister", response.body);

        return response;
      } else {
        printData("Error", response.body);
        return response;
      }
    } catch (e) {
      printData('Error', e.toString());
      return Future.error(handleHttpError(e));
    }
  }

  Future<PackageOrderResponseModel> getPackageOrdersData(
      {required String orderId}) async {
    PackageOrderResponseModel data = PackageOrderResponseModel();
    try {
      Map<String, String> headers2 = {
        'Content-Type': 'application/json',
        'Accept': 'application/json',
        'Authorization': 'Bearer ${globals.token}'
      };
      var url =
          Uri.parse("${Endpoints.baseUrl}Logistics/package-orders/$orderId");
      var response = await http.get(url, headers: headers2);
      if (response.statusCode == 200 || response.statusCode == 201) {
        final item = json.decode(response.body);
        data = PackageOrderResponseModel.fromJson(
            item); // Mapping json response to our data model
        printData('Order Details', data);
        return data;
      } else {
        printData('Order Details Error', response.body);
        return data;
      }
    } catch (e) {
      printData('Error', e.toString());
      return Future.error(handleHttpError(e));
    }
  }

  Future<List<PackageOrderResponseModel>> getAllPackageOrdersData() async {
    List<PackageOrderResponseModel> data = [];
    try {
      Map<String, String> headers2 = {
        'Content-Type': 'application/json',
        'Accept': 'application/json',
        'Authorization': 'Bearer ${globals.token}',
      };
      var url = Uri.parse(
        "${Endpoints.baseUrl}Logistics/package-orders?AnyItem=${globals.userId}",
      );
      var response = await http.get(url, headers: headers2);
      if (response.statusCode == 200 || response.statusCode == 201) {
        var data1 = packageOrderResponseModelFromJson(response.body);
        data = data1;
        printData('All Package Order', response.body);
        return data;
      } else {
        printData('All Package Order Error', response.body);
      }
    } catch (e) {
      printData('Error', e.toString());
      return Future.error(handleHttpError(e));
    }
    return data;
  }

  // Stream Package Order
  Stream<List<PackageOrderResponseModel>>
      getAllStreamPackageOrdersData() async* {
    List<PackageOrderResponseModel> data = [];
    try {
      Map<String, String> headers2 = {
        'Content-Type': 'application/json',
        'Accept': 'application/json',
        'Authorization': 'Bearer ${globals.token}'
      };
      var url = Uri.parse(
          "${Endpoints.baseUrl}Logistics/package-orders?AnyItem=${globals.userId}");
      var response = await http.get(url, headers: headers2);
      if (response.statusCode == 200 || response.statusCode == 201) {
        var data1 = packageOrderResponseModelFromJson(response.body);
        data = data1;
        printData('All Package Order Stream', response.body);
        yield data;
      } else {
        printData('All Package Order Stream Error2S', response.body);
      }
    } catch (e) {
      printData('Error', e.toString());
      yield* Stream.error(e);
    }
  }

  Future<http.Response> updateOrder({required String orderId}) async {
    try {
      var stingUrl =
          Uri.parse("${Endpoints.baseUrl}Logistics/package-orders/$orderId");
      Map<String, String> headers = {
        'Content-Type': 'application/json',
        'Accept': 'application/json',
        'Authorization': 'Bearer ${globals.token}'
      };
      final msg = jsonEncode(
        {
          "orderStatus": "Closed",
        },
      );
      http.Response response =
          await http.put(stingUrl, body: msg, headers: headers);
      printData("dataResponse", response.body);
      if (response.statusCode == 200 || response.statusCode == 201) {
        printData("successRegister", response.body);

        return response;
      } else {
        printData("Error", response.body);
        return response;
      }
    } catch (e) {
      printData('Error', e.toString());
      return Future.error(handleHttpError(e));
    }
  }
}
