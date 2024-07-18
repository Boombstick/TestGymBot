namespace TestGymBot.API.Request
{
    public record PersonRequest(
        long UserId,
        long ChatId,
        string UserName,
        string FirstName, 
        string LastName);
}
