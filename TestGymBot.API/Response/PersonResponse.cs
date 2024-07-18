namespace TestGymBot.API.Response
{
    public record PersonResponse(
        Guid Id,
        long UserId,
        long ChatId,
        string UserName,
        string FirstName,
        string LastName);
}
