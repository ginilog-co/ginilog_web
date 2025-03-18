// ignore_for_file: use_build_context_synchronously, library_private_types_in_public_api

import 'package:ginilog_customer_app/core/components/utils/size_config.dart';
import 'package:ginilog_customer_app/core/components/widgets/back_icon.dart';
import 'package:ginilog_customer_app/core/components/widgets/custom_snackbar.dart';
import 'package:ginilog_customer_app/core/components/widgets/input.dart';
import 'package:ginilog_customer_app/core/features/account/services/account_services.dart';

import '../../../components/utils/colors.dart';
import '../../../components/utils/package_export.dart';
import '../../../components/widgets/app_text.dart';
import '../states/account_provider.dart';

class FeedbackPage extends ConsumerStatefulWidget {
  const FeedbackPage({
    super.key,
  });

  @override
  _FeedbackPageState createState() => _FeedbackPageState();
}

class _FeedbackPageState extends ConsumerState<FeedbackPage> {
  final _formkey = GlobalKey<FormState>();
  late TextEditingController _feedback;
  bool isProcessing = false;
  bool loading = false;
  final _focusFeedback = FocusNode();

  @override
  void initState() {
    _feedback = TextEditingController();
    super.initState();
  }

  @override
  void dispose() {
    _feedback.dispose();
    super.dispose();
  }

  bool _isProcessing = false;

  @override
  Widget build(BuildContext context) {
    double height = MediaQuery.of(context).size.height;
    double width = MediaQuery.of(context).size.width;
    final key = GlobalKey<ScaffoldMessengerState>();
    final accountProviderd = ref.read(accountProvider.notifier);
    accountProviderd.getAccount();
    var user = accountProviderd.userData;

    return Container(
        color: Colors.white,
        height: height,
        width: width,
        child: Scaffold(
          appBar: PreferredSize(
              preferredSize: Size.fromHeight(SizeConfig.heightAdjusted(16)),
              child: Padding(
                padding: EdgeInsets.only(top: SizeConfig.heightAdjusted(10)),
                child: const Column(
                  children: [
                    GlobalBackButton(
                        backText: "Contact Us", showBackButton: true),
                  ],
                ),
              )),
          key: key,
          backgroundColor: Colors.transparent,
          body: GestureDetector(
            onTap: () {
              _focusFeedback.unfocus();
            },
            child: ListView(
              children: <Widget>[
                const SizedBox(
                  height: 10,
                ),
                Image.asset(
                  'assets/images/logo_path.png',
                  height: 150,
                  color: AppColors.primary,
                ),
                Padding(
                  padding: EdgeInsets.only(
                      left: width / 20, right: width / 20, top: 10),
                  child: const AppText(
                      isBody: true,
                      text:
                          "If you are having trouble placing and completing orders, or you have any question or queries, please feel free to email us",
                      textAlign: TextAlign.start,
                      fontSize: 73,
                      color: AppColors.black,
                      fontStyle: FontStyle.normal,
                      fontWeight: FontWeight.bold),
                ),
                Padding(
                  padding: EdgeInsets.only(
                      left: width / 20, right: width / 20, top: 10),
                  child: GestureDetector(
                    onTap: () async {
                      Uri phoneNo = Uri.parse('mailto:info@ginilog.com');
                      if (await launchUrl(phoneNo)) {
                        //dialer opened
                      } else {
                        //dailer is not opened
                      }
                    },
                    child: const AppText(
                        isBody: true,
                        text: "info@ginilog.com",
                        textAlign: TextAlign.start,
                        fontSize: 73,
                        color: AppColors.primary,
                        fontStyle: FontStyle.normal,
                        fontWeight: FontWeight.bold),
                  ),
                ),
                Padding(
                  padding: EdgeInsets.only(
                      left: width / 20, right: width / 20, top: 10),
                  child: GestureDetector(
                    onTap: () async {
                      Uri phoneNo = Uri.parse('tel:08166516944');
                      if (await launchUrl(phoneNo)) {
                        //dialer opened
                      } else {
                        //dailer is not opened
                      }
                    },
                    child: const AppText(
                        isBody: true,
                        text: "0816 651 6944",
                        textAlign: TextAlign.start,
                        fontSize: 73,
                        color: AppColors.primary,
                        fontStyle: FontStyle.normal,
                        fontWeight: FontWeight.bold),
                  ),
                ),
                Padding(
                  padding: EdgeInsets.only(
                      left: width / 20, right: width / 20, top: 10),
                  child: const AppText(
                      isBody: true,
                      text: "a member of our team will attend to you",
                      textAlign: TextAlign.start,
                      fontSize: 73,
                      color: AppColors.black,
                      fontStyle: FontStyle.normal,
                      fontWeight: FontWeight.bold),
                ),
                Container(
                  margin: const EdgeInsets.symmetric(vertical: 0),
                  child: Form(
                    key: _formkey,
                    child: Column(children: [
                      Padding(
                        padding: EdgeInsets.only(
                            left: width / 20, right: width / 20, top: 10),
                        child: const AppText(
                            isBody: false,
                            text: "Leave a message with us",
                            textAlign: TextAlign.start,
                            fontSize: 73,
                            color: AppColors.black,
                            fontStyle: FontStyle.normal,
                            fontWeight: FontWeight.bold),
                      ),
                      Padding(
                        padding:
                            const EdgeInsets.only(top: 10, left: 10, right: 10),
                        child: DescriptionInput(
                          controller: _feedback,
                          hintText: "What Complaints do you have?",
                          labelText: "Complaints",
                          focusNode: _focusFeedback,
                          keyboard: TextInputType.text,
                          maxLength: 200,
                          validator: (value) {
                            if (value!.isEmpty) {
                              return 'Empty Field';
                            }
                            return null;
                          },
                          toggleEye: () {},
                          onSaved: (value) {},
                          onTap: () {},
                          onChanged: (String? value) {},
                        ),
                      ),
                      _isProcessing
                          ? const CircularProgressIndicator()
                          : Padding(
                              padding: EdgeInsets.only(
                                  left: width / 4, right: width / 4, top: 80),
                              child: InkWell(
                                onTap: () async {
                                  _focusFeedback.unfocus();

                                  if (_formkey.currentState!.validate()) {
                                    setState(() {});
                                    debugPrint("Email: ${_feedback.text}");
                                    var response = await AccountService()
                                        .sendFeedBack(
                                            feedback: _feedback.text.trim(),
                                            phoneNo: user!.phoneNo.toString());
                                    setState(() {
                                      _isProcessing = false;
                                    });
                                    if (response.statusCode == 200 ||
                                        response.statusCode == 201) {
                                      showCustomSnackbar(context,
                                          title: "Success",
                                          content:
                                              "Feed back sent Successfully",
                                          type: SnackbarType.error,
                                          isTopPosition: false);

                                      Navigator.pop(context);
                                    } else {
                                      showCustomSnackbar(context,
                                          title: "Error Code",
                                          content: "Error: ${response.body}",
                                          type: SnackbarType.error,
                                          isTopPosition: false);
                                    }
                                  }
                                },
                                child: Container(
                                    alignment: Alignment.center,
                                    decoration: BoxDecoration(
                                      color: AppColors.primary,
                                      borderRadius: BorderRadius.circular(40),
                                    ),
                                    child: const Padding(
                                      padding: EdgeInsets.only(
                                          left: 10,
                                          right: 10,
                                          top: 15,
                                          bottom: 15),
                                      child: AppText(
                                          isBody: true,
                                          text: "Send",
                                          textAlign: TextAlign.start,
                                          fontSize: 73,
                                          color: AppColors.white,
                                          fontStyle: FontStyle.normal,
                                          fontWeight: FontWeight.w600),
                                    )),
                              ),
                            ),
                    ]),
                  ),
                ),
              ],
            ),
          ),
        ));
  }
}
