namespace swishes.Infrastructure.Repositories
{
    public enum RepositoryOperationResult
    {
        Success = 0,
        UnknownError = 1,
        EntityAlreadyExists = 2,
        EntityNotFound = 3,
        NotAllEntitiesProcessed = 4,
        NullEntity = 5
    }
}