import 'package:ginilog_customer_app/core/components/widgets/app_text.dart';

import '../utils/colors.dart';
import '../utils/package_export.dart';

class DropdownFor extends StatelessWidget {
  const DropdownFor({
    super.key,
    required this.hint,
    // required this.selected,
    required this.itemList,
    this.selectDrop,
    required this.selectedItem,
    this.validator,
    this.borderRadius,
    this.borderColor = Colors.grey,
    this.hintStyleColor,
    this.enable = true,
    this.fillColor,
  });

  final String hint;
  //final String selected;
  final String? selectedItem;
  final List<String> itemList;
  final Function(String?)? selectDrop;
  final String? Function(String?)? validator;
  final double? borderRadius;
  final Color? borderColor;
  final Color? hintStyleColor;
  final bool enable;
  final Color? fillColor;

  @override
  Widget build(BuildContext context) {
    return DropdownButtonHideUnderline(
      child: DropdownButtonFormField2<String>(
        style: TextStyle(
          color: AppColors.black,
          fontFamily: "Montserrat",
          fontSize: fontSized(context, 85),
        ),
        validator: (value) => value == null ? 'Choose an item' : null,
        decoration: InputDecoration(
          //suffixIcon: suffix,
          fillColor: !enable
              ? fillColor ?? Color(0xFFeeeeee)
              : fillColor ?? Colors.transparent,
          // fillColor: Colors.white,
          filled: true,
          hintText: hint,
          hintStyle: TextStyle(
            color: hintStyleColor ?? Colors.grey,
            fontSize: fontSized(context, 22),
          ),
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
        ),

        value: selectedItem, //controller.selectedCategory,
        onChanged: selectDrop!,
        items: itemList.map((value) {
          return DropdownMenuItem<String>(
            value: value,
            child: Text(
              value,
              style: TextStyle(
                color: AppColors.black,
                fontFamily: "Montserrat",
                fontSize: fontSized(context, 85),
              ),
            ),
          );
        }).toList(),
      ),
    );
  }
}
