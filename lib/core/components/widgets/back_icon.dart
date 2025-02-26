// ignore_for_file: deprecated_member_use

import 'package:ginilog_customer_app/core/components/utils/colors.dart';
import 'package:ginilog_customer_app/core/components/utils/package_export.dart';
import 'package:ginilog_customer_app/core/components/utils/size_config.dart';
import 'package:ginilog_customer_app/core/components/widgets/app_text.dart';

class GlobalBackButton extends StatelessWidget {
  const GlobalBackButton(
      {super.key,
      this.backText,
      this.buttonElements,
      this.color = Colors.black,
      this.showBackButton = true});

  final String? backText;
  final bool showBackButton;
  final Color color;
  final List<Widget>? buttonElements;

  @override
  Widget build(BuildContext context) {
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
                                borderRadius: BorderRadius.circular(8),
                              ),
                              child: Padding(
                                padding: const EdgeInsets.only(
                                    left: 10.0, right: 10, top: 5, bottom: 5),
                                child: SvgPicture.asset(
                                    'assets/svgs/back_button.svg',
                                    width: 20,
                                    color: Colors.white),
                              ),
                            ),
                          )
                        : Gap(6.heightAdjusted),
                    Gap(1.heightAdjusted),
                    AppText(
                        isBody: true,
                        text: backText ?? "",
                        textAlign: TextAlign.start,
                        fontSize: 78,
                        color: color,
                        fontStyle: FontStyle.normal,
                        fontWeight: FontWeight.w700)
                  ],
                ),
                const Spacer(),
                Row(children: buttonElements ?? [])
              ],
            ),
          ),
          const Divider(),
        ],
      ),
    );
  }
}
