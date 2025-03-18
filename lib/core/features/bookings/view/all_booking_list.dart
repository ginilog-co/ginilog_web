import 'package:ginilog_customer_app/core/components/utils/colors.dart';
import 'package:ginilog_customer_app/core/components/utils/package_export.dart';
import 'package:ginilog_customer_app/core/components/widgets/app_text.dart';
import 'package:ginilog_customer_app/core/components/widgets/input.dart';
import 'package:ginilog_customer_app/core/features/bookings/model/accomodation_response_model.dart';
import 'package:ginilog_customer_app/core/features/bookings/widget/accomodation_item_widget.dart';

class BookingsListTab extends ConsumerStatefulWidget {
  final List<AccomodationResponseModel> allAccomodations;
  const BookingsListTab({super.key, required this.allAccomodations});

  @override
  ConsumerState<BookingsListTab> createState() => _BookingsListTabState();
}

class _BookingsListTabState extends ConsumerState<BookingsListTab> {
  List<String> locality = ["All"];
  String selectedLocality = "All";
  String searchQuery = "";
  List<AccomodationResponseModel> accommodations = [];
  List<AccomodationResponseModel> filteredAccomodations = [];

  @override
  void initState() {
    accommodations = widget.allAccomodations;
    super.initState();
    locality.addAll(
      accommodations.map((ticket) => ticket.accomodationType!).toSet().toList()
        ..sort(),
    );
    filteredAccomodations = accommodations;
  }

  void _filterByLocality(String locality) {
    setState(() {
      selectedLocality = locality;
      _applyFilters();
    });
  }

  void _applyFilters() {
    String query = searchQuery.toLowerCase();

    setState(() {
      filteredAccomodations = accommodations.where((ticket) {
        bool matchesLocality = selectedLocality == "All" ||
            ticket.accomodationType == selectedLocality;
        bool matchesSearch =
            ticket.accomodationName!.toLowerCase().contains(query);
        bool matchesState = ticket.state!.toLowerCase().contains(query);
        bool locality = ticket.locality!.toLowerCase().contains(query);
        bool matchType = ticket.accomodationType!.toLowerCase().contains(query);
        return matchesLocality &&
            (matchesSearch || matchesState || locality || matchType);
      }).toList();
    });
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
        backgroundColor: AppColors.white,
        body: SingleChildScrollView(
          child: Column(
            mainAxisAlignment: MainAxisAlignment.start,
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              accommodations.isEmpty
                  ? SizedBox.shrink()
                  : Padding(
                      padding: const EdgeInsets.all(8.0),
                      child: SerachInput(
                        hintText: "Accomodation Names or locations",
                        labelText: "",
                        readOnly: false,
                        prefixIcon: Icons.search,
                        prefix: Icon(
                          Icons.search,
                          size: 40,
                          color: AppColors.grey.withValues(),
                        ),
                        keyboard: TextInputType.text,
                        styleColor: AppColors.black,
                        labelColor: AppColors.black,
                        hintStyleColor: AppColors.black,
                        onChanged: (value) {
                          setState(() {
                            searchQuery = value!;
                          });
                          _applyFilters();
                        },
                        validator: (value) {
                          return null;
                        },
                        toggleEye: () {},
                        onSaved: (value) {},
                        onTap: () {},
                      ),
                    ),
              // Locality Filter Chips
              accommodations.isEmpty
                  ? SizedBox.shrink()
                  : SingleChildScrollView(
                      scrollDirection: Axis.horizontal,
                      padding: const EdgeInsets.symmetric(
                          horizontal: 10, vertical: 8),
                      child: Row(
                        children: locality.map((locality) {
                          bool isSelected = selectedLocality == locality;
                          return Padding(
                            padding: const EdgeInsets.symmetric(horizontal: 5),
                            child: ChoiceChip(
                              showCheckmark: false,
                              label: Text(locality),
                              selected: isSelected,
                              onSelected: (bool selected) {
                                if (selected) {
                                  _filterByLocality(locality);
                                }
                              },
                              selectedColor: AppColors.primary,
                              backgroundColor: AppColors.white,
                              side: BorderSide(
                                  color: isSelected
                                      ? Colors.transparent
                                      : AppColors.grey),
                              labelStyle: TextStyle(
                                color: isSelected ? Colors.white : Colors.black,
                                fontWeight: FontWeight.bold,
                              ),
                            ),
                          );
                        }).toList(),
                      ),
                    ),

              SingleChildScrollView(
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  mainAxisAlignment: MainAxisAlignment.start,
                  children: [
                    // Accomodations Section
                    if (filteredAccomodations.isNotEmpty)
                      Column(
                        crossAxisAlignment: CrossAxisAlignment.start,
                        children: [
                          Padding(
                            padding: EdgeInsets.all(10),
                            child: AppText(
                              isBody: false,
                              text: selectedLocality == "All"
                                  ? "Accomodations"
                                  : selectedLocality,
                              fontSize: 78,
                              fontWeight: FontWeight.bold,
                            ),
                          ),
                          ListView.builder(
                            itemCount: filteredAccomodations.length,
                            shrinkWrap: true,
                            physics: const NeverScrollableScrollPhysics(),
                            itemBuilder: (context, index) =>
                                AccomodationItemWidget(
                              dataModel: filteredAccomodations[index],
                            ),
                          ),
                        ],
                      ),

                    // No Results Found Message
                    if (filteredAccomodations.isEmpty)
                      Center(
                        child: Padding(
                          padding: const EdgeInsets.all(20.0),
                          child: Column(
                            children: [
                              const Icon(Icons.search_off,
                                  size: 50, color: AppColors.grey2),
                              const SizedBox(height: 10),
                              const AppText(
                                isBody: true,
                                text: "No matching results found",
                                fontSize: 16,
                                color: AppColors.black,
                                fontWeight: FontWeight.normal,
                              ),
                            ],
                          ),
                        ),
                      ),
                  ],
                ),
              )
            ],
          ),
        ));
  }
}
