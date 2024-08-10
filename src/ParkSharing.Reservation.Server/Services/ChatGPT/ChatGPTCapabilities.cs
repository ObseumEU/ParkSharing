using MassTransit;
using OpenAI.Utilities.FunctionCalling;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace ParkSharing.Services.ChatGPT
{
    public class ChatGPTCapabilities
    {
        IReservationService _reservation;
        IBus _messageBroker;
        ILogger<ChatGPTCapabilities> _log;

        public ChatGPTCapabilities(IReservationService reservation, IBus messageBroker, ILogger<ChatGPTCapabilities> log)
        {
            _reservation = reservation;
            _messageBroker = messageBroker;
            _log = log;
        }

        public static bool IsValidPhoneNumber(string phoneNumber)
        {
            // Normalize the phone number by removing leading and trailing whitespaces
            phoneNumber = phoneNumber.Trim();

            // Regex pattern for Czech and Slovak phone numbers
            string pattern = @"^(\+420|\+421)?\s?\d{3}\s?\d{3}\s?\d{3}$";

            Regex regex = new Regex(pattern);
            return regex.IsMatch(phoneNumber);
        }

        [FunctionDescription("Rezervace parkovacího místa. Neni dovoleno rezervova na delsi dobu nez 3 dny, neni dovoleno rezervovat misto pokud nebyla overena jeho dostupnost metodou GetAllOpenSlots. Navratova hodnota je Nazev parkovaciho mista a celkova cena. Povolene jsou rezervovat jen cele hodiny. Telefonní číslo je povinné a musí jej uživatel zadat.")]
        public async Task<string> ReserveSpot(
            [ParameterDescription("Datetime format yyyy-mm-dd HH:00", Required = true)] string from,
            [ParameterDescription("Datetime format yyyy-mm-dd HH:00", Required = true)] string to,
            string spotName,
            [ParameterDescription("Telefon pro kontakt najemce", Required = true)] string phone)
        {
            if (!TryParseDateTime(from, out DateTime fromDateTime))
            {
                _log.LogWarning($"{from} Invalid From Date");

                return "Invalid 'from' date format.";
            }

            if (!TryParseDateTime(to, out DateTime toDateTime))
            {
                _log.LogWarning($"{to} Invalid To Date");

                return "Invalid 'to' date format.";
            }

            if (!IsValidPhoneNumber(phone))
            {
                _log.LogWarning($"{phone} Invalid Phone Number");
                return "Invalid Phone Number. Valid is for example 724 666 854";
            }

            //fromDateTime = DateTime.SpecifyKind(fromDateTime, DateTimeKind.Local).ToUniversalTime();
            //toDateTime = DateTime.SpecifyKind(toDateTime, DateTimeKind.Local).ToUniversalTime();

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

            return $"Reservation created TotalPrice:{totalPrice} BankAccount To pay:{spot.BankAccount} Owner Phone:{spot.Phone}";
        }

        [FunctionDescription("Tata metoda vrací možné volné termíny a jejich cenu za hodinu. Povolene jsou jen cele hodiny, například od 13:00 do 15:00. Pokud je zdarma napiš to. Návratová hodnota jsou možnosti výběru. Nelze rezervovat více slotů najednou.")]
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

            fromDateTime = DateTime.SpecifyKind(fromDateTime, DateTimeKind.Local);
            toDateTime = DateTime.SpecifyKind(toDateTime, DateTimeKind.Local);

            if (fromDateTime < DateTime.Now || toDateTime < DateTime.Now)
            {
                return "Cannot reserve into history";
            }

            if ((toDateTime - fromDateTime).Days > 3)
            {
                return "From - to range is too big. Max search range 4 days.";
            }

            var freeSlots = await _reservation.GetAllOpenSlots(fromDateTime.AddDays(-1), toDateTime.AddDays(1));


            var res = freeSlots.Select(f =>
            {
                var fromCET = f.From;
                var fromCETStr = fromCET.TimeOfDay == new TimeSpan(0, 0, 0) ? fromCET.AddSeconds(1).ToString("dd MMM yyyy HH:mm") : fromCET.ToString("dd MMM yyyy HH:mm");
                var toCET = f.To;
                var toCETStr = toCET.TimeOfDay == new TimeSpan(0, 0, 0) ? toCET.AddSeconds(1).ToString("dd MMM yyyy HH:mm") : toCET.ToString("dd MMM yyyy HH:mm");

                var result = new
                {
                    From = fromCETStr,
                    To = toCETStr,
                    SpotName = f.SpotName,
                    PricePerHour = f.PricePerHour,
                    FromDayOfWeek = fromCET.DayOfWeek.ToString(),
                    ToDayOfWeek = toCET.DayOfWeek.ToString()
                };
                return result;
            }).ToList();

            if (res.Count == 0)
            {
                return "Not found";
            }

            var serialized = JsonSerializer.Serialize(new
            {
                NotOcupiedTimesOptions = res
            });
            return serialized;
        }

        public bool TryParseDateTime(string input, out DateTime dateTime)
        {
            string[] formats = { "yyyy-MM-dd H:mm", "yyyy-MM-dd HH:mm" };
            return DateTime.TryParseExact(input, formats, CultureInfo.CurrentCulture, DateTimeStyles.None, out dateTime);
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
