import 'dart:convert';
import 'package:http/http.dart' as http;
import 'package:ginilog_customer_app/core/components/helpers/endpoints.dart';
import 'package:ginilog_customer_app/core/components/helpers/globals.dart';
import 'package:ginilog_customer_app/core/components/utils/constants.dart';

class ApiService {
  static Future<String> upload(String file) async {
    String imagePath = "";
    Map<String, String> headers2 = {
      'Content-Type': 'multipart/form-data',
      'Accept': '*/*',
      'Authorization': 'Bearer ${globals.token}',
    };
    var request = http.MultipartRequest(
        'POST', Uri.parse("${Endpoints.baseUrl}${Endpoints.uploadUrl}"));
    request.headers.addAll(headers2);
    request.files.add(await http.MultipartFile.fromPath(
      'file',
      file,
    ));
    var res = await request.send();
    var response = await http.Response.fromStream(res);
    printData('Upload Url', ("${Endpoints.baseUrl}${Endpoints.uploadUrl}"));
    if (response.statusCode == 200) {
      var data = jsonDecode(response.body.toString());
      printData('Api body', (data));
      imagePath = data['imageUrl'].toString();
      return imagePath;
    } else {
      printData('Upload Error', (response.statusCode));
    }
    return imagePath;
  }
}
