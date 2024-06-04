using OpenAI;
using OpenAI.Interfaces;
using OpenAI.Managers;
using OpenAI.ObjectModels.RequestModels;
using OpenAI.Utilities.FunctionCalling;
using System.Collections.Concurrent;
using System.Globalization;
namespace ParkSharing.Services.ChatGPT
{
    public class Session
    {
        public string SessionId { get; set; }
        public string ExternalGPTThreadId { get; set; }
        public string ExternalGPTRunId { get; set; }
        public List<ChatMessage> Activities { get; set; }
    }

    public class SessionService
    {
        public ConcurrentDictionary<string, Session> Sessions = new ConcurrentDictionary<string, Session>();

        public Task<bool> ExistAsync(string id) => Task.FromResult(Sessions.ContainsKey(id));

        public Task<Session> GetAsync(string id) => Task.FromResult(Sessions.TryGetValue(id, out var session) ? session : null);

        public Task<Session> AddAsync(Session session)
        {
            Sessions[session.SessionId] = session;
            return Task.FromResult(session);
        }
    }

    public class ChatGPTService
    {
        private readonly IOpenAIService _openAI;
        private readonly ILogger<ChatGPTService> _logger;
        private readonly IReservationService _reservationService;
        private readonly SessionService _sessions;
        private readonly ChatGPTCapabilities _capabilities;

        public ChatGPTService(ILogger<ChatGPTService> logger, IReservationService reservationService, SessionService sessions, ChatGPTCapabilities capabilities)
        {
            _openAI = new OpenAIService(new OpenAiOptions
            {
                ApiKey = "PUT_SECRET_HERE",
                UseBeta = true
            });
            _logger = logger;
            _reservationService = reservationService;
            _sessions = sessions;
            _capabilities = capabilities;
        }

        public async Task<List<ChatMessage>> Send(List<ChatMessage> messages)
        {
            if (messages.LastOrDefault().Content.Contains("debug"))
            {
                var msgContent = messages.LastOrDefault().Content;
                if (msgContent.Contains("reservation"))
                {
                    var newMsg = await _capabilities.ReserveSpot("2024-03-21 11:00", "2024-03-21 14:00", "GS22", "123123123");
                    messages.Add(ChatMessage.FromAssistant(newMsg));
                }
                else if (msgContent.Contains("avaliable"))
                {
                    var newMsg = await _capabilities.AvaliableSpots("2024-03-21 11:00", "2024-03-21 14:00");
                    messages.Add(ChatMessage.FromAssistant(newMsg));
                }
                else
                {
                    messages.Add(ChatMessage.FromAssistant("Unknown command"));
                }
                return messages;
            }


            var toolDefinitions = FunctionCallingHelper.GetToolDefinitions(_capabilities);

            messages.Insert(0, ChatMessage.FromSystem("Pouzivas v odpovedich markdown. Český chatbot pro sdílení a rezervaci parkovacích míst. Umožňuje majitelům nabízet místa když je nepoužívají a ostatním je rezervovat a platit přes bankovní účet. Uživatelé mohou registrovat místa, nastavovat dostupnost a spravovat nabídky, rezervace omezeny na dvě denně. Komunikace v češtině, pokud uživatel mluvi jinou reci mluvi jinou reci. Nevyplňuj nejasné funkce bez dotazu. Pokud uzivatel zadal email, pouzij jej pro kazdej dotaz. Pokud uzivatel zadal platny kod, pouzij jej pro kazdy dotaz. Pokud chce uzivatel pridavat, menit, mazat svoje parkovaci misto musi se nejdrive identifikovat."));
            messages.Insert(0, ChatMessage.FromSystem($"Datum {DateTime.Now.ToString()}"));
            messages.Insert(0, ChatMessage.FromSystem($"Pokud chce nekdo rezervovat misto, vyber mu nahodne jedno znich a nech si potvrdit ze to je ok nez ho rezervujes."));
            messages.Insert(0, ChatMessage.FromSystem($"Podpora whatsapp 724 676 829"));
            messages.Insert(0, ChatMessage.FromSystem($"Vsechna parkovani jsou venkovni. Nic neni v garazi"));
            messages.Insert(0, ChatMessage.FromSystem($"FAQ: Penize prijdou na ucet majitele parkovaciho stani"));
            messages.Insert(0, ChatMessage.FromSystem($"Jsi assisten co pomaha s parkovanim. S nicim jinym nepomahas."));

            var req = new ChatCompletionCreateRequest
            {
                Tools = toolDefinitions,
                Messages = messages.ToList(),
                Model = "gpt_4o",
                MaxTokens = 200
            };

            ;
            var reply = await _openAI.ChatCompletion.CreateCompletion(req, "gpt-4o");
            if (!reply.Successful)
            {
                messages.Add(ChatMessage.FromAssistant("Ups! Něco se pokazilo"));
                Console.WriteLine(reply.Error?.Message);
                return req.Messages.ToList();
            }

            ChatMessage response = reply.Choices.First().Message;
            req.Messages.Add(response);



            if (response.ToolCalls?.Count > 0 && !string.IsNullOrEmpty(response.ToolCalls[0].FunctionCall.Name))
            {
                do
                {
                    string stringResponse = await ExecuteFunction(response);
                    req.Messages.Add(ChatMessage.FromTool(stringResponse, response.ToolCalls[0].Id));
                    reply = await _openAI.ChatCompletion.CreateCompletion(req, "gpt-4o");
                    response = reply.Choices.First().Message;
                    req.Messages.Add(response);
                } while (response.ToolCalls != null);
                req.Messages.Add(response); //Add answer from assistent

            }
            else
            {
                return req.Messages.ToList();
            }


            return req.Messages.ToList();
        }

        private async Task<string> ExecuteDebugFunction(ChatMessage response)
        {
            Console.WriteLine($"Invoking {response.ToolCalls[0].FunctionCall.Name}");
            var functionCall = response.ToolCalls[0].FunctionCall;
            var result = await FunctionCallingHelper.CallFunction<Task<string>>(functionCall, _capabilities);
            var stringResponse = result.ToString(CultureInfo.CurrentCulture);
            return stringResponse;
        }

        private async Task<string> ExecuteFunction(ChatMessage response)
        {
            Console.WriteLine($"Invoking {response.ToolCalls[0].FunctionCall.Name}");
            var functionCall = response.ToolCalls[0].FunctionCall;
            var result = await FunctionCallingHelper.CallFunction<Task<string>>(functionCall, _capabilities);
            var stringResponse = result.ToString(CultureInfo.CurrentCulture);
            return stringResponse;
        }
    }

    public class ChatGPTResponse
    {
        public string Content { get; set; }
        public FunctionCall FunctionCall { get; set; }
    }
}
