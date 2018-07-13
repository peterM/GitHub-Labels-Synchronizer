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
