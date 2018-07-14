// Copyright (c) 2018 Peter M.
// 
// File: SynchronizerBase.cs 
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

        public abstract Task SynchronizeAsync(OrganisationNameParameter organizationLoginNameParameter, RepositoryNameParameter repositoryNameParameter);
    }
}
