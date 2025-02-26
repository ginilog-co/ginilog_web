import 'package:ginilog_customer_app/core/components/utils/colors.dart';
import 'package:ginilog_customer_app/core/components/utils/helper_functions.dart';
import 'package:ginilog_customer_app/core/components/utils/package_export.dart';
import 'package:ginilog_customer_app/core/components/utils/size_config.dart';
import 'package:ginilog_customer_app/core/components/widgets/app_text.dart';
import 'package:ginilog_customer_app/core/components/widgets/back_icon.dart';
import 'package:ginilog_customer_app/core/features/bookings/model/flight_ticket_response_model.dart';
import 'package:ginilog_customer_app/core/features/bookings/state/booking_state.dart';
import 'package:ginilog_customer_app/core/features/bookings/widget/flight_ticket_widget.dart';

class FlightTicketList extends ConsumerStatefulWidget {
  const FlightTicketList({super.key});

  @override
  ConsumerState<FlightTicketList> createState() => _FlightTicketListState();
}

class _FlightTicketListState extends ConsumerState<FlightTicketList> {
  List<FlightTicketResponseModel> flightTickets = [];
  List<FlightTicketResponseModel> filteredTickets = [];
  List<String> destinations = ["All"];
  String selectedDestination = "All";
  @override
  void initState() {
    super.initState();

    final bookingsProvider = ref.read(bookingProvider.notifier);
    bookingsProvider.getAllAirlineData();
    bookingsProvider.getAllAccomodationData();
    bookingsProvider.getAllFlightTicketData();
    flightTickets = bookingsProvider.allFlightTickets;

    // Extract unique destinations from tickets
    destinations.addAll(
      flightTickets.map((ticket) => ticket.destination!).toSet(),
    );

    filteredTickets = flightTickets;
  }

  void _filterByDestination(String destination) {
    setState(() {
      selectedDestination = destination;
      if (destination == "All") {
        filteredTickets = flightTickets;
      } else {
        filteredTickets = flightTickets
            .where((ticket) => ticket.destination == destination)
            .toList();
      }
    });
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
        backgroundColor: AppColors.white,
        appBar: PreferredSize(
            preferredSize: Size.fromHeight(SizeConfig.heightAdjusted(12)),
            child: Padding(
              padding: EdgeInsets.only(top: SizeConfig.heightAdjusted(10)),
              child: Column(
                children: [
                  const GlobalBackButton(
                      backText: 'Flight Tickets', showBackButton: true),
                ],
              ),
            )),
        body: SingleChildScrollView(
          child: Column(
            mainAxisAlignment: MainAxisAlignment.start,
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              // Destination Filter Chips
              filteredTickets.isEmpty
                  ? SizedBox.shrink()
                  : SingleChildScrollView(
                      scrollDirection: Axis.horizontal,
                      padding: const EdgeInsets.symmetric(
                          horizontal: 10, vertical: 8),
                      child: Row(
                        children: destinations.map((destination) {
                          bool isSelected = selectedDestination == destination;
                          return Padding(
                            padding: const EdgeInsets.symmetric(horizontal: 5),
                            child: ChoiceChip(
                              showCheckmark: false,
                              label: Text(destination),
                              selected: isSelected,
                              onSelected: (bool selected) {
                                if (selected) {
                                  _filterByDestination(destination);
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

              filteredTickets.isEmpty
                  ? Center(
                      child: Column(
                        mainAxisAlignment: MainAxisAlignment.center,
                        crossAxisAlignment: CrossAxisAlignment.center,
                        children: [
                          addVerticalSpacing(context, 55),
                          const AppText(
                              isBody: false,
                              text: "Nothing to show here",
                              textAlign: TextAlign.start,
                              fontSize: 78,
                              color: AppColors.black,
                              fontStyle: FontStyle.normal,
                              fontWeight: FontWeight.bold),
                          const AppText(
                              isBody: true,
                              text:
                                  "You don't have any Flight Ticket at the moment",
                              textAlign: TextAlign.center,
                              fontSize: 70,
                              color: AppColors.black,
                              fontStyle: FontStyle.normal,
                              fontWeight: FontWeight.normal),
                        ],
                      ),
                    )
                  : ListView.builder(
                      itemCount: filteredTickets.length,
                      shrinkWrap: true,
                      physics: const NeverScrollableScrollPhysics(),
                      itemBuilder: (context, index) => FlightTicketWidget(
                            flightTicket: filteredTickets[index],
                          ))
            ],
          ),
        ));
  }
}
