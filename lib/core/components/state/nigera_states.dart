import 'dart:convert';
import 'package:ginilog_customer_app/core/components/helpers/endpoints.dart';
import 'package:ginilog_customer_app/core/components/helpers/globals.dart';
import 'package:ginilog_customer_app/core/components/utils/constants.dart';
import 'package:flutter/foundation.dart';
import 'package:http/http.dart' as http;
import 'package:ginilog_customer_app/core/components/utils/package_export.dart';
import 'package:ginilog_customer_app/core/features/account/model/user_response_model.dart';

class RegionProvider with ChangeNotifier {
  RegionProvider() {
    loadCountries().then((countries) {
      _countries = countries;
      notifyListeners();
    }).catchError((e) {
      // Handle errors gracefully
      if (kDebugMode) {
        print("Failed to load countries: $e");
      }
    });
  }

  List<DeliveryAddress> _countries = [];
  List<DeliveryAddress> get countries => _countries;

  Future<List<DeliveryAddress>> loadCountries() async {
    try {
      Map<String, String> headers2 = {
        'Content-Type': 'application/json',
        'Accept': 'application/json',
        'Authorization': 'Bearer ${globals.token}'
      };
      var url = Uri.parse(
          "${Endpoints.baseUrl}${Endpoints.usersUrl}/${globals.userId}");
      var response = await http.get(url, headers: headers2);

      if (response.statusCode == 200 || response.statusCode == 201) {
        final countriesJson = json.decode(response.body);
        final jsonD = countriesJson["deliveryAddresses"];

        // Ensure proper handling of the delivery addresses list
        if (jsonD is List) {
          printData('User Delivery address', jsonD);
          return jsonD.map<DeliveryAddress>((json) {
            return DeliveryAddress.fromJson(json);
          }).toList()
            ..sort(Utils.ascendingSort);
        } else {
          throw Exception("Expected 'deliveryAddresses' to be a List");
        }
      } else {
        printData('User Details Error', response.body);
        throw Exception("Failed to fetch countries: ${response.statusCode}");
      }
    } catch (e) {
      if (kDebugMode) {
        print("Error in loadCountries: $e");
      }
      rethrow;
    }
  }
}

class Utils {
  static int ascendingSort(DeliveryAddress c1, DeliveryAddress c2) =>
      c1.state!.compareTo(c2.state!);
}

// Provider definition
final regionProvider =
    ChangeNotifierProvider<RegionProvider>((ref) => RegionProvider());
