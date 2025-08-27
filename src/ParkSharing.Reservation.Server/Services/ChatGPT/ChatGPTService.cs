using Microsoft.Extensions.Options;
using OpenAI;
using OpenAI.Interfaces;
using OpenAI.Managers;
using OpenAI.ObjectModels.RequestModels;
using OpenAI.ObjectModels.ResponseModels;
using OpenAI.Utilities.FunctionCalling;
using ParkSharing.Services.ChatGPT.Helpers;
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

    public class ChatGPTSessionService
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
        private readonly ChatGPTSessionService _sessions;
        private readonly ChatGPTCapabilities _capabilities;
        private readonly IOptions<ChatGPTClientOptions> _options;

        public ChatGPTService(IOptions<ChatGPTClientOptions> options, ILogger<ChatGPTService> logger, IReservationService reservationService, ChatGPTSessionService sessions, ChatGPTCapabilities capabilities)
        {
            _openAI = new OpenAIService(new OpenAiOptions
            {
                ApiKey = options.Value.ApiKey,
                UseBeta = true
            });
            _logger = logger;
            _reservationService = reservationService;
            _sessions = sessions;
            _capabilities = capabilities;
            _options = options;
        }

        public async Task<List<ChatMessage>> CreateConversation()
        {
            List<ChatMessage> conversation = new List<ChatMessage>();
            return conversation;
        }

        public async Task<List<ChatMessage>> Send(List<ChatMessage> conversation, string newMessageFromUser)
        {
            //Create conversation if its empty
            if (conversation == null || conversation.Count == 0)
            {
                conversation = await CreateConversation();
            }

            conversation.AddUserMessage(newMessageFromUser);

            if (conversation.LastOrDefault().Content.Contains("debug"))
            {
                return await DebugMessage(conversation);
            }
                
            var response = await GetResponse(conversation);
            conversation.Add(response);

            do
            {

                if (conversation.IsToolCalls())
                {
                    var toolCalls = conversation.GetToolCalls();

                    foreach (var toolCall in toolCalls) //Call multiple call if required
                    {
                        var functionResult = await ExecuteFunction(toolCall.FunctionCall);
                        conversation.AddResponseFromFunction(functionResult, toolCall.Id);
                    }

                    var functionConversationResponse = await GetResponse(conversation);
                    conversation.Add(functionConversationResponse);
                }
            }
            while (conversation.IsToolCalls()); //Loop until all Tool Call not resolved

            return conversation;
        }

        private List<ToolDefinition> GetCapabilities()
        {
            return FunctionCallingHelper.GetToolDefinitions(_capabilities);
        }

        private async Task<ChatMessage> GetResponse(List<ChatMessage> messages)
        {
            var cleanedActivities = messages.ToList();

            for (int i = 0; i < cleanedActivities.Count; i++)
            {
                if (cleanedActivities[0].ToolCalls != null || !string.IsNullOrEmpty(cleanedActivities[0].ToolCallId))
                {
                    cleanedActivities.RemoveAt(0);
                    i--;
                }
                else
                {
                    break;
                }
            }

            var req = new ChatCompletionCreateRequest
            {
                Tools = GetCapabilities(),
                Messages = AddChatDescription(cleanedActivities.ToList()),
                Model = _options.Value.Model,
                MaxTokens = 200
            };

            var reply = await _openAI.ChatCompletion.CreateCompletion(req, "gpt-5");
            if (!reply.Successful)
            {
                _logger.LogError(System.Text.Json.JsonSerializer.Serialize(req) + reply.Error?.Message);
                return ChatMessage.FromAssistant("Ups! Něco se pokazilo");
            }
            _logger.LogInformation($"Request ChatGPT: {System.Text.Json.JsonSerializer.Serialize(req)} Reply: {System.Text.Json.JsonSerializer.Serialize(reply)}");
            ChatMessage response = reply.Choices.First().Message;
            return response;
        }

        private List<ChatMessage> AddChatDescription(List<ChatMessage> messages)
        {
            var newList = messages.ToList();


            newList.Insert(0, ChatMessage.FromSystem($"Datum a čas: {DateTime.Now.ToString("dd. MMMM yyyy")}"));
            newList.Insert(0, ChatMessage.FromSystem("Pomáháš s rezervací venkovních parkovacích míst, nic jiného."));
            newList.Insert(0, ChatMessage.FromSystem(
                "Rezervace:" +
                " Jedno místo najednou." +
                " Náhodně vyber volné místo a potvrď s uživatelem před rezervací." +
                " Max dvě rezervace denně." +
                " Nelze rezervovat více časů najednou." +
                " Pokud není místo, navrhni jiný čas." +
                " Místo musí být dostupné po celou dobu rezervace."
                ));

            newList.Insert(0, ChatMessage.FromSystem(
                "Registrace:" +
                " Pro pronájem místa se registruj na https://parksharing-admin.obseum.cloud."
            ));

            newList.Insert(0, ChatMessage.FromSystem(
                "Komunikace:" +
                " Pouze česky; pokud uživatel mluví jinak, komunikuj v jeho jazyce."
            ));

            newList.Insert(0, ChatMessage.FromSystem(
                "Kontakt:" +
                " Podpora na WhatsApp: +420 724 676 829." +
                " Autor aplikace je něco s ovocem"
            ));

            newList.Insert(0, ChatMessage.FromSystem(
                "Platby:" +
                " Peníze přijdou na účet majitele."
            ));

            newList.Insert(0, ChatMessage.FromSystem(
                "Další pokyny:" +
                " Všechna parkování jsou venkovní." +
                " Aplikce je jen pro pozvané (Předběžný přístup), kteří mají kód. Lze jej získat jen od autora aplikace po splnění přisných podmínek správného velvaráka." +
                " Nepomáhej s ničím mimo tyto instrukce." +
                " Piš jasně a stručně." +
                " Používej ASCII obrázky." +
                " K formátování používej Markdown."
            ));


            newList.Insert(0, ChatMessage.FromUser("Sdílení a rezervace parkovacích míst ve Velvarii! \ud83d\ude97 Stačí napsat na kdy chcete místo rezervovat."));
            return newList;
        }

        private async Task<List<ChatMessage>> DebugMessage(List<ChatMessage> messages)
        {
            await Task.Delay(2000);
            var msgContent = messages.LastOrDefault().Content;
            if (msgContent.Contains("reservation"))
            {
                var newMsg = await _capabilities.ReserveSpot("2024-03-21 11:00", "2024-03-21 14:00", "GS22", "123123123");
                messages.Add(ChatMessage.FromAssistant(newMsg));
            }
            else if (msgContent.Contains("avaliable"))
            {
                //var newMsg = await _capabilities.AvaliableSpots("2024-03-21 11:00", "2024-03-21 14:00");
                //messages.Add(ChatMessage.FromAssistant(newMsg));
            }
            else
            {
                messages.Add(ChatMessage.FromAssistant("Unknown command " + messages.LastOrDefault().Content));
            }
            return messages;
        }

        private async Task<string> ExecuteFunction(FunctionCall? functionCall)
        {
            var result = await FunctionCallingHelper.CallFunction<Task<string>>(functionCall, _capabilities);
            var stringResponse = result.ToString(CultureInfo.CurrentCulture);
            return stringResponse;
        }
    }

    public static class ConversationHelper
    {
        public static List<ChatMessage> AddUserMessage(this List<ChatMessage> conversation, string newMessage)
        {
            conversation.Add(ChatMessage.FromUser(HtmlHelpers.SanitizeHtml(newMessage)));
            return conversation;
        }

        public static List<ChatMessage> AddResponseFromFunction(this List<ChatMessage> conversation, string functionResult, string functionCallId)
        {
            conversation.Add(ChatMessage.FromTool(functionResult, functionCallId));
            return conversation;
        }

        public static bool IsDebug(this List<ChatMessage> conversation)
        {
            return conversation.LastOrDefault().Content.Contains("debug");
        }

        public static bool IsAssistent(this List<ChatMessage> conversation)
        {
            return conversation.LastOrDefault().Role.Equals("assistant");
        }

        public static bool IsUser(this List<ChatMessage> conversation)
        {
            return conversation.LastOrDefault().Role.Equals("user");
        }

        public static bool IsToolCalls(this List<ChatMessage> conversation)
        {
            return conversation.LastOrDefault().ToolCalls != null;
        }

        public static ChatMessage? ToChatMessage(this ChatCompletionCreateResponse response)
        {
            return response.Choices?.First()?.Message;
        }

        public static List<FunctionCall?> GetCallFunctions(this List<ChatMessage> conversation)
        {
            return conversation?.LastOrDefault()?.ToolCalls.Select(tc => tc.FunctionCall).ToList();
        }

        public static List<ToolCall> GetToolCalls(this List<ChatMessage> conversation)
        {
            return conversation?.LastOrDefault()?.ToolCalls.ToList();
        }

        public static bool IsMultipleTooCall(this List<ChatMessage> conversation)
        {
            return conversation?.LastOrDefault()?.ToolCalls.Count > 1;
        }

        public static async Task<string> ExecuteFunction(object capabilities, FunctionCall function)
        {
            var result = await FunctionCallingHelper.CallFunction<Task<string>>(function, capabilities);
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
