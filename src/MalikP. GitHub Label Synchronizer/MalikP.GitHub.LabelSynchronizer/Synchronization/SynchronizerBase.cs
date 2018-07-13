using System;
using System.Threading.Tasks;
using MalikP.GitHub.LabelSynchronizer.Loggers;
using MalikP.GitHub.LabelSynchronizer.Parameters;
using Octokit;

namespace MalikP.GitHub.LabelSynchronizer.Synchronization
{
    public abstract class SynchronizerBase : ISynchronizer
    {
        protected IGitHubClient GitHubClient { get; }

        private readonly IConsoleLogger _logger;

        protected SynchronizerBase(IConsoleLogger logger, UriParameter gitHubUriParameter, OautTokenParameter OAuthTokenParameter)
        {
            GitHubClient = CreateCient(gitHubUriParameter.Value, OAuthTokenParameter.Value);
            _logger = logger;
        }

        private IGitHubClient CreateCient(Uri gitHubUri, string OAuthToken)
        {
            var client = new GitHubClient(new ProductHeaderValue("MalikP.GitHub.LabelSynchronizer"), gitHubUri);

            client.Credentials = new Credentials(OAuthToken);

            return client;
        }

        protected void WriteLog(string message, ConsoleColor color)
        {
            _logger.WriteLog(message, color);
        }

        public abstract Task SynchroniseAsync(OrganisationNameParameter organizationLoginNameParameter, RepositoryNameParameter repositoryNameParameter);
    }
}
