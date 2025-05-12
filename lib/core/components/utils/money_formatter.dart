import 'package:ginilog_customer_app/core/components/utils/package_export.dart';

String moneyFormat(BuildContext context, double money) {
  Locale locale = Localizations.localeOf(context);
  var moneyCode = NumberFormat.simpleCurrency(
    locale: locale.toString(),
    name: "NGN",
  );

  MoneyFormatterOutput fo = MoneyFormatter(amount: money).output;
  return "${moneyCode.currencySymbol}${fo.nonSymbol}";
}

class DateFormatter {
  // e.g. "May 6, 2025"
  static String formatDate(DateTime dateTime) {
    return DateFormat('MMMM d, y').format(dateTime);
  }

  // e.g. "2:35 PM"
  static String formatTime(DateTime dateTime) {
    return DateFormat('h:mm a').format(dateTime);
  }

  // e.g. "May 6, 2025 | 2:35 PM"
  static String formatDateTime(DateTime dateTime) {
    return '${formatDate(dateTime)} @ ${formatTime(dateTime)}';
  }
}
