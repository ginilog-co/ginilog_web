// ignore_for_file: library_private_types_in_public_api

import 'package:ginilog_customer_app/core/components/helpers/globals.dart';
import 'package:ginilog_customer_app/core/components/utils/size_config.dart';
import 'package:ginilog_customer_app/core/components/widgets/back_icon.dart';
import 'package:ginilog_customer_app/core/features/home/model/notification_model.dart';
import 'package:ginilog_customer_app/core/features/home/states/home_state.dart';

import '../../../components/utils/colors.dart';
import '../../../components/utils/helper_functions.dart';
import '../../../components/utils/package_export.dart';
import '../../../components/widgets/app_text.dart';

class NotificationPage extends ConsumerStatefulWidget {
  const NotificationPage({
    super.key,
  });

  @override
  _LoginPageState createState() => _LoginPageState();
}

class _LoginPageState extends ConsumerState<NotificationPage> {
  List<NotificationResponseModel> notifications = [];

  @override
  void initState() {
    super.initState();
    final station = ref.read(homeProvider.notifier);
    station.getAllNotificationData();
    station.notifications;
    var tip = station.notifications
        .where((element) => element.userId == globals.userId)
        .toList();
    notifications.addAll(tip);
  }

  @override
  void dispose() {
    super.dispose();
  }

  List<NotificationResponseModel> getOrdered(
      List<NotificationResponseModel> model) {
    List<NotificationResponseModel> mostActive = model;
    mostActive.sort((a, b) {
      return b.createdAt!.compareTo(a.createdAt!);
    });
    return mostActive;
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: AppColors.white,
      appBar: PreferredSize(
          preferredSize: Size.fromHeight(SizeConfig.heightAdjusted(18)),
          child: Padding(
            padding: EdgeInsets.only(top: SizeConfig.heightAdjusted(10)),
            child: GlobalBackButton(
              backText: "Notifications",
              showBackButton: true,
              buttonElements: [
                IconButton(onPressed: () {}, icon: Icon(Icons.settings))
              ],
            ),
          )),
      body: SafeArea(
        child: Padding(
          padding: const EdgeInsets.only(left: 18.0, right: 18.0),
          child: getOrdered(notifications).isNotEmpty
              ? ListView.builder(
                  shrinkWrap: true,
                  scrollDirection: Axis.vertical,
                  itemCount: getOrdered(notifications).length,
                  itemBuilder: (BuildContext context, int index) {
                    final data3 = getOrdered(notifications)[index];
                    DateTime dt2 = DateTime.parse(data3.createdAt.toString());
                    String date = DateFormat("E, MMM d").format(dt2.toLocal());
                    String time = DateFormat("hh:mm").format(dt2.toLocal());
                    return Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      mainAxisAlignment: MainAxisAlignment.start,
                      children: [
                        Container(
                          decoration: BoxDecoration(
                            // ignore: deprecated_member_use
                            color: AppColors.grey.withOpacity(0.3),
                            borderRadius: BorderRadius.circular(1),
                          ),
                          child: Column(
                            crossAxisAlignment: CrossAxisAlignment.start,
                            mainAxisAlignment: MainAxisAlignment.start,
                            children: [
                              InkWell(
                                  onTap: () {},
                                  child: Padding(
                                    padding: const EdgeInsets.all(8.0),
                                    child: Column(
                                      crossAxisAlignment:
                                          CrossAxisAlignment.start,
                                      mainAxisAlignment:
                                          MainAxisAlignment.start,
                                      children: [
                                        AppText(
                                            isBody: false,
                                            text: "${data3.title}",
                                            textAlign: TextAlign.start,
                                            fontSize: 20,
                                            color: AppColors.black,
                                            fontStyle: FontStyle.normal,
                                            maxLines: 1,
                                            fontWeight: FontWeight.bold),
                                        AppText(
                                            isBody: true,
                                            text: "${data3.body}",
                                            textAlign: TextAlign.start,
                                            fontSize: 29,
                                            color: AppColors.black,
                                            maxLines: 2,
                                            overflow: TextOverflow.ellipsis,
                                            fontStyle: FontStyle.normal,
                                            fontWeight: FontWeight.w400),
                                        addVerticalSpacing(context, 55),
                                        AppText(
                                            isBody: true,
                                            text: "$date $time",
                                            textAlign: TextAlign.start,
                                            fontSize: 25,
                                            color: AppColors.green,
                                            fontStyle: FontStyle.normal,
                                            fontWeight: FontWeight.w400),
                                      ],
                                    ),
                                  )),
                              const Divider(
                                thickness: 1.5,
                                color: AppColors.grey,
                              ),
                            ],
                          ),
                        ),
                        addVerticalSpacing(context, 35)
                      ],
                    );
                  },
                )
              : Center(
                  child: Column(
                    mainAxisAlignment: MainAxisAlignment.center,
                    children: [
                      Image.asset(
                        "assets/images/notification_icon.png",
                        width: 100,
                        height: 100,
                      ),
                      addVerticalSpacing(context, 5),
                      const AppText(
                          isBody: false,
                          text: "Nothing to show here",
                          textAlign: TextAlign.start,
                          fontSize: 35,
                          color: AppColors.black,
                          fontStyle: FontStyle.normal,
                          fontWeight: FontWeight.bold),
                      const AppText(
                          isBody: true,
                          text: "There is no notification yet",
                          textAlign: TextAlign.center,
                          fontSize: 30,
                          color: AppColors.black,
                          fontStyle: FontStyle.normal,
                          fontWeight: FontWeight.normal),
                    ],
                  ),
                ),
        ),
      ),
    );
  }
}
