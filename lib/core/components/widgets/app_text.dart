import 'package:flutter/material.dart';
import 'responsive.dart';

class AppText extends StatelessWidget {
  final String text;
  final double? fontSize;
  final double? linHeight;
  final double? letterSpacing;
  final Color? color;
  final FontWeight? fontWeight;
  final FontStyle? fontStyle;
  final TextAlign? textAlign;
  final TextDecoration? decoration;
  final TextOverflow? overflow;
  final TextWidthBasis? textWidthBasis;
  final TextHeightBehavior? textHeightBehavior;
  final int? maxLines;
  final bool? softWrap;
  final bool isBody;

  const AppText({
    required this.text,
    super.key,
    required this.fontSize,
    this.linHeight,
    this.letterSpacing,
    this.color,
    this.fontWeight,
    this.fontStyle,
    this.textAlign,
    this.decoration,
    this.overflow,
    this.textWidthBasis,
    this.textHeightBehavior,
    this.maxLines,
    this.softWrap,
    required this.isBody,
  });

  @override
  Widget build(BuildContext context) {
    TextScaler textScaler = MediaQuery.of(context).textScaler;
    final style = TextStyle(
      fontSize: fontSized(context, fontSize),
      letterSpacing: letterSpacing ?? 1,
      color: color ?? Colors.black,
      fontStyle: fontStyle ?? FontStyle.normal,
      fontWeight: fontWeight ?? FontWeight.w400,
      decoration: decoration ?? TextDecoration.none,
      height: linHeight ?? 1.5,
      fontFamily: isBody == true ? "Mulish" : "Inter",
    );
    return Text(text,
        style: style,
        textAlign: textAlign,
        overflow: overflow,
        maxLines: maxLines,
        softWrap: softWrap,
        textScaler: textScaler,
        textWidthBasis: textWidthBasis,
        textHeightBehavior: textHeightBehavior);
  }
}

double fontSized(BuildContext context, double? fontSize,
    {double desktopFactor = 6,
    double tabletFactor = 4,
    double mobileFactor = 2,
    double scaleFactor = 4.0,
    double minFontSize = 12.0,
    double maxFontSize = 24.0}) {
  double screenWidth = MediaQuery.of(context).size.width;
  TextScaler textScaler = MediaQuery.of(context).textScaler;

  // Determine scaling factor based on device type
  double scalingFactor = Responsive.isDesktop(context)
      ? desktopFactor
      : Responsive.isTablet(context)
          ? tabletFactor
          : mobileFactor;

  // Calculate base font size
  double baseFontSize = fontSize != null
      ? (screenWidth / scalingFactor) * (fontSize / 100) / scaleFactor
      : 14.0;

  // Clamp to prevent extreme sizes
  double clampedFontSize = baseFontSize.clamp(minFontSize, maxFontSize);

  // Apply text scaling and return final size
  return textScaler.scale(clampedFontSize);
}
