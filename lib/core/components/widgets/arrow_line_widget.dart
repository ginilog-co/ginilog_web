// CustomPainter for horizontal line with arrow
import 'dart:math';

import 'package:ginilog_customer_app/core/components/utils/package_export.dart';

class ArrowPainter extends CustomPainter {
  final bool isArrowAtStart;

  ArrowPainter({this.isArrowAtStart = false});

  @override
  void paint(Canvas canvas, Size size) {
    Paint paint = Paint()
      ..color = Colors.black.withAlpha(170)
      ..strokeWidth = 2
      ..style = PaintingStyle.stroke;

    double startX = 0;
    double endX = size.width;
    double centerY = size.height / 2;
    double arrowSize = 10;
    double angle = pi / 6; // 30 degrees

    // Draw horizontal line
    canvas.drawLine(Offset(startX, centerY), Offset(endX, centerY), paint);

    // Draw arrow at start (←) or at end (→)
    Offset arrowTip =
        isArrowAtStart ? Offset(startX, centerY) : Offset(endX, centerY);
    Offset arrowLeft = Offset(
      arrowTip.dx +
          (isArrowAtStart ? arrowSize * cos(angle) : -arrowSize * cos(angle)),
      arrowTip.dy - arrowSize * sin(angle),
    );
    Offset arrowRight = Offset(
      arrowTip.dx +
          (isArrowAtStart ? arrowSize * cos(angle) : -arrowSize * cos(angle)),
      arrowTip.dy + arrowSize * sin(angle),
    );

    Path arrowPath = Path()
      ..moveTo(arrowTip.dx, arrowTip.dy)
      ..lineTo(arrowLeft.dx, arrowLeft.dy)
      ..moveTo(arrowTip.dx, arrowTip.dy)
      ..lineTo(arrowRight.dx, arrowRight.dy);

    canvas.drawPath(arrowPath, paint);
  }

  @override
  bool shouldRepaint(CustomPainter oldDelegate) => false;
}
