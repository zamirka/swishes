namespace swishes.Infrastructure.Repositories
{
    public enum RepositoryOperationStatus
    {
        Success = 0,
        UnknownError = 1,
        EntityAlreadyExists = 2,
        EntityNotFound = 3,
        NotAllEntitiesProcessed = 4
    }
}