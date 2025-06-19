// ignore_for_file: library_private_types_in_public_api

import 'package:ginilog_customer_app/core/components/utils/app_buttons.dart';
import 'package:ginilog_customer_app/core/components/utils/colors.dart';
import 'package:ginilog_customer_app/core/components/utils/helper_functions.dart';
import 'package:ginilog_customer_app/core/components/utils/package_export.dart';
import 'package:ginilog_customer_app/core/components/utils/size_config.dart';
import 'package:ginilog_customer_app/core/components/widgets/app_text.dart';
import 'package:ginilog_customer_app/core/features/home/controller/place_order.dart';

class SendParcelTypeBottomSheet extends ConsumerStatefulWidget {
  const SendParcelTypeBottomSheet({super.key, required this.phoneNumber});
  final String phoneNumber;
  @override
  _SendParcelTypeBottomSheetState createState() =>
      _SendParcelTypeBottomSheetState();
}

class _SendParcelTypeBottomSheetState
    extends ConsumerState<SendParcelTypeBottomSheet> {
  int selectedIndex = -1;
  String selectedSendParcelMethod = "";

  final List<SendParcelMethod> sendParcelMethods = [
    SendParcelMethod(
      image: "assets/images/rider_icon.png",
      sendParcelMethod: 'Same State',
      subtitle: 'Send Parcel within the same state',
      index: 0,
    ),
    SendParcelMethod(
      image: "assets/images/truck_icon.png",
      sendParcelMethod: 'Inter state',
      subtitle: 'Send Parcel outside your current state',
      index: 1,
    ),
    SendParcelMethod(
      image: "assets/images/chatter_icon.png",
      sendParcelMethod: 'Charter',
      subtitle: 'Charter a vehicle to deliver your parcel',
      index: 2,
    ),
    SendParcelMethod(
      image: "assets/images/flight_icon.png",
      sendParcelMethod: 'International',
      subtitle: 'Send Parcel outside the country',
      index: 3,
    ),
  ];

  @override
  Widget build(BuildContext context) {
    return Container(
      width: double.infinity,
      padding: EdgeInsets.only(top: 10, left: 8, right: 8),
      height: getScreenHeight(context) * 0.65,
      decoration: const BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.vertical(top: Radius.circular(20)),
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          /// Header
          Row(
            mainAxisAlignment: MainAxisAlignment.spaceBetween,
            children: [
              AppText(
                isBody: true,
                text: "Choose the Parcel Type",
                textAlign: TextAlign.start,
                fontSize: 30,
                color: AppColors.black,
                fontStyle: FontStyle.normal,
                fontWeight: FontWeight.w800,
              ),
              addHorizontalSpacing(10),
              const Divider(),
              addHorizontalSpacing(10),
              GestureDetector(
                onTap: () => Navigator.pop(context),
                child: const SizedBox(
                  height: 30,
                  width: 30,
                  child: Icon(Icons.close),
                ),
              ),
            ],
          ),
          addVerticalSpacing(context, 1.2),
          const Divider(),

          addVerticalSpacing(context, 1.2),

          /// Contact Method Selector (GridView)
          Expanded(
            child: GridView.builder(
              shrinkWrap: true,
              itemCount: sendParcelMethods.length,
              padding: EdgeInsets.symmetric(
                vertical: getScreenWidth(context) / 200,
              ),
              gridDelegate: SliverGridDelegateWithFixedCrossAxisCount(
                crossAxisCount: 2, // Adjust to fit your design
                crossAxisSpacing: getScreenWidth(context) / 100,
                mainAxisSpacing: getScreenWidth(context) / 100,
                childAspectRatio: 1.0, // Adjust for better spacing
              ),
              itemBuilder: (context, index) {
                final method = sendParcelMethods[index];
                bool isSelected = method.index == selectedIndex;

                return GestureDetector(
                  onTap: () {
                    setState(() {
                      selectedIndex = method.index;
                      selectedSendParcelMethod = method.sendParcelMethod;
                    });
                  },
                  child: BoxSizer(
                    widthPercent: 45, // responsive width for 2-column grid
                    heightPercent: 23, // adjust for content height
                    safeArea: true,
                    child: SendParcelMethodWidget(
                      method: method,
                      isSelected: isSelected,
                    ),
                  ),
                );
              },
            ),
          ),

          addVerticalSpacing(context, 5),

          /// Continue Button
          Row(
            spacing: 5,
            children: [
              Expanded(
                child: AppButton(
                  text: "Cancel",
                  onPressed: () => Navigator.pop(context),
                  widthPercent: 70,
                  heightPercent: 6,
                  btnColor: AppColors.grey,
                  isLoading: false,
                ),
              ),
              Expanded(
                child:
                    selectedSendParcelMethod.isEmpty
                        ? AppButton(
                          text: "Continue",
                          onPressed: () {},
                          widthPercent: 70,
                          heightPercent: 6,
                          btnColor: AppColors.grey,
                          isLoading: false,
                        )
                        : AppButton(
                          text: "Continue",
                          onPressed: () {
                            Navigator.pop(context);
                            navigateToRoute(
                              context,
                              PlaceOrderScreen(
                                shippingType: selectedSendParcelMethod,
                                userPhone: widget.phoneNumber.toString(),
                              ),
                            );
                          },
                          widthPercent: 70,
                          heightPercent: 6,
                          btnColor: AppColors.primary,
                          isLoading: false,
                        ),
              ),
            ],
          ),
          addVerticalSpacing(context, 5),
        ],
      ),
    );
  }
}

/// Contact Method Widget (Simplified)
class SendParcelMethodWidget extends StatelessWidget {
  final SendParcelMethod method;
  final bool isSelected;

  const SendParcelMethodWidget({
    super.key,
    required this.method,
    required this.isSelected,
  });

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: EdgeInsets.only(left: 4, right: 4),
      decoration: BoxDecoration(
        color:
            isSelected
                ? AppColors.primary.withValues(alpha: 1.3)
                : Colors.white,
        borderRadius: BorderRadius.circular(20),
        border: Border.all(
          color: isSelected ? AppColors.primary : AppColors.grey,
        ),
      ),
      child: Column(
        spacing: getScreenWidth(context) / 80,
        crossAxisAlignment: CrossAxisAlignment.center,
        mainAxisAlignment: MainAxisAlignment.center,
        children: [
          Image.asset(
            method.image,
            height: getScreenWidth(context) / 10,
            width: getScreenWidth(context) / 10,
          ),
          AppText(
            isBody: false,
            text: method.sendParcelMethod,
            textAlign: TextAlign.center,
            fontSize: 35,
            color: isSelected ? AppColors.white : AppColors.black,
            fontStyle: FontStyle.normal,
            fontWeight: FontWeight.bold,
          ),
          AppText(
            isBody: true,
            text: method.subtitle,
            textAlign: TextAlign.center,
            fontSize: 35,
            color: isSelected ? AppColors.white : AppColors.black,
            fontStyle: FontStyle.normal,
            fontWeight: FontWeight.w400,
          ),
        ],
      ),
    );
  }
}

/// Contact Method Model
class SendParcelMethod {
  final String sendParcelMethod;
  final String subtitle;
  final String image;
  final int index;

  SendParcelMethod({
    required this.sendParcelMethod,
    required this.subtitle,
    required this.image,
    required this.index,
  });
}
