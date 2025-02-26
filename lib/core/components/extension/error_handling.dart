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
