using System.Threading.Tasks;
using MalikP.GitHub.LabelSynchronizer.Parameters;

namespace MalikP.GitHub.LabelSynchronizer.Synchronization
{
    public interface ISynchronizer
    {
        Task SynchroniseAsync(OrganisationNameParameter organizationLoginNameParameter, RepositoryNameParameter repositoryNameParameter);
    }
}


