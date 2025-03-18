// ignore_for_file: use_build_context_synchronously, library_private_types_in_public_api

import 'package:ginilog_customer_app/core/components/widgets/app_text.dart';
import 'package:ginilog_customer_app/core/features/home/model/company_response_model.dart';
import 'package:ginilog_customer_app/core/features/home/states/home_state.dart';
import 'package:ginilog_customer_app/core/features/home/widget/region_search_widget.dart';

import '../../../components/utils/colors.dart';
import '../../../components/utils/package_export.dart';

class CompanySelectPage extends ConsumerStatefulWidget {
  final String isSelectionType;

  const CompanySelectPage({
    super.key,
    required this.isSelectionType,
  });

  @override
  _CountryPageState createState() => _CountryPageState();
}

class _CountryPageState extends ConsumerState<CompanySelectPage> {
  String text = '';
  List<LogisticResponseModel> selectedLogistics = [];
  List<LogisticResponseModel> filteredLogistics = [];

  @override
  void initState() {
    super.initState();
    final accountProviderd = ref.read(homeProvider.notifier);
    accountProviderd.getAllLogisticsData();
    selectedLogistics = accountProviderd.allLogisticss
        .where((item) => item.deliveryTypes!.any((cat) =>
            cat.toLowerCase().contains(widget.isSelectionType.toLowerCase())))
        .toList();
    filteredLogistics = accountProviderd.allLogisticss
        .where((item) => item.deliveryTypes!.any((cat) =>
            cat.toLowerCase().contains(widget.isSelectionType.toLowerCase())))
        .toList();
  }

  void _filterItems(String query) {
    setState(() {
      filteredLogistics = selectedLogistics
          .where((item) =>
              item.companyName!.toLowerCase().contains(query.toLowerCase()))
          .toList();
    });
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: AppColors.white,
      appBar: AppBar(
        backgroundColor: AppColors.primary,
        automaticallyImplyLeading: false,
        toolbarHeight: 70,
        elevation: 0,
        title: SearchWidget(
          text: text,
          onChanged: _filterItems,
          hintText: 'Search State',
        ),
      ),
      body: ListView.separated(
        itemCount: filteredLogistics.length,
        separatorBuilder: (context, index) => Divider(),
        itemBuilder: (context, index) {
          return ListTile(
            leading: CircleAvatar(
              backgroundColor: AppColors.grey,
              backgroundImage:
                  NetworkImage(filteredLogistics[index].companyLogo!),
            ),
            title: AppText(
                isBody: true,
                text: "${filteredLogistics[index].companyName}",
                textAlign: TextAlign.start,
                fontSize: 70,
                color: AppColors.black.withAlpha(162),
                fontStyle: FontStyle.normal,
                fontWeight: FontWeight.w800),
            subtitle: AppText(
                isBody: false,
                text: "${filteredLogistics[index].companyInfo}",
                textAlign: TextAlign.start,
                fontSize: 65,
                overflow: TextOverflow.ellipsis,
                maxLines: 3,
                color: AppColors.black.withValues(alpha: 4),
                fontStyle: FontStyle.normal,
                fontWeight: FontWeight.w500),
            trailing:
                Icon(Icons.arrow_forward_ios, color: Colors.grey, size: 18),
            onTap: () {
              Navigator.pop(
                  context, filteredLogistics[index]); // Send back item
            },
          );
        },
      ),
    );
  }
}
