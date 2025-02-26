import '../model/onboarding_model.dart';

List<Map<String, String>> onboardingData = [
  {
    "pre":
        "Track your packages/items from the comfort of your home till final destination",
    "text": "Real-time Tracking",
    "image": "assets/images/mux1.png"
  },
  {
    "pre": "Schedule pick ups and deliveries at your  Convenience",
    "text": "Flexible Scheduling",
    "image": "assets/images/mux2.png"
  },
  {
    "pre": "Plan your trips, make reservations",
    "text": "Plan Trips",
    "image": "assets/images/mux.png"
  },
];
List<OnboardingModel> formattedOnboardingList =
    onboardingData.map((x) => OnboardingModel.fromJson(x)).toList();
