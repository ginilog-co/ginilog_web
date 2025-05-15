using Genilog_WebApi.Model.BookingsModel;
using Genilog_WebApi.Model.LogisticsModel;
namespace Genilog_WebApi.Repository.PlacesRepo
{
    public interface IAccomodationRepository
    {
        Task<IEnumerable<AccomodationDataModel>> GetAllAsync();
        Task<AccomodationDataModel> GetAsync(Guid id);
        Task<AccomodationDataModel> AddAsync(AccomodationDataModel region);
        Task<AccomodationDataModel> DeleteAsync(Guid id);
        Task<AccomodationDataModel> UpdateAsync(Guid id, AccomodationDataModel region);
        Task<bool> AdminIdExistAsync(Guid adminId);
        Task<AccomodationReviewModel> AddAccomodationReviewAsync(AccomodationReviewModel region);
        Task<AccomodationReviewModel> DeleteAccomodationReviewAsync(Guid id);


        // Accomodation Chat
        Task<IEnumerable<AccomodationChatModel>> GetAllAccomodationChatAsync();
        Task<AccomodationChatModel> AddAccomodationChatAsync(AccomodationChatModel region);
        Task<AccomodationChatModel> DeleteAccomodationChatAsync(Guid id);
        Task<AccomodationChatModel> UpdateAccomodationChatAsync(Guid id, AccomodationChatModel region);
        Task<AccomodationChatModel> UpdateAccomodationIsReadChatAsync(Guid id, AccomodationChatModel region);

       
        // Reservations
        Task<IEnumerable<BookAccomodationReservatioModel>> GetAllBookAccomodationReservationAsync();
        Task<BookAccomodationReservatioModel> GetBookAccomodationReservationAsync(Guid id);
        Task<BookAccomodationReservatioModel> AddBookAccomodationReservationAsync(BookAccomodationReservatioModel region);
        Task<BookAccomodationReservatioModel> DeleteBookAccomodationReservationAsync(Guid id);
        Task<BookAccomodationReservatioModel> UpdateBookAccomodationReservationAsync(Guid id, BookAccomodationReservatioModel region);
        Task<BookAccomodationReservatioModel> BookAccomodationReservationRoomExistAsync(int RoomNumber, Guid accommodationId, BookAccomodationReservatioModel region);
       
        
        // Customer
        Task<IEnumerable<CustomerBookedReservation>> GetAllCustomerBookedReservationAsync();
        Task<CustomerBookedReservation> GetCustomerBookedReservationAsync(Guid id);
        Task<CustomerBookedReservation> AddCustomerBookedReservationAsync(CustomerBookedReservation region);
        Task<CustomerBookedReservation> DeleteCustomerBookedReservationAsync(Guid id);
        Task<CustomerBookedReservation> UpdateCustomerBookedReservationAsync(Guid id, CustomerBookedReservation region);
        public bool CustomerBookedReservationDateExistAsync(DateTime startDate, DateTime endDate, List<CustomerBookedReservation> existingReservations);

    }
}
