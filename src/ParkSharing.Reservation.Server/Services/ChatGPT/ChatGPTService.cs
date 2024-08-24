using OpenAI;
using OpenAI.Interfaces;
using OpenAI.Managers;
using OpenAI.ObjectModels.RequestModels;
using OpenAI.ObjectModels.ResponseModels;
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

        public ChatGPTService(ILogger<ChatGPTService> logger, IReservationService reservationService, ChatGPTSessionService sessions, ChatGPTCapabilities capabilities)
        {
            _openAI = new OpenAIService(new OpenAiOptions
            {
                ApiKey = "sk-proj-roWrNK4agpNuCEu1D76DT3BlbkFJcJSu1NGs8GdB09TcctbV",
                UseBeta = true
            });
            _logger = logger;
            _reservationService = reservationService;
            _sessions = sessions;
            _capabilities = capabilities;
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
                Model = "gpt_4o",
                MaxTokens = 200
            };

            var reply = await _openAI.ChatCompletion.CreateCompletion(req, "gpt-4o");
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
            newList.Insert(0, ChatMessage.FromSystem("Úloha: Pomáháš s rezervací a dostupností venkovních parkovacích míst. S ničím jiným nepomáháš."));
            newList.Insert(0, ChatMessage.FromSystem("Rezervace jednoho místa najednou."));
            newList.Insert(0, ChatMessage.FromSystem("Vždy náhodně vyber jedno volné místo a potvrď uživatelem před rezervací."));
            newList.Insert(0, ChatMessage.FromSystem("Max dvě rezervace denně."));
            newList.Insert(0, ChatMessage.FromSystem("Pokud není volné místo, navrhni jiný čas."));
            newList.Insert(0, ChatMessage.FromSystem("Registrace a správa míst: - Pro pronájem místa se registrujte na https://parksharing-admin.obseum.cloud/."));
            newList.Insert(0, ChatMessage.FromSystem("Komunikace: Pouze v češtině, pokud uživatel mluví jiným jazykem, komunikuj v jeho jazyce."));
            newList.Insert(0, ChatMessage.FromSystem("Platby: Peníze přijdou na účet majitele parkovacího místa."));
            newList.Insert(0, ChatMessage.FromSystem("Kontakt: Podpora na WhatsApp +420 724 676 829."));
            newList.Insert(0, ChatMessage.FromSystem("Nelze rezervovat více časů najednou."));
            newList.Insert(0, ChatMessage.FromSystem("Všechna parkování jsou venkovní."));
            newList.Insert(0, ChatMessage.FromSystem("Nepomáhej s ničím, co není uvedeno v instrukcích."));
            newList.Insert(0, ChatMessage.FromSystem("Pis velice jasne instrukce."));

            newList.Insert(0, ChatMessage.FromSystem("Místo pro rezervaci musí být dostupné po celou dobu rezervace."));
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

        //public async Task<List<ChatMessage>> SendOld(List<ChatMessage> messages)
        //{
        //    if (messages.LastOrDefault().Content.Contains("debug"))
        //    {
        //        await Task.Delay(2000);
        //        var msgContent = messages.LastOrDefault().Content;
        //        if (msgContent.Contains("reservation"))
        //        {
        //            var newMsg = await _capabilities.ReserveSpot("2024-03-21 11:00", "2024-03-21 14:00", "GS22", "123123123");
        //            messages.Add(ChatMessage.FromAssistant(newMsg));
        //        }
        //        else if (msgContent.Contains("avaliable"))
        //        {
        //            //var newMsg = await _capabilities.AvaliableSpots("2024-03-21 11:00", "2024-03-21 14:00");
        //            //messages.Add(ChatMessage.FromAssistant(newMsg));
        //        }
        //        else
        //        {
        //            messages.Add(ChatMessage.FromAssistant("Unknown command " + messages.LastOrDefault().Content));
        //        }
        //        return messages;
        //    }

        //    var toolDefinitions = FunctionCallingHelper.GetToolDefinitions(_capabilities);

        //    messages.Insert(0, ChatMessage.FromSystem("Pouzivas v odpovedich markdown. Český chatbot pro sdílení a rezervaci parkovacích míst. Umožňuje majitelům nabízet místa když je nepoužívají a ostatním je rezervovat a platit přes bankovní účet. Uživatelé mohou registrovat místa, nastavovat dostupnost a spravovat nabídky, rezervace omezeny na dvě denně. Komunikace v češtině, pokud uživatel mluvi jinou reci mluvi jinou reci. Nevyplňuj nejasné funkce bez dotazu. Pokud uzivatel zadal email, pouzij jej pro kazdej dotaz. Pokud uzivatel zadal platny kod, pouzij jej pro kazdy dotaz. Pokud chce uzivatel pridavat, menit, mazat svoje parkovaci misto musi se nejdrive identifikovat."));
        //    messages.Insert(0, ChatMessage.FromSystem($"Aktualni datum {DateTime.Now.ToString("dd. MMMM yyyy")}"));
        //    messages.Insert(0, ChatMessage.FromSystem($"Pokud chce nekdo rezervovat misto, vyber mu nahodne jedno znich a nech si potvrdit ze to je ok nez ho rezervujes."));
        //    messages.Insert(0, ChatMessage.FromSystem($"Nelze rezervovat vice casu najendou. Pouze jeden po jednom."));
        //    messages.Insert(0, ChatMessage.FromSystem($"Podpora whatsapp 724 676 829"));
        //    messages.Insert(0, ChatMessage.FromSystem($"Neodpovídej na nic co se netýká rezervace a dostupnosti míst, nebo neni tady napsáno."));
        //    messages.Insert(0, ChatMessage.FromSystem($"Vsechna parkovani jsou venkovni. Nic neni v garazi"));
        //    messages.Insert(0, ChatMessage.FromSystem($"FAQ: Penize prijdou na ucet majitele parkovaciho stani"));
        //    messages.Insert(0, ChatMessage.FromSystem($"Jsi assisten co pomaha s parkovanim. S nicim jinym nepomahas. Pokud neni nic volného, navrhni at vyhledá jiný čas."));
        //    messages.Insert(0, ChatMessage.FromSystem($"Nikdy si nevymýšlej odpověd funkcí."));
        //    messages.Insert(0, ChatMessage.FromSystem($"FAQ: Pokud chce nekdo pronajimat sve misto at se registruje na https://parksharing-admin.obseum.cloud/"));

        //    var req = new ChatCompletionCreateRequest
        //    {
        //        Tools = toolDefinitions,
        //        Messages = messages.Where(m => m != null).ToList(),
        //        Model = "gpt_4o",
        //        MaxTokens = 200
        //    };

        //    var reply = await _openAI.ChatCompletion.CreateCompletion(req, "gpt-4o");
        //    if (!reply.Successful)
        //    {
        //        messages.Add(ChatMessage.FromAssistant("Ups! Něco se pokazilo"));
        //        Console.WriteLine(reply.Error?.Message);
        //        return req.Messages.ToList();
        //    }

        //    ChatMessage response = reply.Choices.First().Message;
        //    req.Messages.Add(response);

        //    if (response.ToolCalls?.Count > 0 && !string.IsNullOrEmpty(response.ToolCalls[0].FunctionCall.Name))
        //    {
        //        do
        //        {
        //            if (req.Messages.LastOrDefault(m => m.Role == "assistant") != null)
        //            {
        //                req.Messages.Remove(req.Messages.LastOrDefault());
        //            }
        //            string stringResponse = await ExecuteFunction(response);
        //            req.Messages.Add(ChatMessage.FromTool(stringResponse, response.ToolCalls[0].Id));
        //            reply = await _openAI.ChatCompletion.CreateCompletion(req, "gpt-4o");
        //            response = reply?.Choices?.First()?.Message;
        //            req.Messages.Add(response);
        //        } while (response?.ToolCalls != null);
        //        req.Messages.Add(response); //Add answer from assistent

        //    }
        //    else
        //    {
        //        return req.Messages.ToList();
        //    }

        //    return req.Messages.ToList();
        //}

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
