// ignore_for_file: use_build_context_synchronously, deprecated_member_use

import 'dart:async';

import 'package:ginilog_customer_app/core/components/helpers/globals.dart';
import 'package:ginilog_customer_app/core/components/utils/constants.dart';
import 'package:ginilog_customer_app/core/components/utils/package_export.dart';
import 'package:ginilog_customer_app/core/features/account/states/account_provider.dart';
import 'package:ginilog_customer_app/core/features/home/states/home_state.dart';
import 'package:ginilog_customer_app/core/features/home/view/home.dart';
import 'package:ginilog_customer_app/core/features/order_history/model/package_orders_model.dart';
import 'package:ginilog_customer_app/core/features/order_history/states/order_state.dart';
import 'package:geolocator/geolocator.dart' as positions;
import 'package:geolocator/geolocator.dart';

class HomeScreen extends ConsumerStatefulWidget {
  const HomeScreen({super.key});

  @override
  ConsumerState<HomeScreen> createState() => HomeScreenController();
}

class HomeScreenController extends ConsumerState<HomeScreen> {
  String? profilePicture = "";
  String? userPhone = "";
  String? firstNames = "";
  String? lastNames = "";
  String allNames = "";
  String city = "";
  positions.Position? _currentPosition;
  String? currentAddress;
  String? cityLocation;
  String? postcodes;
  String? state;
  double? currentLat;
  double? currentLon;
  late List<PackageOrderResponseModel> allOrders;
  Timer? timer;
  int currentIndex = 0;
  final CarouselSliderController carouselController =
      CarouselSliderController();
  late PackageOrderNotifier orderProvider;
  @override
  void initState() {
    _getCurrentPosition();
    super.initState();
    // User Data
    final accountProviderd = ref.read(accountProvider.notifier);
    accountProviderd.getAccount();
    accountProviderd.userData;
    profilePicture =
        accountProviderd.userData?.profilePicture ??
        "${globals.profilePicture}";
    firstNames = accountProviderd.userData?.firstName ?? "${globals.userName}";
    lastNames = accountProviderd.userData?.lastName ?? "";
    allNames = "$firstNames $lastNames";
    userPhone = accountProviderd.userData?.phoneNo ?? "";
    // Logistics Company
    final station = ref.read(homeProvider.notifier);

    // Order
    orderProvider = ref.read(packageOrderProvider.notifier);
    Future.microtask(() {
      orderProvider.getAllPackageOrderData();
      allOrders = orderProvider.allPackageOrders;
      station.getAllLogisticsData();
      station.getAllAdvertData();
    });
    // Connect WebSocket and join this order
    orderProvider.connectAndJoinOrder(
      orderId: globals.userId!,
      isSingle: false,
    );
  }

  @override
  void dispose() {
    orderProvider.disconnect();
    super.dispose();
  }

  Future<void> _getCurrentPosition() async {
    await Geolocator.getCurrentPosition(desiredAccuracy: LocationAccuracy.high)
        .then((positions.Position position) {
          setState(() => _currentPosition = position);
          _getAddressFromLatLng(_currentPosition!);
        })
        .catchError((e) {
          debugPrint(e);
        });
  }

  Future<void> _getAddressFromLatLng(positions.Position position) async {
    await placemarkFromCoordinates(
          _currentPosition!.latitude,
          _currentPosition!.longitude,
        )
        .then((List<Placemark> placemarks) async {
          Placemark place = placemarks[0];
          setState(() {
            currentAddress =
                '${place.street}, ${place.subLocality}, ${place.subAdministrativeArea}, ${place.postalCode}';
            cityLocation = "${place.subAdministrativeArea}";
            state = "${place.administrativeArea}";
            currentLat = _currentPosition!.latitude;
            currentLon = _currentPosition!.longitude;
            postcodes = place.postalCode;
          });
          printData("Location", currentAddress!);
          printData("Latitude", _currentPosition!.latitude);
          printData("Longitude", _currentPosition!.longitude);
          printData("City", cityLocation);
          printData("State", state);
          printData("Postcodes", postcodes);
        })
        .catchError((e) {
          debugPrint(e);
        });
  }

  pageChanged(int index) {
    setState(() {
      currentIndex = index;
    });
  }

  @override
  Widget build(BuildContext context) {
    return HomeScreenView(this);
  }
}
