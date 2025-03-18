// ignore_for_file: use_build_context_synchronously

import 'package:flutter/foundation.dart';
import 'package:ginilog_customer_app/core/components/widgets/custom_snackbar.dart';

import '../../../components/helpers/globals.dart';
import '../../../components/utils/package_export.dart';
import '../services/auth_service.dart';
import '../view/kyc_verification_view.dart';

class KYCVerificationScreen extends ConsumerStatefulWidget {
  const KYCVerificationScreen({
    super.key,
  });
  @override
  ConsumerState<KYCVerificationScreen> createState() =>
      KYCVerificationScreenController();
}

class KYCVerificationScreenController
    extends ConsumerState<KYCVerificationScreen> {
  final GlobalKey<FormState> formKey = GlobalKey<FormState>();
  late TextEditingController phoneNo;
  late TextEditingController dateOfBirth;
  late TextEditingController locationAddress;
  late TextEditingController countryLocation;
  late TextEditingController stateLocation;
  late TextEditingController nextOfKin;
  final FocusNode? phoneNoFocusNode = FocusNode();
  final FocusNode? dateOfBirthFocusNode = FocusNode();
  final FocusNode? locationAddressFocusNode = FocusNode();
  final FocusNode? countryLocationFocusNode = FocusNode();
  final FocusNode? stateLocationFocusNode = FocusNode();
  final FocusNode? nextOfKinFocusNode = FocusNode();

  List<String> languages = [];
  String? sex;
  final String completed = "Yes";
  List<String> sexList = [
    'Male',
    'Female',
  ];

  bool saveButtonPressed = false;
  bool isLoading = false;

  @override
  void initState() {
    super.initState();
    phoneNo = TextEditingController();
    dateOfBirth = TextEditingController();
    locationAddress = TextEditingController();
    countryLocation = TextEditingController();
    stateLocation = TextEditingController();
    nextOfKin = TextEditingController();
  }

  @override
  void dispose() {
    super.dispose();
    phoneNo.dispose();
    dateOfBirth.dispose();
    locationAddress.dispose();
    countryLocation.dispose();
    stateLocation.dispose();
    nextOfKin.dispose();
  }

  bool isChecked = false;
  urlString(String? url) async {
    final link = Uri.parse(url!);
    if (await canLaunchUrl(link)) {
      await launchUrl(link);
    } else {
      throw 'Could not launch $url';
    }
  }

  onChanged(bool? value) {
    setState(() {
      isChecked = value!;
    });
  }

  selectSex(String value) {
    setState(() {
      sex = value;
      if (kDebugMode) {
        print(sex);
      }
    });
  }

  final format = DateFormat("dd MMM, yyyy");

  userRegister() async {
    setState(() {
      saveButtonPressed = true;
    });
    FocusScope.of(context).unfocus();
    if (formKey.currentState!.validate()) {
      formKey.currentState!.save();
      setState(() {
        isLoading = true;
      });
      var response = await AuthService().updateProfileInfo(
        userId: globals.userId.toString(),
        phoneNo: phoneNo.text.trim(),
        address: locationAddress.text.trim(),
        dateOfBirth: dateOfBirth.text.trim(),
        sex: sex.toString(),
        nationality: countryLocation.text.trim(),
        state: stateLocation.text.trim(),
        nextOfKin: nextOfKin.text.trim(),
      );
      if (response.statusCode == 200 || response.statusCode == 201) {
        setState(() {
          isLoading = false;
        });
        // await HomeServices().sendNotification(
        //   title: "KYC Verification",
        //   body: "You have Successfully Done KYC Verification",
        //   notificationType: "KYC Verification",
        // );
        await AuthService().updateDeviceToken();
        //  navigateAndRemoveUntilRoute(context, const ActivateFreeTrialPage());
      } else {
        setState(() {
          isLoading = false;
        });
        showCustomSnackbar(context,
            title: "Error Code",
            content: "Error: ${response.body}",
            type: SnackbarType.error,
            isTopPosition: false);
      }
    }
    setState(() {
      isLoading = false;
    });
  }

  @override
  Widget build(BuildContext context) {
    return KYCVerificationScreenView(this);
  }
}
