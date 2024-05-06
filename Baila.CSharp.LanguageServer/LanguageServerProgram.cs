using Baila.CSharp.LanguageServer;
using MediatR;
using OmniSharp.Extensions.LanguageServer.Protocol.General;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;
using OmniSharp.Extensions.LanguageServer.Server;

var server = await LanguageServer.From(
    options =>
    {
        options
            .WithInput(Console.OpenStandardInput())
            .WithOutput(Console.OpenStandardOutput())
            .OnInitialize((languageServer, request, token) =>
            {
                Console.WriteLine("Initializing...");
                return Task.CompletedTask;
            })
            .OnInitialized((languageServer, request, response, token) =>
            {
                Console.WriteLine("Initialized.");
                response.Capabilities.TextDocumentSync = new TextDocumentSync(
                    TextDocumentSyncKind.Full);
                response.Capabilities.CompletionProvider = new CompletionRegistrationOptions.StaticOptions
                {
                    ResolveProvider = true,
                    TriggerCharacters = new[] {".", " "}
                };
                return Task.CompletedTask;
            })
            .OnStarted((languageServer, token) =>
            {
                Console.WriteLine("Started.");
                return Task.CompletedTask;
            })
            .AddHandler(new InitializeHandler());
    }).ConfigureAwait(false);
    
await server.WaitForExit.ConfigureAwait(false);

namespace Baila.CSharp.LanguageServer
{
    class InitializeHandler : ILanguageProtocolInitializedHandler
    {
        public Task<Unit> Handle(InitializedParams request, CancellationToken cancellationToken)
        {
            Console.WriteLine("Received initialize request");
            return Unit.Task;
        }
    }
}