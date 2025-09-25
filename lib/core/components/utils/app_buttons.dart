import 'package:flutter/cupertino.dart';
import 'package:flutter/material.dart';
import 'package:flutter_spinkit/flutter_spinkit.dart';
import 'package:ginilog_customer_app/core/components/utils/size_config.dart';
import 'colors.dart';

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

  // Optional border settings
  final Color? borderColor;
  final double borderWidth;
  final Widget? child; // ✅ NEW

  const AppButton({
    super.key,
    required this.text,
    required this.onPressed,
    this.widthPercent = 100,
    this.heightPercent = 6,
    this.btnColor = AppColors.primary,
    this.isLoading = false,
    this.textColor = AppColors.white,
    this.fontSize = 15,
    this.borderRadius = 20,
    this.safeArea = false,
    this.borderColor,
    this.borderWidth = 1.5,
    this.child, // ✅ NEW
  });

  @override
  Widget build(BuildContext context) {
    return BoxSizer(
      widthPercent: widthPercent,
      heightPercent: heightPercent,
      safeArea: safeArea,
      child: DecoratedBox(
        decoration: BoxDecoration(
          color: btnColor,
          borderRadius: BorderRadius.circular(borderRadius),
          border:
              borderColor != null
                  ? Border.all(color: borderColor!, width: borderWidth)
                  : null,
        ),
        child: CupertinoButton(
          padding: EdgeInsets.zero,
          borderRadius: BorderRadius.circular(borderRadius),
          color: Colors.transparent, // Color is managed by DecoratedBox
          disabledColor: !isLoading ? Colors.grey : btnColor,
          onPressed: isLoading ? null : onPressed,
          child:
              isLoading
                  ? const SpinKitThreeBounce(color: Colors.white, size: 16)
                  : child ??
                      Text(
                        // ✅ Use custom child if provided
                        text,
                        style: TextStyle(
                          fontSize: fontSize.textSize,
                          fontWeight: FontWeight.bold,
                          fontFamily: "Poppins",
                          color: textColor,
                        ),
                      ),
        ),
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

  const SmallButton({
    super.key,
    this.text,
    this.icon,
    required this.onPressed,
    this.textColor = Colors.blueGrey,
    this.iconColor = Colors.blueGrey,
    this.backgroundColor = AppColors.primary,
    this.fontSize = 18,
    this.fontWeight = FontWeight.w600,
    this.iconSize = 18,
    this.padding = const EdgeInsets.symmetric(horizontal: 8, vertical: 4),
    this.iconOnRight = false,
    this.isCircular = false,
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
              fontSize: fontSize.textSize,
              fontWeight: fontWeight,
              fontFamily: "Manrope",
            ),
          ),
        ),
      );
    }

    if (hasIcon && iconOnRight) {
      children.add(Icon(icon, size: iconSize, color: iconColor));
    }

    return TextButton(
      onPressed: onPressed,
      style: TextButton.styleFrom(
        padding: padding,
        minimumSize: Size.zero,
        backgroundColor: backgroundColor,
        iconSize: 20,
        shape:
            isCircular
                ? const CircleBorder()
                : RoundedRectangleBorder(
                  borderRadius: BorderRadius.circular(8.0),
                ),
        tapTargetSize: MaterialTapTargetSize.shrinkWrap,
      ),
      child: Row(mainAxisSize: MainAxisSize.min, children: children),
    );
  }
}
