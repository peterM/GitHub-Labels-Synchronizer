using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MalikP.GitHub.LabelSynchronizer.Parameters
{
    public class ParameterStore
    {
        readonly List<IParameter> _parameters;

        public ParameterStore()
        {
            _parameters = new List<IParameter>();
        }

        public async Task<bool> InitializeAsync(string[] args)
        {
            bool result = false;

            if (await ValidateAsync(args))
            {
                await CreateUriParameterAsync(args);
                await CreateOautTokebParameterAsync(args);
                await CreateOrganisationNameParameterAsync(args);
                await CreateRepositoryNameParameterAsync(args);

                result = true;
            }

            return result;
        }

        private Task CreateOrganisationNameParameterAsync(string[] args)
        {
            string token = ExtractParamValue(args, "-org=");
            _parameters.Add(new OrganisationNameParameter(token));

            return Task.CompletedTask;
        }

        private Task CreateRepositoryNameParameterAsync(string[] args)
        {
            string token = ExtractParamValue(args, "-repo=");
            _parameters.Add(new RepositoryNameParameter(token));

            return Task.CompletedTask;
        }

        private Task CreateOautTokebParameterAsync(string[] args)
        {
            string token = ExtractParamValue(args, "-token=");
            _parameters.Add(new OautTokenParameter(token));

            return Task.CompletedTask;
        }

        private Task CreateUriParameterAsync(string[] args)
        {
            string uri = ExtractParamValue(args, "-uri=");
            _parameters.Add(new UriParameter(new Uri(uri)));

            return Task.CompletedTask;
        }

        private static string ExtractParamValue(string[] args, string paramName)
        {
            string param = GetParam(args, paramName);
            return GetParamValue(param);
        }

        private static string GetParamValue(string param)
        {
            return param.Split(new char[] { '=' }, StringSplitOptions.RemoveEmptyEntries).Last();
        }

        private static string GetParam(string[] args, string paramName)
        {
            return args.Single(d => d.StartsWith(paramName, StringComparison.InvariantCultureIgnoreCase));
        }

        private Task<bool> ValidateAsync(string[] args)
        {
            bool result = false;

            if (args.Length == 4)
            {
                result = true;
            }

            return Task.FromResult(result);
        }

        public Task<T> QueryParameterAsync<T>()
            where T : IParameter
        {
            return Task.FromResult(_parameters.OfType<T>().First());
        }
    }
}
