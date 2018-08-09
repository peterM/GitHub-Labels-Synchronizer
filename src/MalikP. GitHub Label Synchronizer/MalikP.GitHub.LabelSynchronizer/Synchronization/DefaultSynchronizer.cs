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

        public override async Task SynchronizeAsync(OrganizationNameParameter sourceOrganizationNameParameter,
                                                    RepositoryNameParameter repositoryNameParameter,
                                                    OrganizationNameParameter targetOrganizationNameParameter,
                                                    StrictFlagParameter strictFlagParameter)
        {
            Repository sourceRepository = await GitHubClient.Repository.Get(sourceOrganizationNameParameter.Value, repositoryNameParameter.Value);

            Organization organization = await GitHubClient.Organization
                                                              .Get(targetOrganizationNameParameter.Value);
            IReadOnlyList<Repository> repositories = await GetRepositoriesAsync(sourceRepository, organization);

            await SynchronizeRepositoriesLabels(strictFlagParameter, sourceRepository, repositories);
        }

        public override async Task SynchronizeAsync(OrganizationNameParameter sourceOrganizationLoginNameParameter,
                                                    RepositoryNameParameter sourceRepositoryNameParameter,
                                                    OrganizationNameParameter targetOrganizationLoginNameParameter,
                                                    RepositoryNameParameter targetRepositoryNameParameter,
                                                    StrictFlagParameter strictFlagParameter)
        {
            if (string.Equals(sourceRepositoryNameParameter.Value, targetRepositoryNameParameter.Value))
            {
                WriteLog($"Synchronization stopped. Repo '{sourceRepositoryNameParameter.Value}' and '{targetRepositoryNameParameter.Value}' have to be different", ConsoleColor.Red);
                return;
            }

            Repository sourceRepository = await GitHubClient.Repository.Get(sourceOrganizationLoginNameParameter.Value, sourceRepositoryNameParameter.Value);
            Repository targetRepository = await GitHubClient.Repository.Get(targetOrganizationLoginNameParameter.Value, targetRepositoryNameParameter.Value);

            await SynchronizeRepositoriesLabels(strictFlagParameter, sourceRepository, new[] { targetRepository });
        }

        private async Task SynchronizeRepositoriesLabels(StrictFlagParameter strictFlagParameter, Repository sourceRepository, IEnumerable<Repository> targetRepositories)
        {
            IReadOnlyList<Label> sourceRepositoryLabels = await GetRepositoryLabelsAsync(sourceRepository);

            if (sourceRepositoryLabels.Any())
            {
                foreach (Repository targetRepository in targetRepositories)
                {
                    List<Label> targetRepositoryLabels = (await GetRepositoryLabelsAsync(targetRepository)).ToList();

                    foreach (Label labelItem in sourceRepositoryLabels)
                    {
                        Label existingLabel = targetRepositoryLabels.SingleOrDefault(label => label.Name == labelItem.Name);
                        if (existingLabel != null)
                        {
                            targetRepositoryLabels.Remove(existingLabel);
                            await UpdateLabelAsync(targetRepository, labelItem);
                            continue;
                        }

                        await CreateNewLabelAsync(targetRepository, labelItem);
                    }

                    if (strictFlagParameter != null && strictFlagParameter.Value && targetRepositoryLabels.Any())
                    {
                        foreach (var label in targetRepositoryLabels)
                        {
                            await DeleteLabel(targetRepository, label);
                        }
                    }

                    WriteLog($"Synchronization of labels from source repo: '{sourceRepository.Id} - {sourceRepository.Name}'", ConsoleColor.DarkYellow);
                }
            }
        }

        private async Task<IReadOnlyList<Repository>> GetRepositoriesAsync(Repository repository, Organization organization)
        {
            IReadOnlyList<Repository> repositories = await GitHubClient.Repository
                                                                       .GetAllForUser(organization.Name, ApiOptions.None);

            return repositories.Where(repo => repo.Id != repository.Id)
                               .ToList();
        }

        private Task<IReadOnlyList<Label>> GetRepositoryLabelsAsync(Repository repoItem)
        {
            return GitHubClient.Issue
                               .Labels
                               .GetAllForRepository(repoItem.Id);
        }

        private async Task CreateNewLabelAsync(Repository repoItem, Label labelItem)
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

        private async Task UpdateLabelAsync(Repository repoItem, Label labelItem)
        {
            LabelUpdate labelUpdate = new LabelUpdate(labelItem.Name, labelItem.Color)
            {
                Description = labelItem.Description
            };

            await GitHubClient.Issue
                              .Labels
                              .Update(repoItem.Id, labelItem.Name, labelUpdate)
                              .ConfigureAwait(false);

            WriteLog($"Updated label {labelItem.Name} in Repo: {repoItem.Name}", ConsoleColor.Cyan);
        }

        private async Task DeleteLabel(Repository repository, Label labelItem)
        {
            await GitHubClient.Issue
                              .Labels
                              .Delete(repository.Id, labelItem.Name)
                              .ConfigureAwait(false);

            WriteLog($"Deleted label {labelItem.Name} with Color: #{labelItem.Color} in Repo: {repository.Name}", ConsoleColor.Yellow);
        }
    }
}
