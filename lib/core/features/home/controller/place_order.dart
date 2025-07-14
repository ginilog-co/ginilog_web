// ignore_for_file: use_build_context_synchronously

import 'dart:convert';

import 'package:ginilog_customer_app/core/components/services/upload_service.dart';
import 'package:ginilog_customer_app/core/components/utils/colors.dart';
import 'package:ginilog_customer_app/core/components/utils/constants.dart';
import 'package:ginilog_customer_app/core/components/utils/helper_functions.dart';
import 'package:ginilog_customer_app/core/components/utils/size_config.dart';
import 'package:ginilog_customer_app/core/components/widgets/custom_snackbar.dart';
import 'package:ginilog_customer_app/core/features/account/model/user_response_model.dart';
import 'package:ginilog_customer_app/core/features/account/states/account_provider.dart';
import 'package:ginilog_customer_app/core/features/home/model/company_response_model.dart';
import 'package:ginilog_customer_app/core/features/home/services/home_service.dart';
import 'package:ginilog_customer_app/core/features/home/states/home_state.dart';
import 'package:ginilog_customer_app/core/features/home/view/company_select_page.dart';
import 'package:ginilog_customer_app/core/features/home/view/place_order.dart';
import 'package:ginilog_customer_app/core/features/order_history/model/package_orders_model.dart';
import 'package:ginilog_customer_app/core/features/order_history/services/package_order_service.dart';
import 'package:ginilog_customer_app/core/features/order_history/view/package_information.dart';
import 'package:google_places_autocomplete_text_field/google_places_autocomplete_text_field.dart';
import '../../../components/utils/package_export.dart';

class PlaceOrderScreen extends ConsumerStatefulWidget {
  const PlaceOrderScreen({
    super.key,
    required this.shippingType,
    required this.userPhone,
  });
  final String shippingType;
  final String userPhone;
  @override
  ConsumerState<PlaceOrderScreen> createState() => PlaceOrderScreenController();
}

class PlaceOrderScreenController extends ConsumerState<PlaceOrderScreen> {
  final GlobalKey<FormState> formKey = GlobalKey<FormState>();
  final GlobalKey<FormState> formKey2 = GlobalKey<FormState>();
  PageController pageController = PageController();
  int currentPage = 0;
  // Origin
  late TextEditingController senderName;
  late TextEditingController senderEmail;
  late TextEditingController originAddress;
  late TextEditingController originPhoneNoTec;
  late TextEditingController originPostcodes;
  late TextEditingController originStateController;
  late TextEditingController originCityController;
  double originLatitude = 0.0;
  double originLongitude = 0.0;
  final focusOriginAddress = FocusNode();
  final FocusNode originPhoneNoFocusNode = FocusNode();
  String originCountry = "";

  // Destination
  late TextEditingController receiverName;
  late TextEditingController receiverEmail;
  late TextEditingController destinationAddress;
  late TextEditingController destinationPhoneNoTec;
  late TextEditingController destinationPostcodes;
  late TextEditingController destinationStateController;
  late TextEditingController destinationCityController;
  double destinationLatitude = 0.0;
  double destinationLongitude = 0.0;
  final focusDestinationAddress = FocusNode();
  final FocusNode destinationPhoneNoFocusNode = FocusNode();
  String destinationCountry = "";

  // Package Details
  late TextEditingController packageName;
  late TextEditingController itemCost;
  late TextEditingController itemQuantity;
  late TextEditingController itemWeight;
  late TextEditingController itemModelNumber;
  late TextEditingController itemDescription;
  bool saveButtonPressed = false;
  bool isLoading = false;

  TextEditingController itemType = TextEditingController();
  final TextEditingController _searchController = TextEditingController();

  List<String> allItems = [
    "Electronics",
    "Documents",
    "Clothing",
    "Foodstuff",
    "Others",
  ];
  List<String> filteredItems = [];
  // Company
  TextEditingController company = TextEditingController();
  String companyId = "";
  late AccountNotifier accountProviders;
  late RegisterResponseModel globals;

  @override
  void initState() {
    super.initState();
    final station = ref.read(homeProvider.notifier);
    station.getAllLogisticsData();

    senderName = TextEditingController();
    senderEmail = TextEditingController();
    originAddress = TextEditingController();
    originPhoneNoTec = TextEditingController();
    originPostcodes = TextEditingController();
    originStateController = TextEditingController();
    originCityController = TextEditingController();
    //Destination
    receiverName = TextEditingController();
    receiverEmail = TextEditingController();
    destinationAddress = TextEditingController();
    destinationPhoneNoTec = TextEditingController();
    destinationPostcodes = TextEditingController();
    destinationCityController = TextEditingController();
    destinationStateController = TextEditingController();
    // Package Details
    packageName = TextEditingController();
    itemCost = TextEditingController();
    itemModelNumber = TextEditingController();
    itemQuantity = TextEditingController();
    itemWeight = TextEditingController();
    itemDescription = TextEditingController();
    accountProviders = ref.read(accountProvider.notifier);
    accountProviders.getAccount();
    setState(() {
      globals = accountProviders.userData!;
    });
    userDetails();
  }

  @override
  void dispose() {
    super.dispose();
    senderName.dispose();
    senderEmail.dispose();
    originAddress.dispose();
    originPhoneNoTec.dispose();
    originPostcodes.dispose();
    originCityController.dispose();
    originStateController.dispose();
    //Destination
    receiverName.dispose();
    receiverEmail.dispose();
    destinationAddress.dispose();
    destinationPhoneNoTec.dispose();
    destinationPostcodes.dispose();
    destinationCityController.dispose();
    destinationStateController.dispose();
    // Package Details
    packageName.dispose();
    itemCost.dispose();
    itemDescription.dispose();
    itemModelNumber.dispose();
    itemQuantity.dispose();
    itemWeight.dispose();
  }

  userDetails() {
    setState(() {
      senderEmail.text = globals.email.toString();
      isOriginEmailChanged = globals.email.toString();
      senderName.text = "${globals.firstName} ${globals.lastName}";
      isOriginNameChanged = "${globals.firstName} ${globals.lastName}";
      originPhoneNoTec.text = globals.phoneNo.toString();
      isOriginPhoneNoChanged = globals.phoneNo.toString();
      isItemTypeChanged =
          widget.shippingType.toLowerCase() == "charter".toLowerCase()
              ? "charter"
              : "";
    });
  }

  pageChanged(int index) {
    setState(() {
      currentPage = index;
    });
  }

  //Origin
  String isOriginNameChanged = "";
  String isOriginEmailChanged = "";
  String isOriginAddressChanged = "";
  String isOriginPhoneNoChanged = "";

  originNameOnChanged(String value) {
    setState(() {
      isOriginNameChanged = value;
    });
  }

  originEmailOnChanged(String value) {
    setState(() {
      isOriginEmailChanged = value;
    });
  }

  originAddressOnChanged(String value) {
    setState(() {
      isOriginAddressChanged = value;
    });
  }

  originPhoneNoOnChanged(String value) {
    setState(() {
      isOriginPhoneNoChanged = value;
    });
  }

  //Destination
  String isDestinationNameChanged = "";
  String isDestinationEmailChanged = "";
  String isDestinationAddressChanged = "";
  String isDestinationPhoneNoChanged = "";

  destinationNameOnChanged(String value) {
    setState(() {
      isDestinationNameChanged = value;
    });
  }

  destinationEmailOnChanged(String value) {
    setState(() {
      isDestinationEmailChanged = value;
    });
  }

  destinationAddressOnChanged(String value) {
    setState(() {
      isDestinationAddressChanged = value;
    });
  }

  destinationPhoneNoOnChanged(String value) {
    setState(() {
      isDestinationPhoneNoChanged = value;
    });
  }

  // Package Details
  String isPackageNameChanged = "";

  packageNameOnChanged(String value) {
    setState(() {
      isPackageNameChanged = value;
    });
  }

  String isItemQuantityChanged = "";
  itemQuantityOnChanged(String value) {
    setState(() {
      isItemQuantityChanged = value;
    });
  }

  String isItemCostChanged = "";
  itemCostOnChanged(String value) {
    setState(() {
      isItemCostChanged = value;
    });
  }

  String isItemWeightChanged = "";
  itemWeightOnChanged(String value) {
    setState(() {
      isItemWeightChanged = value;
    });
  }

  String isItemDescriptionChanged = "";
  itemDescriptionOnChanged(String value) {
    setState(() {
      isItemDescriptionChanged = value;
    });
  }

  String isItemModelNumberChanged = "";
  itemModelNumberOnChanged(String value) {
    setState(() {
      isItemModelNumberChanged = value;
    });
  }

  String isItemTypeChanged = "";
  itemTypeOnChanged(String value) {
    setState(() {
      isItemTypeChanged = value;
    });
    printData("Item Type", isItemTypeChanged);
  }

  void showBottomSheet() {
    _searchController.clear(); // Clear search field when opening
    setState(() {
      filteredItems = List.from(allItems);
    });

    showModalBottomSheet(
      backgroundColor: AppColors.white,
      context: context,
      isScrollControlled: true, // Makes BottomSheet expandable
      builder: (context) {
        return StatefulBuilder(
          builder: (context, setState) {
            return Padding(
              padding: EdgeInsets.only(
                bottom:
                    MediaQuery.of(
                      context,
                    ).viewInsets.bottom, // Adjust for keyboard
              ),
              child: Container(
                padding: const EdgeInsets.all(16),
                height: 400,
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    // Search Field
                    TextField(
                      controller: _searchController,
                      decoration: InputDecoration(
                        labelText: "Search",
                        prefixIcon: Icon(Icons.search),
                        border: OutlineInputBorder(),
                      ),
                      onChanged: (value) {
                        setState(() {
                          filteredItems =
                              allItems
                                  .where(
                                    (item) => item.toLowerCase().contains(
                                      value.toLowerCase(),
                                    ),
                                  )
                                  .toList();
                        });
                      },
                    ),
                    const SizedBox(height: 10),
                    Expanded(
                      child:
                          filteredItems.isEmpty
                              ? Center(child: Text("No results found"))
                              : ListView.builder(
                                itemCount: filteredItems.length,
                                itemBuilder: (context, index) {
                                  return ListTile(
                                    title: Text(
                                      filteredItems[index],

                                      style: TextStyle(
                                        fontSize: 18.textSize,
                                        color: AppColors.black,
                                        fontWeight: FontWeight.bold,
                                        fontFamily: "Inter",
                                      ),
                                    ),
                                    onTap: () {
                                      setState(() {
                                        itemType.text = filteredItems[index];
                                        isItemTypeChanged =
                                            filteredItems[index];
                                      });
                                      printData("Item Type", isItemTypeChanged);
                                      Navigator.pop(context);
                                    },
                                  );
                                },
                              ),
                    ),
                  ],
                ),
              ),
            );
          },
        );
      },
    );
  }

  // Select Vehicle
  String selectedVehicle = "Motorcycle"; // Default selection

  final List<Map<String, dynamic>> vehicles = [
    {"name": "Motorcycle", "icon": Icons.motorcycle},
    {"name": "Van", "icon": Icons.directions_bus},
    {"name": "Truck", "icon": Icons.local_shipping},
  ];

  void selectVehicle(String vehicleName) {
    setState(() {
      selectedVehicle = vehicleName;
    });
  }

  // Inter State Vehicle
  // Select Vehicle
  String selectedInterStateVehicle = "Van"; // Default selection

  final List<Map<String, dynamic>> vehiclesInterState = [
    {"name": "Van", "icon": Icons.directions_bus},
    {"name": "Truck", "icon": Icons.local_shipping},
  ];

  void selectInterStateVehicle(String vehicleName) {
    setState(() {
      selectedInterStateVehicle = vehicleName;
    });
  }

  origin(Prediction placeId) async {
    Map<String, dynamic>? placeDetails = await HomeService().getPlaceDetails(
      placeId.placeId!,
    );
    if (placeDetails != null) {
      setState(() {
        originAddress.text = placeId.description.toString();
        originLatitude = double.parse(placeId.lat.toString());
        originLongitude = double.parse(placeId.lng.toString());
        for (var component in placeDetails['address_components']) {
          List types = component['types'];

          if (types.contains('country')) {
            // Country
            originCountry = component['long_name'];
          }
          if (types.contains('administrative_area_level_1')) {
            // state
            originStateController.text = component['long_name'];
          }
          if (types.contains('administrative_area_level_2')) {
            // Local Govt
            originCityController.text = component['long_name'];
          }
          if (types.contains('postal_code')) {
            // Postal Code
            originPostcodes.text = component['long_name'];
          }
        }
      });
    }
  }

  destination(Prediction placeId) async {
    Map<String, dynamic>? placeDetails = await HomeService().getPlaceDetails(
      placeId.placeId!,
    );
    if (placeDetails != null) {
      setState(() {
        destinationAddress.text = placeId.description.toString();
        destinationLatitude = double.parse(placeId.lat.toString());
        destinationLongitude = double.parse(placeId.lng.toString());
        for (var component in placeDetails['address_components']) {
          List types = component['types'];

          if (types.contains('country')) {
            // Country
            destinationCountry = component['long_name'];
          }
          if (types.contains('administrative_area_level_1')) {
            // state
            destinationStateController.text = component['long_name'];
          }
          if (types.contains('administrative_area_level_2')) {
            // Local Govt
            destinationCityController.text = component['long_name'];
          }
          if (types.contains('postal_code')) {
            // Postal Code
            destinationPostcodes.text = component['long_name'];
          }
        }
      });
    }
  }

  // Images of the item
  final List<XFile> imageFiles = [];
  final List<String> imageUrls = [];
  final List<String> _imageCompressed = [];
  XFile? compressedImage;
  int maxImages = 4;
  Future<void> pickImages() async {
    if (imageFiles.length >= maxImages) {
      showCustomSnackbar(
        context,
        title: "Image Select",
        content: "You can only select a maximum of $maxImages images",
        type: SnackbarType.error,
        isTopPosition: false,
      );
      return;
    }

    final ImagePicker picker = ImagePicker();
    final List<XFile> pickedFile = await picker.pickMultiImage(limit: 4);

    for (var i = 0; i < pickedFile.length; i++) {
      // Compress the image
      setState(() {
        imageFiles.add(pickedFile[i]);
        _imageCompressed.add(pickedFile[i].path);
        compressedImage = pickedFile[i];
        maxImages = pickedFile.length;
      });
      String imageUrl = await ApiService.upload(compressedImage!.path);
      setState(() {
        imageUrls.add(imageUrl);
      });

      printData("Image", imageUrls);
    }
  }

  void removeImage(int index) {
    setState(() {
      imageFiles.removeAt(index);
      imageUrls.removeAt(index);
    });
  }

  clearAllImage() => setState(() => imageFiles.clear());

  userRegister() async {
    setState(() {
      saveButtonPressed = true;
    });
    FocusScope.of(context).unfocus();
    if (formKey2.currentState!.validate()) {
      formKey2.currentState!.save();
      setState(() {
        isLoading = true;
      });

      if (destinationStateController.text == originStateController.text) {
        final response = await PackageOrderService().createOrderWithAddress(
          companyId: companyId,
          // Item
          itemName: packageName.text.trim(),
          itemDescription: itemDescription.text.trim(),
          itemCost: num.parse(itemCost.text.trim()),
          itemModelNumber: itemModelNumber.text.trim(),
          itemQuantity: int.parse(itemQuantity.text.trim()),
          itemWeight: num.parse(itemWeight.text.trim()),
          packageType: itemType.text.trim(),
          riderType: selectedVehicle,
          shippingType: widget.shippingType,
          packageImageLists: imageUrls,
          // Sender
          senderName: senderName.text.trim(),
          senderPhoneNo: originPhoneNoTec.text.trim(),
          senderEmail: senderEmail.text.trim(),
          senderAddress: originAddress.text.trim(),
          senderState: originStateController.text.trim(),
          senderLocality: originCityController.text.trim(),
          senderPostalCode: originPostcodes.text.trim(),
          senderLatitude: originLatitude,
          senderLongitude: originLongitude,
          senderCountry: originCountry,
          // Receiver
          recieverName: receiverName.text.trim(),
          recieverEmail: receiverEmail.text.trim(),
          recieverPhoneNo: destinationPhoneNoTec.text.trim(),
          recieverAddress: destinationAddress.text.trim(),
          recieverState: destinationStateController.text.trim(),
          recieverLocality: destinationCityController.text.trim(),
          recieverPostalCode: destinationPostcodes.text.trim(),
          recieverLatitude: destinationLatitude,
          recieverLongitude: destinationLongitude,
          recieverCountry: destinationCountry,
        );
        if (response.statusCode == 200 || response.statusCode == 201) {
          final item = json.decode(response.body);
          var data = PackageOrderResponseModel.fromJson(item);
          setState(() {
            isLoading = false;
          });
          showCustomSnackbar(
            context,
            title: "Package Order",
            content: "Order Created Successfully",
            type: SnackbarType.success,
            isTopPosition: false,
          );
          navigateToRoute(
            context,
            PackageInformationPage(order: data, userPhone: widget.userPhone),
          );
        } else {
          setState(() {
            isLoading = false;
          });
          showCustomSnackbar(
            context,
            title: "Package Order",
            content: "Error in Creating The Order ${response.body}",
            type: SnackbarType.error,
            isTopPosition: false,
          );
        }
      } else {
        setState(() {
          isLoading = false;
        });
        showCustomSnackbar(
          context,
          title: "Same State Order",
          content: "Choose the same State and place the order",
          type: SnackbarType.error,
          isTopPosition: false,
        );
      }
      setState(() {
        isLoading = false;
      });
    }
  }

  interState() async {
    setState(() {
      saveButtonPressed = true;
    });
    FocusScope.of(context).unfocus();
    if (formKey2.currentState!.validate()) {
      formKey2.currentState!.save();
      setState(() {
        isLoading = true;
      });

      final response = await PackageOrderService().createOrderWithAddress(
        companyId: companyId,
        // Item
        itemName: packageName.text.trim(),
        itemDescription: itemDescription.text.trim(),
        itemCost: num.parse(itemCost.text.trim()),
        itemModelNumber: itemModelNumber.text.trim(),
        itemQuantity: int.parse(itemQuantity.text.trim()),
        itemWeight: num.parse(itemWeight.text.trim()),
        packageType: itemType.text.trim(),
        riderType: selectedInterStateVehicle,
        shippingType: widget.shippingType,
        packageImageLists: imageUrls,
        // Sender
        senderName: senderName.text.trim(),
        senderPhoneNo: originPhoneNoTec.text.trim(),
        senderEmail: senderEmail.text.trim(),
        senderAddress: originAddress.text.trim(),
        senderState: originStateController.text.trim(),
        senderLocality: originCityController.text.trim(),
        senderPostalCode: originPostcodes.text.trim(),
        senderLatitude: originLatitude,
        senderLongitude: originLongitude,
        senderCountry: originCountry,
        // Receiver
        recieverName: receiverName.text.trim(),
        recieverEmail: receiverEmail.text.trim(),
        recieverPhoneNo: destinationPhoneNoTec.text.trim(),
        recieverAddress: destinationAddress.text.trim(),
        recieverState: destinationStateController.text.trim(),
        recieverLocality: destinationCityController.text.trim(),
        recieverPostalCode: destinationPostcodes.text.trim(),
        recieverLatitude: destinationLatitude,
        recieverLongitude: destinationLongitude,
        recieverCountry: destinationCountry,
      );
      if (response.statusCode == 200 || response.statusCode == 201) {
        final item = json.decode(response.body);
        var data = PackageOrderResponseModel.fromJson(item);
        setState(() {
          isLoading = false;
        });
        showCustomSnackbar(
          context,
          title: "Package Order",
          content: "Order Created Successfully",
          type: SnackbarType.success,
          isTopPosition: false,
        );
        navigateToRoute(
          context,
          PackageInformationPage(order: data, userPhone: widget.userPhone),
        );
      } else {
        setState(() {
          isLoading = false;
        });
        showCustomSnackbar(
          context,
          title: "Package Order",
          content: "Error in Creating The Order ${response.body}",
          type: SnackbarType.error,
          isTopPosition: false,
        );
      }

      setState(() {
        isLoading = false;
      });
    }
  }

  otherOnes() async {
    setState(() {
      saveButtonPressed = true;
    });
    FocusScope.of(context).unfocus();
    if (formKey2.currentState!.validate()) {
      formKey2.currentState!.save();
      setState(() {
        isLoading = true;
      });

      final response = await PackageOrderService().createOrderWithAddress(
        companyId: companyId,
        // Item
        itemName: packageName.text.trim(),
        itemDescription: itemDescription.text.trim(),
        itemCost: num.parse(itemCost.text.trim()),
        itemModelNumber: itemModelNumber.text.trim(),
        itemQuantity: int.parse(itemQuantity.text.trim()),
        itemWeight: num.parse(itemWeight.text.trim()),
        packageType:
            widget.shippingType.toLowerCase() == "charter".toLowerCase()
                ? "Charter"
                : itemType.text.trim(),
        riderType:
            widget.shippingType.toLowerCase() == "charter".toString()
                ? "Pickup"
                : "Plane Flight",
        shippingType: widget.shippingType,
        packageImageLists: imageUrls,
        // Sender
        senderName: senderName.text.trim(),
        senderPhoneNo: originPhoneNoTec.text.trim(),
        senderEmail: senderEmail.text.trim(),
        senderAddress: originAddress.text.trim(),
        senderState: originStateController.text.trim(),
        senderLocality: originCityController.text.trim(),
        senderPostalCode: originPostcodes.text.trim(),
        senderLatitude: originLatitude,
        senderLongitude: originLongitude,
        senderCountry: originCountry,
        // Receiver
        recieverName: receiverName.text.trim(),
        recieverEmail: receiverEmail.text.trim(),
        recieverPhoneNo: destinationPhoneNoTec.text.trim(),
        recieverAddress: destinationAddress.text.trim(),
        recieverState: destinationStateController.text.trim(),
        recieverLocality: destinationCityController.text.trim(),
        recieverPostalCode: destinationPostcodes.text.trim(),
        recieverLatitude: destinationLatitude,
        recieverLongitude: destinationLongitude,
        recieverCountry: destinationCountry,
      );
      if (response.statusCode == 200 || response.statusCode == 201) {
        final item = json.decode(response.body);
        var data = PackageOrderResponseModel.fromJson(item);
        setState(() {
          isLoading = false;
        });
        showCustomSnackbar(
          context,
          title: "Package Order",
          content: "Order Created Successfully",
          type: SnackbarType.success,
          isTopPosition: false,
        );
        navigateToRoute(
          context,
          PackageInformationPage(order: data, userPhone: widget.userPhone),
        );
      } else {
        setState(() {
          isLoading = false;
        });
        showCustomSnackbar(
          context,
          title: "Package Order",
          content: "Error in Creating The Order ${response.body}",
          type: SnackbarType.error,
          isTopPosition: false,
        );
      }

      setState(() {
        isLoading = false;
      });
    }
  }

  @override
  Widget build(BuildContext context) {
    return PlaceOrderScreenView(this);
  }

  LogisticResponseModel logisticData = LogisticResponseModel();
  void navigateToSelectionPage() async {
    final result = await Navigator.push(
      context,
      MaterialPageRoute(
        builder:
            (context) =>
                CompanySelectPage(isSelectionType: widget.shippingType),
      ),
    );

    if (result != null) {
      setState(() {
        logisticData = result; // Update selected item
        company.text = logisticData.companyName.toString();
        companyId = logisticData.id.toString();
      });
    }
    printData("Logistic", logisticData.companyName);
  }
}
