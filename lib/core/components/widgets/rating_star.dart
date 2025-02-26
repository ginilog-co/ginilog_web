import 'package:ginilog_customer_app/core/components/utils/package_export.dart';

class StarRating extends StatelessWidget {
  final double rating; // Rating value (e.g., 4.5)
  final double starSize; // Size of stars
  final Color filledColor; // Color for filled stars
  final Color emptyColor; // Color for empty stars

  const StarRating({
    super.key,
    required this.rating,
    this.starSize = 24.0,
    this.filledColor = Colors.amber,
    this.emptyColor = Colors.grey,
  });

  @override
  Widget build(BuildContext context) {
    return Row(
      mainAxisSize: MainAxisSize.min,
      children: List.generate(5, (index) {
        double starValue = index + 1;
        IconData icon;

        if (rating >= starValue) {
          icon = Icons.star; // Fully filled star
        } else if (rating >= starValue - 0.5) {
          icon = Icons.star_half; // Half-filled star
        } else {
          icon = Icons.star_border; // Empty star
        }

        return Icon(icon,
            size: starSize,
            color: rating >= starValue ? filledColor : emptyColor);
      }),
    );
  }
}
