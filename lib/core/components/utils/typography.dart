import 'package:flutter/material.dart';

import 'colors.dart';

class AppTypography {
  static const String fontFamily = 'Mulish';

  static TextStyle h1({Color? color = AppColors.black}) => TextStyle(
      fontFamily: fontFamily,
      fontStyle: FontStyle.normal,
      fontWeight: FontWeight.w700,
      fontSize: 28,
      color: color);

  static TextStyle h2({Color? color = AppColors.black}) => TextStyle(
      fontFamily: fontFamily,
      fontStyle: FontStyle.normal,
      fontWeight: FontWeight.w500,
      fontSize: 24,
      color: color);

  static TextStyle h3({Color? color = AppColors.black}) => TextStyle(
      fontFamily: fontFamily,
      fontStyle: FontStyle.normal,
      fontWeight: FontWeight.w500,
      fontSize: 18,
      color: color);

  static TextStyle title({Color? color = AppColors.black}) => TextStyle(
      fontFamily: fontFamily,
      fontStyle: FontStyle.normal,
      fontWeight: FontWeight.w600,
      fontSize: 20,
      height: 1.5,
      color: color);

  static TextStyle sub1({Color? color = AppColors.black}) => TextStyle(
      fontFamily: fontFamily,
      fontStyle: FontStyle.normal,
      fontWeight: FontWeight.w500,
      fontSize: 16,
      color: color);

  static TextStyle body({Color? color = AppColors.black}) => TextStyle(
      fontFamily: fontFamily,
      fontStyle: FontStyle.italic,
      fontWeight: FontWeight.w200,
      fontSize: 15,
      color: color);

  static TextStyle body2({Color? color = AppColors.black}) => TextStyle(
      fontFamily: fontFamily,
      fontStyle: FontStyle.italic,
      fontWeight: FontWeight.w200,
      fontSize: 14,
      color: color);

  static TextStyle button({Color? color = AppColors.black}) => TextStyle(
      fontFamily: fontFamily,
      fontStyle: FontStyle.normal,
      fontWeight: FontWeight.w500,
      fontSize: 16,
      color: color);

  static TextStyle buttonMedium({Color? color = AppColors.black}) => TextStyle(
      fontFamily: fontFamily,
      fontStyle: FontStyle.normal,
      fontWeight: FontWeight.w500,
      fontSize: 14,
      color: color);

  static TextStyle buttonSmall({Color? color = AppColors.black}) => TextStyle(
      fontFamily: fontFamily,
      fontStyle: FontStyle.normal,
      fontWeight: FontWeight.w500,
      fontSize: 13,
      color: color);

  static TextStyle caption({Color? color = AppColors.black}) => TextStyle(
      fontFamily: fontFamily,
      fontStyle: FontStyle.italic,
      fontWeight: FontWeight.w200,
      fontSize: 12,
      color: color);

  static TextStyle dynamicStyle({
    bool inherit = true,
    Color? color = AppColors.black,
    Color? backgroundColor,
    double? fontSize,
    FontWeight? fontWeight,
    FontStyle? fontStyle,
    double? letterSpacing,
    double? wordSpacing,
    TextBaseline? textBaseline,
    double? height,
    TextLeadingDistribution? leadingDistribution,
    Locale? locale,
    Paint? foreground,
    Paint? background,
    List<Shadow>? shadows,
    List<FontFeature>? fontFeatures,
    List<FontVariation>? fontVariations,
    TextDecoration? decoration,
    Color? decorationColor,
    TextDecorationStyle? decorationStyle,
    double? decorationThickness,
    String? debugLabel,
    List<String>? fontFamilyFallback,
    String? package,
    TextOverflow? overflow,
  }) =>
      TextStyle(
        color: color,
        background: background,
        fontFamily: 'Mulish',
        leadingDistribution: leadingDistribution,
        backgroundColor: backgroundColor,
        debugLabel: debugLabel,
        decoration: decoration,
        decorationColor: decorationColor,
        decorationStyle: decorationStyle,
        decorationThickness: decorationThickness,
        fontFamilyFallback: fontFamilyFallback,
        fontFeatures: fontFeatures,
        fontSize: fontSize,
        fontStyle: fontStyle,
        // fontVariations: fontVariations,
        fontWeight: fontWeight,
        foreground: foreground,
        height: height,
        inherit: inherit,
        letterSpacing: letterSpacing,
        locale: locale,
        overflow: overflow,
        package: package,
        shadows: shadows,
        textBaseline: textBaseline,
        wordSpacing: wordSpacing,
      );
}
