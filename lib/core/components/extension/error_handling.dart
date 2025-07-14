import 'dart:convert';
import 'dart:io';

String handleHttpError(dynamic error) {
  if (error is SocketException) {
    return 'No Internet connection.';
  } else if (error is HttpException) {
    return 'Server error: ${error.message}';
  } else if (error is FormatException) {
    return 'Bad response format.';
  } else {
    return 'Unexpected error: $error';
  }
}

String getErrorMessageFromResponse(int statusCode, String responseBody) {
  if (statusCode >= 500) {
    return "Server error. Please try again later.";
  }

  try {
    final decoded = json.decode(responseBody);

    if (decoded is Map) {
      // 1. Handle general message
      if (decoded.containsKey('message') && decoded['message'] is String) {
        return decoded['message'];
      }

      // 2. Handle specific field errors like Email, PhoneNo, etc.
      for (final key in ['Email', 'PhoneNo', 'Username', 'Password']) {
        if (decoded.containsKey(key) &&
            decoded[key] is List &&
            decoded[key].isNotEmpty) {
          return decoded[key][0]; // First message in the list
        }
      }
    } else if (decoded is String) {
      // 3. If the backend returned just a string like "User not found."
      return decoded;
    }

    return "An error occurred. Please try again.";
  } catch (_) {
    // If it's a raw string and not JSON
    if (responseBody.trim().isNotEmpty &&
        !responseBody.trim().startsWith('{')) {
      return responseBody.trim();
    }

    return "An error occurred. Please try again.";
  }
}

String handleError(dynamic error) {
  if (error is SocketException) {
    return 'No Internet connection.';
  } else if (error is HttpException) {
    return 'Server error: ${error.message}';
  } else if (error is FormatException) {
    return 'Bad response format.';
  } else {
    return 'Unexpected error: $error';
  }
}
