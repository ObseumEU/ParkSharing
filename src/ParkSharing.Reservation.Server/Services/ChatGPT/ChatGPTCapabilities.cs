using App.Context.Models;
using MassTransit;
using OpenAI.Utilities.FunctionCalling;
using System.Globalization;
using System.Text.Json;

namespace ParkSharing.Services.ChatGPT
{
    public class ChatGPTCapabilities
    {
        IReservationService _reservation;
        IBus _messageBroker;
        public ChatGPTCapabilities(IReservationService reservation, IBus messageBroker)
        {
            _reservation = reservation;
            _messageBroker = messageBroker;
        }

        [FunctionDescription("Rezervace parkovacího místa. Neni dovoleno rezervova na delsi dobu nez 3 dny. Navratova hodnota je Nazev parkovaciho mista. Rezervovat lze jen volna mista ziskane funkci AvaliableSpots. Sam vyber nahodne nektere misto")]
        public async Task<string> ReserveSpot(
            [ParameterDescription("Datetime format yyyy-mm-dd HH:00")] string from,
            [ParameterDescription("Datetime format yyyy-mm-dd HH:00")] string to, 
            string spotName,
            [ParameterDescription("Telefon pro kontakt")] string phone)
        {
            if (!TryParseDateTime(from, out DateTime fromDateTime))
            {
                return "Invalid 'from' date format.";
            }

            if (!TryParseDateTime(to, out DateTime toDateTime))
            {
                return "Invalid 'to' date format.";
            }

            var spot = await _reservation.GetParkingSpotByNameAsync(spotName);

            var totalPrice = spot.PricePerHour * (toDateTime - fromDateTime).Hours;

            var id = Guid.NewGuid().ToString();
            var result = await _reservation.ReserveAsync(spot.Name, new ReservationSpot()
            {
                Phone = phone,
                End = toDateTime,
                Start = fromDateTime,
                Price = (int)totalPrice,
                PublicId = id
            });

            if(result == false)
            {
                return $"Reservation not created, spot is already reserved for this time.";
            }

            return $"Reservation created ID:{id}";
        }

        [FunctionDescription("Vrací seznam volných parkovacích míst pro dané datum. Navratova hodnota je seznam volnych parkovacich mist.")]
        public async Task<string> AvaliableSpots(
            [ParameterDescription("Datetime format yyyy-mm-dd HH:00")] string from,
            [ParameterDescription("Datetime format yyyy-mm-dd HH:00")] string to)
        {
            if (!TryParseDateTime(from, out DateTime fromDateTime))
            {
                return "Invalid 'from' date format.";
            }

            if (!TryParseDateTime(to, out DateTime toDateTime))
            {
                return "Invalid 'to' date format.";
            }
            var freeSpots = await _reservation.GetAvailableSpotsAsync(fromDateTime, toDateTime);
            var result = new
            {
                spots = freeSpots.Select(s => new
                {
                    s.Name,
                    s.PricePerHour,
                    totalPrice = s.PricePerHour * (toDateTime - fromDateTime).Hours
                })
            };
            return JsonSerializer.Serialize(result);
        }

        private bool TryParseDateTime(string input, out DateTime dateTime)
        {
            string[] formats = { "yyyy-MM-dd HH:00" };
            return DateTime.TryParseExact(input, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime);
        }


        [FunctionDescription("Detail o parkovacim miste. Zobrazit vzdy pri potvrzeni rezervace")]
        public async Task<string> SpotDetail(string spot)
        {
            // Sanitize the input
            if (!Guid.TryParse(spot, out Guid spotGuid))
            {
                return JsonSerializer.Serialize(new { error = "Invalid spot identifier." });
            }

            // Get the parking spot details
            var parkingSpot = await _reservation.GetParkingSpotAsync(spotGuid);
            if (parkingSpot == null)
            {
                return JsonSerializer.Serialize(new { error = "Parking spot not found." });
            }

            // Prepare the result
            var result = new
            {
                Name = parkingSpot.Name,
                PricePerHour = parkingSpot.PricePerHour
            };

            // Serialize the result to JSON
            return JsonSerializer.Serialize(result);
        }
    }

    public class SpotDetails
    {
        public string Name { get; set; }
        public string BankAccount { get; set; }
        public string PricePerHour { get; set; }
    }
}
