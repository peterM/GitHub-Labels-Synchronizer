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
        static async Task Main(string[] args)
        {
            ParameterStore parameterStore = new ParameterStore();

            if (await parameterStore.InitializeAsync(args))
            {
                ISynchronizer Synchronizer = new DefaultSynchronizer(new ConsoleLogger(), await parameterStore.QueryParameterAsync<UriParameter>(), await parameterStore.QueryParameterAsync<OautTokenParameter>());
                await Synchronizer.SynchronizeAsync(await parameterStore.QueryParameterAsync<OrganisationNameParameter>(), await parameterStore.QueryParameterAsync<RepositoryNameParameter>());
            }
            else
            {
                await WriteHelpAsync();
            }

            Console.ReadKey();
        }

        private static Task WriteHelpAsync()
        {
            Console.ForegroundColor = ConsoleColor.Green;

            Console.WriteLine($"{"".PadLeft(20, '#')} HELP { "".PadRight(20, '#')}");

            Console.WriteLine("Specify github uri (https://github.domain.com/)");
            Console.WriteLine($"{"".PadLeft(7, ' ')}-uri=<OrganisationName>");

            Console.WriteLine("Specify personal oauth token with rights");
            Console.WriteLine($"{"".PadLeft(7, ' ')}-token=<personalToken>");

            Console.WriteLine("Specify organisation where you want to sync labels");
            Console.WriteLine($"{"".PadLeft(7, ' ')}-org=<OrganisationName>");

            Console.WriteLine("Specify repository in that organiosation which will be source of labels");
            Console.WriteLine($"{"".PadLeft(7, ' ')}-repo=<RepositoryName>");

            Console.WriteLine();
            Console.WriteLine("Example: ");
            Console.WriteLine($"{"".PadLeft(7, ' ')} MalikP.GitHub.LabelSynchronizer -uri=https://github.domain.com/ -token=<personalToken> -org=<OrganisationName> -repo=<RepositoryName>");

            Console.ResetColor();

            return Task.CompletedTask;
        }
    }
}
