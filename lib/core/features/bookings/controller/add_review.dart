// ignore_for_file: use_build_context_synchronously

import 'package:ginilog_customer_app/core/components/utils/helper_functions.dart';
import 'package:ginilog_customer_app/core/components/utils/package_export.dart';
import 'package:ginilog_customer_app/core/components/widgets/custom_snackbar.dart';
import 'package:ginilog_customer_app/core/features/bookings/services/booking_services.dart';
import 'package:ginilog_customer_app/core/features/bookings/view/accommodation/add_accomdation_review.dart';

class AddAccomodationReviewScreen extends ConsumerStatefulWidget {
  const AddAccomodationReviewScreen({
    super.key,
    required this.accomodationId,
    required this.accomodationName,
    required this.accomodationLogo,
  });

  final String accomodationId;
  final String accomodationName;
  final String accomodationLogo;
  @override
  ConsumerState<AddAccomodationReviewScreen> createState() =>
      AddAccomodationReviewScreenController();
}

class AddAccomodationReviewScreenController
    extends ConsumerState<AddAccomodationReviewScreen> {
  final GlobalKey<FormState> formKey = GlobalKey<FormState>();

  late TextEditingController reviewController;
  final focusReview = FocusNode();

  bool saveButtonPressed = false;
  bool isLoading = false;
  bool isAlreadyReview = false;
  String reviewMessage = "";
  double ratingNum = 0.0;
  @override
  void initState() {
    super.initState();
    reviewController = TextEditingController();
  }

  @override
  void dispose() {
    super.dispose();
    reviewController.dispose();
  }

  double isReviewChanged = 0.0;

  reviewControllerOnChanged(double value) {
    setState(() {
      isReviewChanged = value;
    });
  }

  String isReviewMessageChanged = "";

  reviewMessageOnChanged(String value) {
    setState(() {
      isReviewMessageChanged = value;
    });
  }

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
      final response = await BookingsService().addAccomodationReview(
        accomodationId: widget.accomodationId,
        reviewMessage: reviewController.text.trim(),
        ratingNum: isReviewChanged,
      );

      if (response.statusCode == 200 || response.statusCode == 201) {
        setState(() {
          isLoading = false;
        });
        showCustomSnackbar(context,
            title: "Accomodation Review",
            content: "Accomodation Review Added Successfully",
            type: SnackbarType.success,
            isTopPosition: false);
        navigateBack(context);
      } else {
        setState(() {
          isLoading = false;
        });

        showCustomSnackbar(context,
            title: "Accomodation Review Error",
            content: response.body,
            type: SnackbarType.error,
            isTopPosition: false);
      }
    }
  }

  @override
  Widget build(BuildContext context) {
    return AddAccomodationReviewScreenView(this);
  }
}
