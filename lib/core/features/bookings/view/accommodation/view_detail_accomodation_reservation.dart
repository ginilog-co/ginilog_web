// ignore_for_file: library_private_types_in_public_api

import 'package:ginilog_customer_app/core/components/utils/colors.dart';
import 'package:ginilog_customer_app/core/components/utils/helper_functions.dart';
import 'package:ginilog_customer_app/core/components/utils/money_formatter.dart';
import 'package:ginilog_customer_app/core/components/utils/package_export.dart';
import 'package:ginilog_customer_app/core/components/utils/size_config.dart';
import 'package:ginilog_customer_app/core/components/widgets/app_text.dart';
import 'package:ginilog_customer_app/core/components/widgets/back_icon.dart';
import 'package:ginilog_customer_app/core/features/bookings/controller/book_reservation.dart';
import 'package:ginilog_customer_app/core/features/bookings/model/accomodation_reservations_response_model.dart';
import 'package:ginilog_customer_app/core/features/bookings/model/accomodation_response_model.dart';
import 'package:ginilog_customer_app/core/features/bookings/state/booking_state.dart';

class ViewAccomodationReservationPage extends ConsumerStatefulWidget {
  const ViewAccomodationReservationPage({
    required this.reservation,
    super.key,
  });
  final AccomodationReservationResponseModel reservation;
  @override
  _LoginPageState createState() => _LoginPageState();
}

class _LoginPageState extends ConsumerState<ViewAccomodationReservationPage> {
  AccomodationResponseModel accomodationData = AccomodationResponseModel();
  final ScrollController _scrollController = ScrollController();
  bool _showBottomButton = false;
  final GlobalKey _inBodyButtonKey = GlobalKey();
  @override
  void initState() {
    super.initState();
    final bookings = ref.read(bookingProvider.notifier);
    bookings.getAllAccomodationData();
    accomodationData =
        bookings.fetchAccomodationById(widget.reservation.accomodationId!)!;
    // Listen to scroll events
    _scrollController.addListener(_checkInBodyButtonVisibility);
  }

  @override
  void dispose() {
    _scrollController.dispose();
    super.dispose();
  }

  void _checkInBodyButtonVisibility() {
    RenderBox? box =
        _inBodyButtonKey.currentContext?.findRenderObject() as RenderBox?;
    if (box != null) {
      Offset position = box.localToGlobal(Offset.zero);
      bool isButtonVisible = position.dy < MediaQuery.of(context).size.height;

      setState(() {
        _showBottomButton = !isButtonVisible;
      });
    }
  }

  final PageController _pageController = PageController();
  int selectedIndex = 0;

  void _onThumbnailTap(int index) {
    setState(() {
      selectedIndex = index;
    });
    _pageController.animateToPage(index,
        duration: Duration(milliseconds: 300), curve: Curves.easeInOut);
  }

  Widget _description(String description) {
    TextScaler textScaler = MediaQuery.of(context).textScaler;
    return ExpandableNotifier(
        child: Padding(
      padding: const EdgeInsets.all(0),
      child: Column(
        children: <Widget>[
          ScrollOnExpand(
            scrollOnExpand: true,
            scrollOnCollapse: false,
            child: ExpandablePanel(
              theme: const ExpandableThemeData(
                headerAlignment: ExpandablePanelHeaderAlignment.center,
                tapBodyToCollapse: true,
              ),
              header: Padding(
                  padding: const EdgeInsets.all(0),
                  child: Text(
                    "Description",
                    textScaler: textScaler,
                    style: TextStyle(
                        fontSize: fontSized(context, 42),
                        fontWeight: FontWeight.bold,
                        fontFamily: "Inter",
                        color: AppColors.black),
                  )),
              collapsed: Text(description,
                  softWrap: true,
                  maxLines: 4,
                  overflow: TextOverflow.ellipsis,
                  textScaler: textScaler,
                  style: TextStyle(
                      fontSize: fontSized(context, 32),
                      fontWeight: FontWeight.w500,
                      color: AppColors.black)),
              expanded: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: <Widget>[
                  for (var _ in Iterable.generate(1))
                    Padding(
                        padding: const EdgeInsets.only(bottom: 10),
                        child: Text(description,
                            softWrap: true,
                            overflow: TextOverflow.fade,
                            textScaler: textScaler,
                            style: TextStyle(
                                fontSize: fontSized(context, 32),
                                fontWeight: FontWeight.w500,
                                fontFamily: "Mulish",
                                color: AppColors.black))),
                ],
              ),
              builder: (_, collapsed, expanded) {
                return Padding(
                  padding: const EdgeInsets.only(left: 0, right: 0, bottom: 0),
                  child: Expandable(
                    collapsed: collapsed,
                    expanded: expanded,
                    theme: const ExpandableThemeData(crossFadePoint: 0),
                  ),
                );
              },
            ),
          ),
        ],
      ),
    ));
  }

  Widget buildItem(BuildContext context, AccomodationResponseModel userChat) {
    var reviews = userChat.accomodationReviewModels!.take(5).toList();
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        // Top PageView Carousel
        SizedBox(
          height: 270,
          child: PageView.builder(
            controller: _pageController,
            itemCount: widget.reservation.roomImages!.length,
            onPageChanged: (index) {
              setState(() {
                selectedIndex = index;
              });
            },
            itemBuilder: (context, index) {
              return Container(
                width: MediaQuery.of(context).size.width,
                decoration: BoxDecoration(
                  borderRadius: BorderRadius.circular(1),
                  image: DecorationImage(
                    image: NetworkImage(widget.reservation.roomImages![index]),
                    fit: BoxFit.cover,
                  ),
                ),
              );
            },
          ),
        ),

        SizedBox(height: 15),

        // Bottom Thumbnails Row
        SingleChildScrollView(
          scrollDirection: Axis.horizontal,
          child: Row(
            mainAxisAlignment: MainAxisAlignment.center,
            children:
                List.generate(widget.reservation.roomImages!.length, (index) {
              return GestureDetector(
                onTap: () => _onThumbnailTap(index),
                child: Container(
                  margin: EdgeInsets.symmetric(horizontal: 5),
                  width: 80,
                  height: 60,
                  decoration: BoxDecoration(
                    borderRadius: BorderRadius.circular(8),
                    border: Border.all(
                      color: selectedIndex == index
                          ? Colors.blueAccent
                          : Colors.grey.shade300,
                      width: 2,
                    ),
                    image: DecorationImage(
                      image:
                          NetworkImage(widget.reservation.roomImages![index]),
                      fit: BoxFit.cover,
                    ),
                  ),
                ),
              );
            }),
          ),
        ),
        Padding(
          padding: const EdgeInsets.only(left: 10, right: 10),
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Row(
                children: [
                  AppText(
                      isBody: false,
                      text: "${userChat.accomodationName}",
                      textAlign: TextAlign.start,
                      fontSize: 55,
                      color: AppColors.primaryDark,
                      fontStyle: FontStyle.normal,
                      fontWeight: FontWeight.bold),
                  Spacer(),
                  Container(
                    margin: const EdgeInsets.all(5),
                    height: 50.0,
                    width: 50.0,
                    decoration: BoxDecoration(
                      border: Border.all(
                        color: AppColors.grey,
                        width: 1,
                      ),
                      borderRadius: BorderRadius.circular(50),
                      //set border radius to 50% of square height and width
                      image: DecorationImage(
                        image:
                            NetworkImage(userChat.accomodationLogo.toString()),
                        fit: BoxFit.contain, //change image fill type
                      ),
                    ),
                  ),
                ],
              ),
              addVerticalSpacing(context, 2),
              Row(
                spacing: 10,
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  SvgPicture.asset(
                    'assets/svgs/loaction_icon.svg',
                    width: 20,
                  ),
                  Expanded(
                    child: AppText(
                        isBody: true,
                        text:
                            "${userChat.location}, ${userChat.locality}, ${userChat.state}",
                        textAlign: TextAlign.start,
                        fontSize: 35,
                        color: AppColors.black,
                        fontStyle: FontStyle.normal,
                        fontWeight: FontWeight.w500),
                  ),
                ],
              ),
              addVerticalSpacing(context, 2),
              const AppText(
                  isBody: true,
                  text: "Opens Monday - Sunday",
                  textAlign: TextAlign.start,
                  fontSize: 32,
                  color: AppColors.black,
                  fontStyle: FontStyle.normal,
                  fontWeight: FontWeight.w600),
              addVerticalSpacing(context, 2),
              AppText(
                  isBody: true,
                  text:
                      "Booking Price: ${moneyFormat(context, widget.reservation.roomPrice!.toDouble())}",
                  textAlign: TextAlign.start,
                  fontSize: 32,
                  color: AppColors.primaryDark,
                  fontStyle: FontStyle.normal,
                  fontWeight: FontWeight.w600)
            ],
          ),
        ),
        addVerticalSpacing(context, 5),
        Padding(
          padding: const EdgeInsets.all(8.0),
          child: AppText(
              isBody: true,
              text: "Room Features",
              textAlign: TextAlign.center,
              fontSize: 32,
              color: AppColors.black,
              fontStyle: FontStyle.normal,
              fontWeight: FontWeight.bold),
        ),
        Padding(
          padding: const EdgeInsets.all(8.0),
          child: Wrap(
            spacing: 5,
            runSpacing: 5,
            children:
                List.generate(widget.reservation.roomFeatures!.length, (index) {
              return Container(
                width: getScreenWidth(context) / 3.5,
                decoration: BoxDecoration(
                  color: AppColors.grey.withAlpha(40),
                  borderRadius: BorderRadius.circular(8),
                ),
                alignment: Alignment.center,
                child: Padding(
                  padding: EdgeInsets.only(top: 5.0, bottom: 5),
                  child: AppText(
                      isBody: true,
                      text: widget.reservation.roomFeatures![index],
                      textAlign: TextAlign.center,
                      fontSize: 32,
                      color: AppColors.black,
                      fontStyle: FontStyle.normal,
                      fontWeight: FontWeight.w500),
                ),
              );
            }),
          ),
        ),
        addVerticalSpacing(context, 5),
        userChat.accomodationDescription!.isEmpty
            ? const SizedBox.shrink()
            : Padding(
                padding: const EdgeInsets.only(left: 20, right: 20),
                child: _description("${userChat.accomodationDescription}")),
        addVerticalSpacing(context, 5),
        Align(
          alignment: Alignment.centerRight,
          child: GestureDetector(
            key: _inBodyButtonKey,
            onTap: () async {
              navigateToRoute(
                  context,
                  BookReservationScreen(
                    reservationId: widget.reservation.id.toString(),
                    bookingPrice: widget.reservation.roomPrice!,
                    maximumNoOfGuest:
                        widget.reservation.maximumNoOfGuest!.toInt(),
                    reservationName:
                        widget.reservation.accomodationName.toString(),
                    reservationAddress: userChat.location.toString(),
                  ));
            },
            child: Container(
              margin: const EdgeInsets.only(left: 20, right: 20),
              decoration: BoxDecoration(
                color: AppColors.primary,
                borderRadius: BorderRadius.circular(5),
                boxShadow: const [
                  BoxShadow(
                      blurRadius: 15.0, color: Color.fromRGBO(0, 0, 0, 0.2)),
                ],
              ),
              child: const Padding(
                padding:
                    EdgeInsets.only(left: 25.0, right: 25, top: 15, bottom: 15),
                child: AppText(
                    isBody: true,
                    text: "Book Now",
                    textAlign: TextAlign.center,
                    fontSize: 32,
                    color: AppColors.white,
                    fontStyle: FontStyle.normal,
                    fontWeight: FontWeight.w900),
              ),
            ),
          ),
        ),
        Padding(
          padding: const EdgeInsets.all(8.0),
          child: AppText(
              isBody: true,
              text: "${widget.reservation.accomodationType} Images",
              textAlign: TextAlign.center,
              fontSize: 32,
              color: AppColors.black,
              fontStyle: FontStyle.normal,
              fontWeight: FontWeight.bold),
        ),
        SingleChildScrollView(
          scrollDirection: Axis.horizontal,
          child: Row(
            mainAxisAlignment: MainAxisAlignment.center,
            children:
                List.generate(userChat.accomodationImages!.length, (index) {
              return Container(
                margin: EdgeInsets.symmetric(horizontal: 5),
                width: 100,
                height: 100,
                decoration: BoxDecoration(
                  borderRadius: BorderRadius.circular(8),
                  border: Border.all(
                    color: Colors.grey.shade300,
                    width: 2,
                  ),
                  image: DecorationImage(
                    image: NetworkImage(userChat.accomodationImages![index]),
                    fit: BoxFit.cover,
                  ),
                ),
              );
            }),
          ),
        ),
        Padding(
          padding: const EdgeInsets.all(8.0),
          child: AppText(
              isBody: true,
              text: "${widget.reservation.accomodationType} Facilities",
              textAlign: TextAlign.center,
              fontSize: 32,
              color: AppColors.black,
              fontStyle: FontStyle.normal,
              fontWeight: FontWeight.bold),
        ),
        Padding(
          padding: const EdgeInsets.all(8.0),
          child: Wrap(
            spacing: 5,
            runSpacing: 5,
            children:
                List.generate(userChat.accomodationFacilities!.length, (index) {
              return Container(
                width: getScreenWidth(context) / 3.5,
                decoration: BoxDecoration(
                  color: AppColors.grey6,
                  borderRadius: BorderRadius.circular(8),
                ),
                alignment: Alignment.center,
                child: Padding(
                  padding: EdgeInsets.only(top: 5.0, bottom: 5),
                  child: AppText(
                      isBody: true,
                      text: userChat.accomodationFacilities![index],
                      textAlign: TextAlign.center,
                      fontSize: 32,
                      color: AppColors.black,
                      fontStyle: FontStyle.normal,
                      fontWeight: FontWeight.w500),
                ),
              );
            }),
          ),
        ),
        addVerticalSpacing(context, 5),
        const Divider(
          thickness: 0.7,
          color: AppColors.grey,
        ),
        Padding(
            padding: const EdgeInsets.only(left: 15, right: 15),
            child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                mainAxisAlignment: MainAxisAlignment.start,
                children: [
                  Row(
                    children: [
                      const AppText(
                          isBody: true,
                          text: "Reviews",
                          textAlign: TextAlign.start,
                          fontSize: 35,
                          color: AppColors.black,
                          fontStyle: FontStyle.normal,
                          fontWeight: FontWeight.bold),
                      const Spacer(),
                      GestureDetector(
                        onTap: () {
                          // navigateToRoute(
                          //     context,
                          //     ViewAllReviewPagePage(
                          //       reviews: userChat.gasStationReviewModels!,
                          //     ));
                        },
                        child: const AppText(
                            isBody: true,
                            text: "View All",
                            textAlign: TextAlign.start,
                            fontSize: 35,
                            decoration: TextDecoration.underline,
                            color: AppColors.primary,
                            fontStyle: FontStyle.normal,
                            fontWeight: FontWeight.bold),
                      ),
                    ],
                  ),
                  const SizedBox(
                    height: 3,
                  ),
                  Padding(
                    padding: const EdgeInsets.only(left: 0, right: 0),
                    child: Column(
                        children: List.generate(reviews.length, (index) {
                      return Column(
                        crossAxisAlignment: CrossAxisAlignment.start,
                        children: [
                          Row(
                            children: [
                              Container(
                                margin: const EdgeInsets.all(5),
                                height: 30.0,
                                width: 30.0,
                                decoration: BoxDecoration(
                                  border: Border.all(
                                    color: AppColors.grey,
                                    width: 1,
                                  ),
                                  borderRadius: BorderRadius.circular(50),
                                  //set border radius to 50% of square height and width
                                  image: DecorationImage(
                                    image: NetworkImage(
                                        reviews[index].profileImage.toString()),
                                    fit:
                                        BoxFit.contain, //change image fill type
                                  ),
                                ),
                              ),
                              addHorizontalSpacing(10),
                              AppText(
                                  isBody: true,
                                  text: "${reviews[index].userName}",
                                  textAlign: TextAlign.start,
                                  fontSize: 36,
                                  color: AppColors.primaryDark,
                                  fontStyle: FontStyle.normal,
                                  fontWeight: FontWeight.bold),
                            ],
                          ),
                          RatingBar.readOnly(
                            isHalfAllowed: true,
                            alignment: Alignment.centerLeft,
                            halfFilledIcon: Icons.star_half,
                            filledIcon: Icons.star,
                            emptyIcon: Icons.star_border,
                            emptyColor: Colors.yellow,
                            halfFilledColor: Colors.grey,
                            initialRating: reviews[index].ratingNum!.toDouble(),
                            size: 15,
                          ),
                          addVerticalSpacing(context, 4),
                          AppText(
                              isBody: true,
                              text: "${reviews[index].reviewMessage}",
                              textAlign: TextAlign.start,
                              fontSize: 36,
                              color: AppColors.primaryDark,
                              fontStyle: FontStyle.normal,
                              fontWeight: FontWeight.bold),
                        ],
                      );
                    })),
                  ),
                ])),
      ],
    );
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: AppColors.white,
      appBar: PreferredSize(
          preferredSize: Size.fromHeight(SizeConfig.heightAdjusted(10.9)),
          child: Padding(
            padding: EdgeInsets.only(top: SizeConfig.heightAdjusted(10)),
            child: GlobalBackButton(
                backText: "${widget.reservation.accomodationName}",
                showBackButton: true),
          )),
      body: Stack(
        children: [
          Positioned.fill(
            child: SingleChildScrollView(
              controller: _scrollController,
              child: Container(
                decoration: BoxDecoration(
                  color: AppColors.white,
                  borderRadius: BorderRadius.circular(1),
                ),
                child: buildItem(context, accomodationData),
              ),
            ),
          ),
          // Bottom "Book" Button (only shown when in-body button is not visible)
          if (_showBottomButton)
            Positioned(
              left: 20,
              right: 20,
              bottom: 20,
              child: GestureDetector(
                onTap: () async {
                  navigateToRoute(
                      context,
                      BookReservationScreen(
                        reservationId: widget.reservation.id.toString(),
                        bookingPrice: widget.reservation.roomPrice!,
                        maximumNoOfGuest:
                            widget.reservation.maximumNoOfGuest!.toInt(),
                        reservationName:
                            widget.reservation.accomodationName.toString(),
                        reservationAddress:
                            accomodationData.location.toString(),
                      ));
                },
                child: Container(
                  decoration: BoxDecoration(
                    color: AppColors.primary,
                    borderRadius: BorderRadius.circular(5),
                    boxShadow: const [
                      BoxShadow(
                        blurRadius: 15.0,
                        color: Color.fromRGBO(0, 0, 0, 0.2),
                      ),
                    ],
                  ),
                  padding: const EdgeInsets.symmetric(vertical: 20),
                  alignment: Alignment.center,
                  child: const AppText(
                    isBody: true,
                    text: "Book Now",
                    textAlign: TextAlign.center,
                    fontSize: 22,
                    color: AppColors.white,
                    fontStyle: FontStyle.normal,
                    fontWeight: FontWeight.w900,
                  ),
                ),
              ),
            )
        ],
      ),
    );
  }
}
