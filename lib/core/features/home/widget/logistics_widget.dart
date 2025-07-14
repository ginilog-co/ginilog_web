// ignore_for_file: library_private_types_in_public_api

import 'package:ginilog_customer_app/core/components/utils/colors.dart';
import 'package:ginilog_customer_app/core/components/utils/package_export.dart';
import 'package:ginilog_customer_app/core/components/utils/size_config.dart';
import 'package:ginilog_customer_app/core/features/account/model/user_response_model.dart';
import 'package:ginilog_customer_app/core/features/home/model/company_response_model.dart';

import '../../../components/widgets/app_text.dart';

class LogisticsWidget extends ConsumerStatefulWidget {
  const LogisticsWidget({super.key, required this.dataModel});

  final List<LogisticResponseModel> dataModel;
  @override
  _LogisticsWidgetState createState() => _LogisticsWidgetState();
}

class _LogisticsWidgetState extends ConsumerState<LogisticsWidget> {
  DeliveryAddress? country2;
  num distance = 10.1;
  @override
  void initState() {
    super.initState();
  }

  @override
  Widget build(BuildContext context) {
    var post = widget.dataModel.toList();
    return Container(
      color: AppColors.white,
      child:
          post.isEmpty
              ? const Center(
                child: Column(
                  mainAxisAlignment: MainAxisAlignment.center,
                  crossAxisAlignment: CrossAxisAlignment.center,
                  children: [
                    AppText(
                      isBody: true,
                      text: "Coming soon!",
                      textAlign: TextAlign.start,
                      fontSize: 85,
                      color: AppColors.black,
                      fontStyle: FontStyle.normal,
                      fontWeight: FontWeight.bold,
                    ),
                    Padding(
                      padding: EdgeInsets.all(8.0),
                      child: AppText(
                        isBody: true,
                        text:
                            "There is no available Logistics Company in Your location",
                        textAlign: TextAlign.center,
                        fontSize: 18,
                        color: AppColors.black,
                        fontStyle: FontStyle.normal,
                        fontWeight: FontWeight.normal,
                      ),
                    ),
                  ],
                ),
              )
              : Column(
                children: List.generate(post.length, (index) {
                  final data3 = post[index];
                  return Column(
                    children: [
                      InkWell(
                        onTap: () {},
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
                                borderRadius: BorderRadius.circular(50),
                                //set border radius to 50% of square height and width
                                image: DecorationImage(
                                  image: NetworkImage(
                                    data3.companyLogo.toString(),
                                  ),
                                  fit: BoxFit.contain, //change image fill type
                                ),
                              ),
                            ),
                            addHorizontalSpacing(10),
                            Expanded(
                              child: Column(
                                crossAxisAlignment: CrossAxisAlignment.start,
                                children: [
                                  AppText(
                                    isBody: true,
                                    text: "${data3.companyName}",
                                    textAlign: TextAlign.start,
                                    fontSize: 85,
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
                                    fontSize: 65,
                                    color: AppColors.black,
                                    fontStyle: FontStyle.normal,
                                    fontWeight: FontWeight.w500,
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
                      const Divider(thickness: 0.7, color: AppColors.grey),
                    ],
                  );
                }),
              ),
    );
  }
}
