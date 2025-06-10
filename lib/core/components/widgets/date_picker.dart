// ignore_for_file: deprecated_member_use, use_build_context_synchronously
import 'package:flutter/cupertino.dart';
import 'package:flutter/material.dart';

import 'dart:io';
import 'package:intl/intl.dart';

class CustomDateTimePickerField extends StatefulWidget {
  final bool showDate;
  final bool showTime;

  const CustomDateTimePickerField({
    super.key,
    this.showDate = true,
    this.showTime = true,
  });

  @override
  State<CustomDateTimePickerField> createState() =>
      _CustomDateTimePickerFieldState();
}

class _CustomDateTimePickerFieldState extends State<CustomDateTimePickerField> {
  final TextEditingController _controller = TextEditingController();

  Future<void> _pickDateTime(BuildContext context) async {
    DateTime now = DateTime.now();
    DateTime? selectedDateTime;

    if (Platform.isIOS) {
      // Show Cupertino Modal Popup
      await showCupertinoModalPopup(
        context: context,
        builder: (BuildContext context) {
          DateTime tempDateTime = now;

          return Container(
            height: 300,
            color: Colors.white,
            child: Column(
              children: [
                SizedBox(
                  height: 250,
                  child: CupertinoDatePicker(
                    mode:
                        widget.showDate && widget.showTime
                            ? CupertinoDatePickerMode.dateAndTime
                            : widget.showDate
                            ? CupertinoDatePickerMode.date
                            : CupertinoDatePickerMode.time,
                    initialDateTime: now,
                    onDateTimeChanged: (DateTime value) {
                      tempDateTime = value;
                    },
                  ),
                ),
                CupertinoButton(
                  child: const Text("Done"),
                  onPressed: () {
                    Navigator.of(context).pop();
                    selectedDateTime = tempDateTime;
                    _formatAndSet(selectedDateTime!);
                  },
                ),
              ],
            ),
          );
        },
      );
    } else {
      // Android: Material Pickers
      DateTime? pickedDate;
      TimeOfDay? pickedTime;

      if (widget.showDate) {
        pickedDate = await showDatePicker(
          context: context,
          initialDate: now,
          firstDate: DateTime(2000),
          lastDate: DateTime(2100),
        );
        if (pickedDate == null) return;
      }

      if (widget.showTime) {
        pickedTime = await showTimePicker(
          context: context,
          initialTime: TimeOfDay.now(),
        );
        if (pickedTime == null) return;
      }

      selectedDateTime = DateTime(
        pickedDate?.year ?? now.year,
        pickedDate?.month ?? now.month,
        pickedDate?.day ?? now.day,
        pickedTime?.hour ?? 0,
        pickedTime?.minute ?? 0,
      );

      _formatAndSet(selectedDateTime);
    }
  }

  void _formatAndSet(DateTime dateTime) {
    String formatted = '';

    if (widget.showDate && widget.showTime) {
      formatted = DateFormat('yyyy-MM-dd HH:mm').format(dateTime);
    } else if (widget.showDate) {
      formatted = DateFormat('yyyy-MM-dd').format(dateTime);
    } else if (widget.showTime) {
      formatted = DateFormat('HH:mm').format(dateTime);
    }

    setState(() {
      _controller.text = formatted;
    });
  }

  @override
  Widget build(BuildContext context) {
    return TextFormField(
      controller: _controller,
      readOnly: true,
      decoration: const InputDecoration(
        labelText: 'Select Date &/or Time',
        suffixIcon: Icon(Icons.calendar_today),
        border: OutlineInputBorder(),
      ),
      onTap: () => _pickDateTime(context),
    );
  }
}

Future<String?> pickDateTime({
  required BuildContext context,
  bool showDate = true,
  bool showTime = true,
}) async {
  FocusScope.of(context).requestFocus(FocusNode()); // Dismiss keyboard
  DateTime now = DateTime.now();
  DateTime? selectedDateTime;

  if (Platform.isIOS) {
    DateTime tempDateTime = now;

    await showCupertinoModalPopup(
      context: context,
      builder: (BuildContext ctx) {
        return Container(
          height: 300,
          color: Colors.white,
          child: Column(
            children: [
              SizedBox(
                height: 250,
                child: CupertinoDatePicker(
                  mode:
                      showDate && showTime
                          ? CupertinoDatePickerMode.dateAndTime
                          : showDate
                          ? CupertinoDatePickerMode.date
                          : CupertinoDatePickerMode.time,
                  initialDateTime: now,
                  onDateTimeChanged: (DateTime value) {
                    tempDateTime = value;
                  },
                ),
              ),
              CupertinoButton(
                child: const Text("Done"),
                onPressed: () {
                  Navigator.of(ctx).pop();
                  selectedDateTime = tempDateTime;
                },
              ),
            ],
          ),
        );
      },
    );
  } else {
    DateTime? pickedDate;
    TimeOfDay? pickedTime;

    if (showDate) {
      pickedDate = await showDatePicker(
        context: context,
        initialDate: now,
        firstDate: DateTime(2000),
        lastDate: DateTime(2100),
      );
      if (pickedDate == null) return null;
    }

    if (showTime) {
      pickedTime = await showTimePicker(
        context: context,
        initialTime: TimeOfDay.now(),
      );
      if (pickedTime == null) return null;
    }

    selectedDateTime = DateTime(
      pickedDate?.year ?? now.year,
      pickedDate?.month ?? now.month,
      pickedDate?.day ?? now.day,
      pickedTime?.hour ?? 0,
      pickedTime?.minute ?? 0,
    );
  }

  if (selectedDateTime == null) return null;

  // Format the result
  if (showDate && showTime) {
    return DateFormat('yyyy-MM-dd HH:mm').format(selectedDateTime!);
  } else if (showDate) {
    return DateFormat('yyyy-MM-dd').format(selectedDateTime!);
  } else if (showTime) {
    return DateFormat('HH:mm').format(selectedDateTime!);
  } else {
    return null;
  }
}
