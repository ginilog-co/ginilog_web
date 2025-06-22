import 'package:ginilog_customer_app/core/components/utils/app_buttons.dart';
import 'package:ginilog_customer_app/core/components/utils/colors.dart';
import 'package:ginilog_customer_app/core/components/utils/helper_functions.dart';
import 'package:ginilog_customer_app/core/components/utils/size_config.dart';
import 'package:ginilog_customer_app/core/components/widgets/app_text.dart';
import 'package:ginilog_customer_app/core/components/widgets/custom_snackbar.dart';
import 'package:table_calendar/table_calendar.dart';

import '../../../components/utils/package_export.dart';

class BookingDateSelect extends StatefulWidget {
  const BookingDateSelect({
    super.key,
    required this.onDateSelected,
    required this.bookedDates,
  });

  final Set<DateTime> bookedDates;
  final Function(DateTime start, DateTime end) onDateSelected;

  @override
  State<BookingDateSelect> createState() => _BookingDateSelectState();
}

class _BookingDateSelectState extends State<BookingDateSelect> {
  DateTime? selectedStartDate;
  DateTime? selectedEndDate;
  bool _isRangeAvailable(DateTime start, DateTime end) {
    DateTime current = start;
    while (!current.isAfter(end)) {
      if (widget.bookedDates.contains(
        DateTime.utc(current.year, current.month, current.day),
      )) {
        return false;
      }
      current = current.add(Duration(days: 1));
    }
    return true;
  }

  @override
  Widget build(BuildContext context) {
    TextScaler textScaler = MediaQuery.of(context).textScaler;

    DateTime? tempStart = selectedStartDate;
    DateTime? tempEnd = selectedEndDate;
    return Dialog(
      shape: const RoundedRectangleBorder(),
      elevation: 0,
      backgroundColor: Colors.transparent,
      child: StatefulBuilder(
        builder: (context, setState) {
          return Container(
            width: double.infinity,
            decoration: BoxDecoration(
              color: AppColors.white,
              borderRadius: BorderRadius.circular(30),
            ),
            child: Column(
              mainAxisAlignment: MainAxisAlignment.center,
              mainAxisSize: MainAxisSize.min,
              children: [
                addVerticalSpacing(context, 4),
                Row(
                  children: [
                    addHorizontalSpacing(5),

                    Expanded(
                      child: RichText(
                        textAlign: TextAlign.start,
                        textScaler: textScaler,
                        text: TextSpan(
                          style: const TextStyle(color: Colors.black),
                          children: [
                            TextSpan(
                              text:
                                  "Select the range of dates that is not marked as booked",
                              style: TextStyle(
                                color: Colors.black,
                                fontWeight: FontWeight.w400,
                                fontSize: fontSized(context, 45),
                                fontFamily: "Inter",
                              ),
                            ),
                            TextSpan(
                              text: " Red Color ",
                              style: TextStyle(
                                color: AppColors.primary,
                                fontWeight: FontWeight.bold,
                                fontSize: fontSized(context, 45),
                                fontFamily: "Mulish",
                              ),
                            ),
                          ],
                        ),
                      ),
                    ),
                    IconButton(
                      onPressed: () {
                        Navigator.of(context).pop(false);
                      },
                      icon: Icon(Icons.close, color: AppColors.primary),
                    ),
                    addHorizontalSpacing(5),
                  ],
                ),

                BoxSizer(
                  widthPercent: 100, // Adjust the width based on screen
                  heightPercent: 50, // Adjust height to fit your design
                  safeArea: true,
                  child: TableCalendar(
                    firstDay: DateTime.utc(2024, 1, 1),
                    lastDay: DateTime.utc(2026, 12, 31),
                    focusedDay: DateTime.now(),
                    selectedDayPredicate:
                        (day) => day == tempStart || day == tempEnd,
                    onDaySelected: (selectedDay, _) {
                      final now = DateTime.now();
                      final normalized = DateTime.utc(
                        selectedDay.year,
                        selectedDay.month,
                        selectedDay.day,
                      );
                      final today = DateTime.utc(now.year, now.month, now.day);

                      if (normalized.isBefore(today)) {
                        showCustomSnackbar(
                          context,
                          title: "Invalid Date",
                          content: "You cannot select a date in the past.",
                          type: SnackbarType.error,
                          isTopPosition: true,
                        );
                        return;
                      }
                      if (widget.bookedDates.contains(normalized)) return;
                      if (tempStart == null ||
                          (tempStart != null && tempEnd != null)) {
                        tempStart = normalized;
                        tempEnd = null;
                      } else {
                        if (normalized.isBefore(tempStart!)) {
                          if (_isRangeAvailable(normalized, tempStart!)) {
                            tempEnd = tempStart;
                            tempStart = normalized;
                          }
                        } else {
                          if (_isRangeAvailable(tempStart!, normalized)) {
                            tempEnd = normalized;
                          }
                        }
                      }
                      setState(() {});
                    },
                    calendarBuilders: CalendarBuilders(
                      defaultBuilder: (context, day, _) {
                        final isBooked = widget.bookedDates.contains(
                          DateTime.utc(day.year, day.month, day.day),
                        );
                        final isStartOrEnd = day == tempStart || day == tempEnd;
                        final inRange =
                            tempStart != null &&
                            tempEnd != null &&
                            day.isAfter(tempStart!) &&
                            day.isBefore(tempEnd!);

                        return Container(
                          margin: EdgeInsets.all(4),
                          decoration: BoxDecoration(
                            color:
                                isBooked
                                    ? AppColors.primary
                                    : isStartOrEnd
                                    ? AppColors.blue
                                    : inRange
                                    ? Colors.blue.withOpacity(0.4)
                                    : null,
                            borderRadius: BorderRadius.circular(8),
                          ),
                          child: Center(
                            child: Text(
                              '${day.day}',
                              style: TextStyle(
                                color:
                                    isBooked
                                        ? AppColors.white
                                        : AppColors.black,
                              ),
                            ),
                          ),
                        );
                      },
                    ),
                  ),
                ),
                AppButton(
                  text: "Select Date",
                  onPressed: () {
                    if (tempStart != null && tempEnd != null) {
                      widget.onDateSelected(tempStart!, tempEnd!);
                      Navigator.pop(context);
                    } else {
                      showCustomSnackbar(
                        context,
                        title: "Invalid Selection",
                        content: "Please select a valid date range.",
                        type: SnackbarType.error,
                        isTopPosition: true,
                      );
                    }
                  },
                  widthPercent: 70,
                  heightPercent: 5,
                  btnColor: AppColors.primary,
                  isLoading: false,
                ),
                addVerticalSpacing(context, 4),
              ],
            ),
          );
        },
      ),
    );
  }
}
