using System;

namespace MalikP.GitHub.LabelSynchronizer.Parameters
{
    public sealed class UriParameter : Parameter<Uri>
    {
        public UriParameter(Uri value) : base(value)
        {
        }
    }
}
