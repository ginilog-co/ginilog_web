// ignore_for_file: deprecated_member_use

import 'package:ginilog_customer_app/core/components/utils/colors.dart';
import 'package:ginilog_customer_app/core/components/utils/package_export.dart';
import 'package:ginilog_customer_app/core/components/utils/size_config.dart';
import 'package:ginilog_customer_app/core/components/widgets/app_text.dart';

class GlobalBackButton extends StatelessWidget {
  const GlobalBackButton({
    super.key,
    this.backText,
    this.buttonElements,
    this.color = Colors.black,
    this.showBackButton = true,
  });

  final String? backText;
  final bool showBackButton;
  final Color color;
  final List<Widget>? buttonElements;

  @override
  Widget build(BuildContext context) {
    //
    return SizedBox(
      width: double.infinity,
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Padding(
            padding: EdgeInsets.only(right: 3.heightAdjusted),
            child: Row(
              mainAxisAlignment: MainAxisAlignment.start,
              children: [
                Gap(3.heightAdjusted),
                Row(
                  children: [
                    showBackButton
                        ? InkWell(
                          onTap: () => Navigator.pop(context),
                          child: Container(
                            decoration: BoxDecoration(
                              color: AppColors.darkBlue,
                              borderRadius: BorderRadius.circular(5),
                            ),
                            child: Padding(
                              padding: const EdgeInsets.only(
                                left: 5,
                                right: 5,
                                top: 5,
                                bottom: 5,
                              ),
                              child: SvgPicture.asset(
                                'assets/svgs/back_button.svg',
                                width: 15,
                                color: Colors.white,
                              ),
                            ),
                          ),
                        )
                        : addHorizontalSpacing(2),
                    addHorizontalSpacing(1),
                    AppText(
                      isBody: true,
                      text: backText ?? "",
                      textAlign: TextAlign.start,
                      fontSize: 38,
                      color: color,
                      fontStyle: FontStyle.normal,
                      fontWeight: FontWeight.w700,
                    ),
                  ],
                ),
                const Spacer(),
                Row(children: buttonElements ?? []),
              ],
            ),
          ),
          const Divider(),
        ],
      ),
    );
  }
}

AppBar buildFlexibleAppBar({
  required BuildContext context,
  String backIconAsset = 'assets/svgs/back_button.svg',
  double iconWidth = 15.0,
  VoidCallback? onBack,
  Color backgroundColor = AppColors.white,
  Color surfaceTintColor = AppColors.white,
  Color foregroundColor = AppColors.white,
  bool showBackButton = true,
  Widget? title,
  List<Widget>? actions,
  Widget? bottomWidget, // e.g. Search bar, filter chips
  double bottomHeight = 56.0,
  bool automaticallyImplyLeading = false,
  bool centerTitle = false,
}) {
  return AppBar(
    automaticallyImplyLeading: automaticallyImplyLeading,
    backgroundColor: backgroundColor,
    surfaceTintColor: surfaceTintColor,
    foregroundColor: foregroundColor,
    elevation: 0,
    centerTitle: centerTitle,
    leading:
        showBackButton
            ? IconButton(
              onPressed: onBack ?? () => Navigator.pop(context),
              icon: SvgPicture.asset(backIconAsset, width: iconWidth),
            )
            : null,
    title: title,
    actions: actions,
    bottom:
        bottomWidget != null
            ? PreferredSize(
              preferredSize: Size.fromHeight(bottomHeight),
              child: bottomWidget,
            )
            : null,
  );
}
