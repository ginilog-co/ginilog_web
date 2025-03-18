// ignore_for_file: use_build_context_synchronously

import 'package:ginilog_customer_app/core/components/state/connectivity_state.dart';
import 'package:ginilog_customer_app/core/components/utils/constants.dart';
import 'package:ginilog_customer_app/core/components/utils/package_export.dart';
import 'package:ginilog_customer_app/core/components/widgets/custom_snackbar.dart';
import 'package:ginilog_customer_app/core/features/account/model/user_response_model.dart';
import 'package:ginilog_customer_app/core/features/account/states/account_provider.dart';
import 'package:ginilog_customer_app/core/features/bookings/view/accommodation/book_reservation.dart';
import 'package:ginilog_customer_app/core/features/bookings/widget/booking_payment.dart';

class BookReservationScreen extends ConsumerStatefulWidget {
  const BookReservationScreen(
      {super.key,
      required this.reservationId,
      required this.bookingPrice,
      required this.maximumNoOfGuest});
  final String reservationId;
  final num bookingPrice;
  final int maximumNoOfGuest;
  @override
  ConsumerState<BookReservationScreen> createState() =>
      BookReservationScreenController();
}

class BookReservationScreenController
    extends ConsumerState<BookReservationScreen> {
  final GlobalKey<FormState> formKey = GlobalKey<FormState>();

  late TextEditingController email;
  late TextEditingController fistNameTec;
  late TextEditingController lastNameTEC;
  late TextEditingController phoneNo;
  late TextEditingController numberOfGuest;
  late TextEditingController comment;
  late TextEditingController reservationStartDate;
  late TextEditingController reservationEndDate;
  int noOfDays = 0;
  final focusEmail = FocusNode();
  late FocusNode firstNameFocusNode = FocusNode();
  late FocusNode lastNameFocusNode = FocusNode();
  final focusPhoneNo = FocusNode();

  bool isLoading = false;
  final format = DateFormat("dd MMM, yyyy hh:mm a");
  DateTime firstDate = DateTime.now();
  DateTime lastDate = DateTime.now();
  bool isDataSelected = false;
  late AccountNotifier accountProviders;
  late RegisterResponseModel globals;
  int selected = 0;
  String paymentMethodUse = "";
  @override
  void initState() {
    super.initState();
    email = TextEditingController();
    fistNameTec = TextEditingController();
    lastNameTEC = TextEditingController();
    phoneNo = TextEditingController();
    numberOfGuest = TextEditingController();
    comment = TextEditingController();
    reservationStartDate = TextEditingController();
    reservationEndDate = TextEditingController();
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
    email.dispose();
    fistNameTec.dispose();
    lastNameTEC.dispose();
    numberOfGuest.dispose();
    comment.dispose();
    phoneNo.dispose();
  }

  userDetails() {
    setState(() {
      email.text = globals.email.toString();
      isEmailChanged = globals.email.toString();
      fistNameTec.text = globals.firstName.toString();
      isFirstNameChanged = globals.firstName.toString();
      lastNameTEC.text = globals.lastName.toString();
      isLastNameChanged = globals.lastName.toString();
      phoneNo.text = globals.phoneNo.toString();
      isPhoneNoChanged = globals.phoneNo.toString();
    });
  }

  String isEmailChanged = "";
  String isCommentChanged = "";
  String isFirstNameChanged = "";
  String isLastNameChanged = "";
  String isPhoneNoChanged = "";
  String isNumberOfGuestChanged = "";
  String isReservationStartDateChanged = "";
  String isReservationEndDateChanged = "";
  String selectedCountryCode = "+234"; // Default country code

  emailOnChanged(String value) {
    setState(() {
      isEmailChanged = value;
    });
    printData("identifier", isEmailChanged);
  }

  commentOnChanged(String value) {
    setState(() {
      isCommentChanged = value;
    });
    printData("identifier", isCommentChanged);
  }

  firstNameOnChanged(String value) {
    setState(() {
      isFirstNameChanged = value;
    });
    printData("identifier", isFirstNameChanged);
  }

  lastNameOnChanged(String value) {
    setState(() {
      isLastNameChanged = value;
    });
    printData("identifier", isLastNameChanged);
  }

  phoneNoChanged(String value) {
    setState(() {
      isPhoneNoChanged = value;
    });
    printData("identifier", isPhoneNoChanged);
  }

  phoneNoCountryCodeChanged(String value) {
    setState(() {
      selectedCountryCode = value;
    });
    printData("identifier", isPhoneNoChanged);
  }

  int noOfGuest = 0;
  numberOfGuestChanged(String value) {
    setState(() {
      isNumberOfGuestChanged = value;
      noOfGuest = int.tryParse(value) ?? 0;
    });
    printData("identifier", isNumberOfGuestChanged);
  }

  reservationStartChanged(String value, DateTime? selectedDateTime) {
    setState(() {
      isReservationStartDateChanged = value;
      firstDate = selectedDateTime!;
      reservationStartDate.text = format.format(selectedDateTime);
    });
    printData("identifier", isReservationStartDateChanged);
  }

  reservationEndChanged(String value, DateTime? selectedDateTime) {
    setState(() {
      isReservationEndDateChanged = value;
      lastDate = selectedDateTime!;
      reservationEndDate.text = format.format(selectedDateTime);
    });
    printData("identifier", isReservationEndDateChanged);
  }

  Future<DateTime?> pickDateTime(
      BuildContext context, DateTime? initialDate) async {
    final DateTime? date = await showDatePicker(
      context: context,
      initialDate: initialDate ?? DateTime.now(),
      firstDate: DateTime.now(),
      lastDate: DateTime(2200),
    );

    if (date == null) return null; // User canceled date selection

    final TimeOfDay? time = await showTimePicker(
      context: context,
      initialTime: TimeOfDay.fromDateTime(initialDate ?? DateTime.now()),
    );

    if (time == null) return date; // No time selected, return only date

    return DateTime(date.year, date.month, date.day, time.hour, time.minute);
  }

  userRegister() async {
    var connectivityStatusProvider = ref.read(connectivityStatusProviders);
    FocusScope.of(context).unfocus();
    if (formKey.currentState!.validate()) {
      formKey.currentState!.save();
      setState(() {
        isLoading = true;
      });

      if (connectivityStatusProvider == ConnectivityStatus.isConnected) {
        if (noOfGuest > widget.maximumNoOfGuest) {
          setState(() {
            isLoading = false;
          });
          showCustomSnackbar(context,
              title: "Maximum Guest",
              content:
                  "The no of Guest you selected is is more than the maximum number ${widget.maximumNoOfGuest}",
              type: SnackbarType.error,
              isTopPosition: false);
        } else {
          setState(() {
            isLoading = false;
          });
          DateTime date1 = firstDate;
          DateTime date2 = lastDate;
          Duration difference = date2.difference(date1);
          int daysBetween = difference.inDays;
          showModalBottomSheet(
            context: context,
            isScrollControlled: true,
            shape: RoundedRectangleBorder(
              borderRadius: BorderRadius.vertical(top: Radius.circular(20)),
            ),
            builder: (context) => BookingPaymentBottomSheet(
                amount: widget.bookingPrice.toDouble(),
                reservationId: widget.reservationId,
                customerName:
                    "${fistNameTec.text.trim()} ${lastNameTEC.text.trim()}",
                customerEmail: email.text.trim(),
                customerPhoneNumber:
                    "$selectedCountryCode${phoneNo.text.trim()}",
                numberOfGuests: int.parse(numberOfGuest.text.trim()),
                comment: comment.text.isEmpty ? "" : comment.text.trim(),
                reservationStartDate: reservationStartDate.text.trim(),
                reservationEndDate: reservationEndDate.text.trim(),
                noOfDays: daysBetween),
          );
        }
      } else {
        setState(() {
          isLoading = false;
        });
        showCustomSnackbar(context,
            title: "Network Connection",
            content: "No Internet Connection",
            type: SnackbarType.error,
            isTopPosition: false);
      }
      setState(() {
        isLoading = false;
      });
    }
  }

  @override
  Widget build(BuildContext context) {
    return BookReservationScreenView(this);
  }
}
