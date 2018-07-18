// Copyright (c) 2018 Peter M.
// 
// File: ParameterStore.cs 
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
                await CreateSourceOrganizationNameParameterAsync(args);
                await CreateSourceRepositoryNameParameterAsync(args);
                await CreateTargetOrganizationNameParameterAsync(args);
                await CreateTargetRepositoryNameParameterAsync(args);
                await CreateStrictFlagParameterAsync(args);

                result = true;
            }

            return result;
        }

        private Task CreateStrictFlagParameterAsync(string[] args)
        {
            string strictFlag = ExtractParamValue(args, "-strict=");
            bool strictFlagValue = false;
            if (!string.IsNullOrWhiteSpace(strictFlag) && bool.TryParse(strictFlag, out strictFlagValue))
            {
                _parameters.Add(new StrictFlagParameter(strictFlagValue));
            }

            return Task.CompletedTask;
        }

        private Task CreateSourceOrganizationNameParameterAsync(string[] args)
        {
            string organization = ExtractParamValue(args, "-source-org=");
            if (!string.IsNullOrWhiteSpace(organization))
            {
                _parameters.Add(new SourceOrganizationNameParameter(organization));
            }

            return Task.CompletedTask;
        }

        private Task CreateTargetOrganizationNameParameterAsync(string[] args)
        {
            string organization = ExtractParamValue(args, "-target-org=");
            if (!string.IsNullOrWhiteSpace(organization))
            {
                _parameters.Add(new TargetOrganizationNameParameter(organization));
            }

            return Task.CompletedTask;
        }

        private Task CreateSourceRepositoryNameParameterAsync(string[] args)
        {
            string repoName = ExtractParamValue(args, "-source-repo=");
            if (!string.IsNullOrWhiteSpace(repoName))
            {
                _parameters.Add(new SourceRepositoryNameParameter(repoName));
            }

            return Task.CompletedTask;
        }

        private Task CreateTargetRepositoryNameParameterAsync(string[] args)
        {
            string repoName = ExtractParamValue(args, "-target-repo=");
            if (!string.IsNullOrWhiteSpace(repoName))
            {
                _parameters.Add(new TargetRepositoryNameParameter(repoName));
            }

            return Task.CompletedTask;
        }

        private Task CreateOautTokebParameterAsync(string[] args)
        {
            string token = ExtractParamValue(args, "-token=");

            if (!string.IsNullOrWhiteSpace(token))
            {
                _parameters.Add(new OautTokenParameter(token));
            }

            return Task.CompletedTask;
        }

        private Task CreateUriParameterAsync(string[] args)
        {
            string uri = ExtractParamValue(args, "-uri=");

            if (!string.IsNullOrWhiteSpace(uri))
            {
                _parameters.Add(new UriParameter(new Uri(uri)));
            }

            return Task.CompletedTask;
        }

        private static string ExtractParamValue(string[] args, string paramName)
        {
            string param = GetParam(args, paramName);
            return GetParamValue(param);
        }

        private static string GetParamValue(string param)
        {
            return param?.Split(new char[] { '=' }, StringSplitOptions.RemoveEmptyEntries).Last();
        }

        private static string GetParam(string[] args, string paramName)
        {
            return args.SingleOrDefault(d => d.StartsWith(paramName, StringComparison.InvariantCultureIgnoreCase));
        }

        // TODO: need to rework parameter validation
        private Task<bool> ValidateAsync(string[] args)
        {
            bool result = false;

            if (args.Length == 4 || args.Length == 5 || args.Length == 6 || args.Length == 7)
            {
                result = true;
            }

            return Task.FromResult(result);
        }

        public Task<T> QueryParameterAsync<T>()
            where T : IParameter
        {
            return Task.FromResult(_parameters.OfType<T>().FirstOrDefault());
        }
    }
}
