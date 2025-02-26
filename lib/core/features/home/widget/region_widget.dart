import 'package:ginilog_customer_app/core/components/utils/colors.dart';
import 'package:ginilog_customer_app/core/components/utils/package_export.dart';
import 'package:ginilog_customer_app/core/components/widgets/app_text.dart';
import 'package:ginilog_customer_app/core/features/account/model/user_response_model.dart';

class RegionListTileWidget extends StatelessWidget {
  final DeliveryAddress country;
  final bool isNative;
  final bool isSelected;
  final ValueChanged<DeliveryAddress> onSelectedCountry;

  const RegionListTileWidget({
    super.key,
    required this.country,
    required this.isNative,
    required this.isSelected,
    required this.onSelectedCountry,
  });

  @override
  Widget build(BuildContext context) {
    final selectedColor = Theme.of(context).primaryColor;

    return ListTile(
      onTap: () => onSelectedCountry(country),
      // leading: FlagWidget(code: country.code),
      title: AppText(
        isBody: false,
        text: isNative
            ? "${country.state},${country.city}"
            : "${country.state},${country.city}",
        textAlign: TextAlign.start,
        fontSize: 18,
        color: AppColors.black,
        fontStyle: FontStyle.normal,
        fontWeight: FontWeight.bold,
      ),
      subtitle: AppText(
        isBody: true,
        text:
            isNative ? country.address.toString() : country.address.toString(),
        textAlign: TextAlign.start,
        fontSize: 18,
        color: AppColors.black,
        fontStyle: FontStyle.normal,
        fontWeight: FontWeight.bold,
      ),
      trailing: isSelected == false
          ? Icon(Icons.check, color: selectedColor, size: 26)
          : const SizedBox.shrink(),
    );
  }
}
