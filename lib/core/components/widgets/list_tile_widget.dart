import 'package:ginilog_customer_app/core/components/utils/colors.dart';
import 'package:ginilog_customer_app/core/components/utils/package_export.dart';
import 'package:ginilog_customer_app/core/components/widgets/app_text.dart';

class CustomListTile extends StatelessWidget {
  final String title;
  final String subtitle;
  final String imageUrl;
  final VoidCallback? onTap;

  const CustomListTile({
    super.key,
    required this.title,
    required this.subtitle,
    required this.imageUrl,
    this.onTap,
  });

  @override
  Widget build(BuildContext context) {
    return ListTile(
      leading: Image.asset(height: 25, width: 25, imageUrl),
      title: AppText(
        isBody: true,
        text: title,
        textAlign: TextAlign.start,
        fontSize: 15,
        color: AppColors.black.withAlpha(162),
        fontStyle: FontStyle.normal,
        fontWeight: FontWeight.w800,
      ),
      subtitle: AppText(
        isBody: false,
        text: subtitle,
        textAlign: TextAlign.start,
        fontSize: 12,
        color: AppColors.black.withValues(alpha: 4),
        fontStyle: FontStyle.normal,
        fontWeight: FontWeight.w500,
      ),
      trailing: Icon(Icons.arrow_forward_ios, color: Colors.grey, size: 18),
      onTap: onTap, // Handle tap event
    );
  }
}
