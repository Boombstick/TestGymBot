using Telegram.Bot;
using TestGymBot.Decors;
using Telegram.Bot.Types;
using TestGymBot.Providers;
using TestGymBot.DataAccess;
using TestGymBot.Domain.Constants;
using TestGymBot.Application.Configs;
using TestGymBot.Application.Services;
using TestGymBot.DataAccess.Repositories;
using Microsoft.Extensions.Caching.Memory;
using TestGymBot.Domain.Abstractions.Services;
using Microsoft.Extensions.DependencyInjection;
using TestGymBot.Domain.Abstractions.Repositories;

namespace TestGymBot
{
    internal class Program
    {

        public static IServiceProvider services = new ServiceCollection()
            .AddDbContext<TgBotDbContext>()
            .AddScoped<ITimesService, TimesService>()
            .AddScoped<IPersonsService, PersonsService>()
            .AddScoped<ITimeRepository, TimeRepository>()
            .AddScoped<IPersonsRepository, PersonsRepository>()
            .AddScoped<ISimpleRecordRepository, RecordsRepository>()
            .AddTransient<IPersonPropsService, PersonPropsService>()
            .AddTransient<IPersonPropsRepository, PersonPropsRepository>()
            .AddTransient<UpdateProvider>()
            .AddMemoryCache()
            .AddSingleton<CacheDecor>()
            .AddSingleton<MapsterConfig>()
            .BuildServiceProvider();
        public static CacheDecor cache = services.GetRequiredService<CacheDecor>();
        static void Main(string[] args)
        {
            var credentialPath = "C:\\Users\\Boomb\\Desktop\\credential.txt";
            var botApi = System.IO.File.ReadAllText(credentialPath);
            var botClient = new TelegramBotClient(botApi);
            MapsterConfig mapsterConfig = new MapsterConfig();
            botClient.StartReceiving(Update, Error);
            Console.WriteLine("Бот запущен");
            Console.ReadLine();
        }
        private static async Task Update(ITelegramBotClient client, Update update, CancellationToken token)
        {
            long chatId = 0;
            try
            {
                if (update.Message is null)
                {
                    var lastTime = Math.Abs((int)update.CallbackQuery?.Message?.Date.Subtract(DateTime.UtcNow).Minutes);
                    if (lastTime > 30)
                    {
                        await client.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id, "Похоже, вы захотели воспользоваться ботом, когда он спал", replyMarkup: ConstantInlineMarkups.GetKeyBoard(ConstantInlineMarkups.KeyboardEnum.StartKeyboardMarkup));
                        return;
                    }
                    chatId = update.CallbackQuery.Message.Chat.Id;
                }
                else
                {
                    var lastTime = Math.Abs((int)update.Message?.Date.Subtract(DateTime.UtcNow).Minutes);
                    if (lastTime > 30)
                    {
                        await client.SendTextMessageAsync(update.Message.Chat.Id, "Похоже, вы захотели воспользоваться ботом, когда он спал", replyMarkup: ConstantInlineMarkups.GetKeyBoard(ConstantInlineMarkups.KeyboardEnum.StartKeyboardMarkup));
                        return;
                    }
                    chatId = update.Message.Chat.Id;
                }


                UpdateProvider provider;
                if (cache.TryGetValue(chatId, out UpdateProvider? _provider))
                {
                    provider = _provider;
                    await provider.PostUpdateAsync(client, update);
                    cache.UpdateEntry(chatId, provider, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(30)));
                }
                else
                {
                    provider = services.GetRequiredService<UpdateProvider>();
                    await provider.PostUpdateAsync(client, update);
                    cache.Set(chatId, provider, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(30)));
                }

                //#region[Files]
                //if (update.Message.VideoNote is not null)
                //{
                //    using (Stream str = new FileStream("C:\\Users\\Boomb\\Desktop\\Bot\\Зарядка\\123.mp4", FileMode.Create))
                //    {
                //        var note = await client.GetInfoAndDownloadFileAsync(update.Message.VideoNote.FileId, str);
                //    }
                //    await client.SendTextMessageAsync(user.ChatId, "Сохранил", replyMarkup: ConstantKeyboardMarkups.FilesKeyboardMarkup);
                //    return;
                //}

                //if (update.Message.Text is not null && update.Message.Text.ToLower().Equals("зарядка"))
                //{
                //    using (Stream str = new FileStream("C:\\Users\\Boomb\\Desktop\\Bot\\Зарядка\\123.mp4", FileMode.Open))
                //    {
                //        await client.SendTextMessageAsync(user.ChatId, "Подожди малеха, сейчас потренируешься");
                //        await client.SendVideoNoteAsync(user.ChatId, InputFile.FromStream(str));
                //        await client.SendTextMessageAsync(user.ChatId, "Держи братка", replyMarkup: ConstantKeyboardMarkups.FilesKeyboardMarkup);
                //    }
                //    return;
                //}
                //if (update.Message.Text is not null && update.Message.Text.ToLower().Equals("загрузить кружок"))
                //{
                //    await client.SendTextMessageAsync(user.ChatId, "Жду кружок", replyMarkup: ConstantKeyboardMarkups.FilesKeyboardMarkup);
                //    return;
                //}

                //#endregion
            }
            catch (Exception ex)
            {
                await client.SendTextMessageAsync(update.Message is null ? update.CallbackQuery.Message.Chat.Id : update.Message.Chat.Id, "Упс, что-то пошло не так", replyMarkup: ConstantInlineMarkups.GetKeyBoard(ConstantInlineMarkups.KeyboardEnum.StartKeyboardMarkup));
                await client.SendTextMessageAsync(5460746302, "У меня внутри что-то сломалось" + ex.Message + $"({chatId})", replyMarkup: ConstantInlineMarkups.GetKeyBoard(ConstantInlineMarkups.KeyboardEnum.StartKeyboardMarkup));
            }

        }
        private static Task Error(ITelegramBotClient client, Exception exception, CancellationToken token)
        {
            return Task.CompletedTask;
        }
    }
}
