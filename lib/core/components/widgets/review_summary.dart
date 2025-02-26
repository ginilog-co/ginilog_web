import 'package:flutter/material.dart';

class ReviewSummary extends StatelessWidget {
  final Map<int, int> reviews; // {5: 25328, 4: 5979, 3: 1961, 2: 1049, 1: 1303}

  const ReviewSummary({super.key, required this.reviews});

  int get totalReviews => reviews.values.fold(0, (a, b) => a + b);

  @override
  Widget build(BuildContext context) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        ...reviews.entries.map((entry) {
          int star = entry.key;
          int count = entry.value;
          double percentage = (count / totalReviews);

          return Padding(
            padding: const EdgeInsets.symmetric(vertical: 4),
            child: Row(
              children: [
                Icon(Icons.star, color: Colors.amber, size: 20),
                Text(" $star", style: const TextStyle(fontSize: 16)),
                const SizedBox(width: 5),
                Expanded(
                  child: LinearProgressIndicator(
                    value: percentage,
                    backgroundColor: Colors.grey[300],
                    color: Colors.blue,
                    minHeight: 10,
                    borderRadius: BorderRadius.circular(5),
                  ),
                ),
                const SizedBox(width: 10),
                Text(
                  count.toString(),
                  style: const TextStyle(fontSize: 16),
                ),
              ],
            ),
          );
        }),
      ],
    );
  }
}
