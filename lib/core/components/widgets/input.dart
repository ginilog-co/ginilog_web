// ignore_for_file: prefer_const_constructors, constant_identifier_names, annotate_overrides, overridden_fields, use_key_in_widget_constructors, must_be_immutable

import 'package:ginilog_customer_app/core/components/utils/package_export.dart';
import 'package:ginilog_customer_app/core/components/utils/size_config.dart';
import '../utils/colors.dart';

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

  const SerachInput({
    this.hintText,
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
    this.inputIcon,
  });

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
        fontSize: 25.textSize,
      ),
      decoration: InputDecoration(
        prefixIcon: prefix,
        focusedErrorBorder: OutlineInputBorder(
          borderRadius: BorderRadius.circular(5),
          borderSide: BorderSide(color: Colors.grey, width: 1.0),
        ),
        border: OutlineInputBorder(
          borderRadius: BorderRadius.circular(5),
          borderSide: const BorderSide(color: AppColors.black),
        ),
        errorBorder: OutlineInputBorder(
          borderRadius: BorderRadius.circular(5),
          borderSide: BorderSide(color: Colors.red, width: 2.0),
        ),

        fillColor:
            !enable
                ? fillColor ?? Color(0xFFeeeeee)
                : fillColor ?? Colors.transparent,
        // fillColor: Colors.white,
        filled: false,
        enabledBorder: OutlineInputBorder(
          borderRadius: BorderRadius.circular(5),
          borderSide: BorderSide(color: Colors.grey, width: 2.0),
        ),
        focusedBorder: OutlineInputBorder(
          borderRadius: BorderRadius.circular(5),
          borderSide: BorderSide(color: AppColors.grey, width: 2.0),
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
  final VoidCallback? onTap;
  final Widget? prefix;
  final Widget? suffix;
  final bool isNotePad;
  final String? confirmPasswordMatch;
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
    this.confirmPasswordMatch,
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
      onTap: widget.onTap,
      enabled: true,
      maxLines: widget.isNotePad ? 10 : 1,
      minLines: widget.isNotePad ? 6 : 1,
      autovalidateMode: AutovalidateMode.onUserInteraction,
      textAlign: widget.isCenterText ? TextAlign.center : TextAlign.start,
      textCapitalization:
          widget.keyBoardType == TextInputType.name
              ? TextCapitalization.sentences
              : TextCapitalization.none,
      inputFormatters: _buildInputFormatters(),
      style: TextStyle(
        color: AppColors.black,
        fontFamily: "Mulish",
        fontSize: 17.textSize,
      ),
      decoration: InputDecoration(
        floatingLabelBehavior: FloatingLabelBehavior.auto,
        labelText: widget.fieldName,
        isDense: true,
        contentPadding: EdgeInsets.symmetric(
          vertical: 2.heightAdjusted,
          horizontal: 2.widthAdjusted,
        ),
        suffixIcon:
            widget.isEyeVisible
                ? IconButton(
                  onPressed: () {
                    setState(() {
                      widget.obscureText = !widget.obscureText;
                    });
                  },
                  icon: Icon(
                    widget.obscureText
                        ? Icons.visibility_outlined
                        : Icons.visibility_off_outlined,
                    color: AppColors.black,
                    size: 25,
                  ),
                )
                : widget.suffix,
        prefix: widget.prefix,
        border: OutlineInputBorder(
          borderRadius: BorderRadius.circular(widget.borderRadius ?? 5),
          borderSide: BorderSide(color: AppColors.grey2, width: 0.5),
        ),
        enabledBorder: OutlineInputBorder(
          borderRadius: BorderRadius.circular(widget.borderRadius ?? 5),
          borderSide: BorderSide(color: AppColors.grey2, width: 0.5),
        ),
        errorBorder: OutlineInputBorder(
          borderRadius: BorderRadius.circular(widget.borderRadius ?? 5),
          borderSide: const BorderSide(color: Colors.red, width: 0.5),
        ),
        focusedBorder: OutlineInputBorder(
          borderRadius: BorderRadius.circular(widget.borderRadius ?? 5),
          borderSide: BorderSide(color: AppColors.primary, width: 0.5),
        ),
      ),
      validator: (value) => _validateField(value),
    );
  }

  List<TextInputFormatter> _buildInputFormatters() {
    List<TextInputFormatter> formatters = [];

    if (widget.removeSpace) {
      formatters.add(FilteringTextInputFormatter.deny(RegExp(r"\s\b|\b\s")));
    }

    if (widget.keyBoardType == TextInputType.phone) {
      formatters.add(FilteringTextInputFormatter.deny(RegExp(r'^0+')));
    }

    if (widget.keyBoardType == TextInputType.number) {
      formatters.add(FilteringTextInputFormatter.digitsOnly);
    }

    formatters.add(LengthLimitingTextInputFormatter(widget.maxLength));

    return formatters;
  }

  String? _validateField(String? value) {
    if (widget.isOptional && (value == null || value.isEmpty)) {
      return null;
    }

    if (value == null || value.isEmpty) {
      return 'This input is empty';
    }

    if (widget.obscureText) {
      // Confirm password match check (only for confirm password field)
      if (widget.confirmPasswordMatch != null &&
          value != widget.confirmPasswordMatch) {
        return 'Passwords do not match';
      }
    }

    if (widget.keyBoardType == TextInputType.emailAddress) {
      String trimmed = value.trim();
      if (!EmailValidator.validate(trimmed)) {
        return 'Not a valid email';
      }
    }

    if (widget.keyBoardType == TextInputType.phone && value.length != 10) {
      return 'Phone number must be 10 digits';
    }

    if (widget.keyBoardType == TextInputType.number) {
      final numberOnlyRegex = RegExp(r'^\d+$');
      if (!numberOnlyRegex.hasMatch(value)) {
        return 'Only digits are allowed. No special characters.';
      }
    }

    return null;
  }
}
