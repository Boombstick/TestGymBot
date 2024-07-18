using System.Text;
using Telegram.Bot;
using FluentDateTime;
using TestGymBot.Decors;
using TestGymBot.Domain;
using System.Reflection;
using Telegram.Bot.Types;
using TestGymBot.Domain.Constants;
using TestGymBot.Domain.Attributes;
using Telegram.Bot.Types.ReplyMarkups;
using Microsoft.Extensions.Caching.Memory;
using TestGymBot.Domain.Abstractions.Services;
using static TestGymBot.Domain.Constants.ConstantInlineMarkups;

namespace TestGymBot.Controllers
{
    public class MasterController
    {
        private Person _person;
        private Update _update;
        private CacheDecor _cache;
        private UpdateType _updateType;
        private string _checkBox = "[x]";
        private ITelegramBotClient _client;
        private const string _separator = "-------------------";


        private const string _changeKeyboardCacheKey = "ChangeKeyboard";
        private const string _keyboardCacheKey = "keyboard";

        private long chatId;
        private int messageId;

        private ITimesService _timeService;
        private MasterStatus state = MasterStatus.Default;
        private ChangeStatus _changeState = ChangeStatus.Default;
        private Week _week = Week.Current;
        private DaysOfWeek _currentDay = DaysOfWeek.Monday;
        private InlineKeyboardMarkup _currentMarkup;

        public MasterController()
        {

        }


        public async Task GetResponse(Person person, ITelegramBotClient client, Update update, UpdateType updateType, CacheDecor cache, ITimesService timeService, IPersonsService personsService)
        {
            _person = person;
            _updateType = updateType;
            _client = client;
            _update = update;
            _cache = cache;
            _timeService = timeService;
            _currentMarkup = update.CallbackQuery.Message.ReplyMarkup;
            chatId = update.CallbackQuery.Message.Chat.Id;
            messageId = update.CallbackQuery.Message.MessageId;

            if (_updateType == UpdateType.Message)
                await GetMessageResponse(_person, _client, _update);

            else
                await GetCallbackResponse(_person, _client, _update);
        }

        private async Task GetCallbackResponse(Person person, ITelegramBotClient client, Update update)
        {
            var updateData = update.CallbackQuery.Data;
            if (updateData == "Вернуться")
            {
                state = MasterStatus.Default;
                person.State = State.Default;
                await client.EditMessageReplyMarkupAsync(chatId, messageId, replyMarkup: GetKeyBoard(KeyboardEnum.StartKeyboardMarkup));
                return;
            }
            if (state == MasterStatus.ChangeTimeTable)
            {
                if (updateData == "Добавить")
                {
                    state = MasterStatus.AddOrDeleteWeek;
                    _changeState = ChangeStatus.Add;
                    await client.EditMessageReplyMarkupAsync(chatId, messageId, replyMarkup: GetKeyBoard(KeyboardEnum.WeekInlineMarkup));
                    return;
                }
                else if (updateData == "Удалить")
                {
                    state = MasterStatus.AddOrDeleteWeek;
                    _changeState = ChangeStatus.Delete;
                    await client.EditMessageReplyMarkupAsync(chatId, messageId, replyMarkup: GetKeyBoard(KeyboardEnum.WeekInlineMarkup));

                    return;
                }
            }
            else if (state == MasterStatus.AddOrDeleteWeek)
            {
                var dayOfWeek = await IsCorrectDayOfWeek(updateData);
                if (dayOfWeek.Flag)
                {
                    _currentDay = dayOfWeek.Day;
                    state = MasterStatus.AddOrDeleteHours;
                    if (!_cache.TryGetValue(_changeKeyboardCacheKey, out InlineKeyboardMarkup markup))
                    {
                        markup = await GetKeyboardForChange(_week, _currentDay, _changeState);
                        _cache.Set(_changeKeyboardCacheKey, markup);
                    }

                    await client.EditMessageReplyMarkupAsync(chatId, messageId, replyMarkup: markup);
                }
            }
            else if (state == MasterStatus.AddOrDeleteHours)
            {
                if (updateData == "Принять")
                {
                    if (_changeState == ChangeStatus.Add)
                    {
                        var validString = await AcceptDay(_currentDay, true);
                        state = MasterStatus.Default;
                        await client.SendTextMessageAsync(chatId, "День изменён", replyMarkup: GetKeyBoard(KeyboardEnum.StartKeyboardMarkup));
                    }
                    else if (_changeState == ChangeStatus.Delete)
                    {
                        var validString = await AcceptDay(_currentDay, true);
                        state = MasterStatus.Default;
                        foreach (var pers in validString.Persons)
                        {
                            await client.SendTextMessageAsync(pers.ChatId, $"Запись на {typeof(DaysOfWeek).GetField(_currentDay.ToString()).GetCustomAttribute<RussianNameAttribute>().Name} отменена");
                        }
                        await client.SendTextMessageAsync(chatId, "День изменён И разосланы сообщения", replyMarkup: GetKeyBoard(KeyboardEnum.StartKeyboardMarkup));
                    }
                    _cache.Remove(_changeKeyboardCacheKey);
                    return;
                }
                else if (updateData == "Отменить")
                {
                    state = MasterStatus.Default;
                    await client.SendTextMessageAsync(chatId, await GetTimeTable(Week.Current), replyMarkup: GetKeyBoard(KeyboardEnum.StartKeyboardMarkup));
                    _cache.Remove(_changeKeyboardCacheKey);
                    return;
                }


                if (!_cache.TryGetValue(_changeKeyboardCacheKey, out InlineKeyboardMarkup markup))
                {
                    markup = await GetKeyboardForChange(_week, _currentDay, _changeState);
                    _cache.Set(_changeKeyboardCacheKey, markup);
                }

                if (updateData.StartsWith(_checkBox))
                {
                    var updateKeyboard = await InlineKeyboardUtils.DeleteHourFlag(update.CallbackQuery.Data, markup);
                    await client.EditMessageReplyMarkupAsync(chatId, messageId, replyMarkup: updateKeyboard);
                }
                else
                {
                    var updateKeyboard = await InlineKeyboardUtils.AddHourFlag(update.CallbackQuery.Data, markup);
                    await client.EditMessageReplyMarkupAsync(chatId, messageId, replyMarkup: updateKeyboard);
                }
            }
            else if (state == MasterStatus.OnTimeTable)
            {
                if (updateData == "Показать")
                {
                    state = MasterStatus.Default;
                    var message = await GetTimeTable(_week);
                    await client.SendTextMessageAsync(chatId, string.IsNullOrEmpty(message) ? "Расписания еще нет" : message, replyMarkup: GetKeyBoard(KeyboardEnum.StartKeyboardMarkup));
                    return;
                }


                if (updateData == "Составить")
                {
                    state = MasterStatus.SetTimeTable;
                    await client.EditMessageReplyMarkupAsync(chatId, messageId, replyMarkup: GetKeyBoard(KeyboardEnum.WeekInlineMarkup));
                    return;
                }

                if (updateData == "Изменить")
                {
                    state = MasterStatus.ChangeTimeTable;
                    await client.EditMessageReplyMarkupAsync(chatId, messageId, replyMarkup: GetKeyBoard(KeyboardEnum.AddOrDelete));
                    return;
                }
                state = MasterStatus.Default;
            }
            else if (state == MasterStatus.ChooseWeek)
            {
                if (updateData == "Прошлая")
                {
                    InlineKeyboardMarkup markup = new InlineKeyboardMarkup(GetKeyBoard(KeyboardEnum.MasterTimeTableInlineMarkup).InlineKeyboard.Select(x => x.Where(q => !q.CallbackData.Contains("менит"))));
                    await client.EditMessageReplyMarkupAsync(update.CallbackQuery.Message.Chat.Id, update.CallbackQuery.Message.MessageId, replyMarkup: markup);
                    _week = Week.Last;
                }
                else if (updateData == "Текущая")
                {
                    await client.EditMessageReplyMarkupAsync(update.CallbackQuery.Message.Chat.Id, update.CallbackQuery.Message.MessageId, replyMarkup: GetKeyBoard(KeyboardEnum.MasterTimeTableInlineMarkup));
                    _week = Week.Current;
                }
                else if (updateData == "Следующая")
                {
                    await client.EditMessageReplyMarkupAsync(update.CallbackQuery.Message.Chat.Id, update.CallbackQuery.Message.MessageId, replyMarkup: GetKeyBoard(KeyboardEnum.MasterTimeTableInlineMarkup));
                    _week = Week.Next;
                }
                state = MasterStatus.OnTimeTable;
            }
            else if (updateData == "Меню")
            {
                state = MasterStatus.Default;
                await client.EditMessageReplyMarkupAsync(update.CallbackQuery.Message.Chat.Id, update.CallbackQuery.Message.MessageId, replyMarkup: GetKeyBoard(KeyboardEnum.MasterMenuInlineMarkup));
            }
            else if (updateData == "Расписание")
            {
                state = MasterStatus.ChooseWeek;
                await client.EditMessageReplyMarkupAsync(update.CallbackQuery.Message.Chat.Id, update.CallbackQuery.Message.MessageId, replyMarkup: GetKeyBoard(KeyboardEnum.WeeksNumbersInlineMarkup));
            }
            else if (updateData == "Профиль")
            {
                person.State = State.InProfile;
                var personProfile = await ProfileUtils.GetPersonProfile(person);
                await client.SendTextMessageAsync(person.ChatId, personProfile, replyMarkup: GetKeyBoard(KeyboardEnum.ProfileKeyboardMarkup));
                state = MasterStatus.Default;
            }
            else if (state == MasterStatus.SetTimeTable)
            {
                var dayOfWeek = await IsCorrectDayOfWeek(updateData);
                if (dayOfWeek.Flag)
                {
                    _currentDay = dayOfWeek.Day;
                    state = MasterStatus.SetHoursTime;
                    //SetDefaultKeyboardMarkupInCache(ConstantInlineMarkups.GetKeyBoard(ConstantInlineMarkups.KeyboardEnum.HoursInlineMarkup), defaultKeyboardCacheKey);
                    await client.EditMessageReplyMarkupAsync(chatId, messageId, replyMarkup: GetKeyBoard(KeyboardEnum.HoursInlineMarkup));
                }
            }
            else if (state == MasterStatus.SetHoursTime)
            {
                if (updateData == "Принять")
                {
                    var validString = await AcceptDay(_currentDay);

                    await client.SendTextMessageAsync(chatId, validString.Content, replyMarkup: GetKeyBoard(KeyboardEnum.StartKeyboardMarkup));
                    _cache.Remove(_keyboardCacheKey + _currentDay);
                    return;
                }
                else if (updateData == "Отменить")
                {
                    await client.SendTextMessageAsync(chatId, await GetTimeTable(Week.Current), replyMarkup: GetKeyBoard(KeyboardEnum.StartKeyboardMarkup));
                    _cache.Remove(_keyboardCacheKey + _currentDay);
                    return;
                }

                if (!_cache.TryGetValue(_keyboardCacheKey + _currentDay, out InlineKeyboardMarkup? markup))
                {
                    markup = GetKeyBoard(KeyboardEnum.HoursInlineMarkup);
                    _cache.Set(_keyboardCacheKey + _currentDay, markup, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(5)));
                }

                if (updateData.StartsWith(_checkBox))
                {
                    var updateKeyboard = await InlineKeyboardUtils.DeleteHourFlag(update.CallbackQuery.Data, markup);
                    await client.EditMessageReplyMarkupAsync(chatId, messageId, replyMarkup: updateKeyboard);
                }
                else
                {
                    var updateKeyboard = await InlineKeyboardUtils.AddHourFlag(update.CallbackQuery.Data, markup);
                    await client.EditMessageReplyMarkupAsync(chatId, messageId, replyMarkup: updateKeyboard);
                }
            }
            else
            {
                state = MasterStatus.Default;
                var personProfile = await ProfileUtils.GetPersonProfile(person);
                await client.SendTextMessageAsync(person.ChatId, personProfile, replyMarkup: GetKeyBoard(KeyboardEnum.ProfileKeyboardMarkup));
            }
        }
        private async Task<string> GetTimeTable(Week week)
        {
            StringBuilder builder = new StringBuilder();
            var date = DateTime.Now.FirstDayOfWeek();

            if (week == Week.Next)
                date = date.WeekAfter();
            else if (week == Week.Last)
                date = date.WeekEarlier();

            var times = _timeService.GetAllTimes(date, date.LastDayOfWeek()).Result.GroupBy(x => x.Date);
            var timesCount = times.Count();

            var separatorCount = 1;

            foreach (var time in times)
            {
                var month = time.Key.Date.Month;
                var day = time.Key.Date.Day;
                builder.AppendLine($"{typeof(DaysOfWeek).GetField(time.Key.Date.DayOfWeek.ToString()).GetCustomAttribute<RussianNameAttribute>().Name} - {(day < 10 ? $"0{day}" : $"{day}")}.{(month < 10 ? $"0{month}" : $"{month}")}");
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

            return builder.ToString();
        }
        private async Task AddTimesToDay(InlineKeyboardMarkup markup, DaysOfWeek currentDay)
        {
            DateTime date = DateTime.Now;
            if (_week == Week.Next)
                date = date.WeekAfter();

            var dayOfWeek = date.FirstDayOfWeek();
            while (dayOfWeek.DayOfWeek != (DayOfWeek)currentDay)
                dayOfWeek = dayOfWeek.AddDays(1);
            List<string> times = new();
            foreach (var keyboard in markup.InlineKeyboard)
                foreach (var button in keyboard)
                    if (button.Text.StartsWith(_checkBox))
                    {
                        var text = button.Text.Replace(_checkBox, "");
                        times.Add(text);
                    }
            await _timeService.AddTimes(times, dayOfWeek);
        }
        private async Task GetMessageResponse(Person person, ITelegramBotClient client, Update update)
        {

        }
        private async Task<InlineKeyboardMarkup> GetKeyboardForChange(Week week, DaysOfWeek currentDay, ChangeStatus changeStatus)
        {
            var date = DateTime.Now;
            if (week == Week.Next)
                date = date.WeekAfter();
            else if (week == Week.Last)
                date = date.WeekEarlier();
            date = date.FirstDayOfWeek();
            while (date.DayOfWeek != (DayOfWeek)currentDay)
                date = date.AddDays(1);
            var times = _timeService.GetAllTimes(date.FirstDayOfWeek(), date.LastDayOfWeek()).Result.Where(x => x.Date.Month == date.Date.Month && x.Date.Day == date.Date.Day).OrderBy(x => x.Period).Select(x => x.Period).ToList();
            List<string> rightTime = times;
            if (changeStatus == ChangeStatus.Add)
                rightTime = ConstantTimesOfDay.ConstantTimes.Except(times).ToList();

            return await InlineKeyboardUtils.CreateInlineKeyboard(rightTime);
        }
        private async Task<(string Content, List<Person> Persons)> AcceptDay(DaysOfWeek currentDay, bool changeDay = false)
        {
            InlineKeyboardMarkup? markup = _currentMarkup;
            StringBuilder builder = new StringBuilder();
            DateTime date = DateTime.Now;

            if (_week == Week.Next)
                date = date.WeekAfter();
            else if (_week == Week.Last)
                date = date.WeekEarlier();

            var dayOfWeek = DateTime.Now.Date.DayOfWeek == DayOfWeek.Sunday ? date.AddDays(1).FirstDayOfWeek(): date.FirstDayOfWeek();
            while (dayOfWeek.DayOfWeek != (DayOfWeek)currentDay)
                dayOfWeek = dayOfWeek.AddDays(1);
            List<string> times = new();
            builder.AppendLine(typeof(DaysOfWeek).GetField(currentDay.ToString()).GetCustomAttribute<RussianNameAttribute>().Name);
            foreach (var keyboard in markup.InlineKeyboard)
                foreach (var button in keyboard)
                    if (button.Text.StartsWith(_checkBox))
                    {
                        var text = button.Text.Replace(_checkBox, "");
                        times.Add(text);
                        builder.AppendLine(text + ":00" + " -");
                    }

            if (changeDay)
            {
                if (_changeState == ChangeStatus.Add)
                    await _timeService.AddTimes(times, dayOfWeek);
                else if (_changeState == ChangeStatus.Delete)
                {
                    var timesForDelete = _timeService.GetAllTimes(date, date.LastDayOfWeek()).Result.Where(q => times.Any(x => x == q.Period)).ToList();
                    var persons = timesForDelete.SelectMany(x => x.Persons).ToList();
                    await _timeService.DeleteTimes(timesForDelete);

                    return (builder.ToString(), persons);
                }
            }
            else
                await _timeService.CreateTime(times, dayOfWeek);
            return (builder.ToString(), null);
        }

        private async Task DeleteRecordsNotification()
        {

        }


        private async Task<(bool Flag, DaysOfWeek Day)> IsCorrectDayOfWeek(string dayOfWeek)
        {
            foreach (var prop in typeof(DaysOfWeek).GetFields().Skip(1))
            {
                if (prop.GetCustomAttribute<RussianNameAttribute>().Name == dayOfWeek)
                {
                    var day = (DaysOfWeek)prop.GetValue(typeof(DaysOfWeek));
                    return (true, day);
                }
            }
            return (false, DaysOfWeek.Default);
        }
        private enum MasterStatus
        {
            Default,
            ChooseWeek,
            ChooseDayOfWeek,
            OnTimeTable,
            WatchTimeTable,
            ChangeTimeTable,
            AddOrDeleteHours,
            SetTimeTable,
            SetHoursTime,
            AddOrDeleteWeek,
            AddHours,
        }
        private enum Week
        {
            Last,
            Current,
            Next
        }
        private enum ChangeStatus
        {
            Add,
            Delete,
            Default
        }

    }
}
