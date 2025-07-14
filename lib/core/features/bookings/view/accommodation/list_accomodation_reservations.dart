import 'package:ginilog_customer_app/core/components/utils/colors.dart';
import 'package:ginilog_customer_app/core/components/utils/package_export.dart';
import 'package:ginilog_customer_app/core/components/utils/size_config.dart';
import 'package:ginilog_customer_app/core/components/widgets/app_text.dart';
import 'package:ginilog_customer_app/core/components/widgets/input.dart';
import 'package:ginilog_customer_app/core/features/bookings/model/accomodation_reservations_response_model.dart';
import 'package:ginilog_customer_app/core/features/bookings/widget/accomodation_reservation_widget.dart';

class AccomodationReservationList extends ConsumerStatefulWidget {
  const AccomodationReservationList({
    super.key,
    required this.accomodationReservations,
  });
  final List<AccomodationReservationResponseModel> accomodationReservations;

  @override
  ConsumerState<AccomodationReservationList> createState() =>
      _AccomodationReservationListState();
}

class _AccomodationReservationListState
    extends ConsumerState<AccomodationReservationList> {
  List<AccomodationReservationResponseModel> accomodationReservations = [];
  List<AccomodationReservationResponseModel> filteredTickets = [];
  List<String> locality = ["All"];
  String selectedLocality = "All";
  String searchQuery = "";
  @override
  void initState() {
    super.initState();
    accomodationReservations = widget.accomodationReservations;

    // Extract unique locality from tickets
    locality.addAll(
      widget.accomodationReservations
          .map((ticket) => ticket.accomodationType!)
          .toSet()
          .toList()
        ..sort(),
    );

    filteredTickets = widget.accomodationReservations;
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
      filteredTickets =
          widget.accomodationReservations.where((ticket) {
            bool matchesLocality =
                selectedLocality == "All" ||
                ticket.accomodationType == selectedLocality;
            bool matchesSearch = ticket.accomodationName!
                .toLowerCase()
                .contains(query);
            bool matchesState = ticket.accomodationState!
                .toLowerCase()
                .contains(query);
            bool locality = ticket.accomodationLocality!.toLowerCase().contains(
              query,
            );
            bool matchType = ticket.accomodationType!.toLowerCase().contains(
              query,
            );
            return matchesLocality &&
                (matchesSearch || matchesState || locality || matchType);
          }).toList();
    });
  }

  @override
  Widget build(BuildContext context) {
    return Column(
      mainAxisAlignment: MainAxisAlignment.start,
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        accomodationReservations.isEmpty
            ? SizedBox.shrink()
            : Padding(
              padding: const EdgeInsets.all(8.0),
              child: SerachInput(
                hintText: "Type an accomodation name or location here",
                labelText: "",
                readOnly: false,
                prefixIcon: Icons.search,
                prefix: Icon(
                  Icons.search,
                  size: 20,
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
        accomodationReservations.isEmpty
            ? SizedBox.shrink()
            : SingleChildScrollView(
              scrollDirection: Axis.horizontal,
              padding: const EdgeInsets.symmetric(horizontal: 10, vertical: 8),
              child: Row(
                children:
                    locality.map((locality) {
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
                            color:
                                isSelected
                                    ? Colors.transparent
                                    : AppColors.grey,
                          ),
                          labelStyle: TextStyle(
                            color: isSelected ? Colors.white : Colors.black,
                            fontWeight: FontWeight.bold,
                          ),
                        ),
                      );
                    }).toList(),
              ),
            ),

        filteredTickets.isEmpty
            ? Center(
              child: Column(
                mainAxisAlignment: MainAxisAlignment.center,
                crossAxisAlignment: CrossAxisAlignment.center,
                children: [
                  addVerticalSpacing(5),
                  const AppText(
                    isBody: false,
                    text: "Nothing to show here",
                    textAlign: TextAlign.start,
                    fontSize: 18,
                    color: AppColors.black,
                    fontStyle: FontStyle.normal,
                    fontWeight: FontWeight.bold,
                  ),
                  const AppText(
                    isBody: true,
                    text:
                        "We don't have any Accomodation Reservations at the moment",
                    textAlign: TextAlign.center,
                    fontSize: 15,
                    color: AppColors.black,
                    fontStyle: FontStyle.normal,
                    fontWeight: FontWeight.normal,
                  ),
                ],
              ),
            )
            : Expanded(
              child: ListView.builder(
                itemCount: filteredTickets.length,
                shrinkWrap: true,
                physics: const ScrollPhysics(),
                itemBuilder:
                    (context, index) => AccomodationReservationWidget(
                      accomodationReservation: filteredTickets[index],
                    ),
              ),
            ),
      ],
    );
  }
}
