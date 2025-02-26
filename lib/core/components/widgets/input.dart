// ignore_for_file: prefer_const_constructors, constant_identifier_names, annotate_overrides, overridden_fields, use_key_in_widget_constructors, must_be_immutable

import 'package:ginilog_customer_app/core/components/utils/package_export.dart';
import '../utils/colors.dart';
import '../utils/helper_functions.dart';
import '../utils/typography.dart';
import 'app_text.dart';

class Input extends StatelessWidget {
  final String? hintText;
  final String? Function(String?)? validator;
  final Function(String?)? onSaved;
  final Function(String?)? onChanged;
  final Function? toggleEye;
  final TextInputType? keyboard;
  final String? init;
  final bool isPassword;
  final bool readOnly;
  final Color? isPasswordColor;
  //final bool showObscureText;
  final bool obscureText;
  final Color? styleColor;

  final String? labelText;
  final String? errorText;
  final dynamic maxLines;
  final Color? borderColor;
  final Color? fillColor;
  final Color? hintColor;
  final Color? hintStyleColor;
  final Color? textColor;
  final Color? labelColor;
  final String? inputIcon;
  final Widget? prefix;
  final Widget? suffix;
  final Key? key;
  final IconData? prefixIcon;
  final bool enable;
  final TextEditingController? controller;
  //final List<TextInputFormatter> inputFormatters;
  final FocusNode? focusNode;
  //final bool alignLabelWithHint;
  final FloatingLabelBehavior? floatingLabelBehavior;
  final Function onTap;
  final double? borderRadius;
  const Input(
      {this.hintText,
      this.labelColor,
      this.fillColor,
      this.textColor,
      this.borderRadius,
      this.validator,
      this.readOnly = false,
      this.hintColor,
      required this.onSaved,
      required this.toggleEye,
      this.init,
      this.isPassword = false,
      this.isPasswordColor,
      //this.showObscureText,
      this.obscureText = false,
      this.keyboard,
      this.styleColor,
      this.hintStyleColor,
      this.enable = true,
      this.labelText,
      this.maxLines = 1,
      this.borderColor = Colors.grey,
      required this.onChanged,
      this.prefix,
      this.suffix,
      this.key,
      this.controller,
      this.focusNode,
      this.prefixIcon,
      this.floatingLabelBehavior,
      required this.onTap,
      this.errorText,
      this.inputIcon});

  @override
  Widget build(BuildContext context) {
    return TextFormField(
      keyboardType: keyboard,
      controller: controller,
      key: key,
      readOnly: readOnly,
      enabled: enable,
      onSaved: onSaved!,
      onChanged: onChanged!,
      validator: validator!,
      obscureText: obscureText,
      initialValue: init,
      focusNode: focusNode,
      autovalidateMode: AutovalidateMode.onUserInteraction,
      decoration: InputDecoration(
        prefixIcon: Icon(
          prefixIcon,
          color: AppColors.grey,
        ),
        focusedErrorBorder: OutlineInputBorder(
          borderRadius: BorderRadius.circular(20),
          borderSide: BorderSide(
            color: Colors.grey,
            width: 1.0,
          ),
        ),
        border: OutlineInputBorder(
          borderRadius: BorderRadius.circular(20),
          borderSide: BorderSide(color: AppColors.primary),
        ),
        errorBorder: OutlineInputBorder(
          borderRadius: BorderRadius.circular(20),
          borderSide: BorderSide(
            color: Colors.red,
            width: 1.0,
          ),
        ),
        suffixIcon: suffix,
        fillColor: !enable
            ? fillColor ?? Color(0xFFeeeeee)
            : fillColor ?? Colors.transparent,
        // fillColor: Colors.white,
        filled: true,
        enabledBorder: OutlineInputBorder(
          borderRadius: BorderRadius.circular(20),
          borderSide: BorderSide(
            color: Colors.grey,
            width: 1.0,
          ),
        ),
        focusedBorder: OutlineInputBorder(
          borderRadius: BorderRadius.circular(20),
          borderSide: BorderSide(
            color: AppColors.primary,
            width: 1.0,
          ),
        ),
        hintText: hintText,
        hintStyle: TextStyle(
          color: hintStyleColor ?? Colors.grey,
          fontSize: 14,
        ),
      ),
    );
  }
}

class PlainInput extends StatelessWidget {
  final String? hintText;
  final String? Function(String?)? validator;
  final Function(String?)? onSaved;
  final Function(String?)? onChanged;
  final Function? toggleEye;
  final TextInputType? keyboard;
  final String? init;
  final bool isPassword;
  final Color? isPasswordColor;
  final bool obscureText;
  final Color? styleColor;
  final Color? hintStyleColor;
  final bool enable;
  final String? labelText;
  final String? errorText;
  final dynamic maxLines;
  final Color? borderColor;
  final Color? fillColor;
  final Color? hintColor;
  final Color? textColor;
  final Color? labelColor;
  final String? inputIcon;
  final Widget? prefix;
  final Widget? suffix;
  final Key? key;
  final IconData? prefixIcon;
  final TextEditingController? controller;
  //final List<TextInputFormatter> inputFormatters;
  final FocusNode? focusNode;
  //final bool alignLabelWithHint;
  final FloatingLabelBehavior? floatingLabelBehavior;
  final Function onTap;
  final double? borderRadius;
  final bool readOnly;
  final int? maxLength;
  const PlainInput(
      {this.hintText,
      this.labelColor,
      this.fillColor,
      this.textColor,
      this.borderRadius,
      this.validator,
      this.hintColor,
      required this.onSaved,
      required this.toggleEye,
      this.init,
      this.isPassword = false,
      this.isPasswordColor,
      this.obscureText = false,
      this.keyboard,
      this.styleColor,
      this.hintStyleColor,
      this.enable = true,
      this.labelText,
      this.maxLines = 1,
      this.borderColor = Colors.grey,
      required this.onChanged,
      this.prefix,
      this.suffix,
      this.key,
      this.controller,
      this.focusNode,
      this.prefixIcon,
      this.floatingLabelBehavior,
      required this.onTap,
      this.errorText,
      this.inputIcon,
      this.readOnly = false,
      this.maxLength});

  @override
  Widget build(BuildContext context) {
    return TextFormField(
      style: AppTypography.dynamicStyle(
        fontSize: fontSized(context, 15),
        fontWeight: FontWeight.w400,
        color: AppColors.black,
      ),
      keyboardType: keyboard,
      controller: controller,
      focusNode: focusNode,
      key: key,
      enabled: enable,
      onSaved: onSaved!,
      onTap: onTap as void Function()?,
      onChanged: onChanged!,
      validator: validator!,
      obscureText: obscureText,
      initialValue: init,
      readOnly: readOnly,
      //  maxLength: maxLength,
      autovalidateMode: AutovalidateMode.onUserInteraction,
      inputFormatters: [
        LengthLimitingTextInputFormatter(maxLength),
      ],
      decoration: InputDecoration(
        border: OutlineInputBorder(
          borderRadius: BorderRadius.circular(borderRadius ?? 20),
          borderSide: BorderSide(color: AppColors.primary),
        ),
        errorBorder: OutlineInputBorder(
          borderRadius: BorderRadius.circular(borderRadius ?? 20),
          borderSide: BorderSide(
            color: Colors.red,
            width: 1.0,
          ),
        ),
        //suffixIcon: suffix,
        fillColor: !enable
            ? fillColor ?? Color(0xFFeeeeee)
            : fillColor ?? Colors.transparent,
        // fillColor: Colors.white,
        filled: true,
        enabledBorder: OutlineInputBorder(
          borderRadius: BorderRadius.circular(borderRadius ?? 20),
          borderSide: BorderSide(
            color: borderColor ?? Colors.grey,
            width: 1.0,
          ),
        ),
        focusedBorder: OutlineInputBorder(
          borderRadius: BorderRadius.circular(borderRadius ?? 20),
          borderSide: BorderSide(
            color: AppColors.primary,
            width: 1.0,
          ),
        ),
        hintText: hintText,
        suffixIcon: suffix,
        // prefixIcon: prefix,
        hintStyle: TextStyle(
          color: hintStyleColor ?? Colors.grey,
          fontSize: fontSized(context, 22),
        ),
      ),
    );
  }
}

class DescriptionInput extends StatelessWidget {
  final String? hintText;
  final String? Function(String?)? validator;
  final Function(String?)? onSaved;
  final Function(String?)? onChanged;
  final Function? toggleEye;
  final TextInputType? keyboard;
  final String? init;
  final bool isPassword;
  final Color? isPasswordColor;
  //final bool showObscureText;
  final bool obscureText;
  final Color? styleColor;
  final Color? hintStyleColor;
  final bool enable;
  final String? labelText;
  final String? errorText;
  final dynamic maxLines;
  final Color? borderColor;
  final Color? fillColor;
  final Color? hintColor;
  final Color? textColor;
  final Color? labelColor;
  final String? inputIcon;
  final Widget? prefix;
  final Widget? suffix;
  final Key? key;
  final IconData? prefixIcon;
  final TextEditingController? controller;
  //final List<TextInputFormatter> inputFormatters;
  final FocusNode? focusNode;
  //final bool alignLabelWithHint;
  final FloatingLabelBehavior? floatingLabelBehavior;
  final Function onTap;
  final double? borderRadius;
  final int? maxLength;
  const DescriptionInput(
      {this.hintText,
      this.labelColor,
      this.fillColor,
      this.textColor,
      this.borderRadius,
      this.validator,
      this.hintColor,
      required this.onSaved,
      required this.toggleEye,
      this.init,
      this.isPassword = false,
      this.isPasswordColor,
      //this.showObscureText,
      this.obscureText = false,
      this.keyboard,
      this.styleColor,
      this.hintStyleColor,
      this.enable = true,
      this.labelText,
      this.maxLines = 1,
      this.borderColor = Colors.grey,
      required this.onChanged,
      this.prefix,
      this.suffix,
      this.key,
      this.controller,
      this.focusNode,
      this.prefixIcon,

      ///this.alignLabelWithHint,
      this.floatingLabelBehavior,
      required this.onTap,
      this.errorText,
      this.inputIcon,
      this.maxLength});

  @override
  Widget build(BuildContext context) {
    return TextFormField(
      style: AppTypography.dynamicStyle(
        fontSize: fontSized(context, 62),
        fontWeight: FontWeight.w400,
        color: AppColors.black,
      ),
      keyboardType: keyboard,
      controller: controller,
      key: key,
      enabled: enable,
      onSaved: onSaved!,
      onChanged: onChanged!,
      validator: validator!,
      obscureText: obscureText,
      initialValue: init,
      maxLines: 10,
      minLines: 6,
      maxLength: maxLength,
      focusNode: focusNode,
      decoration: InputDecoration(
        // prefixIcon: Icon(prefixIcon),
        // focusedErrorBorder: OutlineInputBorder(
        //   borderRadius: BorderRadius.circular(2),
        //   borderSide: BorderSide(
        //     color: Colors.grey,
        //     width: 1.0,
        //   ),
        // ),
        border: OutlineInputBorder(
          borderRadius: BorderRadius.circular(borderRadius ?? 20),
          borderSide: BorderSide(color: AppColors.primary),
        ),
        errorBorder: OutlineInputBorder(
          borderRadius: BorderRadius.circular(borderRadius ?? 20),
          borderSide: BorderSide(
            color: Colors.red,
            width: 1.0,
          ),
        ),
        //suffixIcon: suffix,
        fillColor: !enable
            ? fillColor ?? Color(0xFFeeeeee)
            : fillColor ?? Colors.transparent,
        // fillColor: Colors.white,
        filled: true,
        enabledBorder: OutlineInputBorder(
          borderRadius: BorderRadius.circular(borderRadius ?? 20),
          borderSide: BorderSide(
            color: Colors.grey,
            width: 1.0,
          ),
        ),
        focusedBorder: OutlineInputBorder(
          borderRadius: BorderRadius.circular(borderRadius ?? 20),
          borderSide: BorderSide(
            color: AppColors.primary,
            width: 1.0,
          ),
        ),
        hintText: hintText,
        hintStyle: TextStyle(
          color: hintStyleColor ?? Colors.grey,
          fontSize: fontSized(context, 62),
        ),
      ),
    );
  }
}

class DateWidget extends StatelessWidget {
  final String? hintText;
  final TextEditingController? controller;
  final Function(String?)? onChanged;
  final bool isDated;
  final double? borderRadius;
  final FocusNode? focusNode;
  const DateWidget(
      {super.key,
      this.controller,
      this.onChanged,
      this.hintText,
      this.isDated = false,
      this.borderRadius,
      this.focusNode});

  @override
  Widget build(BuildContext context) {
    return TextFormField(
      readOnly: true,
      controller: controller,
      onChanged: onChanged!,
      focusNode: focusNode,
      style: TextStyle(
        color: AppColors.black,
        fontFamily: "Montserrat",
        fontSize: fontSized(context, 85),
      ),
      autovalidateMode: AutovalidateMode.onUserInteraction,
      decoration: InputDecoration(
        floatingLabelBehavior: FloatingLabelBehavior.auto,
        suffixIcon: const Icon(Icons.calendar_month),
        border: OutlineInputBorder(
            borderRadius: BorderRadius.circular(borderRadius ?? 5),
            borderSide: BorderSide(color: AppColors.darkBlue, width: 1),
            gapPadding: 40),
        enabledBorder: OutlineInputBorder(
            borderRadius: BorderRadius.circular(borderRadius ?? 5),
            borderSide: BorderSide(color: AppColors.grey2, width: 1.5)),
        errorBorder: OutlineInputBorder(
            borderRadius: BorderRadius.circular(borderRadius ?? 5),
            borderSide: const BorderSide(color: Colors.red, width: 1)),
        focusedBorder: OutlineInputBorder(
          borderRadius: BorderRadius.circular(borderRadius ?? 5),
          borderSide: BorderSide(color: AppColors.darkBlue, width: 1),
        ),
        hintText: hintText,
      ),
      onTap: () async {
        await showDatePicker(
          context: context,
          initialDate: DateTime.now(),
          firstDate: isDated == true ? DateTime(2015) : DateTime.now(),
          lastDate: DateTime(2050),
        ).then((selectedDate) {
          if (selectedDate != null) {
            controller!.text = DateFormat('dd MMM, yyyy').format(selectedDate);
          }
        });
      },
      validator: (value) {
        if (value!.isEmpty) {
          return 'please enter date.';
        }
        return null;
      },
    );
  }
}

class SerachInput extends StatelessWidget {
  final String? hintText;
  final String? Function(String?)? validator;
  final Function(String?)? onSaved;
  final Function(String?)? onChanged;
  final Function? toggleEye;
  final TextInputType? keyboard;
  final String? init;
  final bool isPassword;
  final bool readOnly;
  final Color? isPasswordColor;
  //final bool showObscureText;
  final bool obscureText;
  final Color? styleColor;
  final Color? hintStyleColor;
  final bool enable;
  final String? labelText;
  final String? errorText;
  final dynamic maxLines;
  final Color? borderColor;
  final Color? fillColor;
  final Color? hintColor;
  final Color? textColor;
  final Color? labelColor;
  final String? inputIcon;
  final Widget? prefix;
  final Widget? suffix;
  final Key? key;
  final IconData? prefixIcon;
  final TextEditingController? controller;
  //final List<TextInputFormatter> inputFormatters;
  final FocusNode? focusNode;
  //final bool alignLabelWithHint;
  final FloatingLabelBehavior? floatingLabelBehavior;
  final Function onTap;
  final double? borderRadius;

  const SerachInput(
      {this.hintText,
      this.labelColor,
      this.fillColor,
      this.textColor,
      this.borderRadius,
      this.validator,
      this.hintColor,
      required this.onSaved,
      required this.toggleEye,
      this.init,
      this.isPassword = false,
      this.readOnly = false,
      this.isPasswordColor,
      //this.showObscureText,
      this.obscureText = false,
      this.keyboard,
      this.styleColor,
      this.hintStyleColor,
      this.enable = true,
      this.labelText,
      this.maxLines = 1,
      this.borderColor = Colors.white,
      required this.onChanged,
      this.prefix,
      this.suffix,
      this.key,
      this.controller,
      this.focusNode,
      this.prefixIcon,

      ///this.alignLabelWithHint,
      this.floatingLabelBehavior,
      required this.onTap,
      this.errorText,
      this.inputIcon});

  @override
  Widget build(BuildContext context) {
    return TextFormField(
      cursorColor: AppColors.white,
      focusNode: focusNode,
      onTap: onTap as void Function()?,
      keyboardType: keyboard,
      controller: controller,
      key: key,
      enabled: enable,
      onSaved: onSaved!,
      onChanged: onChanged!,
      validator: validator!,
      obscureText: obscureText,
      initialValue: init,
      readOnly: readOnly,
      style: TextStyle(
        color: styleColor ?? AppColors.white,
        fontSize: fontSized(context, 122),
      ),
      decoration: InputDecoration(
        prefixIcon: prefix,
        focusedErrorBorder: OutlineInputBorder(
          borderRadius: BorderRadius.circular(5),
          borderSide: BorderSide(
            color: Colors.grey,
            width: 1.0,
          ),
        ),
        border: OutlineInputBorder(
          borderRadius: BorderRadius.circular(5),
          borderSide: const BorderSide(color: AppColors.black),
        ),
        errorBorder: OutlineInputBorder(
          borderRadius: BorderRadius.circular(5),
          borderSide: BorderSide(
            color: Colors.red,
            width: 2.0,
          ),
        ),

        fillColor: !enable
            ? fillColor ?? Color(0xFFeeeeee)
            : fillColor ?? Colors.transparent,
        // fillColor: Colors.white,
        filled: false,
        enabledBorder: OutlineInputBorder(
          borderRadius: BorderRadius.circular(5),
          borderSide: BorderSide(
            color: Colors.grey,
            width: 2.0,
          ),
        ),
        focusedBorder: OutlineInputBorder(
          borderRadius: BorderRadius.circular(5),
          borderSide: BorderSide(
            color: AppColors.grey,
            width: 2.0,
          ),
        ),
        hintText: hintText,
        hintStyle: TextStyle(
          color: hintStyleColor ?? Colors.white,
          fontSize: 14,
        ),
      ),
    );
  }
}

class InputField extends StatelessWidget {
  const InputField({
    super.key,
    required this.controller,
    this.textInputAction = TextInputAction.next,
    this.textInputType,
    this.hintText,
    this.label,
    this.suffix,
    required this.labelName,
    this.autofocus = false,
    this.isMultipleLines = false,
    this.validator,
    this.onChanged,
    this.fillColor,
    this.hintColor,
    this.hintStyleColor,
    this.prefixIcon,
    this.enable,
    this.focusNode,
  });

  final TextEditingController controller;
  final TextInputAction? textInputAction;
  final FocusNode? focusNode;
  final TextInputType? textInputType;
  final String? label;
  final String? hintText;
  final Widget? suffix;
  final Color? fillColor;
  final Color? hintColor;
  final Color? hintStyleColor;
  final IconData? prefixIcon;
  final bool? enable;
  final bool isMultipleLines;
  final bool autofocus;
  final String? Function(String?)? validator;
  final String labelName;
  final Function(String?)? onChanged;

  @override
  Widget build(BuildContext context) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text(
          labelName,
          style: AppTypography.buttonSmall().copyWith(
              fontWeight: FontWeight.w500, fontSize: fontSized(context, 22)),
        ),
        addVerticalSpacing(context, 52),
        TextFormField(
          style: AppTypography.dynamicStyle(
            fontSize: fontSized(context, 22),
            fontWeight: FontWeight.w400,
            color: AppColors.black,
          ),
          cursorColor: AppColors.primary,
          maxLines: isMultipleLines ? 5 : null,
          autovalidateMode: AutovalidateMode.onUserInteraction,
          keyboardType: textInputType,
          autofocus: autofocus,
          textInputAction: textInputAction ?? TextInputAction.next,
          validator: validator,
          onChanged: onChanged,
          controller: controller,
          focusNode: focusNode,
          decoration: InputDecoration(
            prefixIcon: Icon(prefixIcon),
            focusedErrorBorder: OutlineInputBorder(
              borderRadius: BorderRadius.circular(20),
              borderSide: BorderSide(
                color: Colors.grey,
                width: 1.0,
              ),
            ),
            border: OutlineInputBorder(
              borderRadius: BorderRadius.circular(20),
              borderSide: BorderSide(color: AppColors.primary),
            ),
            errorBorder: OutlineInputBorder(
              borderRadius: BorderRadius.circular(20),
              borderSide: BorderSide(
                color: Colors.red,
                width: 1.0,
              ),
            ),
            suffixIcon: suffix,
            fillColor: !enable!
                ? fillColor ?? Color(0xFFeeeeee)
                : fillColor ?? Colors.transparent,
            // fillColor: Colors.white,
            filled: true,
            enabledBorder: OutlineInputBorder(
              borderRadius: BorderRadius.circular(20),
              borderSide: BorderSide(
                color: Colors.grey,
                width: 1.0,
              ),
            ),
            focusedBorder: OutlineInputBorder(
              borderRadius: BorderRadius.circular(20),
              borderSide: BorderSide(
                color: AppColors.primary,
                width: 1.0,
              ),
            ),
            hintText: hintText,
            hintStyle: TextStyle(
              color: hintStyleColor ?? Colors.grey,
              fontSize: 14,
            ),
          ),
        ),
      ],
    );
  }
}

enum KeyboardType { NUMBER, TEXT, EMAIL, PHONE }

class GlobalTextField extends StatefulWidget {
  final String fieldName;
  final TextInputType keyBoardType;
  final FocusNode? focusNode;
  final TextEditingController textController;
  final int maxLength;
  final bool isCenterText;
  final bool isEyeVisible;
  final bool removeSpace;
  bool obscureText;
  final bool isOptional;
  final Function(String?)? onChanged;
  final bool readOnly;
  final double? borderRadius;
  final Function? onTap;
  final Widget? prefix;
  final Widget? suffix;
  final bool isNotePad;
  GlobalTextField({
    super.key,
    required this.fieldName,
    required this.keyBoardType,
    required this.textController,
    this.focusNode,
    this.removeSpace = true,
    this.obscureText = false,
    this.isCenterText = false,
    this.isEyeVisible = false,
    this.isOptional = false,
    this.readOnly = false,
    this.onChanged,
    this.maxLength = 35,
    this.borderRadius,
    this.onTap,
    this.prefix,
    this.suffix,
    this.isNotePad = false,
  });

  @override
  State<GlobalTextField> createState() => _GlobalTextFieldState();
}

class _GlobalTextFieldState extends State<GlobalTextField> {
  @override
  Widget build(BuildContext context) {
    return TextFormField(
      controller: widget.textController,
      keyboardType: widget.keyBoardType,
      obscureText: widget.obscureText,
      focusNode: widget.focusNode,
      onChanged: widget.onChanged,
      readOnly: widget.readOnly,
      onTap: widget.onTap as void Function()?,
      enabled: true,
      maxLines: widget.isNotePad == true ? 10 : 1,
      minLines: widget.isNotePad == true ? 6 : 1,
      autovalidateMode: AutovalidateMode.onUserInteraction,
      textAlign: widget.isCenterText ? TextAlign.center : TextAlign.start,
      textCapitalization: widget.keyBoardType == TextInputType.name
          ? TextCapitalization.sentences
          : TextCapitalization.none,
      inputFormatters: [
        widget.removeSpace
            ? FilteringTextInputFormatter.deny(RegExp(r"\s\b|\b\s"))
            : LengthLimitingTextInputFormatter(widget.maxLength),
        widget.keyBoardType == TextInputType.phone
            ? FilteringTextInputFormatter.deny(RegExp(r'^0+'))
            : LengthLimitingTextInputFormatter(widget.maxLength),
        LengthLimitingTextInputFormatter(widget.maxLength),
      ],
      style: TextStyle(
        color: AppColors.black,
        fontFamily: "Montserrat",
        fontSize: fontSized(context, 85),
      ),
      decoration: InputDecoration(
        floatingLabelBehavior: FloatingLabelBehavior.auto,
        labelText: widget.fieldName,
        isDense: true,
        contentPadding: const EdgeInsets.only(top: 15, bottom: 15, left: 5),
        suffixIcon: widget.isEyeVisible == true
            ? Visibility(
                visible: widget.isEyeVisible,
                child: IconButton(
                    onPressed: () => setState(
                        () => widget.obscureText = !widget.obscureText),
                    icon: widget.obscureText
                        ? Icon(
                            Icons.visibility_outlined,
                            color: AppColors.black,
                            size: 25,
                          )
                        : Icon(
                            Icons.visibility_off_outlined,
                            color: AppColors.black,
                            size: 25,
                          )),
              )
            : widget.suffix,
        prefix: widget.prefix,
        border: OutlineInputBorder(
            borderRadius: BorderRadius.circular(widget.borderRadius ?? 5),
            borderSide: BorderSide(color: AppColors.darkBlue, width: 1),
            gapPadding: 40),
        enabledBorder: OutlineInputBorder(
            borderRadius: BorderRadius.circular(widget.borderRadius ?? 5),
            borderSide: BorderSide(color: AppColors.grey2, width: 1.5)),
        errorBorder: OutlineInputBorder(
            borderRadius: BorderRadius.circular(widget.borderRadius ?? 5),
            borderSide: const BorderSide(color: Colors.red, width: 1)),
        focusedBorder: OutlineInputBorder(
          borderRadius: BorderRadius.circular(widget.borderRadius ?? 5),
          borderSide: BorderSide(color: AppColors.darkBlue, width: 1),
        ),
      ),
      validator: (value) {
        if (widget.isOptional && value!.isEmpty) {
          return null;
        }

        if (value!.isEmpty) {
          return 'This input is empty';
        } else if (widget.keyBoardType == TextInputType.emailAddress) {
          String trimValue = widget.textController.text.trim();
          if (EmailValidator.validate(trimValue) == false) {
            return 'Not a valid email';
          }
        } else if (widget.keyBoardType == TextInputType.phone) {
          if (value.length != 10) {
            return 'Phone number must be 10 digits';
          }
        } else if (widget.keyBoardType == TextInputType.name) {
          if (value.length < 3) {
            return 'Not a valid name';
          }
        } else {
          return null;
        }
        return null;
      },
    );
  }
}
