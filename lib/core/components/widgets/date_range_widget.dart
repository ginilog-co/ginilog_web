// ignore_for_file: library_private_types_in_public_api, use_build_context_synchronously

import 'package:ginilog_customer_app/core/components/utils/app_buttons.dart';
import 'package:ginilog_customer_app/core/components/utils/colors.dart';
import 'package:ginilog_customer_app/core/components/utils/helper_functions.dart';
import 'package:ginilog_customer_app/core/components/utils/package_export.dart';
import 'package:ginilog_customer_app/core/components/widgets/app_text.dart';

class DateRangeWidget extends ConsumerStatefulWidget {
  const DateRangeWidget({
    super.key,
  });

  @override
  _HomePageState createState() => _HomePageState();
}

class _HomePageState extends ConsumerState<DateRangeWidget> {
  DateRange daysOld = DateRange();
  final format = DateFormat("dd MMM, yyyy hh:mm a"); // ✅ Includes time
  late TextEditingController startDate;
  late TextEditingController endDate;
  final FocusNode? startDateFocusNode = FocusNode();
  final FocusNode? endDateFocusNode = FocusNode();
  DateTime firstDate = DateTime.now();
  DateTime lastDate = DateTime.now();
  bool isDataSelected = false;
  final GlobalKey<FormState> formKey = GlobalKey<FormState>();
  @override
  void initState() {
    startDate = TextEditingController();
    endDate = TextEditingController();
    super.initState();
  }

  @override
  void dispose() {
    super.dispose();
    startDate.dispose();
    endDate.dispose();
  }

  Future<DateTime?> _pickDateTime(
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

  @override
  Widget build(BuildContext context) {
    return Container(
      height: getScreenHeight(context) / 3,
      padding: const EdgeInsets.all(8),
      alignment: Alignment.center,
      child: Form(
        key: formKey,
        child: Column(
          children: [
            addVerticalSpacing(context, 35),
            Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                const AppText(
                    isBody: true,
                    text: "Start Date",
                    textAlign: TextAlign.center,
                    fontSize: 78,
                    color: AppColors.black,
                    fontStyle: FontStyle.normal,
                    fontWeight: FontWeight.w700),
                DateTimeField(
                  format: format,
                  onShowPicker: (context, currentValue) async {
                    final selectedDateTime =
                        await _pickDateTime(context, currentValue);
                    if (selectedDateTime != null) {
                      setState(() {
                        firstDate = selectedDateTime;
                        startDate.text = format.format(selectedDateTime);
                      });
                    }
                    return selectedDateTime;
                  },
                  focusNode: startDateFocusNode,
                  autovalidateMode: AutovalidateMode.onUserInteraction,
                  controller: startDate,
                  validator: (value) =>
                      value == null ? "Enter a valid Date" : null,
                  style: TextStyle(
                    fontSize: fontSized(context, 78),
                    fontWeight: FontWeight.w700,
                    fontStyle: FontStyle.normal,
                  ),
                  decoration: InputDecoration(
                    filled: true,
                    fillColor: AppColors.white,
                    focusedBorder: const OutlineInputBorder(
                      borderRadius: BorderRadius.all(Radius.circular(2)),
                      borderSide: BorderSide(width: 1, color: AppColors.black),
                    ),
                    disabledBorder: const OutlineInputBorder(
                      borderRadius: BorderRadius.all(Radius.circular(2)),
                      borderSide: BorderSide(width: 1, color: Colors.orange),
                    ),
                    enabledBorder: OutlineInputBorder(
                      borderRadius: BorderRadius.circular(2),
                      borderSide: const BorderSide(
                        color: Colors.grey,
                        width: 1.0,
                      ),
                    ),
                    border: OutlineInputBorder(
                      borderRadius: BorderRadius.circular(2),
                      borderSide: const BorderSide(color: AppColors.primary),
                    ),
                    errorBorder: OutlineInputBorder(
                      borderRadius: BorderRadius.circular(2),
                      borderSide: const BorderSide(
                        color: Colors.red,
                        width: 1.0,
                      ),
                    ),
                    hintText: "Start Date",
                    hintStyle: TextStyle(
                        fontSize: fontSized(context, 78),
                        fontWeight: FontWeight.w700,
                        fontStyle: FontStyle.normal,
                        color: AppColors.grey),
                  ),
                ),
              ],
            ),
            addVerticalSpacing(context, 5),
            Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                const AppText(
                    isBody: true,
                    text: "End Date",
                    textAlign: TextAlign.center,
                    fontSize: 78,
                    color: AppColors.black,
                    fontStyle: FontStyle.normal,
                    fontWeight: FontWeight.w700),
                DateTimeField(
                  format: format,
                  onShowPicker: (context, currentValue) async {
                    final date = await showDatePicker(
                        context: context,
                        initialDate: currentValue ?? DateTime.now(),
                        firstDate: DateTime(1900),
                        lastDate: DateTime(2200));
                    setState(() {
                      lastDate = date!;
                    });

                    return date;
                  },
                  focusNode: endDateFocusNode,
                  autovalidateMode: AutovalidateMode.onUserInteraction,
                  controller: endDate,
                  validator: (value) =>
                      value == null ? "Enter a valid Date" : null,
                  style: TextStyle(
                    fontSize: fontSized(context, 78),
                    fontWeight: FontWeight.w700,
                    fontStyle: FontStyle.normal,
                  ),
                  decoration: InputDecoration(
                    filled: true,
                    fillColor: AppColors.white,
                    focusedBorder: const OutlineInputBorder(
                      borderRadius: BorderRadius.all(Radius.circular(2)),
                      borderSide: BorderSide(width: 1, color: AppColors.black),
                    ),
                    disabledBorder: const OutlineInputBorder(
                      borderRadius: BorderRadius.all(Radius.circular(2)),
                      borderSide: BorderSide(width: 1, color: Colors.orange),
                    ),
                    enabledBorder: OutlineInputBorder(
                      borderRadius: BorderRadius.circular(2),
                      borderSide: const BorderSide(
                        color: Colors.grey,
                        width: 1.0,
                      ),
                    ),
                    border: OutlineInputBorder(
                      borderRadius: BorderRadius.circular(2),
                      borderSide: const BorderSide(color: AppColors.primary),
                    ),
                    errorBorder: OutlineInputBorder(
                      borderRadius: BorderRadius.circular(2),
                      borderSide: const BorderSide(
                        color: Colors.red,
                        width: 1.0,
                      ),
                    ),
                    hintText: "End Date",
                    hintStyle: TextStyle(
                        fontSize: fontSized(context, 78),
                        fontWeight: FontWeight.w700,
                        fontStyle: FontStyle.normal,
                        color: AppColors.grey),
                  ),
                ),
              ],
            ),
            addVerticalSpacing(context, 3),
            appButton("Select", getScreenWidth(context), () {
              if (formKey.currentState!.validate()) {
                formKey.currentState!.save();
                DateTime date1 = firstDate;

                DateTime date2 = lastDate;

                Duration difference = date2.difference(date1);

                int daysBetween = difference.inDays;
                DateRange dateRange = DateRange(
                    startDate: firstDate,
                    endDate: lastDate,
                    daysOld: daysBetween);
                setState(() {
                  daysOld = dateRange;
                  daysOld = dateRange;

                  isDataSelected = true;
                });
                debugPrint("Days between dates: $daysOld");
                Navigator.pop(context, daysOld);
              }
            }, AppColors.primary, false),
            // addVerticalSpacing(context, 20),
          ],
        ),
      ),
    );
  }
}

class DateRange {
  DateRange({this.startDate, this.endDate, this.daysOld});
  final DateTime? startDate;
  final DateTime? endDate;
  final int? daysOld;
}
