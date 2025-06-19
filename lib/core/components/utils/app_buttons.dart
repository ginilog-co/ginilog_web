import 'package:flutter/cupertino.dart';
import 'package:flutter/material.dart';
import 'package:flutter_spinkit/flutter_spinkit.dart';
import 'package:ginilog_customer_app/core/components/utils/size_config.dart';
import '../widgets/app_text.dart';
import 'colors.dart';

class Button extends StatelessWidget {
  final String text;
  final void Function() onPressed;
  final Color? btncolor;
  final Color? textcolor;
  final bool isLoading;
  final double widthPercent;
  final double heightPercent;
  final bool safeArea;

  const Button({
    super.key,
    required this.text,
    required this.onPressed,
    this.isLoading = false,
    this.btncolor = AppColors.primary,
    this.textcolor = AppColors.white,
    this.widthPercent = 100,
    this.heightPercent = 7.5, // Adjust for your button height
    this.safeArea = false,
  });

  @override
  Widget build(BuildContext context) {
    return BoxSizer(
      widthPercent: widthPercent,
      heightPercent: heightPercent,
      safeArea: safeArea,
      child: MaterialButton(
        elevation: 0.5,
        color: btncolor,
        onPressed: onPressed,
        shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(10)),
        child:
            !isLoading
                ? Text(
                  text,
                  style: TextStyle(
                    fontWeight: FontWeight.w600,
                    fontSize: fontSized(
                      context,
                      44,
                    ), // Use your scaling font util
                    color: textcolor,
                    fontFamily: 'Mulish',
                  ),
                )
                : const SpinKitThreeBounce(color: AppColors.white, size: 16),
      ),
    );
  }
}

class ButtonSO extends StatelessWidget {
  final String text;
  final void Function() onPressed;
  final Color? btncolor;
  final Color? textcolor;
  final Color? borderColor;
  final double widthPercent;
  final double heightPercent;
  final bool safeArea;

  const ButtonSO({
    super.key,
    required this.text,
    required this.onPressed,
    this.btncolor = AppColors.primaryDark,
    this.textcolor = AppColors.white,
    this.borderColor = AppColors.white,
    this.widthPercent = 100,
    this.heightPercent = 7.5,
    this.safeArea = false,
  });

  @override
  Widget build(BuildContext context) {
    return BoxSizer(
      widthPercent: widthPercent,
      heightPercent: heightPercent,
      safeArea: safeArea,
      child: OutlinedButton(
        style: OutlinedButton.styleFrom(
          backgroundColor: btncolor,
          side: BorderSide(color: borderColor ?? Colors.white, width: 1.0),
          shape: RoundedRectangleBorder(
            borderRadius: BorderRadius.circular(10),
          ),
        ),
        onPressed: onPressed,
        child: Center(
          child: Text(
            text,
            style: TextStyle(
              fontWeight: FontWeight.w600,
              fontSize: fontSized(context, 18), // Responsive text
              color: textcolor,
              fontFamily: 'Mulish',
            ),
          ),
        ),
      ),
    );
  }
}

class AppButton extends StatelessWidget {
  final String text;
  final VoidCallback onPressed;
  final double widthPercent;
  final double heightPercent;
  final Color btnColor;
  final bool isLoading;
  final Color textColor;
  final double fontSize;
  final double borderRadius;
  final bool safeArea;

  const AppButton({
    super.key,
    required this.text,
    required this.onPressed,
    this.widthPercent = 80,
    this.heightPercent = 6,
    this.btnColor = Colors.blue,
    this.isLoading = false,
    this.textColor = Colors.white,
    this.fontSize = 14,
    this.borderRadius = 5,
    this.safeArea = false,
  });

  @override
  Widget build(BuildContext context) {
    return BoxSizer(
      widthPercent: widthPercent,
      heightPercent: heightPercent,
      safeArea: safeArea,
      child: CupertinoButton(
        onPressed: isLoading ? null : onPressed,
        color: btnColor,
        disabledColor: !isLoading ? Colors.grey : btnColor,
        borderRadius: BorderRadius.circular(borderRadius),
        padding: EdgeInsets.zero,
        child:
            !isLoading
                ? Text(
                  text,
                  style: TextStyle(
                    fontSize: fontSized(context, fontSize),
                    fontWeight: FontWeight.bold,
                    fontFamily: "Mulish",
                    color: textColor,
                  ),
                )
                : const SpinKitThreeBounce(color: Colors.white, size: 16),
      ),
    );
  }
}

class SmallButton extends StatelessWidget {
  final String? text;
  final IconData? icon;
  final VoidCallback onPressed;
  final Color textColor;
  final Color iconColor;
  final Color backgroundColor;
  final double fontSize;
  final FontWeight fontWeight;
  final double iconSize;
  final EdgeInsets padding;
  final bool iconOnRight;
  final bool isCircular;

  /// New: Width & height percentages (0–100)
  final double widthPercent;
  final double heightPercent;
  final bool safeArea;

  const SmallButton({
    super.key,
    this.text,
    this.icon,
    required this.onPressed,
    this.textColor = Colors.white,
    this.iconColor = Colors.white,
    this.backgroundColor = AppColors.primary,
    this.fontSize = 13,
    this.fontWeight = FontWeight.w600,
    this.iconSize = 18,
    this.padding = const EdgeInsets.symmetric(horizontal: 8, vertical: 4),
    this.iconOnRight = false,
    this.isCircular = false,
    this.widthPercent = 20, // Default to small
    this.heightPercent = 6, // Default to small
    this.safeArea = true,
  });

  @override
  Widget build(BuildContext context) {
    final hasText = text != null && text!.isNotEmpty;
    final hasIcon = icon != null;

    List<Widget> children = [];

    if (hasIcon && !iconOnRight) {
      children.add(Icon(icon, size: iconSize, color: iconColor));
    }

    if (hasText) {
      children.add(
        Padding(
          padding:
              hasIcon
                  ? const EdgeInsets.symmetric(horizontal: 4.0)
                  : EdgeInsets.zero,
          child: Text(
            text!,
            style: TextStyle(
              color: textColor,
              fontSize: fontSized(context, fontSize),
              fontWeight: fontWeight,
              fontFamily: "Mulish",
            ),
          ),
        ),
      );
    }

    if (hasIcon && iconOnRight) {
      children.add(Icon(icon, size: iconSize, color: iconColor));
    }

    return BoxSizer(
      widthPercent: widthPercent,
      heightPercent: heightPercent,
      safeArea: safeArea,
      child: TextButton(
        onPressed: onPressed,
        style: TextButton.styleFrom(
          padding: padding,
          minimumSize: Size.zero,
          backgroundColor: backgroundColor,
          shape:
              isCircular
                  ? const CircleBorder()
                  : RoundedRectangleBorder(
                    borderRadius: BorderRadius.circular(8.0),
                  ),
          tapTargetSize: MaterialTapTargetSize.shrinkWrap,
        ),
        child: Row(mainAxisSize: MainAxisSize.min, children: children),
      ),
    );
  }
}
