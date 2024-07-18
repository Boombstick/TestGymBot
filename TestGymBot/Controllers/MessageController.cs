using Telegram.Bot;
using TestGymBot.Domain;
using TestGymBot.Decors;
using System.Reflection;
using Telegram.Bot.Types;
using TestGymBot.Domain.Constants;
using TestGymBot.Domain.Attributes;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Caching.Memory;
using TestGymBot.Domain.Abstractions.Services;

namespace TestGymBot.Controllers
{
    public class MessageController
    {
        private readonly IPersonsService _personsService;
        private readonly CacheDecor _cache;
        private readonly ITimesService _timesService;
        private const string startText = "Thank you for deciding to take part in the testing of the bot\r\nIf you have any problems or suggestions, please write to @boombosc\r\n(Спасибо что решили принять участие в тестировании бота\r\nПри появлении проблем или предложений прошу написать @boombosc)";
        public MessageController(IPersonsService personsService, ITimesService timesService, CacheDecor cache)
        {
            _cache = cache;
            _personsService = personsService;
            _timesService = timesService;
        }

        public async Task InMessage(ITelegramBotClient client, Update update)
        {
            var chatId = update.Message.Chat.Id;
            var user = update.Message.From;
            var textMessage = update.Message.Text;

            var person = await GetPerson(user);

            #region Telegram commands
            if (textMessage.StartsWith('/'))
            {
                if (textMessage == "/start")
                {
                    await client.SendTextMessageAsync(chatId, startText,replyMarkup: ConstantInlineMarkups.GetKeyBoard(ConstantInlineMarkups.KeyboardEnum.StartKeyboardMarkup));
                    return;
                }
                if (textMessage == "/profile")
                {
                    person.State = State.InProfile;
                    
                    var personProfile = await ProfileUtils.GetPersonProfile(person);
                    await client.SendTextMessageAsync(person.ChatId, personProfile, replyMarkup: ConstantInlineMarkups.GetKeyBoard(ConstantInlineMarkups.KeyboardEnum.ProfileKeyboardMarkup));
                }
                return;
            }

            #endregion
            if (person.State == State.ChangeProfile)
            {
                if (!IsCorrectEntry(textMessage))
                {
                    await client.SendTextMessageAsync(chatId, "Некорректная запись, пожалуйста скопируйте сообщение выше");
                    return;
                }
                await client.SendTextMessageAsync(chatId, textMessage, replyMarkup: ConstantInlineMarkups.GetKeyBoard(ConstantInlineMarkups.KeyboardEnum.ProfileUpdateKeyboardMarkup));
                return;
            }
            await client.SendTextMessageAsync(chatId, "Пу пу пууу", replyMarkup: ConstantInlineMarkups.GetKeyBoard(ConstantInlineMarkups.KeyboardEnum.StartKeyboardMarkup));
        }

        private async Task<Person> GetPerson(User user)
        {
            _cache.TryGetValue(user.Username, out Person? person);
            if (person is null)
            {
                person = await _personsService.GetPersonByTelegramId(user);
                _cache.Set(user.Username, person, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(30)));
            }
            return person;
        }
        private async Task UpdatePersonInCache<T>(T key,Person person) => _cache.UpdateEntry(key,person, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(30)));
        private bool IsCorrectEntry(string? textMessage)
        {
            var propMatches = Regex.Matches(textMessage, @"[А-Я]\w*");
            var propCount = propMatches.Count;
            var props = typeof(PersonProps).GetProperties().Skip(1).ToArray();

            for (int i = 0; i < propCount; i++)
                if (propMatches[i].ToString() != props[i].GetCustomAttribute<RussianNameAttribute>().Name)
                    return false;

            return true;
        }


    }
}
