using App.Context.Models;
using MassTransit;
using OpenAI.Utilities.FunctionCalling;
using System;
using System.Globalization;
using System.Runtime.InteropServices;
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

        [FunctionDescription("Rezervace parkovacího místa. Neni dovoleno rezervova na delsi dobu nez 3 dny, neni dovoleno rezervovat misto pokud neni volne. Navratova hodnota je Nazev parkovaciho mista a celkova cena. Rezervovat lze jen volna mista ziskane funkci AvaliableSpots. Sam vyber nahodne nektere misto. Povolene jsou rezervovat jen cele hodiny")]
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

            fromDateTime = DateTime.SpecifyKind(fromDateTime, DateTimeKind.Local).ToUniversalTime();
            toDateTime = DateTime.SpecifyKind(toDateTime, DateTimeKind.Local).ToUniversalTime();

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

            if (result == false)
            {
                return $"Reservation not created, spot is already reserved for this time.";
            }

            return $"Reservation created TotalPrice:{totalPrice} BankAccount To pay:{spot.BankAccount}";
        }

        [FunctionDescription("Tata metoda vrací možné volné termíny a jejich cenu za hodinu. Povolene jsou jen cele hodiny, například od 13:00 do 15:00. Pokud je zdarma napiš to. Návratová hodnota možnosti výběru")]
        public async Task<string> GetAllOpenSlots(
          [ParameterDescription("Datetime format yyyy-mm-dd HH:00")] string from,
          [ParameterDescription("Datetime format yyyy-mm-dd HH:00")] string to)
        {
            TimeZoneInfo cetZone;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                cetZone = TimeZoneInfo.FindSystemTimeZoneById("W. Europe Standard Time");
            }
            else
            {
                cetZone = TimeZoneInfo.FindSystemTimeZoneById("Europe/Berlin");
            }

            if (!TryParseDateTime(from, out DateTime fromDateTime))
            {
                return "Invalid 'from' date format.";
            }

            if (!TryParseDateTime(to, out DateTime toDateTime))
            {
                return "Invalid 'to' date format.";
            }

            fromDateTime = DateTime.SpecifyKind(fromDateTime, DateTimeKind.Local).ToUniversalTime();
            toDateTime = DateTime.SpecifyKind(toDateTime, DateTimeKind.Local).ToUniversalTime();

            if ((toDateTime - fromDateTime).Days > 3)
            {
                return "From - to range is too big. Max search range 4 days.";
            }

            var freeSlots = await _reservation.GetAllOpenSlots(fromDateTime, toDateTime);


            var res = freeSlots.Select(f =>
            {
                var fromCET = f.From.ToLocalTime();
                var toCET = f.To.ToLocalTime();
                var result = new
                {
                    From = fromCET.ToString("dd MMM yyyy HH:mm"),
                    To = toCET.ToString("dd MMM yyyy HH:mm"),
                    SpotName = f.SpotName,
                    PricePerHour = f.PricePerHour
                };
                return result;
            }).ToList();

            if (res.Count == 0)
            {
                return "Not found";
            }

            var serialized = JsonSerializer.Serialize(new
            {
                Options = res
            });
            return serialized;
        }

        private bool TryParseDateTime(string input, out DateTime dateTime)
        {
            string[] formats = { "yyyy-MM-dd H:mm", "yyyy-MM-dd HH:mm" };
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
