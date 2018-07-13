using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MalikP.GitHub.LabelSynchronizer.Loggers;
using MalikP.GitHub.LabelSynchronizer.Parameters;
using Octokit;

namespace MalikP.GitHub.LabelSynchronizer.Synchronization
{
    public class DefaultSynchronizer : SynchronizerBase
    {
        public DefaultSynchronizer(IConsoleLogger consoleLogger, UriParameter gitHubUriParameter, OautTokenParameter OAuthTokenParameter)
            : base(consoleLogger, gitHubUriParameter, OAuthTokenParameter)
        {
        }

        public override async Task SynchroniseAsync(OrganisationNameParameter organizationLoginNameParameter, RepositoryNameParameter repositoryNameParameter)
        {
            Repository sourceRepository = await GitHubClient.Repository.Get(organizationLoginNameParameter.Value, repositoryNameParameter.Value);
            IReadOnlyList<Label> currentRepositoryLabels = await GetRepositoryLabels(sourceRepository);

            if (currentRepositoryLabels != null && currentRepositoryLabels.Any())
            {
                Organization organization = await GitHubClient.Organization
                                                              .Get(organizationLoginNameParameter.Value);

                IReadOnlyList<Repository> repositories = await GetRepositories(sourceRepository, organization);
                foreach (Repository repositoryItem in repositories)
                {
                    IReadOnlyList<Label> repoItemlabels = await GetRepositoryLabels(repositoryItem);

                    foreach (Label labelItem in currentRepositoryLabels)
                    {
                        Label existingLabel = repoItemlabels.SingleOrDefault(label => label.Name == labelItem.Name);
                        if (existingLabel != null)
                        {
                            await UpdateLabel(repositoryItem, labelItem);
                            continue;
                        }

                        await CreateNewLabel(repositoryItem, labelItem);
                    }
                }
            }

            WriteLog($"Synchronization of labels from source repo: '{sourceRepository.Id} - {sourceRepository.Name}'", ConsoleColor.DarkYellow);
        }

        private async Task<IReadOnlyList<Repository>> GetRepositories(Repository repository, Organization organization)
        {
            IReadOnlyList<Repository> repositories = await GitHubClient.Repository
                                                                       .GetAllForUser(organization.Name, ApiOptions.None);

            return repositories.Where(repo => repo.Id != repository.Id)
                               .ToList();
        }

        private Task<IReadOnlyList<Label>> GetRepositoryLabels(Repository repoItem)
        {
            return GitHubClient.Issue
                               .Labels
                               .GetAllForRepository(repoItem.Id);
        }

        private async Task CreateNewLabel(Repository repoItem, Label labelItem)
        {
            NewLabel newLabel = new NewLabel(labelItem.Name, labelItem.Color)
            {
                Description = labelItem.Description
            };

            await GitHubClient.Issue
                              .Labels
                              .Create(repoItem.Id, newLabel)
                              .ConfigureAwait(false);

            WriteLog($"Created label {labelItem.Name} with Color: #{labelItem.Color} in Repo: {repoItem.Name}", ConsoleColor.Green);
        }

        private async Task UpdateLabel(Repository repoItem, Label labelItem)
        {
            LabelUpdate labelUpdate = new LabelUpdate(labelItem.Name, labelItem.Color)
            {
                Description = labelItem.Color
            };

            await GitHubClient.Issue
                              .Labels
                              .Update(repoItem.Id, labelItem.Name, labelUpdate)
                              .ConfigureAwait(false);

            WriteLog($"Updated label {labelItem.Name} in Repo: {repoItem.Name}", ConsoleColor.Cyan);
        }
    }
}