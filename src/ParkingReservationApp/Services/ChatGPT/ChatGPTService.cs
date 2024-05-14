using Microsoft.AspNetCore.Http;
using OpenAI;
using OpenAI.Builders;
using OpenAI.Interfaces;
using OpenAI.Managers;
using OpenAI.ObjectModels;
using OpenAI.ObjectModels.RequestModels;
using OpenAI.ObjectModels.SharedModels;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using OpenAI.Utilities;
using OpenAI.Utilities.FunctionCalling;
using System.Globalization;
using System.Reflection;
using System.Text.Json;
namespace ParkingReservationApp.Services.ChatGPT
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
            var toolDefinitions = FunctionCallingHelper.GetToolDefinitions(_capabilities);

            messages.Insert(0, ChatMessage.FromSystem("Pouzivas v odpovedich markdown. Český chatbot pro sdílení a rezervaci parkovacích míst. Umožňuje majitelům nabízet místa když je nepoužívají a ostatním je rezervovat a platit přes bankovní účet. Uživatelé mohou registrovat místa, nastavovat dostupnost a spravovat nabídky, rezervace omezeny na dvě denně. Komunikace v češtině, pokud uživatel mluvi jinou reci mluvi jinou reci. Nevyplňuj nejasné funkce bez dotazu."));
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
                    string stringResponse = ExecuteFunction(response);
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

        private string ExecuteFunction(ChatMessage response)
        {
            Console.WriteLine($"Invoking {response.ToolCalls[0].FunctionCall.Name}");
            var functionCall = response.ToolCalls[0].FunctionCall;
            var result = FunctionCallingHelper.CallFunction<string>(functionCall, _capabilities);
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
