using System.Text;
using Telegram.Bot;
using FluentDateTime;
using TestGymBot.Decors;
using TestGymBot.Domain;
using System.Reflection;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TestGymBot.Domain.Constants;
using TestGymBot.Domain.Attributes;
using System.Text.RegularExpressions;
using Telegram.Bot.Types.ReplyMarkups;
using Microsoft.Extensions.Caching.Memory;
using TestGymBot.Domain.Abstractions.Services;
using Microsoft.Extensions.DependencyInjection;

namespace TestGymBot.Controllers
{
    public class CallbackQueryController
    {
        public readonly long[] MasterIds = { 5403248533, 5460746302 };
        private const string _masterKey = "master";
        private const string _separator = "-------------------";
        private readonly IPersonsService _personsService;
        private readonly CacheDecor _cache;
        private readonly ITimesService _timesService;
        private Status state = Status.Default;
        private string _checkBox = "[x]";
        private DaysOfWeek _currentDay;
        private Person person;
        private Stack<StackRecord> callbackStack = new();

        public CallbackQueryController(IPersonsService personsService, ITimesService timesService, CacheDecor cache)
        {
            _cache = cache;
            _personsService = personsService;
            _timesService = timesService;
        }

        public async Task InCallbackQuery(ITelegramBotClient client, Update update)
        {
            var chatId = update.CallbackQuery.Message.Chat.Id;
            var messageId = update.CallbackQuery.Message.MessageId;
            var user = update.CallbackQuery.From;
            var textMessage = update.CallbackQuery.Message.Text;
            var updateData = update.CallbackQuery.Data;

            person = await GetPerson(user);


            if (IsMaster(person))
            {
                if (_cache.TryGetValue(_masterKey, out MasterController? controller))
                    await controller.GetResponse(person, client, update, UpdateType.CallbackQuery, _cache, _timesService, _personsService);
                else
                {
                    controller = new MasterController();
                    await controller.GetResponse(person, client, update, UpdateType.CallbackQuery, _cache, _timesService, _personsService);
                }
                _cache.Set(_masterKey, controller);

                return;
            }
            if (updateData == "Вернуться")
            {
                state = Status.Default;
                person.State = State.Default;
                await client.EditMessageReplyMarkupAsync(chatId, messageId, replyMarkup: ConstantInlineMarkups.GetKeyBoard(ConstantInlineMarkups.KeyboardEnum.StartKeyboardMarkup));
                return;
            }
            if (person.State == State.ChangeProfile)
            {
                if (updateData == "Сохранить")
                {
                    person.State = State.InProfile;
                    await UpdatePersonProps(person, textMessage, Program.services.GetService<IPersonPropsService>());
                    var personProfile = await ProfileUtils.GetPersonProfile(person);
                    await client.SendTextMessageAsync(person.ChatId, personProfile, replyMarkup: ConstantInlineMarkups.GetKeyBoard(ConstantInlineMarkups.KeyboardEnum.ProfileKeyboardMarkup));
                    return;
                }

                if (!IsCorrectEntry(textMessage))
                {
                    await client.SendTextMessageAsync(chatId, "Некорректная запись, пожалуйста скопируйте сообщение выше");
                    return;
                }
                await client.SendTextMessageAsync(chatId, textMessage, replyMarkup: ConstantInlineMarkups.GetKeyBoard(ConstantInlineMarkups.KeyboardEnum.ProfileUpdateKeyboardMarkup));
                return;
            }
            if (person.State == State.InProfile)
            {
                if (updateData == "Обновить данные")
                {
                    StringBuilder builder = new StringBuilder();
                    foreach (var prop in typeof(PersonProps).GetProperties())
                    {
                        if (prop.Name == "Id")
                            continue;

                        builder.AppendLine($"{prop.GetCustomAttribute<RussianNameAttribute>().Name} - {prop.GetValue(person.Props)}");
                    }
                    await client.SendTextMessageAsync(chatId, "`" + builder.ToString() + "`", parseMode: ParseMode.Markdown);
                    person.State = State.ChangeProfile;
                    await UpdatePersonInCache(person.Id, person);
                    return;
                }
            }

            if (state == Status.AcceptRequest)
            {
                if (updateData == "Принять")
                {
                    var stackLastValue = callbackStack.Pop();
                    if (stackLastValue.controllerStatus == Status.WantCancelRecord)
                    {
                        
                        var response = await CancelRecords(stackLastValue.updateData);
                        if (!response.Status)
                        {
                            await client.SendTextMessageAsync(chatId, response.Error);
                        }
                    }
                    if (stackLastValue.controllerStatus == Status.ChooseTime)
                    {
                        var response = await AcceptDay(stackLastValue.updateData, _currentDay);
                        if (!response.Status)
                        {
                            await client.SendTextMessageAsync(chatId, response.Error);
                        }

                    }
                    else
                        throw new Exception("Невозможный заход в метод");
                }
                state = Status.Default;
                await client.SendTextMessageAsync(chatId, await GetTimeTable(), replyMarkup: ConstantInlineMarkups.GetKeyBoard(ConstantInlineMarkups.KeyboardEnum.StartKeyboardMarkup));
                return;
            }
            if (state == Status.ChooseTime)
            {
                var text = $"Запись на {GetDayOfWeekRussianName((DayOfWeek)_currentDay)} в {updateData}";
                await client.SendTextMessageAsync(chatId, text, replyMarkup: new InlineKeyboardMarkup(new List<InlineKeyboardButton>() {
                InlineKeyboardButton.WithCallbackData(text:"Принять",callbackData:"Принять"),
                InlineKeyboardButton.WithCallbackData(text:"Отмена",callbackData:"Отмена")
            }));
                callbackStack.Push(new StackRecord(person.State, state, updateData));
                state = Status.AcceptRequest;
                return;
            }
            if (state == Status.WantRecord)
            {
                var dayOfWeek = await IsCorrectDayOfWeek(updateData);
                if (dayOfWeek.Flag)
                {
                    _currentDay = dayOfWeek.Day;
                    var keyboard = await GetKeyboardAsync(_currentDay);
                    if (keyboard.InlineKeyboard.First().Count() < 1)
                    {
                        await client.SendTextMessageAsync(chatId, "На этот день записи нет", replyMarkup: ConstantInlineMarkups.GetKeyBoard(ConstantInlineMarkups.KeyboardEnum.WeekInlineMarkup));
                        return;
                    }

                    await client.EditMessageReplyMarkupAsync(chatId, messageId, replyMarkup: keyboard);
                    state = Status.ChooseTime;
                }
                return;
            }
            if (state == Status.WantCancelRecord)
            {
                var messageText = updateData.Split(" ");
                var text = $"Отмена записи на {messageText.First()} в {messageText.Last()}";
                await client.SendTextMessageAsync(chatId, text, replyMarkup: new InlineKeyboardMarkup(new List<InlineKeyboardButton>() {
                InlineKeyboardButton.WithCallbackData(text:"Принять",callbackData:"Принять"),
                InlineKeyboardButton.WithCallbackData(text:"Отмена",callbackData:"Отмена")
            }));
                callbackStack.Push(new StackRecord(person.State, state, updateData));
                state = Status.AcceptRequest;
                return;
            }
            if (updateData == "Профиль")
            {
                person.State = State.InProfile;
                var personProfile = await ProfileUtils.GetPersonProfile(person);
                await client.SendTextMessageAsync(person.ChatId, personProfile, replyMarkup: ConstantInlineMarkups.GetKeyBoard(ConstantInlineMarkups.KeyboardEnum.ProfileKeyboardMarkup));
                state = Status.Default;
                return;
            }
            if (updateData == "Меню")
            {
                state = Status.Default;
                await client.EditMessageReplyMarkupAsync(chatId, messageId, replyMarkup: ConstantInlineMarkups.GetKeyBoard(ConstantInlineMarkups.KeyboardEnum.MenuInlineMarkup));
                return;
            }
            if (updateData == "Запись")
            {
                state = Status.WantRecord;
                var replyMarkup = await GetRecordsDay();
                await client.EditMessageReplyMarkupAsync(chatId, messageId, replyMarkup: replyMarkup);
                return;
            }
            if (updateData == "Отмена записи")
            {
                if (person.Times is null || person.Times.Count() < 1)
                {
                    await client.SendTextMessageAsync(chatId, "У вас нет активных записей", replyMarkup: ConstantInlineMarkups.GetKeyBoard(ConstantInlineMarkups.KeyboardEnum.StartKeyboardMarkup));
                    return;
                }
                state = Status.WantCancelRecord;
                var markup = GetCustomKeyBoard(person.Times, true);
                await client.EditMessageReplyMarkupAsync(chatId, messageId, replyMarkup: markup);
                return;
            }
            else if (updateData == "Расписание")
            {
                var text = await GetTimeTable();
                var markup = ConstantInlineMarkups.GetKeyBoard(ConstantInlineMarkups.KeyboardEnum.MenuInlineMarkup);
                var change = markup.InlineKeyboard.First().ToList();
                foreach (var item in change)
                {
                    if (item.Text.Contains("асписани"))
                    {
                        change.Remove(item);
                        break;
                    }
                }
                markup = new InlineKeyboardMarkup(new List<List<InlineKeyboardButton>>() { change, markup.InlineKeyboard.Last().ToList() });
                await client.SendTextMessageAsync(chatId, text, replyMarkup: markup);
                return;
            }
            state = Status.Default;
            await client.SendTextMessageAsync(person.ChatId, await ProfileUtils.GetPersonProfile(person), replyMarkup: ConstantInlineMarkups.GetKeyBoard(ConstantInlineMarkups.KeyboardEnum.ProfileKeyboardMarkup));
        }
        private bool IsMaster(Person person)
        {
            if (MasterIds.Contains(person.UserId))
                return true;

            return false;
        }

        private async Task<(bool Status,string Error)> CancelRecords(string period)
        {
            var text = period.Split(" ");
            GetDayOfWeekFromRussianName(text.First());
            var time = person.Times.FirstOrDefault(x => x.Period == period);
            if (!time.Persons.Contains(person))
                return (false, "Записи на это время нет");
            
            await DeleteRecord(person,time);
            return (true,string.Empty);
        }

        private async Task<InlineKeyboardMarkup> GetRecordsDay()
        {
            var date = await GetCorrectFirstDateOfWeek();
            var times = await _timesService.GetAllTimes(date, date.LastDayOfWeek());
            var buttonRows = new List<List<InlineKeyboardButton>>();
            var listRow = new List<InlineKeyboardButton>();
            foreach (var time in times.OrderBy(x => x.Date).GroupBy(x => x.Date))
            {
                var text = GetDayOfWeekRussianName(time.Key.Date.DayOfWeek);
                listRow.Add(InlineKeyboardButton.WithCallbackData(text: text, callbackData: text));
            }
            buttonRows.Add(listRow);
            var keyboardMarkup = new InlineKeyboardMarkup(buttonRows);
            return keyboardMarkup;


        }
        private async Task<string> GetTimeTable()
        {
            StringBuilder builder = new StringBuilder();
            var date = await GetCorrectFirstDateOfWeek();
            var times = _timesService.GetAllTimes(date, date.LastDayOfWeek()).Result.OrderBy(x => x.Date).GroupBy(x => x.Date);
            var timesCount = times.Count();

            var separatorCount = 1;
            foreach (var time in times)
            {
                var month = time.Key.Date.Month;
                var day = time.Key.Date.Day;
                builder.AppendLine($"{GetDayOfWeekRussianName(time.Key.Date.DayOfWeek)} - {(day < 10 ? $"0{day}" : $"{day}")}.{(month < 10 ? $"0{month}" : $"{month}")}");

                foreach (var period in time.OrderBy(x => int.Parse(x.Period)))
                {
                    builder.Append(period.Period + ":00" + " - ");
                    builder.AppendJoin(",", period.Persons.Select(x => x.UserName));
                    builder.Append("\n");
                }
                if (separatorCount != timesCount)
                {
                    builder.AppendLine(_separator);
                    separatorCount++;
                }
            }

            var result = builder.ToString();
            if (string.IsNullOrEmpty(result))
                return "Расписания на неделю еще нет, ждёмс";


            return result;
        }
        private async Task<(bool Status,string Error)> AcceptDay(string period, DaysOfWeek currentDay)
        {
            var times = await GetAllTimeForDay(currentDay, true);


            var time = times.FirstOrDefault(x => x.Period == period);
            if (time.Persons.Contains(person))
                return (false, "Запись на это время уже есть");
            
            await CreateRecord(person,time);
            return (true, string.Empty);
        }
        private InlineKeyboardMarkup GetCustomKeyBoard(List<Time> times, bool withDayOfWeek = false)
        {
            var buttonRows = new List<List<InlineKeyboardButton>>();
            var listRow = new List<InlineKeyboardButton>();
            if (withDayOfWeek)
                foreach (var time in times)
                {
                    var text = $"{GetDayOfWeekRussianName(time.Date.DayOfWeek)} {time.Period}";
                    listRow.Add(InlineKeyboardButton.WithCallbackData(text: text, callbackData: text));
                }

            else
                foreach (var time in times)
                    listRow.Add(InlineKeyboardButton.WithCallbackData(text: time.Period, callbackData: time.Period));


            buttonRows.Add(listRow);
            buttonRows.Add(new List<InlineKeyboardButton>() {
                InlineKeyboardButton.WithCallbackData(text:"Принять",callbackData:"Принять"),
                InlineKeyboardButton.WithCallbackData(text:"Отмена",callbackData:"Отмена")
            });
            var keyboardMarkup = new InlineKeyboardMarkup(buttonRows);
            return keyboardMarkup;
        }

        private static string GetDayOfWeekRussianName(DayOfWeek dayOfWeek)
        {
            return typeof(DaysOfWeek).GetField(dayOfWeek.ToString()).GetCustomAttribute<RussianNameAttribute>().Name;
        }
        private static DayOfWeek GetDayOfWeekFromRussianName(string dayName)
        {

            var asdasd = typeof(DaysOfWeek).GetFields().Skip(1);
            foreach (var item in asdasd)
            {
                if (item.GetCustomAttribute<RussianNameAttribute>().Name== dayName)
                {
                    var adsa = typeof(DayOfWeek).GetFields().Skip(1);
                    foreach (var dayOfWeek in adsa)
                    {
                        if (dayOfWeek.Name == item.Name)
                        {
                            var aa = Enum.GetValues(typeof(DayOfWeek)).Cast<DayOfWeek>();
                            var asdasdasdasdas = aa.First(x => x.GetType().Name == dayOfWeek.Name);
                            var asdasdasdasda = typeof(DayOfWeek).GetField(dayOfWeek.Name).ReflectedType;
                            return (DayOfWeek)dayOfWeek.GetRawConstantValue();
                        }
                        
                    }
                    return DayOfWeek.Monday;
                }
            };
            return DayOfWeek.Monday;
        }

        private async Task<InlineKeyboardMarkup> GetKeyboardAsync(DaysOfWeek currentDay, bool withTrack = false)
        {
            var times = await GetAllTimeForDay(currentDay, withTrack);
            var markup = GetCustomKeyBoard(times);
            return markup;
        }
        private async Task UpdateTime(Time time)
        {
            _timesService.UpdateTime(time);
        }
        private async Task CreateRecord(Person person, Time time)
        {
            await _timesService.CreateRecord(person.Id,time.Id);
            if (person.Times.Any(x => x.Id != time.Id))
                person.Times.Add(time);
            UpdatePersonInCache(person.UserName, person);
        }
        private async Task DeleteRecord(Person person, Time time)
        {
            await _timesService.DeleteRecord(person.Id,time.Id);
            person.Times.Remove(time);
            UpdatePersonInCache(person.UserName, person);
        }
        private async Task<List<Time>> GetAllTimeForDay(DaysOfWeek dayOfWeek, bool withTrack = false)
        {
            var date = await GetCorrectFirstDateOfWeek();
            List<Time> times;
            if (withTrack)
                times = await _timesService.GetAllTimesWithTrack(date, date.LastDayOfWeek(), dayOfWeek);
            else
                times = await _timesService.GetAllTimes(date, date.LastDayOfWeek(), dayOfWeek);
            return times;
        }
        private async Task<(bool Flag, DaysOfWeek Day)> IsCorrectDayOfWeek(string dayOfWeeek)
        {
            foreach (var prop in typeof(DaysOfWeek).GetFields().Skip(1))
            {
                if (prop.GetCustomAttribute<RussianNameAttribute>().Name == dayOfWeeek)
                {
                    var day = (DaysOfWeek)prop.GetValue(typeof(DaysOfWeek));
                    return (true, day);
                }
            }
            return (false, DaysOfWeek.Default);
        }
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
        private async Task UpdatePersonProps(Person person, string propsText, IPersonPropsService propService)
        {
            var propsArray = propsText.Split("\n").Select(s => int.Parse(Regex.Match(s, @"\d+").Value)).ToArray();
            var props = new PersonProps(person.Props.Id, propsArray[0], propsArray[1], propsArray[2], propsArray[3], propsArray[4]
                , propsArray[5], propsArray[6], propsArray[7], propsArray[8], propsArray[9], propsArray[10]);

            await propService.UpdatePersonProps(props);
            person.Props = props;
            await UpdatePersonInCache(person.Id, person);
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
        private async Task UpdatePersonInCache<T>(T key, Person person)
        {
            _cache.UpdateEntry(key, person, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(30)));
        }


        private async Task<DateTime> GetCorrectFirstDateOfWeek()
        {
            return DateTime.Now.Date.DayOfWeek == DayOfWeek.Sunday ? DateTime.Now.AddDays(1).FirstDayOfWeek() : DateTime.Now.FirstDayOfWeek();
        }


        //private async Task SendErrorMessage(string message) => await client.SendTextMessageAsync(chatId, response.Error);
        private enum Status
        {
            Default,
            WantRecord,
            ChooseDayOfWeek,
            ChooseTime,
            WatchTimeTable,
            WantCancelRecord,
            AcceptRequest,
        }

        private record StackRecord(
            State personState,
            Status controllerStatus,
            string updateData);
    }
}
