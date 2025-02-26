import 'package:ginilog_customer_app/core/components/utils/package_export.dart';

String moneyFormat(BuildContext context, double money) {
  Locale locale = Localizations.localeOf(context);
  var moneyCode =
      NumberFormat.simpleCurrency(locale: locale.toString(), name: "NGN");

  MoneyFormatterOutput fo = MoneyFormatter(amount: money).output;
  return "${moneyCode.currencySymbol}${fo.nonSymbol}";
}
