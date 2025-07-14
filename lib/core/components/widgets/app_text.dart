import 'package:flutter/material.dart';
import 'package:ginilog_customer_app/core/components/utils/size_config.dart';
// ignore: unused_import
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
      fontSize: fontSize?.textSize ?? 20.textSize,
      letterSpacing: letterSpacing ?? 1,
      color: color ?? Colors.black,
      fontStyle: fontStyle ?? FontStyle.normal,
      fontWeight: fontWeight ?? FontWeight.w400,
      decoration: decoration ?? TextDecoration.none,
      height: linHeight ?? 1.5,
      fontFamily: isBody == true ? "Mulish" : "Inter",
    );
    return Text(
      text,
      style: style,
      textAlign: textAlign,
      overflow: overflow,
      maxLines: maxLines,
      softWrap: softWrap,
      textWidthBasis: textWidthBasis,
      textHeightBehavior: textHeightBehavior,
    );
  }
}
