import '../../../components/architecture/mvc.dart';
import '../../../components/routes/route.dart';
import '../../../components/utils/app_buttons.dart';
import '../../../components/utils/colors.dart';
import '../../../components/utils/helper_functions.dart';
import '../../../components/utils/package_export.dart';
import '../../../components/widgets/app_text.dart';
import '../controller/onboarding_controller.dart';

class OnboardingView
    extends StatelessView<OnboardingScreen, OnboardingScreenController> {
  const OnboardingView(super.state, {super.key});

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: AppColors.black,
      body: Stack(
        children: [
          SizedBox(
            height: getScreenHeight(context),
            width: getScreenWidth(context),
            child: PageView.builder(
                controller: controller.pageController,
                itemCount: controller.onboardingList.length,
                itemBuilder: (_, index) {
                  return Container(
                    height: getScreenHeight(context),
                    width: getScreenWidth(context),
                    decoration: BoxDecoration(
                      color: AppColors.black,
                      image: DecorationImage(
                          image: AssetImage(controller
                              .onboardingList[index].image
                              .toString()),
                          fit: BoxFit.cover),
                    ),
                    child: Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        addVerticalSpacing(context, 90.9),
                        Padding(
                          padding: const EdgeInsets.only(right: 10, left: 10),
                          child: Column(
                            crossAxisAlignment: CrossAxisAlignment.center,
                            children: [
                              AppText(
                                isBody: false,
                                text: controller.onboardingList[index].text
                                    .toString(),
                                textAlign: TextAlign.center,
                                fontSize: 100,
                                color: AppColors.white,
                                fontStyle: FontStyle.normal,
                                fontWeight: FontWeight.bold,
                              ),
                              addVerticalSpacing(context, 8),
                              AppText(
                                isBody: true,
                                text: controller.onboardingList[index].pre
                                    .toString(),
                                textAlign: TextAlign.center,
                                fontSize: 86,
                                color: AppColors.white,
                                fontStyle: FontStyle.normal,
                                fontWeight: FontWeight.w400,
                              ),
                              addVerticalSpacing(context, 40.2),
                              Center(
                                child: SmoothPageIndicator(
                                  controller: controller
                                      .pageController, // PageController
                                  count: controller.onboardingList.length,
                                  effect: const ExpandingDotsEffect(
                                      radius: 10,
                                      expansionFactor: 2,
                                      dotColor: Color(0xffC5CAE8),
                                      activeDotColor: AppColors.red,
                                      dotHeight: 10,
                                      dotWidth: 10), // your preferred effect
                                ),
                              ),
                              addVerticalSpacing(context, 30),
                            ],
                          ),
                        ),
                      ],
                    ),
                  );
                }),
          ),
          Positioned(
            top: 60,
            left: 5,
            child: Row(
              mainAxisAlignment: MainAxisAlignment.start,
              crossAxisAlignment: CrossAxisAlignment.center,
              children: [
                addHorizontalSpacing(10),
                Image.asset(
                  "assets/images/logo_path.png",
                  height: 30,
                ),
                addHorizontalSpacing(10),
                const AppText(
                  isBody: true,
                  text: "Ginilog",
                  textAlign: TextAlign.start,
                  fontSize: 50,
                  color: AppColors.white,
                  fontStyle: FontStyle.normal,
                  fontWeight: FontWeight.w800,
                ),
              ],
            ),
          ),
          Positioned(
            bottom: 10,
            left: 20,
            right: 250,
            child: Padding(
              padding: const EdgeInsets.only(right: 10, left: 10, bottom: 10),
              child: SizedBox(
                height: 50,
                child: appButton2(
                    child: AppText(
                      isBody: true,
                      text: "SKip",
                      textAlign: TextAlign.start,
                      fontSize: 35,
                      color: AppColors.white,
                      fontStyle: FontStyle.normal,
                      fontWeight: FontWeight.w800,
                    ),
                    btnColor: Colors.transparent,
                    border: Border.all(color: AppColors.white),
                    onPressed: () async {
                      navPush(context, RootRoutes.login);
                      await controller.storeOnboardInfo();
                    }),
              ),
            ),
          ),
          Positioned(
            bottom: 10,
            left: 250,
            right: 20,
            child: Padding(
              padding: const EdgeInsets.only(right: 10, left: 10, bottom: 10),
              child: SizedBox(
                height: 50,
                child: Button(
                    isLoading: false,
                    btncolor: AppColors.primary,
                    text: "Next",
                    onPressed: () async {
                      controller.currentIndex == 2
                          ? navPush(context, RootRoutes.login)
                          : controller.nextPage();
                      await controller.storeOnboardInfo();
                    }),
              ),
            ),
          ),
        ],
      ),
    );
  }
}
