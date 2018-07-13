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
                await Synchronizer.SynchroniseAsync(await parameterStore.QueryParameterAsync<OrganisationNameParameter>(), await parameterStore.QueryParameterAsync<RepositoryNameParameter>());
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
