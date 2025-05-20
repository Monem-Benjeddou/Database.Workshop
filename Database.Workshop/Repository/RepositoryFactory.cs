using Database.Workshop.Models;

namespace Database.Workshop.Repository;

public class RepositoryFactory(IServiceProvider serviceProvider, RepositoryType repoType)
{
    public IUserRepository Create()
    {
        return repoType switch
        {
            RepositoryType.Ef => serviceProvider.GetRequiredService<EfUserRepository>(),
            RepositoryType.Ado => serviceProvider.GetRequiredService<AdoUserRepository>(),
            RepositoryType.Dapper => serviceProvider.GetRequiredService<DapperUserRepository>(),
            _ => throw new NotSupportedException()
        };
    }
}