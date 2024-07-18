using Microsoft.Data.SqlClient;

namespace TestGymBot.Notification
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var credentialPath = "C:\\Users\\Boomb\\Desktop\\credential.txt";
            var botApi = File.ReadAllText(credentialPath);
            var notificationText = "Тренировка через 1 час";
            string connectionString = "Server = MYBOOK_ZENITH\\SQLEXPRESS;Database=TestGymDb;Trusted_Connection=True;TrustServerCertificate=Yes";


            var sqlExpression = "Select ChatId From Persons\r\nWhere \r\nId=Some(\r\nSelect PersonId From Records \r\nWhere \r\nTimeId=Some(\r\nSelect Id From Times \r\nWhere \r\nDAY(Date)=DAY(GETDATE())\r\nand MONTH(Date)=MONTH(GETDATE()) \r\nand YEAR(Date)=YEAR(GETDATE())\r\nand Period = DATENAME(hour,GETDATE())+1))";
            while (true)
            {
                if (DateTime.Now.Minute==0)
                {

                    List<long> chatIds = new List<long>();
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        using (HttpClient client = new HttpClient())
                        {
                            await connection.OpenAsync();
                            SqlCommand command = new SqlCommand(sqlExpression, connection);
                            using (SqlDataReader reader = await command.ExecuteReaderAsync())
                            {

                                if (reader.HasRows)
                                {
                                    while (await reader.ReadAsync())
                                    {
                                        long chatId = reader.GetInt64(0);
                                        chatIds.Add(chatId);
                                        var request = new HttpRequestMessage(HttpMethod.Post, $"https://api.telegram.org/bot{botApi}/sendMessage?chat_id={chatId}&text={notificationText}");
                                        await client.SendAsync(request);
                                    }
                                }
                            }
                        }
                    }
                    var text = string.Join(",",chatIds);
                    Console.WriteLine($"Оповещения разосланы {text}");
                    Thread.Sleep(3_600_000);
                }
                else
                {
                    Thread.Sleep(60000);
                }
            }
        }
    }
}
