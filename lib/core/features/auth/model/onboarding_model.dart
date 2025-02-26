class OnboardingModel {
  String? pre;
  String? text;
  String? image;

  OnboardingModel({
    this.pre,
    this.text,
    this.image,
  });

  OnboardingModel.fromJson(Map<String, dynamic> json) {
    pre = json['pre'];
    text = json['text'];
    image = json['image'];
  }
}
