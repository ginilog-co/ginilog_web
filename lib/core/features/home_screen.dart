// ignore_for_file: library_private_types_in_public_api, use_build_context_synchronously, deprecated_member_use

import 'dart:ui';
import 'package:ginilog_customer_app/core/components/helpers/globals.dart';
import 'package:ginilog_customer_app/core/components/utils/constants.dart';
import 'package:ginilog_customer_app/core/features/account/services/account_services.dart';
import 'package:ginilog_customer_app/core/features/account/view/account.dart';
import 'package:ginilog_customer_app/core/features/bookings/controller/bookings.dart';
import 'package:ginilog_customer_app/core/features/home/controller/home.dart';
import 'package:ginilog_customer_app/core/features/order_history/controller/order_history.dart';
import 'package:geolocator/geolocator.dart' as positions;
import 'package:geolocator/geolocator.dart';
import 'package:ginilog_customer_app/core/features/auth/services/auth_service.dart';

import '../components/utils/colors.dart';
import '../components/utils/package_export.dart';

final GlobalKey<NavigatorState> navKey = GlobalKey<NavigatorState>();

class HomeScreenPage extends StatefulWidget {
  const HomeScreenPage({super.key, required this.imdex});
  final int imdex;
  @override
  _NavBarfeaturestate createState() => _NavBarfeaturestate();
}

class _NavBarfeaturestate extends State<HomeScreenPage>
    with WidgetsBindingObserver {
  int _currentIndex = 0;
  positions.Position? _currentPosition;
  String? _currentAddress;
  String? _city;
  String? _state;
  @override
  void initState() {
    super.initState();
    deviceToken();
    _getCurrentPosition();
    setState(() {
      _currentIndex = widget.imdex;
    });
    WidgetsBinding.instance.addObserver(this); // Add observer
    setStatus(true);
  }

  @override
  void dispose() {
    WidgetsBinding.instance.removeObserver(this); // Remove observer
    super.dispose();
  }

  void setStatus(bool status) async {
    await AccountService().updateProfile(availability: status);
  }

  @override
  void didChangeAppLifecycleState(AppLifecycleState state) {
    debugPrint("Lifecycle state: $state");
    switch (state) {
      case AppLifecycleState.resumed:
        setStatus(true); // App is active
        break;
      case AppLifecycleState.inactive:
        debugPrint("App is inactive.");
        setStatus(false); // Handle inactive state here, if needed
        break;
      case AppLifecycleState.paused:
        setStatus(false); // App is in the background
        break;
      case AppLifecycleState.detached:
        debugPrint("App is detached.");
        setStatus(false); // Handle detached state here, if needed
        break;
      case AppLifecycleState.hidden:
        debugPrint("App is hidden.");
        setStatus(false); // Handle hidden state here, if needed
        break;
    }
  }

  Future<bool> _handleLocationPermission() async {
    bool serviceEnabled;
    positions.LocationPermission permission;

    serviceEnabled = await positions.Geolocator.isLocationServiceEnabled();
    if (!serviceEnabled) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(
          content: Text(
            'Location services are disabled. Please enable the services',
          ),
        ),
      );
      return false;
    }
    permission = await positions.Geolocator.checkPermission();
    if (permission == LocationPermission.denied) {
      permission = await Geolocator.requestPermission();
      if (permission == LocationPermission.denied) {
        ScaffoldMessenger.of(context).showSnackBar(
          const SnackBar(content: Text('Location permissions are denied')),
        );
        return false;
      }
    }
    if (permission == LocationPermission.deniedForever) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(
          content: Text(
            'Location permissions are permanently denied, we cannot request permissions.',
          ),
        ),
      );
      return false;
    }
    return true;
  }

  Future<void> _getCurrentPosition() async {
    final hasPermission = await _handleLocationPermission();

    if (!hasPermission) return;
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
            _currentAddress =
                '${place.street}, ${place.subLocality}, ${place.subAdministrativeArea}, ${place.postalCode}';
            _city = "${place.subAdministrativeArea}";
            _state = "${place.administrativeArea}";
          });
          await globals.init();
          printData("Location", _currentAddress!);
          printData("Latitude", _currentPosition!.latitude);
          printData("Longitude", _currentPosition!.longitude);
          printData("City", _city);
          printData("State", _state);
        })
        .catchError((e) {
          debugPrint(e);
        });
  }

  int tabselected = 0;

  Future<bool> _onPopInvoked(bool isPop, pop) async {
    if (_currentIndex != 0) {
      setState(() {
        _currentIndex = 0;
      });
      return false;
    }
    return (await showDialog(
          context: context,
          builder:
              (context) => BackdropFilter(
                filter: ImageFilter.blur(sigmaX: 6, sigmaY: 6),
                child: AlertDialog(
                  title: const Text(
                    'Exit App',
                    style: TextStyle(color: AppColors.primary),
                  ),
                  content: const Text(
                    'Do you want to exit the app?',
                    style: TextStyle(color: AppColors.primary),
                  ),
                  backgroundColor: Colors.white,
                  shape: RoundedRectangleBorder(
                    borderRadius: BorderRadius.circular(15),
                  ),
                  actions: <Widget>[
                    TextButton(
                      child: const Text(
                        'Yes',
                        style: TextStyle(color: Colors.black),
                      ),
                      onPressed: () {
                        SystemNavigator.pop();
                      },
                    ),
                    TextButton(
                      child: const Text(
                        'No',
                        style: TextStyle(color: AppColors.primary),
                      ),
                      onPressed: () {
                        Navigator.of(context).pop(false);
                      },
                    ),
                  ],
                ),
              ),
        )) ??
        false;
  }

  deviceToken() async {
    await AuthService().updateDeviceToken();
  }

  @override
  Widget build(BuildContext context) {
    List<Widget> children = [
      const HomeScreen(),
      const OrderHistoryScreen(),
      const BookingsScreen(),
      const AccountPage(),
    ];
    return Consumer(
      builder: (context, WidgetRef ref, Widget? child) {
        return PopScope(
          onPopInvokedWithResult: _onPopInvoked,
          canPop: false,
          child: Scaffold(
            backgroundColor: Colors.white,
            body: children[_currentIndex],
            bottomNavigationBar: BottomNavigationBar(
              backgroundColor: Colors.white,
              selectedLabelStyle: const TextStyle(
                fontSize: 10,
                fontWeight: FontWeight.w400,
                fontFamily: 'Inter',
              ),
              unselectedLabelStyle: const TextStyle(
                fontSize: 10,
                fontWeight: FontWeight.w400,
                fontFamily: 'Inter',
              ),
              selectedItemColor: AppColors.primary,
              unselectedItemColor: Colors.grey,
              currentIndex: _currentIndex,
              type: BottomNavigationBarType.fixed,
              onTap: (index) {
                onTabTapped(index);
              },
              items: [
                BottomNavigationBarItem(
                  icon: Padding(
                    padding: const EdgeInsets.only(bottom: 5.0),
                    child: Image.asset(
                      "assets/images/home.png",
                      width: 20,
                      color:
                          _currentIndex == 0 ? AppColors.primary : Colors.grey,
                    ),
                  ),
                  label: 'Home',
                ),
                BottomNavigationBarItem(
                  icon: Padding(
                    padding: const EdgeInsets.only(bottom: 5.0, right: 8),
                    child: Image.asset(
                      "assets/images/track_history.png",
                      width: 20,
                      color:
                          _currentIndex == 1 ? AppColors.primary : Colors.grey,
                    ),
                  ),
                  label: 'Track',
                ),
                BottomNavigationBarItem(
                  icon: Padding(
                    padding: const EdgeInsets.only(bottom: 5.0),
                    child: Image.asset(
                      "assets/images/bookings.png",
                      width: 20,
                      color:
                          _currentIndex == 2 ? AppColors.primary : Colors.grey,
                    ),
                  ),
                  label: 'Bookings',
                ),
                BottomNavigationBarItem(
                  icon: Padding(
                    padding: const EdgeInsets.only(bottom: 5.0),
                    child: Image.asset(
                      "assets/images/account_icon.png",
                      width: 20,
                      color:
                          _currentIndex == 3 ? AppColors.primary : Colors.grey,
                    ),
                  ),
                  label: 'Profile',
                ),
              ],
            ),
          ),
        );
      },
    );
  }

  void onTabTapped(int index) {
    setState(() {
      _currentIndex = index;
    });
  }
}
