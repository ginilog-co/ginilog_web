// ignore_for_file: library_private_types_in_public_api, deprecated_member_use

import 'package:ginilog_customer_app/core/components/utils/app_buttons.dart';
import 'package:ginilog_customer_app/core/components/utils/colors.dart';
import 'package:ginilog_customer_app/core/components/utils/constants.dart';
import 'package:ginilog_customer_app/core/components/utils/helper_functions.dart';
import 'package:ginilog_customer_app/core/components/utils/package_export.dart';
import 'package:ginilog_customer_app/core/components/utils/size_config.dart';
import 'package:ginilog_customer_app/core/components/widgets/app_text.dart';
import 'package:ginilog_customer_app/core/components/widgets/back_icon.dart';
import 'package:ginilog_customer_app/core/features/order_history/model/package_orders_model.dart';

class OrderLiveTrackingPage extends ConsumerStatefulWidget {
  const OrderLiveTrackingPage({super.key, required this.order});

  final PackageOrderResponseModel order;

  @override
  _LoginPageState createState() => _LoginPageState();
}

class _LoginPageState extends ConsumerState<OrderLiveTrackingPage> {
  PackageOrderResponseModel order = PackageOrderResponseModel();

  late GoogleMapController mapController;

  @override
  void initState() {
    order = widget.order;
    super.initState();
  }

  @override
  void dispose() {
    super.dispose();
  }

  List<OrderDeliveryFlow> orderDeliveryFlows(List<OrderDeliveryFlow> flows) {
    if (flows.isEmpty) return [];
    // Sorting by `dateTime` (earliest first, latest last)
    flows.sort(
      (a, b) => a.updatedAt!.toLocal().compareTo(b.updatedAt!.toLocal()),
    );
    return flows;
  }

  @override
  Widget build(BuildContext context) {
    final key = GlobalKey<ScaffoldMessengerState>();
    final current = LatLng(
      order.currentLatitude!.toDouble(),
      order.currentLongitude!.toDouble(),
    );
    return Scaffold(
      backgroundColor: AppColors.white,
      appBar: PreferredSize(
        preferredSize: Size.fromHeight(SizeConfig.heightAdjusted(15)),
        child: Padding(
          padding: EdgeInsets.only(top: SizeConfig.heightAdjusted(10)),
          child: GlobalBackButton(
            backText: 'Live Order Tracking',
            showBackButton: true,
          ),
        ),
      ),
      key: key,
      body: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        spacing: 10,
        children: [
          Expanded(
            flex: 2,
            child: GoogleMap(
              initialCameraPosition: CameraPosition(
                target: current,
                zoom: 14.0,
              ),
              buildingsEnabled: false,
              myLocationButtonEnabled: false,
              compassEnabled: false,
              markers: {
                Marker(
                  markerId: const MarkerId("currentLocation"),
                  position: current,
                  infoWindow: InfoWindow(
                    title: "Current Location",
                    snippet: order.currentLocation!,
                  ),
                  icon: BitmapDescriptor.defaultMarkerWithHue(
                    BitmapDescriptor.hueRed,
                  ),
                ),
              },
              onMapCreated: (controller) {
                mapController = controller;
              },
            ),
          ),
          Padding(
            padding: const EdgeInsets.only(left: 10.0, right: 10),
            child: const AppText(
              isBody: false,
              text: "Tracking Number",
              textAlign: TextAlign.start,
              fontSize: 35,
              color: AppColors.black,
              fontStyle: FontStyle.normal,
              fontWeight: FontWeight.w700,
            ),
          ),
          Padding(
            padding: const EdgeInsets.only(left: 10.0, right: 10),
            child: Row(
              spacing: 5,
              children: [
                SvgPicture.asset('assets/svgs/track_num_icon.svg', width: 20),
                AppText(
                  isBody: true,
                  text: "TN: ${order.trackingNum}",
                  textAlign: TextAlign.start,
                  fontSize: 30,
                  color: AppColors.green,
                  fontStyle: FontStyle.normal,
                  fontWeight: FontWeight.bold,
                ),
              ],
            ),
          ),
          Expanded(
            flex: 2,
            child: ListView.builder(
              padding: EdgeInsets.only(left: 10, right: 10),
              itemCount: orderDeliveryFlows(order.orderDeliveryFlows!).length,
              itemBuilder: (context, index) {
                final status =
                    orderDeliveryFlows(order.orderDeliveryFlows!)[index];
                DateTime dt2 = DateTime.parse(
                  status.updatedAt!.toLocal().toString(),
                );
                String date = DateFormat(
                  "E, MMM d hh:mm a",
                ).format(dt2.toLocal());
                return buildTrackFlow(
                  status: status.orderStatus!,
                  currentStatus: order.orderStatus!.name.firstCap,
                  statusDate: date,
                  location: status.currentLocation!,
                  isLast:
                      index ==
                      order.orderDeliveryFlows!.length -
                          1, // Check if it's the last item
                );
              },
            ),
          ),
          Align(
            alignment: Alignment.bottomRight,
            child: Padding(
              padding: const EdgeInsets.all(8.0),
              child: AppButton(
                text: "Go Back",
                onPressed: () async {
                  navigateBack(context);
                },
                widthPercent: 70,
                heightPercent: 5,
                btnColor: AppColors.primary,
                isLoading: false,
              ),
            ),
          ),
          addVerticalSpacing(context, 5),
        ],
      ),
    );
  }

  Widget buildTrackFlow({
    required OrderClassState status,
    required String currentStatus,
    required String statusDate,
    required String location,
    required bool isLast,
  }) {
    return Row(
      crossAxisAlignment: CrossAxisAlignment.start,
      spacing: 10,
      children: [
        Column(
          mainAxisAlignment: MainAxisAlignment.start,
          crossAxisAlignment: CrossAxisAlignment.center,
          children: [
            Icon(
              status.name.firstCap == currentStatus
                  ? Icons.check_box_outline_blank
                  : Icons.check_box_outlined,
              color:
                  status.name.firstCap == currentStatus
                      ? AppColors.grey
                      : AppColors.green,
              size: 30,
            ),
            !isLast
                ? CustomPaint(
                  size: const Size(1, 48),
                  painter: DashedLineVerticalPainter(),
                )
                : SizedBox.shrink(),
          ],
        ),
        Expanded(
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              AppText(
                isBody: false,
                text: "Package Status: ${status.name.firstCap}",
                textAlign: TextAlign.start,
                fontSize: 40,
                color:
                    status.name.firstCap == currentStatus
                        ? AppColors.grey
                        : AppColors.green,
                fontStyle: FontStyle.normal,
                fontWeight: FontWeight.bold,
              ),
              AppText(
                isBody: true,
                text: statusDate,
                textAlign: TextAlign.start,
                fontSize: 25,
                color: AppColors.black,
                fontStyle: FontStyle.normal,
                fontWeight: FontWeight.bold,
              ),
              AppText(
                isBody: true,
                text: location,
                textAlign: TextAlign.start,
                overflow: TextOverflow.ellipsis,
                maxLines: 1,
                fontSize: 25,
                color: AppColors.black,
                fontStyle: FontStyle.normal,
                fontWeight: FontWeight.bold,
              ),
            ],
          ),
        ),
      ],
    );
  }
}
