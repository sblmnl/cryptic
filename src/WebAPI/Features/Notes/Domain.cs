namespace WebAPI.Features.Notes;

public static class Domain
{
    public record DeleteAfter
    {
        public record Reading(bool DoNotWarn) : DeleteAfter;
        public record Time(DateTimeOffset DeleteAt) : DeleteAfter;

        private DeleteAfter() { }

        public static DeleteAfter From(DateTimeOffset? deleteAt, bool doNotWarn)
        {
            return deleteAt is null
                ? new Reading(doNotWarn)
                : new Time(deleteAt ?? default);
        }
    }
    
    public record Note(
        Guid Id,
        string Content,
        DeleteAfter DeleteAfter);
}
