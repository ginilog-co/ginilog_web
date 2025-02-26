import 'dart:io';

import 'package:ginilog_customer_app/core/components/utils/app_buttons.dart';
import 'package:ginilog_customer_app/core/components/utils/size_config.dart';
import 'package:ginilog_customer_app/core/components/widgets/back_icon.dart';
import 'package:ginilog_customer_app/core/features/home/controller/place_order.dart';
import 'package:google_places_autocomplete_text_field/google_places_autocomplete_text_field.dart';

import '../../../components/architecture/mvc.dart';
import '../../../components/utils/colors.dart';
import '../../../components/utils/helper_functions.dart';
import '../../../components/utils/package_export.dart';
import '../../../components/widgets/app_text.dart';
import '../../../components/widgets/input.dart';

class PlaceOrderScreenView
    extends StatelessView<PlaceOrderScreen, PlaceOrderScreenController> {
  const PlaceOrderScreenView(super.state, {super.key});

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: AppColors.white,
      appBar: PreferredSize(
          preferredSize: Size.fromHeight(SizeConfig.heightAdjusted(16)),
          child: Padding(
            padding: EdgeInsets.only(top: SizeConfig.heightAdjusted(10)),
            child: const GlobalBackButton(
                backText: "Place Your Order Here", showBackButton: true),
          )),
      body: Container(
        height: getScreenHeight(context),
        width: getScreenWidth(context),
        color: AppColors.white,
        child: Column(
          children: [
            // Clickable Page Indicators
            Padding(
              padding: const EdgeInsets.all(6.0),
              child: Row(
                children: [
                  // Indicator 1 (Clickable)
                  Expanded(
                    child: GestureDetector(
                      onTap: () {
                        controller.pageController.animateToPage(
                          0,
                          duration: Duration(milliseconds: 300),
                          curve: Curves.easeInOut,
                        );
                      },
                      child: Container(
                        height: 8,
                        decoration: BoxDecoration(
                          color: controller.isOriginAddressChanged.isEmpty ||
                                  controller.isOriginNameChanged.isEmpty ||
                                  controller.isOriginEmailChanged.isEmpty ||
                                  controller.isOriginPhoneNoChanged.isEmpty ||
                                  controller.isDestinationNameChanged.isEmpty ||
                                  controller
                                      .isDestinationEmailChanged.isEmpty ||
                                  controller
                                      .isDestinationAddressChanged.isEmpty ||
                                  controller
                                      .isDestinationPhoneNoChanged.isEmpty ||
                                  controller.isPackageNameChanged.isEmpty ||
                                  controller.isItemQuantityChanged.isEmpty ||
                                  controller.isItemCostChanged.isEmpty ||
                                  controller.isItemModelNumberChanged.isEmpty
                              ? Colors.grey
                              : Colors.green,
                          borderRadius: BorderRadius.circular(5),
                        ),
                      ),
                    ),
                  ),
                  const SizedBox(width: 8),

                  // Indicator 2 (Clickable)
                  Expanded(
                    child: GestureDetector(
                      onTap: () {
                        controller.pageController.animateToPage(
                          1,
                          duration: Duration(milliseconds: 300),
                          curve: Curves.easeInOut,
                        );
                      },
                      child: Container(
                        height: 8,
                        decoration: BoxDecoration(
                          color: controller.isItemDescriptionChanged.isEmpty ||
                                  controller.isItemTypeChanged.isEmpty ||
                                  controller.companyId.isEmpty ||
                                  controller.imageUrls.isEmpty
                              ? Colors.grey
                              : Colors.green,
                          borderRadius: BorderRadius.circular(5),
                        ),
                      ),
                    ),
                  ),
                ],
              ),
            ),

            Expanded(
              child: PageView(
                physics: NeverScrollableScrollPhysics(),
                controller: controller.pageController,
                onPageChanged: (index) {
                  controller.pageChanged(index);
                },
                children: [
                  SingleChildScrollView(
                    child: Form(
                      key: controller.formKey,
                      child: Padding(
                        padding: const EdgeInsets.only(
                          left: 14.0,
                          right: 14.0,
                          top: 10,
                        ),
                        child: Column(
                          mainAxisAlignment: MainAxisAlignment.start,
                          crossAxisAlignment: CrossAxisAlignment.start,
                          spacing: 10,
                          children: [
                            Text(
                              "Origin Details",
                              style: TextStyle(
                                  fontSize: fontSized(context, 92),
                                  color: AppColors.black,
                                  fontWeight: FontWeight.bold,
                                  fontFamily: "Inter"),
                            ),
                            GooglePlacesAutoCompleteTextFormField(
                              style: TextStyle(
                                color: AppColors.black,
                                fontFamily: "Montserrat",
                                fontSize: fontSized(context, 85),
                              ),
                              textEditingController: controller.originAddress,
                              googleAPIKey:
                                  "AIzaSyA1WkH5DbnyUVLhPtqo_qj3Bmr0uKPolSw",
                              decoration: InputDecoration(
                                hintText: 'Enter Origin your address',
                                labelText: 'Origin',
                                labelStyle: TextStyle(
                                  color: AppColors.black,
                                  fontFamily: "Montserrat",
                                  fontSize: fontSized(context, 85),
                                ),
                                border: OutlineInputBorder(),
                              ),
                              validator: (value) {
                                if (value!.isEmpty) {
                                  return 'Please enter some text';
                                }
                                return null;
                              },
                              // proxyURL: _yourProxyURL,
                              onChanged: (String? value) {
                                controller.originAddressOnChanged(value!);
                              },
                              maxLines: 1,
                              overlayContainerBuilder: (child) => Material(
                                elevation: 1.0,
                                color: Colors.white,
                                borderRadius: BorderRadius.circular(12),
                                child: child,
                              ),
                              onPlaceDetailsWithCoordinatesReceived:
                                  (prediction) async {
                                controller.origin(prediction);
                              },
                              onSuggestionClicked: (prediction) => controller
                                  .originAddress.text = prediction.description!,
                              minInputLength: 3,
                            ),
                            addVerticalSpacing(context, 0.05),
                            GlobalTextField(
                              fieldName: 'Sender Name',
                              keyBoardType: TextInputType.name,
                              removeSpace: false,
                              obscureText: false,
                              textController: controller.senderName,
                              onChanged: (String? value) {
                                controller.originNameOnChanged(value!);
                              },
                            ),
                            addVerticalSpacing(context, 0.05),
                            GlobalTextField(
                              fieldName: 'Sender Email',
                              keyBoardType: TextInputType.emailAddress,
                              obscureText: false,
                              textController: controller.senderEmail,
                              onChanged: (String? value) {
                                controller.originEmailOnChanged(value!);
                              },
                            ),
                            addVerticalSpacing(context, 0.05),
                            GlobalTextField(
                              fieldName: 'Phone Number',
                              keyBoardType: TextInputType.phone,
                              obscureText: false,
                              textController: controller.originPhoneNoTec,
                              onChanged: (String? value) {
                                controller.originPhoneNoOnChanged(value!);
                              },
                            ),
                            Text(
                              "Destination Details",
                              style: TextStyle(
                                  fontSize: fontSized(context, 92),
                                  color: AppColors.black,
                                  fontWeight: FontWeight.bold,
                                  fontFamily: "Inter"),
                            ),
                            GooglePlacesAutoCompleteTextFormField(
                              style: TextStyle(
                                color: AppColors.black,
                                fontFamily: "Montserrat",
                                fontSize: fontSized(context, 85),
                              ),
                              textEditingController:
                                  controller.destinationAddress,
                              googleAPIKey:
                                  "AIzaSyA1WkH5DbnyUVLhPtqo_qj3Bmr0uKPolSw",
                              decoration: InputDecoration(
                                hintText: 'Enter Destination your address',
                                labelText: 'Reciever Address',
                                labelStyle: TextStyle(
                                  color: AppColors.black,
                                  fontFamily: "Montserrat",
                                  fontSize: fontSized(context, 85),
                                ),
                                border: OutlineInputBorder(),
                              ),
                              validator: (value) {
                                if (value!.isEmpty) {
                                  return 'Please enter some text';
                                }
                                return null;
                              },
                              // proxyURL: _yourProxyURL,
                              onChanged: (String? value) {
                                controller.destinationAddressOnChanged(value!);
                              },
                              maxLines: 1,
                              overlayContainerBuilder: (child) => Material(
                                elevation: 1.0,
                                color: Colors.white,
                                borderRadius: BorderRadius.circular(12),
                                child: child,
                              ),
                              onPlaceDetailsWithCoordinatesReceived:
                                  (prediction) async {
                                controller.destination(prediction);
                              },
                              onSuggestionClicked: (prediction) => controller
                                  .destinationAddress
                                  .text = prediction.description!,
                              minInputLength: 3,
                            ),
                            addVerticalSpacing(context, 0.05),
                            GlobalTextField(
                              fieldName: 'Receiver Name',
                              keyBoardType: TextInputType.name,
                              removeSpace: false,
                              obscureText: false,
                              textController: controller.receiverName,
                              onChanged: (String? value) {
                                controller.destinationNameOnChanged(value!);
                              },
                            ),
                            addVerticalSpacing(context, 0.05),
                            GlobalTextField(
                              fieldName: 'Receiver Email',
                              keyBoardType: TextInputType.emailAddress,
                              obscureText: false,
                              textController: controller.receiverEmail,
                              onChanged: (String? value) {
                                controller.destinationEmailOnChanged(value!);
                              },
                            ),
                            addVerticalSpacing(context, 0.05),
                            GlobalTextField(
                              fieldName: 'Receiver Phone Number',
                              keyBoardType: TextInputType.phone,
                              obscureText: false,
                              textController: controller.destinationPhoneNoTec,
                              onChanged: (String? value) {
                                controller.destinationPhoneNoOnChanged(value!);
                              },
                            ),
                            Text(
                              "Package Details",
                              style: TextStyle(
                                  fontSize: fontSized(context, 92),
                                  color: AppColors.black,
                                  fontWeight: FontWeight.bold,
                                  fontFamily: "Inter"),
                            ),
                            GlobalTextField(
                              fieldName: 'Package Name',
                              keyBoardType: TextInputType.name,
                              obscureText: false,
                              removeSpace: false,
                              textController: controller.packageName,
                              onChanged: (String? value) {
                                controller.packageNameOnChanged(value!);
                              },
                            ),
                            Row(
                              spacing: 5,
                              children: [
                                Expanded(
                                  child: GlobalTextField(
                                    fieldName: 'Package Model Number',
                                    keyBoardType: TextInputType.name,
                                    obscureText: false,
                                    textController: controller.itemModelNumber,
                                    onChanged: (String? value) {
                                      controller
                                          .itemModelNumberOnChanged(value!);
                                    },
                                  ),
                                ),
                                Expanded(
                                  child: GlobalTextField(
                                    fieldName: 'Package Weight',
                                    keyBoardType: TextInputType.number,
                                    obscureText: false,
                                    textController: controller.itemWeight,
                                    onChanged: (String? value) {
                                      controller.itemWeightOnChanged(value!);
                                    },
                                  ),
                                ),
                              ],
                            ),
                            Row(
                              spacing: 5,
                              children: [
                                Expanded(
                                  child: GlobalTextField(
                                    fieldName: 'Package Cost',
                                    keyBoardType: TextInputType.number,
                                    obscureText: false,
                                    textController: controller.itemCost,
                                    onChanged: (String? value) {
                                      controller.itemCostOnChanged(value!);
                                    },
                                  ),
                                ),
                                Expanded(
                                  child: GlobalTextField(
                                    fieldName: 'Package Quantity',
                                    keyBoardType: TextInputType.number,
                                    obscureText: false,
                                    textController: controller.itemQuantity,
                                    onChanged: (String? value) {
                                      controller.itemQuantityOnChanged(value!);
                                    },
                                  ),
                                ),
                              ],
                            ),
                            addVerticalSpacing(context, 5),
                            controller.isOriginAddressChanged.isEmpty ||
                                    controller
                                        .isDestinationAddressChanged.isEmpty ||
                                    controller
                                        .isDestinationPhoneNoChanged.isEmpty ||
                                    controller.isOriginNameChanged.isEmpty ||
                                    controller.isOriginEmailChanged.isEmpty ||
                                    controller.isOriginPhoneNoChanged.isEmpty ||
                                    controller
                                        .isDestinationNameChanged.isEmpty ||
                                    controller
                                        .isDestinationEmailChanged.isEmpty ||
                                    controller.isPackageNameChanged.isEmpty ||
                                    controller.isItemQuantityChanged.isEmpty ||
                                    controller.isItemCostChanged.isEmpty ||
                                    controller.isItemWeightChanged.isEmpty ||
                                    controller.isItemModelNumberChanged.isEmpty
                                ? appButton("Next", getScreenWidth(context),
                                    () {}, AppColors.grey, false)
                                : appButton("Next", getScreenWidth(context),
                                    () {
                                    controller.pageController.nextPage(
                                        duration: Duration(milliseconds: 300),
                                        curve: Curves.easeInOut);
                                  }, AppColors.primary, false),
                            addVerticalSpacing(context, 5),
                          ],
                        ),
                      ),
                    ),
                  ),
                  // Second Page
                  SingleChildScrollView(
                    child: Form(
                      key: controller.formKey2,
                      child: Padding(
                        padding: const EdgeInsets.only(
                          left: 14.0,
                          right: 14.0,
                          top: 10,
                        ),
                        child: Column(
                          mainAxisAlignment: MainAxisAlignment.start,
                          crossAxisAlignment: CrossAxisAlignment.start,
                          spacing: 10,
                          children: [
                            Text(
                              "More Details",
                              style: TextStyle(
                                  fontSize: fontSized(context, 92),
                                  color: AppColors.black,
                                  fontWeight: FontWeight.bold,
                                  fontFamily: "Inter"),
                            ),
                            addVerticalSpacing(context, 0.05),
                            widget.shippingType == "charter"
                                ? SizedBox.shrink()
                                : GlobalTextField(
                                    fieldName: 'Package Type',
                                    keyBoardType: TextInputType.name,
                                    obscureText: false,
                                    readOnly: true,
                                    textController: controller.itemType,
                                    isEyeVisible: false,
                                    suffix: Icon(Icons.arrow_drop_down),
                                    onChanged: (String? value) {
                                      controller.itemTypeOnChanged(value!);
                                    },
                                    onTap: controller.showBottomSheet,
                                  ),
                            GlobalTextField(
                              fieldName: widget.shippingType == "charter"
                                  ? "List other items here"
                                  : 'Package Description',
                              keyBoardType: TextInputType.multiline,
                              removeSpace: false,
                              obscureText: false,
                              isNotePad: true,
                              maxLength: 200,
                              textController: controller.itemDescription,
                              onChanged: (String? value) {
                                controller.itemDescriptionOnChanged(value!);
                              },
                            ),
                            Padding(
                              padding: const EdgeInsets.all(8.0),
                              child: DottedBorder(
                                borderType: BorderType.RRect,
                                radius: const Radius.circular(12),
                                padding: const EdgeInsets.all(6),
                                child: SizedBox(
                                  height: 150,
                                  child: controller.imageFiles.isNotEmpty
                                      ? SingleChildScrollView(
                                          scrollDirection: Axis.horizontal,
                                          child: SizedBox(
                                            height: 150,
                                            child: Row(
                                              children: List.generate(
                                                  controller.imageFiles.length,
                                                  (index) => Padding(
                                                        padding:
                                                            const EdgeInsets
                                                                .all(8.0),
                                                        child: Stack(
                                                          children: [
                                                            Image.file(
                                                                File(controller
                                                                    .imageFiles[
                                                                        index]
                                                                    .path),
                                                                fit: BoxFit
                                                                    .cover),
                                                            Positioned(
                                                              right: 0,
                                                              child: IconButton(
                                                                icon: const Icon(
                                                                    Icons
                                                                        .remove_circle,
                                                                    color: Colors
                                                                        .red),
                                                                onPressed: () =>
                                                                    controller
                                                                        .removeImage(
                                                                            index),
                                                              ),
                                                            ),
                                                          ],
                                                        ),
                                                      )),
                                            ),
                                          ),
                                        )
                                      : const Center(
                                          child: Text(
                                              'Kindly upload the pictures of the Items')),
                                ),
                              ),
                            ),
                            Row(
                              mainAxisAlignment: MainAxisAlignment.spaceEvenly,
                              children: [
                                ElevatedButton(
                                  onPressed: controller.pickImages,
                                  child: const Text("Add Images"),
                                ),
                                ElevatedButton(
                                  onPressed: controller.imageFiles.isNotEmpty
                                      ? () => controller.clearAllImage()
                                      : null,
                                  child: const Text("Remove All"),
                                ),
                              ],
                            ),
                            GlobalTextField(
                              fieldName: 'Select Hub Here',
                              keyBoardType: TextInputType.name,
                              obscureText: false,
                              readOnly: true,
                              textController: controller.company,
                              isEyeVisible: false,
                              suffix: Icon(Icons.arrow_drop_down),
                              onChanged: (String? value) {},
                              onTap: () {
                                controller.navigateToSelectionPage();
                              },
                            ),
                            addVerticalSpacing(context, 5),
                            controller.widget.shippingType == "Inter state"
                                ? Row(
                                    mainAxisAlignment: MainAxisAlignment.center,
                                    crossAxisAlignment:
                                        CrossAxisAlignment.center,
                                    children: controller.vehiclesInterState
                                        .map((vehicle) {
                                      bool isSelected = controller
                                              .selectedInterStateVehicle ==
                                          vehicle["name"];
                                      return GestureDetector(
                                        onTap: () =>
                                            controller.selectInterStateVehicle(
                                                vehicle["name"]),
                                        child: Container(
                                          padding: EdgeInsets.all(15),
                                          margin: EdgeInsets.symmetric(
                                              horizontal: 10),
                                          decoration: BoxDecoration(
                                            color: isSelected
                                                ? Colors.blue
                                                : Colors.grey[300],
                                            borderRadius:
                                                BorderRadius.circular(25),
                                            border: Border.all(
                                              color: isSelected
                                                  ? Colors.blueAccent
                                                  : Colors.grey,
                                              width: 2,
                                            ),
                                          ),
                                          child: Row(
                                            mainAxisSize: MainAxisSize.min,
                                            spacing: 5,
                                            children: [
                                              Icon(vehicle["icon"],
                                                  size: 15,
                                                  color: isSelected
                                                      ? Colors.white
                                                      : Colors.black),
                                              AppText(
                                                  isBody: true,
                                                  text: "${vehicle["name"]}",
                                                  textAlign: TextAlign.start,
                                                  fontSize: 70,
                                                  color: isSelected
                                                      ? AppColors.white
                                                      : AppColors.black
                                                          .withAlpha(162),
                                                  fontStyle: FontStyle.normal,
                                                  fontWeight: FontWeight.w800),
                                            ],
                                          ),
                                        ),
                                      );
                                    }).toList(),
                                  )
                                : controller.widget.shippingType == "Same State"
                                    ? SingleChildScrollView(
                                        scrollDirection: Axis.horizontal,
                                        child: Row(
                                          mainAxisAlignment:
                                              MainAxisAlignment.center,
                                          children: controller.vehicles
                                              .map((vehicle) {
                                            bool isSelected =
                                                controller.selectedVehicle ==
                                                    vehicle["name"];
                                            return GestureDetector(
                                              onTap: () =>
                                                  controller.selectVehicle(
                                                      vehicle["name"]),
                                              child: Container(
                                                padding: EdgeInsets.all(15),
                                                margin: EdgeInsets.symmetric(
                                                    horizontal: 10),
                                                decoration: BoxDecoration(
                                                  color: isSelected
                                                      ? Colors.blue
                                                      : Colors.grey[300],
                                                  borderRadius:
                                                      BorderRadius.circular(25),
                                                  border: Border.all(
                                                    color: isSelected
                                                        ? Colors.blueAccent
                                                        : Colors.grey,
                                                    width: 2,
                                                  ),
                                                ),
                                                child: Row(
                                                  mainAxisSize:
                                                      MainAxisSize.min,
                                                  spacing: 5,
                                                  children: [
                                                    Icon(vehicle["icon"],
                                                        size: 15,
                                                        color: isSelected
                                                            ? Colors.white
                                                            : Colors.black),
                                                    AppText(
                                                        isBody: true,
                                                        text:
                                                            "${vehicle["name"]}",
                                                        textAlign:
                                                            TextAlign.start,
                                                        fontSize: 70,
                                                        color: isSelected
                                                            ? AppColors.white
                                                            : AppColors.black
                                                                .withAlpha(162),
                                                        fontStyle:
                                                            FontStyle.normal,
                                                        fontWeight:
                                                            FontWeight.w800),
                                                  ],
                                                ),
                                              ),
                                            );
                                          }).toList(),
                                        ),
                                      )
                                    : SizedBox.shrink(),
                            addVerticalSpacing(context, 5),
                            controller.isItemDescriptionChanged.isEmpty ||
                                    controller.isItemTypeChanged.isEmpty
                                ? appButton("Submit", getScreenWidth(context),
                                    () {}, AppColors.grey, controller.isLoading)
                                : appButton("Submit", getScreenWidth(context),
                                    () {
                                    widget.shippingType == "Same State"
                                        ? controller.userRegister()
                                        : controller.widget.shippingType ==
                                                "Inter state"
                                            ? controller.interState()
                                            : controller.otherOnes();
                                  }, AppColors.primary, controller.isLoading),
                            addVerticalSpacing(context, 5),
                          ],
                        ),
                      ),
                    ),
                  ),
                ],
              ),
            ),
          ],
        ),
      ),
    );
  }
}
