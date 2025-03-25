// ignore_for_file: public_member_api_docs, sort_constructors_first
// ignore_for_file: library_private_types_in_public_api, use_build_context_synchronously
import 'dart:io';

import 'package:ginilog_customer_app/core/components/services/upload_service.dart';
import 'package:ginilog_customer_app/core/components/utils/helper_functions.dart';
import 'package:ginilog_customer_app/core/components/utils/size_config.dart';
import 'package:ginilog_customer_app/core/components/widgets/back_icon.dart';
import 'package:ginilog_customer_app/core/features/account/services/account_services.dart';

import '../../../components/utils/app_buttons.dart';
import '../../../components/utils/colors.dart';
import '../../../components/utils/package_export.dart';
import '../../../components/widgets/app_text.dart';
import '../../../components/widgets/input.dart';
import '../model/user_response_model.dart';
import '../states/account_provider.dart';

class AccountDetailsPage extends ConsumerStatefulWidget {
  const AccountDetailsPage({
    super.key,
  });

  @override
  _LoginPageState createState() => _LoginPageState();
}

class _LoginPageState extends ConsumerState<AccountDetailsPage> {
  String selectedCarType = "";
  final _formkey = GlobalKey<FormState>();
  late TextEditingController _phoneNo;
  late TextEditingController _firstName;
  late TextEditingController _lastName;

  bool isProcessing = false;

  bool loading = false;

  late AccountNotifier accountProviders;
  late RegisterResponseModel globals;

  @override
  void initState() {
    _phoneNo = TextEditingController();
    _firstName = TextEditingController();
    _lastName = TextEditingController();

    accountProviders = ref.read(accountProvider.notifier);
    accountProviders.getAccount();
    setState(() {
      globals = accountProviders.userData!;
    });
    userDetails();
    super.initState();
  }

  @override
  void dispose() {
    _phoneNo.dispose();
    _firstName.dispose();
    _lastName.dispose();
    super.dispose();
  }

  userDetails() {
    setState(() {
      _firstName.text = "${globals.firstName}";
      _lastName.text = "${globals.lastName}";
      _phoneNo.text = globals.phoneNo.toString();
    });
  }

  File? pickedCv2;
  String imageFile = "";
  void handleCVUpload2() async {
    try {
      final ImagePicker picker = ImagePicker();
      final XFile? image = await picker.pickImage(source: ImageSource.gallery);
      if (image != null) {
        // String extension = image.path.split(".").last;
        //  String targetPath = "${Directory.systemTemp.path}/temp.$extension";
        // final compressedImage = await FlutterImageCompress.compressAndGetFile(
        //   image.path,
        //   targetPath,
        //   quality: 50,
        // );
        setState(() {
          pickedCv2 = File(image.path.toString());
          imageFile = image.path.toString();
        });
      }
      String imageUrl = await ApiService.upload(image!.path);
      AccountService().updateProfilePics(
          userId: globals.id.toString(), imageFile: imageUrl);
    } catch (e) {
      debugPrint(e.toString());
    }
  }

  Widget buildTop(BuildContext context) {
    return SingleChildScrollView(
      physics: const ScrollPhysics(),
      child: Column(
          mainAxisAlignment: MainAxisAlignment.start,
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Center(
              child: Stack(
                children: [
                  Container(
                      margin: const EdgeInsets.only(left: 0, right: 0, top: 20),
                      child: pickedCv2 == null
                          ? GestureDetector(
                              onTap: () async {
                                handleCVUpload2();
                              },
                              child: CircleAvatar(
                                radius: 50,
                                backgroundColor: Colors.grey,
                                child: CircleAvatar(
                                    backgroundColor: Colors.white,
                                    radius: 47,
                                    backgroundImage: globals.profilePicture
                                            .toString()
                                            .isEmpty
                                        ? const NetworkImage(
                                            "https://upload.wikimedia.org/wikipedia/commons/thumb/d/d3/Microsoft_Account.svg/512px-Microsoft_Account.svg.png?20170218203212")
                                        // ignore: unnecessary_null_comparison
                                        : globals.profilePicture == null
                                            ? const NetworkImage(
                                                "https://upload.wikimedia.org/wikipedia/commons/thumb/d/d3/Microsoft_Account.svg/512px-Microsoft_Account.svg.png?20170218203212")
                                            : NetworkImage(globals
                                                .profilePicture
                                                .toString())),
                              ),
                            )
                          : Center(
                              child: Stack(
                                children: [
                                  GestureDetector(
                                    onTap: () async {
                                      handleCVUpload2();
                                    },
                                    child: Container(
                                      margin: const EdgeInsets.only(
                                          left: 0, right: 0),
                                      child: CircleAvatar(
                                        radius: 50,
                                        backgroundColor: Colors.grey,
                                        child: CircleAvatar(
                                          backgroundColor: Colors.white,
                                          radius: 47,
                                          backgroundImage:
                                              FileImage(pickedCv2!),
                                        ),
                                      ),
                                    ),
                                  ),
                                ],
                              ),
                            )),
                  const Positioned(
                      top: 96,
                      left: 50,
                      right: 20,
                      child: Icon(Icons.camera_alt,
                          color: AppColors.primary, size: 30)),
                ],
              ),
            ),
            addVerticalSpacing(context, 2),
            ListTile(
              title: const AppText(
                  isBody: false,
                  text: "Email",
                  textAlign: TextAlign.start,
                  fontSize: 33,
                  color: AppColors.black,
                  fontStyle: FontStyle.normal,
                  fontWeight: FontWeight.bold),
              subtitle: AppText(
                  isBody: true,
                  text: "${globals.email}",
                  textAlign: TextAlign.start,
                  fontSize: 27,
                  color: AppColors.black,
                  fontStyle: FontStyle.normal,
                  fontWeight: FontWeight.w600),
              onTap: () {},
            ),
            ListTile(
              title: const AppText(
                  isBody: false,
                  text: "Phone Number",
                  textAlign: TextAlign.start,
                  fontSize: 33,
                  color: AppColors.black,
                  fontStyle: FontStyle.normal,
                  fontWeight: FontWeight.bold),
              subtitle: AppText(
                  isBody: true,
                  text: "${globals.phoneNo}",
                  textAlign: TextAlign.start,
                  fontSize: 27,
                  color: AppColors.black,
                  fontStyle: FontStyle.normal,
                  fontWeight: FontWeight.w600),
              onTap: () {},
            ),
            Padding(
              padding: const EdgeInsets.only(left: 0, right: 0),
              child: ExpansionTile(
                backgroundColor: AppColors.white,
                collapsedBackgroundColor: AppColors.white,
                collapsedIconColor: AppColors.primary,
                iconColor: AppColors.primary,
                textColor: AppColors.primary,
                collapsedTextColor: AppColors.white,
                title: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    const AppText(
                        isBody: false,
                        text: "Name",
                        textAlign: TextAlign.start,
                        fontSize: 33,
                        color: AppColors.black,
                        fontStyle: FontStyle.normal,
                        fontWeight: FontWeight.bold),
                    AppText(
                        isBody: true,
                        text: "${globals.firstName} ${globals.lastName}",
                        textAlign: TextAlign.start,
                        fontSize: 27,
                        color: AppColors.black,
                        fontStyle: FontStyle.normal,
                        fontWeight: FontWeight.w600),
                  ],
                ),
                children: [
                  Container(
                    color: AppColors.white,
                    padding: const EdgeInsets.all(20),
                    width: double.infinity,
                    child: Column(
                      children: [
                        Form(
                          key: _formkey,
                          child: Column(
                            crossAxisAlignment: CrossAxisAlignment.start,
                            children: [
                              GlobalTextField(
                                fieldName: 'First Name',
                                keyBoardType: TextInputType.name,
                                removeSpace: false,
                                obscureText: false,
                                textController: _firstName,
                                onChanged: (String? value) {},
                              ),
                              addVerticalSpacing(context, 5),
                              GlobalTextField(
                                fieldName: 'Last Name',
                                keyBoardType: TextInputType.name,
                                removeSpace: false,
                                obscureText: false,
                                textController: _lastName,
                                onChanged: (String? value) {},
                              ),
                            ],
                          ),
                        ),
                        addVerticalSpacing(context, 3),
                        Padding(
                          padding:
                              const EdgeInsets.only(left: 35.0, right: 35.0),
                          child: appButton2(
                              onPressed: () async {
                                if (_formkey.currentState!.validate()) {
                                  accountProviders.updateNames(
                                    context: context,
                                    userId: globals.id.toString(),
                                    firstName: _firstName.text.trim(),
                                    lastName: _lastName.text.trim(),
                                  );
                                  Navigator.pop(context);
                                }
                              },
                              child: const AppText(
                                  isBody: true,
                                  text: "Update",
                                  textAlign: TextAlign.center,
                                  fontSize: 27,
                                  color: AppColors.white,
                                  fontStyle: FontStyle.normal,
                                  fontWeight: FontWeight.w600)),
                        ),
                      ],
                    ),
                  )
                ],
              ),
            ),
          ]),
    );
  }

  @override
  Widget build(BuildContext context) {
    return Container(
        height: MediaQuery.of(context).size.height,
        width: MediaQuery.of(context).size.width,
        color: Colors.white,
        child: Scaffold(
          backgroundColor: AppColors.white,
          appBar: PreferredSize(
              preferredSize: Size.fromHeight(SizeConfig.heightAdjusted(12)),
              child: Padding(
                padding: EdgeInsets.only(top: SizeConfig.heightAdjusted(10)),
                child: const Column(
                  children: [
                    GlobalBackButton(
                        backText: 'Profile Updates', showBackButton: true),
                  ],
                ),
              )),
          body: buildTop(context),
        ));
  }
}
