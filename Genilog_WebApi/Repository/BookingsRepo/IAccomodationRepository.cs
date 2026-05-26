using Genilog_WebApi.Model;
using Genilog_WebApi.Model.BookingsModel;
using Genilog_WebApi.Model.GeneraModel;
namespace Genilog_WebApi.Repository.BookingsRepo
{
    public interface IAccomodationRepository
    {
        // Accomodation
        Task<IEnumerable<AccomodationDataModel>> GetAllAsync();
        Task<PageModel<AccomodationDataModelDto>> GetAllPaginationsAccomodationAsync(FilterLocationData filter);
        Task<AccomodationDataModel> GetAsync(Guid id);
        Task<AccomodationDataModel> AddAsync(AccomodationDataModel region);
        Task<AccomodationDataModel> DeleteAsync(Guid id);
        Task<AccomodationDataModel> UpdateAsync(Guid id, AccomodationDataModel region);
        Task<bool> AdminIdExistAsync(Guid adminId);

        // ACCOMMODATION REVIEW
        Task<AccomodationReviewModel> AddAccomodationReviewAsync(AccomodationReviewModel region);
        Task<IEnumerable<AccomodationReviewModel>> GetAccomodationReviewByIdAsync(Guid accomodationId);
        Task<AccomodationReviewModel> DeleteAccomodationReviewAsync(Guid id);


        // Accomodation Chat
        Task<IEnumerable<AccomodationChatModel>> GetAllAccomodationChatAsync();
        Task<IEnumerable<AccomodationChatModel>> GetAllAccomodationChatByIdAsync(Guid accomodationId);
        Task<AccomodationChatModel> AddAccomodationChatAsync(AccomodationChatModel region);
        Task<AccomodationChatModel> DeleteAccomodationChatAsync(Guid id);
        Task<AccomodationChatModel> UpdateAccomodationChatAsync(Guid id, AccomodationChatModel region);
        Task<AccomodationChatModel> UpdateAccomodationIsReadChatAsync(Guid id, AccomodationChatModel region);


        // Book Accomodation Reservation
        Task<IEnumerable<BookAccomodationReservatioModel>> GetAllBookAccomodationReservationAsync();
        Task<PageModel<BookAccomodationReservatioModelDto>> GetAllPageBookAccomodationReservationAsync(FilterLocationData filter);
        Task<BookAccomodationReservatioModel> GetBookAccomodationReservationAsync(Guid id);
        Task<BookAccomodationReservatioModel> AddBookAccomodationReservationAsync(BookAccomodationReservatioModel region);
        Task<BookAccomodationReservatioModel> DeleteBookAccomodationReservationAsync(Guid id);
        Task<BookAccomodationReservatioModel> UpdateBookAccomodationReservationAsync(Guid id, BookAccomodationReservatioModel region);
        Task<BookAccomodationReservatioModel> BookAccomodationReservationRoomExistAsync(int RoomNumber, Guid accommodationId, BookAccomodationReservatioModel region);


        // Customer Booked Reservation
        Task<IEnumerable<CustomerBookedReservation>> GetAllCustomerBookedReservationAsync();
        Task<PageModel<CustomerBookedReservationDto>> GetAllPageCustomerBookedReservationAsync(FilterLocationData filter);
        Task<CustomerBookedReservation> GetCustomerBookedReservationAsync(Guid id);
        Task<CustomerBookedReservation?> GetCustomerBookedReservationByTicketNumAsync(string ticketNum);
        Task<CustomerBookedReservation> AddCustomerBookedReservationAsync(CustomerBookedReservation region);
        Task<CustomerBookedReservation> DeleteCustomerBookedReservationAsync(Guid id);
        Task<CustomerBookedReservation> UpdateCustomerBookedReservationAsync(Guid id, CustomerBookedReservation region);
        public bool CustomerBookedReservationDateExistAsync(DateTime startDate, DateTime endDate, List<CustomerBookedReservation> existingReservations);

    }
}
