// ignore_for_file: library_private_types_in_public_api, deprecated_member_use

import 'dart:async';
import 'dart:math';

import 'package:ginilog_customer_app/core/components/utils/colors.dart';
import 'package:ginilog_customer_app/core/components/utils/package_export.dart';
import 'package:ginilog_customer_app/core/components/utils/size_config.dart';
import 'package:ginilog_customer_app/core/components/widgets/back_icon.dart';
import 'package:ginilog_customer_app/core/features/order_history/states/order_state.dart';

class OrderLiveTrackingPage extends ConsumerStatefulWidget {
  const OrderLiveTrackingPage(
      {super.key,
      required this.orderId,
      required this.sourceLatitude,
      required this.sourceLongitude,
      required this.destinationLatitude,
      required this.destinationLongitude});

  final String orderId;
  final double sourceLatitude;
  final double sourceLongitude;
  final double destinationLatitude;
  final double destinationLongitude;

  @override
  _LoginPageState createState() => _LoginPageState();
}

class _LoginPageState extends ConsumerState<OrderLiveTrackingPage> {
  late PackageOrderNotifier orderProviders;
  final Completer<GoogleMapController> mapController =
      Completer(); //controller for Google map
  BitmapDescriptor currentIcon = BitmapDescriptor.defaultMarker;
  BitmapDescriptor destinationIcon = BitmapDescriptor.defaultMarker;
  BitmapDescriptor sourceIcon = BitmapDescriptor.defaultMarker;

  @override
  void initState() {
    orderProviders = ref.read(packageOrderProvider.notifier);
    super.initState();
    WidgetsBinding.instance
        .addPostFrameCallback((_) async => await initializeMap());
    BitmapDescriptor.asset(
            ImageConfiguration.empty, "assets/images/delivery_guy.png",
            height: 40, width: 40)
        .then(
      (icon) {
        currentIcon = icon;
      },
    );
    BitmapDescriptor.asset(
            ImageConfiguration.empty, "assets/images/home-delivery.png",
            height: 30, width: 30)
        .then(
      (icon) {
        destinationIcon = icon;
      },
    );
    BitmapDescriptor.asset(
            ImageConfiguration.empty, "assets/images/package_station.png",
            height: 30, width: 30)
        .then(
      (icon) {
        sourceIcon = icon;
      },
    );
  }

  @override
  void dispose() {
    super.dispose();
  }

  // Calculate the bearing between two LatLng points
  double calculateBearing(LatLng start, LatLng end) {
    double startLat = start.latitude * (pi / 180);
    double startLng = start.longitude * (pi / 180);
    double endLat = end.latitude * (pi / 180);
    double endLng = end.longitude * (pi / 180);

    double dLon = endLng - startLng;
    double x = sin(dLon) * cos(endLat);
    double y =
        cos(startLat) * sin(endLat) - sin(startLat) * cos(endLat) * cos(dLon);

    double bearing = atan2(x, y) * (180 / pi); // Convert to degrees
    return (bearing + 360) % 360; // Normalize to 0–360
  }

  Map<PolylineId, Polyline> polylines = {};
  Future<void> initializeMap() async {
    final coordinates = await fetchPolylinePoints();
    generatePolyLineFromPoints(coordinates);
  }

  Future<List<LatLng>> fetchPolylinePoints() async {
    final polylinePoints = PolylinePoints();
    final result = await polylinePoints.getRouteBetweenCoordinates(
      googleApiKey: "AIzaSyCuU7j9XnHs31-I6NE7cz_SxOw3lzScFuo",
      request: PolylineRequest(
        mode: TravelMode.driving,
        origin: PointLatLng(widget.sourceLatitude, widget.sourceLongitude),
        destination: PointLatLng(
            widget.destinationLatitude, widget.destinationLongitude),
      ),
    );
    if (result.points.isNotEmpty) {
      return result.points
          .map((point) => LatLng(point.latitude, point.longitude))
          .toList();
    } else {
      debugPrint(result.errorMessage);
      return [];
    }
  }

  Future<void> generatePolyLineFromPoints(
      List<LatLng> polylineCoordinates) async {
    const id = PolylineId('polyline');

    final polyline = Polyline(
      polylineId: id,
      color: AppColors.red,
      points: polylineCoordinates,
      width: 8,
    );

    setState(() => polylines[id] = polyline);
  }

  transition(LatLng result) async {
    GoogleMapController googleMapController = await mapController.future;
    googleMapController.animateCamera(
      CameraUpdate.newCameraPosition(
        CameraPosition(
          zoom: 13.5,
          target: result,
        ),
      ),
    );
    setState(() {});
  }

  // final String _mapStyle = '''
  // [
  //   {
  //     "featureType": "administrative",
  //     "elementType": "all",
  //     "stylers": [
  //       { "visibility": "on" }
  //     ]
  //   },
  //   {
  //     "featureType": "landscape",
  //     "elementType": "all",
  //     "stylers": [
  //       { "visibility": "on" }
  //     ]
  //   },
  //   {
  //     "featureType": "poi",
  //     "elementType": "all",
  //     "stylers": [
  //       { "visibility": "on" }
  //     ]
  //   },
  //   {
  //     "featureType": "road",
  //     "elementType": "all",
  //     "stylers": [
  //       { "visibility": "on" }
  //     ]
  //   },
  //   {
  //     "featureType": "transit",
  //     "elementType": "all",
  //     "stylers": [
  //       { "visibility": "on" }
  //     ]
  //   },
  //   {
  //     "featureType": "water",
  //     "elementType": "all",
  //     "stylers": [
  //       { "visibility": "off" }
  //     ]
  //   }
  // ]
  // ''';

  // Widget buildItem(BuildContext context, DocumentSnapshot? documentSnapshot) {
  //   if (documentSnapshot == null) {
  //     return const Center(
  //       child: CircularProgressIndicator(
  //         valueColor: AlwaysStoppedAnimation(Colors.cyan),
  //       ),
  //     );
  //   }
  //   OrderTrackModel userChat = OrderTrackModel.fromDocument(documentSnapshot);

  //   final googlePlex = LatLng(userChat.sourceLatitude!.toDouble(),
  //       userChat.sourceLongitude!.toDouble());
  //   printData("Data", googlePlex);
  //   final current = LatLng(userChat.currentLatitude!.toDouble(),
  //       userChat.currentLongitude!.toDouble());
  //   final destination = LatLng(userChat.destinationLatitude!.toDouble(),
  //       userChat.destinationLongitude!.toDouble());

  //   // Add the initial marker to the map

  //   return GoogleMap(
  //     // style: _mapStyle,
  //     compassEnabled: false,
  //     trafficEnabled: false, // Enable real-time traffic
  //     mapType: MapType.normal, // Normal map type
  //     gestureRecognizers: <Factory<OneSequenceGestureRecognizer>>{
  //       Factory<OneSequenceGestureRecognizer>(
  //         () => EagerGestureRecognizer(),
  //       )
  //     },
  //     circles: {
  //       Circle(
  //         circleId: const CircleId('circle1'),
  //         center: googlePlex, // Center of the circle
  //         radius: 500, // Radius in meters
  //         fillColor:
  //             Colors.blue.withOpacity(0.5), // Fill color with transparency
  //         strokeColor: Colors.blue, // Stroke color
  //         strokeWidth: 2, // Stroke width in pixels
  //       ),
  //       Circle(
  //         circleId: const CircleId('circle2'),
  //         center: current, // Center of another circle
  //         radius: 300,
  //         fillColor: Colors.green.withOpacity(0.5),
  //         strokeColor: Colors.green,
  //         strokeWidth: 2,
  //       ),
  //       Circle(
  //         circleId: const CircleId('circle2'),
  //         center: destination, // Center of another circle
  //         radius: 300,
  //         fillColor: Colors.red.withOpacity(0.5),
  //         strokeColor: Colors.red,
  //         strokeWidth: 2,
  //       ),
  //     },
  //     onCameraMove: (position) {
  //       position = CameraPosition(
  //         target: current, // Initial position
  //         zoom: 14.0,
  //       );
  //     },
  //     heatmaps: {
  //       Heatmap(
  //         heatmapId: const HeatmapId("example_heatmap"),
  //         data: <WeightedLatLng>[
  //           WeightedLatLng(
  //             googlePlex, // Point 1
  //             weight: 2.0, // Weight for the point
  //           ),
  //           WeightedLatLng(
  //             current, // Point 2
  //             weight: 3.0,
  //           ),
  //           WeightedLatLng(
  //             destination, // Point 3
  //             weight: 1.0,
  //           ),
  //         ],
  //         gradient: const HeatmapGradient(
  //           <HeatmapGradientColor>[
  //             HeatmapGradientColor(Colors.green, 0.2),
  //             HeatmapGradientColor(Colors.red, 1.0)
  //           ], // Gradient colors
  //           colorMapSize: 256, // Transition points
  //         ),
  //         radius: const HeatmapRadius.fromPixels(
  //             40), // Radius of each point in pixels
  //         opacity: 0.6, // Opacity of the heatmap
  //       ),
  //     },
  //     initialCameraPosition: CameraPosition(
  //       target: current,
  //       zoom: 13,
  //     ),
  //     markers: {
  //       Marker(
  //         markerId: const MarkerId('currentLocation'),
  //         icon: currentIcon,
  //         infoWindow: const InfoWindow(
  //           title: "Bike Location",
  //         ),
  //         position: current,
  //         rotation: userChat.orderStatus == "Picked"
  //             ? _calculateBearing(current, googlePlex) + 100
  //             : _calculateBearing(current, destination) + 100,
  //         anchor: const Offset(0.4, 0.5), // Center the icon
  //       ),
  //       //_deliveryGuyMarker,
  //       Marker(
  //         markerId: const MarkerId('sourceLocation'),
  //         infoWindow: const InfoWindow(
  //           title: "Logistics Company Location",
  //         ),
  //         icon: sourceIcon,
  //         position: googlePlex,
  //       ),
  //       Marker(
  //         markerId: const MarkerId('destinationLocation'),
  //         icon: destinationIcon,
  //         infoWindow: const InfoWindow(
  //           title: "Home Location",
  //         ),
  //         position: destination,
  //       )
  //     },
  //     polylines: Set<Polyline>.of(polylines.values),
  //     onMapCreated: (controller) {
  //       controller.animateCamera(CameraUpdate.newCameraPosition(CameraPosition(
  //         target: current,
  //         zoom: 13,
  //       )));
  //       setState(() {
  //         mapController.complete(controller);
  //       });
  //     },
  //   );
  // }

  @override
  Widget build(BuildContext context) {
    final key = GlobalKey<ScaffoldMessengerState>();

    return Scaffold(
      backgroundColor: AppColors.white,
      appBar: PreferredSize(
          preferredSize: Size.fromHeight(SizeConfig.heightAdjusted(15)),
          child: Padding(
            padding: EdgeInsets.only(top: SizeConfig.heightAdjusted(10)),
            child: const Column(
              children: [
                GlobalBackButton(
                    backText: 'Live Order Tracking', showBackButton: true),
              ],
            ),
          )),
      key: key,
      body: Column(),
    );
  }
}
