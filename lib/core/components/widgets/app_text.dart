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
    final style = TextStyle(
      fontSize: fontSized(context, fontSize),
      letterSpacing: letterSpacing ?? 1,
      color: color ?? Colors.black,
      fontStyle: fontStyle ?? FontStyle.normal,
      fontWeight: fontWeight ?? FontWeight.w400,
      decoration: decoration ?? TextDecoration.none,
      height: linHeight ?? 1.5,
      fontFamily: isBody == true ? "Montserrat" : "Inter",
    );
    return Text(text,
        style: style,
        textAlign: textAlign,
        overflow: overflow,
        maxLines: maxLines,
        softWrap: softWrap,
        textScaler: const TextScaler.linear(1),
        textWidthBasis: textWidthBasis,
        textHeightBehavior: textHeightBehavior);
  }
}

double fontSized(BuildContext context, double? fontSize,
    {double desktopFactor = 6,
    double tabletFactor = 4,
    double mobileFactor = 2,
    double scaleFactor = 4.0}) {
  double screenWidth = MediaQuery.of(context).size.width;
  double devicePixelRatio = MediaQuery.of(context).devicePixelRatio;
  double fontSizeEd;

  // Calculate font size based on device type and pixel ratio
  if (Responsive.isDesktop(context)) {
    fontSizeEd = fontSize == null
        ? 14 / devicePixelRatio
        : (screenWidth / desktopFactor) *
            (fontSize / 100) /
            (devicePixelRatio * scaleFactor);
  } else if (Responsive.isTablet(context)) {
    fontSizeEd = fontSize == null
        ? 14 / devicePixelRatio
        : (screenWidth / tabletFactor) *
            (fontSize / 100) /
            (devicePixelRatio * scaleFactor);
  } else {
    fontSizeEd = fontSize == null
        ? 14 / devicePixelRatio
        : (screenWidth / mobileFactor) *
            (fontSize / 100) /
            (devicePixelRatio * scaleFactor);
  }

  return fontSizeEd;
}
