using OpenAI.Utilities.FunctionCalling;
using System.Text.Json;

namespace ParkSharing.Services.ChatGPT
{
    public class ChatGPTCapabilities
    {

        [FunctionDescription("Rezervace parkovacího místa. Neni dovoleno rezervova na delsi dobu nez 3 dny. Navratova hodnota je Nazev parkovaciho mista. Rezervovat lze jen volna mista ziskane funkci AvaliableSpots. Sam vyber nahodne nektere misto")]
        public string ReserveSpot(
            [ParameterDescription("Datetime format yyyy-mm-dd HH:00")] string from,
            [ParameterDescription("Datetime format yyyy-mm-dd HH:00")] string to, 
            string spot)
        {
            Console.WriteLine("Reserved spot from {0} to {1}, price 0 Kč/hod", from, to);
            return $"CS{Random.Shared.Next(1,100)}";
        }

        [FunctionDescription("Vrací seznam volných parkovacích míst pro dané datum. Navratova hodnota je seznam volnych parkovacich mist.")]
        public string AvaliableSpots(string from, string to)
        {
            return $"CS222;20 Kč/hod\n;" +
                   $"CS452,0 Kč/hod\n;" +
                   $"CS26,0 Kč/hod\n;" +
                   $"CS122,250 Kč/hod\n";
        }


        [FunctionDescription("Detail o parkovacim miste")]
        public string SpotDetail(string spot)
        {
            return JsonSerializer.Serialize(new SpotDetails()
            {
                BankAccount = "asdsad",
                Name = "asdsad",
                PricePerHour = "asdsad"
            });
        }


        [FunctionDescription("Registrovat parkovaci misto pro sdileni. Vlastnik musi pouzit svuj unikatni kod. Email je identifikator uzivatele co pozadavek zpracovava. Po vytvoreni je misto plne dostupne.")]
        public string CreateParkingSpot(string email, string securityCode, string spotId, string pricePerHour)
        {
            if(securityCode == "1234")
            {
                return "Created";
            }
            else
            {
                return "Not authorized. Wrong code.";
            }
        }

        [FunctionDescription("Odstranit parkovaci misto. Email je identifikator uzivatele co pozadavek zpracovava. Email je identifikator uzivatele co pozadavek zpracovava.")]
        public string DeleteParkingPlace(string email, string securityCode, string spotId)
        {
            if (securityCode == "1234")
            {
                return "Created";
            }
            else
            {
                return "Not authorized. Wrong code.";
            }
        }

        [FunctionDescription("Upravit dostupnost parkovaciho mista. Uzivatel musi zadat security code a ownerId. Email je identifikator uzivatele co pozadavek zpracovava.")]
        public string ChangeAvaliabilityOdParkingSpot(string email, string securityCode, string spotId)
        {
            if (securityCode == "1234")
            {
                return "Created";
            }
            else
            {
                return "Not authorized. Wrong code.";
            }
        }

        [FunctionDescription("Overit credential uzivatele. ")]
        public string AuthenticateUser(string email, string securityCode)
        {
            if (securityCode == "1234")
            {
                return "Authorized";
            }
            else
            {
                return "Not authorized. Wrong code.";
            }
        }
    }

    public class SpotDetails
    {
        public string Name { get; set; }
        public string BankAccount { get; set; }
        public string PricePerHour { get; set; }
    }
}
