// Copyright (c) 2018 Peter M.
// 
// File: Program.cs 
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
using MalikP.GitHub.LabelSynchronizer.Synchronization;

namespace MalikP.GitHub.LabelSynchronizer
{
    class Program
    {
        private static IConsoleLogger _logger = new ConsoleLogger();

        static async Task Main(string[] args)
        {
            ParameterStore parameterStore = new ParameterStore();

            if (await parameterStore.InitializeAsync(args))
            {
                ISynchronizer Synchronizer = new DefaultSynchronizer(_logger, await parameterStore.QueryParameterAsync<UriParameter>(), await parameterStore.QueryParameterAsync<OautTokenParameter>());

                TargetRepositoryNameParameter targetRepositoryNameParameter = await parameterStore.QueryParameterAsync<TargetRepositoryNameParameter>();
                StrictFlagParameter strictFlagParameter = await parameterStore.QueryParameterAsync<StrictFlagParameter>();

                if (targetRepositoryNameParameter == null)
                {
                    await Synchronizer.SynchronizeAsync(await parameterStore.QueryParameterAsync<OrganizationNameParameter>(), await parameterStore.QueryParameterAsync<SourceRepositoryNameParameter>(), strictFlagParameter);
                }
                else
                {
                    var sourceOrganizationNameParameter = await parameterStore.QueryParameterAsync<SourceOrganizationNameParameter>();
                    OrganizationNameParameter targetOrganizationParameter = (OrganizationNameParameter)await parameterStore.QueryParameterAsync<TargetOrganizationNameParameter>() ?? sourceOrganizationNameParameter;
                    await Synchronizer.SynchronizeAsync(sourceOrganizationNameParameter, await parameterStore.QueryParameterAsync<SourceRepositoryNameParameter>(), targetOrganizationParameter, targetRepositoryNameParameter, strictFlagParameter);
                }
            }
            else
            {
                await WriteHelpAsync();
            }

            Console.ReadKey();
        }

        private static Task WriteHelpAsync()
        {
            _logger.WriteLog($"{"".PadLeft(40, '#')} HELP { "".PadRight(40, '#')}", ConsoleColor.Red);

            _logger.WriteLog("Specify github uri (https://github.domain.com/)", ConsoleColor.Cyan);
            _logger.WriteLog($"{"".PadLeft(7, ' ')}-uri=<OrganisationName>", ConsoleColor.Green);

            _logger.WriteLog("Specify personal oauth token with rights", ConsoleColor.Cyan);
            _logger.WriteLog($"{"".PadLeft(7, ' ')}-token=<personalToken>", ConsoleColor.Green);

            _logger.WriteLog("Specify if synchronization will be strict or not. This means if also can delete labels or only create or update", ConsoleColor.Cyan);
            _logger.WriteLog($"{"".PadLeft(7, ' ')}-strict=<true|false>", ConsoleColor.Green);

            _logger.WriteLog("Specify organization where you want to sync labels", ConsoleColor.Cyan);
            _logger.WriteLog($"{"".PadLeft(7, ' ')}-source-org=<OrganisationName>", ConsoleColor.Green);

            _logger.WriteLog("Specify repository in that organization which will be source of labels", ConsoleColor.Cyan);
            _logger.WriteLog($"{"".PadLeft(7, ' ')}-source-repo=<RepositoryName>", ConsoleColor.Green);

            _logger.WriteLog("Specify organization where is located target repository", ConsoleColor.Cyan);
            _logger.WriteLog($"{"".PadLeft(7, ' ')}-target-org=<OrganisationName>", ConsoleColor.Green);

            _logger.WriteLog("Specify repository in that organiosation which will be target and wher labels will be synchronized", ConsoleColor.Cyan);
            _logger.WriteLog($"{"".PadLeft(7, ' ')}-target-repo=<RepositoryName>", ConsoleColor.Green);

            _logger.WriteLog("", ConsoleColor.Green);
            _logger.WriteLog("Example when we want synchronize labels across all organization repositories: ", ConsoleColor.Cyan);
            _logger.WriteLog($"{"".PadLeft(7, ' ')} MalikP.GitHub.LabelSynchronizer -uri=https://github.domain.com/ -token=<personalToken> -source-org=<OrganisationName> -source-repo=<RepositoryName>", ConsoleColor.Green);

            _logger.WriteLog("", ConsoleColor.Green);
            _logger.WriteLog("Example when we want synchronize labels only in specific repository from specific repository: ", ConsoleColor.Cyan);
            _logger.WriteLog($"{"".PadLeft(7, ' ')} MalikP.GitHub.LabelSynchronizer -uri=https://github.domain.com/ -token=<personalToken> -source-org=<OrganisationName> -source-repo=<RepositoryName> -target-repo=<RepositoryName>", ConsoleColor.Green);

            _logger.WriteLog("", ConsoleColor.Green);
            _logger.WriteLog("Example when we want synchronize labels only in specific repository from specific repository and both are in different organizations: ", ConsoleColor.Cyan);
            _logger.WriteLog($"{"".PadLeft(7, ' ')} MalikP.GitHub.LabelSynchronizer -uri=https://github.domain.com/ -token=<personalToken> -source-org=<OrganisationName> -source-repo=<RepositoryName> -target-org=<OrganisationName> -target-repo=<RepositoryName>", ConsoleColor.Green);

            _logger.WriteLog($"{"".PadLeft(86, '#')}", ConsoleColor.Red);

            Console.ResetColor();

            return Task.CompletedTask;
        }
    }
}
