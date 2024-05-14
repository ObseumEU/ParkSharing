using OpenAI.Utilities.FunctionCalling;
using System.Text.Json;

namespace ParkingReservationApp.Services.ChatGPT
{
    public class ChatGPTCapabilities
    {
        [FunctionDescription("Rezervace parkovacího místa. Neni dovoleno rezervova na delsi dobu nez 3 dny. Argumenty pouzivej v formatu yyyy-mm-dd HH:00. Navratova hodnota je Nazev parkovaciho mista. Rezervovat lze jen volna mista ziskane funkci AvaliableSpots. Sam vyber nahodne nektere misto")]
        public string ReserveSpot(string from, string to, string spot)
        {
            Console.WriteLine("Reserved spot from {0} to {1}", from, to);
            return $"CS{Random.Shared.Next(1,100)}";
        }

        [FunctionDescription("Vrací seznam volných parkovacích míst pro dané datum. Navratova hodnota je seznam volnych parkovacich mist.")]
        public string AvaliableSpots(string from, string to)
        {
            return $"CS222;CS452;CS26;CS122";
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
    }

    public class SpotDetails
    {
        public string Name { get; set; }
        public string BankAccount { get; set; }
        public string PricePerHour { get; set; }
    }
}
