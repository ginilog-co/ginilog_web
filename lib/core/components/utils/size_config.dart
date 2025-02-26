import 'package:flutter/material.dart';

/// Model class for Size Configurations

class SizeConfig {
  static MediaQueryData? _mediaQueryData;
  static double? screenWidth;
  static double? screenHeight;
  static Orientation? orientation;
  static late double _screenWidth;
  static late double _screenHeight;
  static double _blockWidth = 0;
  static double _blockHeight = 0;

  static late double _textMultiplier;
  static late double _imageSizeMultiplier;
  static late double _heightMultiplier;
  static late double _widthMultiplier;

  void init(BuildContext context, BoxConstraints constraints,
      Orientation orientation) {
    _mediaQueryData = MediaQuery.of(context);
    screenWidth = _mediaQueryData!.size.width;
    screenHeight = _mediaQueryData!.size.height;
    orientation = _mediaQueryData!.orientation;
    if (orientation == Orientation.portrait) {
      _screenWidth = constraints.maxWidth;
      _screenHeight = constraints.maxHeight;
      if (_screenWidth < 450) {}
    } else {
      _screenWidth = constraints.maxHeight;
      _screenHeight = constraints.maxWidth;
    }

    _blockWidth = _screenWidth / 100;
    _blockHeight = _screenHeight / 100;

    _textMultiplier = _blockHeight;
    _imageSizeMultiplier = _blockWidth;
    _heightMultiplier = _blockHeight;
    _widthMultiplier = _blockWidth;
  }

  static double heightAdjusted(double height) {
    if (_heightMultiplier > 5.92) {
      return 1.2 * height * SizeConfig._widthMultiplier;
    } else {
      return height * SizeConfig._widthMultiplier;
    }
  }

  static double widthAdjusted(double width) {
    return width * SizeConfig._heightMultiplier;
  }

  static double textAdjusted(double fontSize) {
    return (fontSize * SizeConfig._textMultiplier) / 10;
  }

  static double imageAdjusted(double imageSize) {
    return imageSize * SizeConfig._imageSizeMultiplier;
  }
}

// double widgetHeight(double height) {
//   return SizeConfig.heightAdjusted(height);
// }

extension WidgetHeight on num {
  double get heightAdjusted => SizeConfig.heightAdjusted(toDouble());
}

// double widgetWidth(double height) {
//   return SizeConfig.widthAdjusted(height);
// }

extension WidgetWidth on num {
  double get widthAdjusted => SizeConfig.widthAdjusted(toDouble());
}
