// ignore_for_file: library_private_types_in_public_api

import 'dart:math';

import 'package:ginilog_customer_app/core/components/state/nigera_states.dart';
import 'package:ginilog_customer_app/core/components/utils/size_config.dart';
import 'package:ginilog_customer_app/core/components/widgets/back_icon.dart';
import 'package:ginilog_customer_app/core/features/home/model/company_response_model.dart';
import 'package:ginilog_customer_app/core/features/home/states/home_state.dart';

import '../../../components/utils/colors.dart';
import '../../../components/utils/helper_functions.dart';
import '../../../components/utils/package_export.dart';
import '../../../components/widgets/app_text.dart';

class ViewAllLogisticsPage extends ConsumerStatefulWidget {
  const ViewAllLogisticsPage({
    required this.state,
    required this.city,
    required this.latitude,
    required this.longitude,
    super.key,
  });
  final String state;
  final String city;
  final num latitude;
  final num longitude;
  @override
  _LoginPageState createState() => _LoginPageState();
}

class _LoginPageState extends ConsumerState<ViewAllLogisticsPage> {
  List<LogisticResponseModel> allLogisticss = [];
  num distance = 10.1;
  @override
  void initState() {
    super.initState();
    final station = ref.read(homeProvider.notifier);
    station.getAllLogisticsData();
    station.allLogisticss;
    allLogisticss = station.allLogisticss.toList();
    final provider = ref.read(regionProvider);
    provider.loadCountries();
  }

  @override
  void dispose() {
    super.dispose();
  }

  List<LogisticResponseModel> getLogisticsByState() {
    return allLogisticss.where((element) {
      return (element.state!.toLowerCase().contains(
                widget.state.toLowerCase(),
              ) ||
              element.locality!.toLowerCase().contains(
                widget.city.toLowerCase(),
              )) &&
          (element.available == true);
    }).toList();
  }

  num calculateDistance(lat1, lon1, lat2, lon2) {
    const earthRadiusKm = 6371; // Radius of Earth in kilometers

    // Convert degrees to radians
    num dLat = _degreesToRadians(lat2 - lat1);
    num dLon = _degreesToRadians(lon2 - lon1);

    // Haversine formula
    num a =
        sin(dLat / 2) * sin(dLat / 2) +
        cos(_degreesToRadians(lat1)) *
            cos(_degreesToRadians(lat2)) *
            sin(dLon / 2) *
            sin(dLon / 2);
    num c = 2 * atan2(sqrt(a), sqrt(1 - a));

    return earthRadiusKm * c; // Distance in kilometers
  }

  num _degreesToRadians(num degrees) {
    return degrees * pi / 180;
  }

  @override
  Widget build(BuildContext context) {
    var dataModel =
        getLogisticsByState().where((element) {
          num distanceFilter = calculateDistance(
            widget.latitude,
            widget.longitude,
            element.latitude!,
            element.longitude!,
          );
          return distanceFilter <= distance;
        }).toList();
    return Scaffold(
      backgroundColor: AppColors.white,
      appBar: PreferredSize(
        preferredSize: Size.fromHeight(SizeConfig.heightAdjusted(16)),
        child: Padding(
          padding: EdgeInsets.only(top: SizeConfig.heightAdjusted(10)),
          child: const GlobalBackButton(
            backText: "Logistics Company",
            showBackButton: true,
          ),
        ),
      ),
      body: SafeArea(
        child: Padding(
          padding: const EdgeInsets.only(left: 18.0, right: 18.0),
          child: Column(
            children: [
              addVerticalSpacing(context, 20),
              Expanded(
                child:
                    dataModel.isNotEmpty
                        ? ListView.builder(
                          shrinkWrap: true,
                          scrollDirection: Axis.vertical,
                          itemCount: dataModel.length,
                          itemBuilder: (BuildContext context, int index) {
                            final data3 = dataModel[index];
                            return Column(
                              children: [
                                InkWell(
                                  onTap: () {
                                    // navigateToRoute(
                                    //     context,
                                    //     LogisticsDetailsPage(
                                    //       station: data3,
                                    //       addressCatList: widget.addressCatList,
                                    //     ));
                                  },
                                  child: Row(
                                    children: [
                                      addHorizontalSpacing(10),
                                      Container(
                                        margin: const EdgeInsets.all(5),
                                        height: 100.0,
                                        width: 100.0,
                                        decoration: BoxDecoration(
                                          border: Border.all(
                                            color: AppColors.grey,
                                            width: 1,
                                          ),
                                          borderRadius: BorderRadius.circular(
                                            50,
                                          ),
                                          //set border radius to 50% of square height and width
                                          image: DecorationImage(
                                            image: NetworkImage(
                                              data3.companyLogo.toString(),
                                            ),
                                            fit:
                                                BoxFit
                                                    .contain, //change image fill type
                                          ),
                                        ),
                                      ),
                                      addHorizontalSpacing(10),
                                      Expanded(
                                        child: Column(
                                          crossAxisAlignment:
                                              CrossAxisAlignment.start,
                                          children: [
                                            AppText(
                                              isBody: true,
                                              text: "${data3.companyName}",
                                              textAlign: TextAlign.start,
                                              fontSize: 15,
                                              color: AppColors.black,
                                              fontStyle: FontStyle.normal,
                                              maxLines: 1,
                                              fontWeight: FontWeight.bold,
                                            ),
                                            AppText(
                                              isBody: true,
                                              text:
                                                  "${data3.companyAddress}, ${data3.locality}, ${data3.state}",
                                              textAlign: TextAlign.start,
                                              fontSize: 25,
                                              color: AppColors.black,
                                              fontStyle: FontStyle.normal,
                                              fontWeight: FontWeight.w700,
                                            ),
                                            const AppText(
                                              isBody: true,
                                              text: "",
                                              textAlign: TextAlign.start,
                                              fontSize: 25,
                                              color: AppColors.green,
                                              fontStyle: FontStyle.normal,
                                              fontWeight: FontWeight.w400,
                                            ),
                                          ],
                                        ),
                                      ),
                                    ],
                                  ),
                                ),
                                const Divider(
                                  thickness: 0.7,
                                  color: AppColors.grey,
                                ),
                              ],
                            );
                          },
                        )
                        : Center(
                          child: Column(
                            mainAxisAlignment: MainAxisAlignment.center,
                            crossAxisAlignment: CrossAxisAlignment.center,
                            children: [
                              Image.asset(
                                "assets/images/empty.png",
                                width: 100,
                                height: 100,
                              ),
                              addVerticalSpacing(context, 50),
                              const Padding(
                                padding: EdgeInsets.all(8.0),
                                child: AppText(
                                  isBody: false,
                                  text: "Coming soon!",
                                  textAlign: TextAlign.start,
                                  fontSize: 25,
                                  color: AppColors.black,
                                  fontStyle: FontStyle.normal,
                                  fontWeight: FontWeight.bold,
                                ),
                              ),
                              const AppText(
                                isBody: true,
                                text:
                                    "There is no available Logistics Company in Your location",
                                textAlign: TextAlign.center,
                                fontSize: 20,
                                color: AppColors.black,
                                fontStyle: FontStyle.normal,
                                fontWeight: FontWeight.normal,
                              ),
                            ],
                          ),
                        ),
              ),
            ],
          ),
        ),
      ),
    );
  }
}
