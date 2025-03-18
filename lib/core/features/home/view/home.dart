// ignore_for_file: deprecated_member_use

import 'package:ginilog_customer_app/core/components/utils/colors.dart';
import 'package:ginilog_customer_app/core/components/utils/helper_functions.dart';
import 'package:ginilog_customer_app/core/components/utils/package_export.dart';
import 'package:ginilog_customer_app/core/components/widgets/app_text.dart';
import 'package:ginilog_customer_app/core/components/widgets/arrow_line_widget.dart';
import 'package:ginilog_customer_app/core/features/bookings/view/accommodation/list_accomodation_reservations.dart';
import 'package:ginilog_customer_app/core/features/home/controller/home.dart';
import 'package:ginilog_customer_app/core/features/home/widget/order_page_widget.dart';
import 'package:ginilog_customer_app/core/features/home/widget/send_parcel_bottomsheet.dart';
import 'package:ginilog_customer_app/core/features/home_screen.dart';
import '../../../components/architecture/mvc.dart';

class HomeScreenView extends StatelessView<HomeScreen, HomeScreenController> {
  const HomeScreenView(super.state, {super.key});

  @override
  Widget build(BuildContext context) {
    // final provider = controller.ref.watch(getListGasStation);
    return Scaffold(
      body: SafeArea(
        child: Container(
          height: getScreenHeight(context),
          width: getScreenWidth(context),
          color: AppColors.white,
          child: SingleChildScrollView(
            child: Padding(
              padding: const EdgeInsets.only(left: 10.0, right: 10.0, top: 10),
              child: Column(
                mainAxisAlignment: MainAxisAlignment.start,
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  GestureDetector(
                    onTap: () {
                      navigateToRoute(
                          context,
                          HomeScreenPage(
                            imdex: 3,
                          ));
                    },
                    child: Row(
                      spacing: 5,
                      children: [
                        CircleAvatar(
                          backgroundColor: AppColors.grey5,
                          radius: 25,
                          backgroundImage: controller.profilePicture!.isEmpty ||
                                  controller.profilePicture == null
                              ? AssetImage("assets/images/profile_icon.png")
                              : NetworkImage(
                                  controller.profilePicture.toString()),
                        ),
                        AppText(
                            isBody: false,
                            text: controller.allNames,
                            textAlign: TextAlign.start,
                            fontSize: 80,
                            color: AppColors.black,
                            fontStyle: FontStyle.normal,
                            fontWeight: FontWeight.bold),
                      ],
                    ),
                  ),

                  addVerticalSpacing(context, 5),
                  CarouselSlider(
                    carouselController: controller.carouselController,
                    options: CarouselOptions(
                      autoPlay: true,
                      viewportFraction: 1,
                      aspectRatio: 16 / 9,
                      height: 200,
                      initialPage: 0,
                      enlargeCenterPage: true,
                      onPageChanged: (index, reason) {
                        controller.pageChanged(index);
                      },
                    ),
                    items: List.generate(
                      4,
                      (index) => InkWell(
                        onTap: () {},
                        child: Card(
                          child: Stack(
                            children: [
                              Container(
                                height: 200,
                                decoration: BoxDecoration(
                                  color: Colors.grey,
                                  borderRadius: BorderRadius.circular(10),
                                  image: DecorationImage(
                                    fit: BoxFit.fill,
                                    image: NetworkImage(
                                      "https://api-data.ginilog.com/uploads/89f72fb5-c511-46e9-a745-255957197616.png",
                                    ),
                                  ),
                                ),
                              ),
                            ],
                          ),
                        ),
                      ),
                    ),
                  ),
                  addVerticalSpacing(context, 5),
                  // Smooth Page Indicator
                  Center(
                    child: AnimatedSmoothIndicator(
                      activeIndex: controller.currentIndex,
                      count: 4,
                      effect: ExpandingDotsEffect(
                        dotHeight: 8,
                        dotWidth: 8,
                        activeDotColor: Colors.blue,
                        dotColor: Colors.grey,
                      ),
                      onDotClicked: (index) {
                        controller.carouselController.animateToPage(index);
                      },
                    ),
                  ),
                  addVerticalSpacing(context, 5),
                  Row(
                    spacing: 20,
                    children: [
                      Expanded(
                        child: CustomPaint(
                          size: Size(100, 10), // Adjust width here
                          painter: ArrowPainter(isArrowAtStart: false),
                        ),
                      ),
                      const AppText(
                          isBody: true,
                          text: "Our Services",
                          textAlign: TextAlign.start,
                          fontSize: 100,
                          color: AppColors.black,
                          fontStyle: FontStyle.normal,
                          fontWeight: FontWeight.w500),
                      Expanded(
                        child: CustomPaint(
                          size: Size(100, 10), // Adjust width here
                          painter: ArrowPainter(isArrowAtStart: true),
                        ),
                      ),
                    ],
                  ),
                  addVerticalSpacing(context, 5),
                  SingleChildScrollView(
                    scrollDirection: Axis.horizontal,
                    child: Row(
                      spacing: 10,
                      children: [
                        SizedBox(
                          width: getScreenWidth(context) / 2,
                          height: 250,
                          child: GestureDetector(
                            onTap: () {
                              showModalBottomSheet(
                                context: context,
                                isScrollControlled: true,
                                backgroundColor: Colors.transparent,
                                builder: (context) => SendParcelTypeBottomSheet(
                                  phoneNumber: controller.userPhone.toString(),
                                ),
                              );
                            },
                            child: Column(
                              children: [
                                Container(
                                  width: getScreenWidth(context) / 2,
                                  height: 200,
                                  decoration: BoxDecoration(
                                      borderRadius: BorderRadius.only(
                                          topLeft: Radius.circular(10),
                                          topRight: Radius.circular(10)),
                                      image: DecorationImage(
                                          fit: BoxFit.fill,
                                          image: AssetImage(
                                              "assets/images/place_order_items.png"))),
                                ),
                                addVerticalSpacing(context, 2),
                                const AppText(
                                    isBody: true,
                                    text: "Send A Parcel",
                                    textAlign: TextAlign.start,
                                    fontSize: 70,
                                    color: AppColors.black,
                                    fontStyle: FontStyle.normal,
                                    fontWeight: FontWeight.w400),
                              ],
                            ),
                          ),
                        ),
                        SizedBox(
                          width: getScreenWidth(context) / 2,
                          height: 250,
                          child: GestureDetector(
                            onTap: () {
                              navigateToRoute(
                                  context, AccomodationReservationList());
                            },
                            child: Column(
                              children: [
                                Container(
                                  width: getScreenWidth(context) / 2,
                                  height: 200,
                                  decoration: BoxDecoration(
                                      borderRadius: BorderRadius.only(
                                          topLeft: Radius.circular(10),
                                          topRight: Radius.circular(10)),
                                      image: DecorationImage(
                                          fit: BoxFit.fill,
                                          image: AssetImage(
                                              "assets/images/book_hotel.png"))),
                                ),
                                addVerticalSpacing(context, 2),
                                const AppText(
                                    isBody: true,
                                    text: "Accommodation Bookings",
                                    textAlign: TextAlign.start,
                                    fontSize: 70,
                                    color: AppColors.black,
                                    fontStyle: FontStyle.normal,
                                    fontWeight: FontWeight.w400),
                              ],
                            ),
                          ),
                        ),
                      ],
                    ),
                  ),
                  addVerticalSpacing(context, 5),
                  const AppText(
                      isBody: true,
                      text: "Recent Orders",
                      textAlign: TextAlign.start,
                      fontSize: 80,
                      color: AppColors.black,
                      fontStyle: FontStyle.normal,
                      fontWeight: FontWeight.bold),
                  addVerticalSpacing(context, 3),
                  controller.allOrders.isEmpty
                      ? SizedBox.shrink()
                      : OrderPageWidget(
                          allOrder: controller.allOrders,
                          userPhone: controller.userPhone.toString(),
                        ),
                ],
              ),
            ),
          ),
        ),
      ),
    );
  }

  Widget buildOrderContainer(BuildContext context, String title, String image) {
    return Container(
      decoration: BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.all(Radius.circular(10)),
        border: Border.all(color: Colors.white.withOpacity(0.5), width: 1.5),
      ),
      child: Card(
        color: AppColors.white,
        elevation: 2,
        child: Stack(
          children: [
            Image.asset(
              image,
              height: 150,
              width: 150,
            ),
            Positioned(
              bottom: 10,
              right: 23,
              child: AppText(
                  isBody: true,
                  text: title,
                  textAlign: TextAlign.start,
                  fontSize: 75,
                  color: AppColors.black,
                  fontStyle: FontStyle.normal,
                  fontWeight: FontWeight.w400),
            ),
          ],
        ),
      ),
    );
  }
}
