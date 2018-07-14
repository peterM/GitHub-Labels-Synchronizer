// Copyright (c) 2018 Peter M.
// 
// File: DefaultSynchronizer.cs 
// Company: MalikP.
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
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

        public override async Task SynchronizeAsync(OrganisationNameParameter organizationLoginNameParameter, RepositoryNameParameter repositoryNameParameter)
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

        public override async Task SynchronizeAsync(OrganisationNameParameter organizationLoginNameParameter, RepositoryNameParameter sourceRepositoryNameParameter, RepositoryNameParameter targetRepositoryNameParameter)
        {
            if (string.Equals(sourceRepositoryNameParameter.Value, targetRepositoryNameParameter.Value))
            {
                WriteLog($"Synchronization stopped. Repo '{sourceRepositoryNameParameter.Value}' and '{targetRepositoryNameParameter.Value}' have to be different", ConsoleColor.Red);
                return;
            }

            Repository sourceRepository = await GitHubClient.Repository.Get(organizationLoginNameParameter.Value, sourceRepositoryNameParameter.Value);
            Repository targetRepository = await GitHubClient.Repository.Get(organizationLoginNameParameter.Value, targetRepositoryNameParameter.Value);

            IReadOnlyList<Label> sourceRepositoryLabels = await GetRepositoryLabels(sourceRepository);
            IReadOnlyList<Label> targetRepositoryLabels = await GetRepositoryLabels(targetRepository);

            if (sourceRepositoryLabels.Any())
            {
                foreach (Label labelItem in sourceRepositoryLabels)
                {
                    Label existingLabel = targetRepositoryLabels.SingleOrDefault(label => label.Name == labelItem.Name);
                    if (existingLabel != null)
                    {
                        await UpdateLabel(targetRepository, labelItem);
                        continue;
                    }

                    await CreateNewLabel(targetRepository, labelItem);
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