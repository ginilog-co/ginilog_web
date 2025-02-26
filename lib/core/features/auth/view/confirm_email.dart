import 'package:flutter/material.dart';
import 'package:flutter_spinkit/flutter_spinkit.dart';
import 'package:pinput/pinput.dart';
import 'package:stop_watch_timer/stop_watch_timer.dart';

import '../../../components/architecture/mvc.dart';
import '../../../components/helpers/globals.dart';
import '../../../components/utils/app_buttons.dart';
import '../../../components/utils/colors.dart';
import '../../../components/utils/helper_functions.dart';
import '../../../components/widgets/app_text.dart';
import '../controller/confirm_email.dart';

class ConfirmEmailAddressView extends StatelessView<ConfirmEmailAddressScreen,
    ConfirmEmailAddressController> {
  const ConfirmEmailAddressView(super.state, {super.key});

  @override
  Widget build(BuildContext context) {
    return Scaffold(
        backgroundColor: AppColors.white,
        body: SingleChildScrollView(
          child: SafeArea(
            child: Padding(
              padding: const EdgeInsets.only(
                  right: 24.0, bottom: 24, left: 24, top: 100),
              child: Column(
                children: [
                  Image.asset(
                    "assets/images/emailIcon.png",
                    height: 150,
                    width: 150,
                  ),
                  const AppText(
                      isBody: false,
                      text: "Check your email",
                      textAlign: TextAlign.start,
                      fontSize: 10,
                      color: AppColors.black,
                      fontStyle: FontStyle.normal,
                      fontWeight: FontWeight.bold),
                  const SizedBox(height: 8),
                  RichText(
                    textAlign: TextAlign.center,
                    text: TextSpan(
                      style: const TextStyle(color: Colors.black),
                      children: [
                        TextSpan(
                          text: "we’ve sent the code to \n",
                          style: TextStyle(
                            color: Colors.black,
                            fontWeight: FontWeight.w400,
                            fontSize: fontSized(context, 18),
                            fontFamily: "Montserrat",
                          ),
                        ),
                        TextSpan(
                          text: widget.fromLogin == false
                              ? widget.email
                              : widget.email,
                          style: TextStyle(
                            color: AppColors.primary,
                            fontWeight: FontWeight.w400,
                            fontSize: fontSized(context, 22),
                            fontFamily: "Montserrat",
                          ),
                        ),
                      ],
                    ),
                  ),

                  //const SizedBox(height: 8),

                  const SizedBox(height: 60),
                  Pinput(
                    length: 5,
                    controller: controller.pinPutController,
                    onCompleted: (value) => controller.submit(),
                    onChanged: (value) {
                      controller.getPin(value);
                    },
                  ),
                  const SizedBox(height: 32),
                  IgnorePointer(
                    ignoring:
                        controller.pin.isEmpty || controller.pin.length < 5,
                    child: controller.isLoading
                        ? const SpinKitThreeBounce(
                            color: AppColors.primary,
                            size: 16,
                          )
                        : appButtons(
                            "VERIFY MAIL",
                            getScreenWidth(context),
                            () {
                              controller.submit();
                            },
                            controller.pin.isEmpty || controller.pin.length < 5
                                ? AppColors.grey
                                : AppColors.primary,
                          ),
                  ),

                  const SizedBox(height: 50),
                  StreamBuilder<int>(
                    stream: globals.stopWatchTimer!.rawTime,
                    initialData: 0,
                    builder: (context, snap) {
                      final value = snap.data;
                      final displayTime = StopWatchTimer.getDisplayTime(
                        value!,
                        milliSecond: false,
                        minute: true,
                        hours: false,
                      );
                      return Text(
                        'Code expires in: $displayTime',
                        style: TextStyle(
                          color: globals.stopWatchTimer!.isRunning
                              ? AppColors.primary
                              : AppColors.white,
                        ),
                      );
                    },
                  ),
                  addVerticalSpacing(context, 50),

                  appButton2(
                    height: 55,
                    borderRadius: const BorderRadius.all(Radius.circular(20)),
                    onPressed: () async {
                      globals.stopWatchTimer!.isRunning
                          ? null
                          : controller.resendCode();
                    },
                    btnColor: AppColors.white,
                    border: Border.all(color: AppColors.primary, width: 1),
                    child: const AppText(
                        isBody: true,
                        text: "Send again",
                        textAlign: TextAlign.center,
                        fontSize: 20,
                        color: AppColors.primary,
                        fontStyle: FontStyle.normal,
                        fontWeight: FontWeight.w600),
                  )
                ],
              ),
            ),
          ),
        ));
  }
}
