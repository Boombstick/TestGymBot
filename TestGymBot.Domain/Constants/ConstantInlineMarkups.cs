using System.Reflection;
using Telegram.Bot.Types.ReplyMarkups;

namespace TestGymBot.Domain.Constants
{
    public static class ConstantInlineMarkups
    {
        private static readonly InlineKeyboardMarkup AddOrDelete = new(new[]{
                new[] {
                    InlineKeyboardButton.WithCallbackData(text:"Добавить",callbackData:"Добавить"),
                    InlineKeyboardButton.WithCallbackData(text:"Удалить",callbackData:"Удалить")
                }});
        private static readonly InlineKeyboardMarkup MenuInlineMarkup = new(new[]{
                new[] {
                    InlineKeyboardButton.WithCallbackData(text:"Расписание",callbackData:"Расписание"),
                    InlineKeyboardButton.WithCallbackData(text:"Запись",callbackData:"Запись"),
                    InlineKeyboardButton.WithCallbackData(text:"Отмена записи",callbackData:"Отмена записи")
                } });
        private static readonly InlineKeyboardMarkup WeekInlineMarkup = new(new[]{
                new[] {
                    InlineKeyboardButton.WithCallbackData(text:"Пн",callbackData:"Пн"),
                    InlineKeyboardButton.WithCallbackData(text:"Вт",callbackData:"Вт"),
                    InlineKeyboardButton.WithCallbackData(text:"Ср",callbackData:"Ср"),
                    InlineKeyboardButton.WithCallbackData(text:"Чт",callbackData:"Чт"),
                    InlineKeyboardButton.WithCallbackData(text:"Пт",callbackData:"Пт"),
                    InlineKeyboardButton.WithCallbackData(text:"Сб",callbackData:"Сб"),
                }});
        private static readonly InlineKeyboardMarkup HoursInlineMarkup = new(new[]{
                new[] {
                    InlineKeyboardButton.WithCallbackData(text:"9",callbackData:"9"),
                    InlineKeyboardButton.WithCallbackData(text:"10",callbackData:"10"),
                    InlineKeyboardButton.WithCallbackData(text:"11",callbackData:"11"),
                    InlineKeyboardButton.WithCallbackData(text:"12",callbackData:"12"),
                    InlineKeyboardButton.WithCallbackData(text:"13",callbackData:"13"),
                    InlineKeyboardButton.WithCallbackData(text:"14",callbackData:"14"),
                },
                new[]{
                    InlineKeyboardButton.WithCallbackData(text:"15",callbackData:"15"),
                    InlineKeyboardButton.WithCallbackData(text:"16",callbackData:"16"),
                    InlineKeyboardButton.WithCallbackData(text:"17",callbackData:"17"),
                    InlineKeyboardButton.WithCallbackData(text:"18",callbackData:"18"),
                    InlineKeyboardButton.WithCallbackData(text:"19",callbackData:"19"),
                    InlineKeyboardButton.WithCallbackData(text:"20",callbackData:"20"),
                    InlineKeyboardButton.WithCallbackData(text:"21",callbackData:"21"),
                },
                new[]{
                    InlineKeyboardButton.WithCallbackData(text:"Принять",callbackData:"Принять"),
                    InlineKeyboardButton.WithCallbackData(text:"Отменить",callbackData:"Отменить")
                }
        });
        private static readonly InlineKeyboardMarkup FilesKeyboardMarkup = new(new[] {
                new [] {

                    InlineKeyboardButton.WithCallbackData(text:"Зарядка",callbackData:"Зарядка"),
                    InlineKeyboardButton.WithCallbackData(text:"Загрузить кружок",callbackData:"Загрузить кружок"),
                    InlineKeyboardButton.WithCallbackData(text:"Кардио",callbackData:"Кардио"),
                    InlineKeyboardButton.WithCallbackData(text:"Силовые тренировки",callbackData:"Силовые тренировки")
                }});
        private static readonly InlineKeyboardMarkup StartKeyboardMarkup = new(new[]{
                new[] {
                    InlineKeyboardButton.WithCallbackData(text:"Меню",callbackData:"Меню"),
                    InlineKeyboardButton.WithCallbackData(text:"Профиль",callbackData:"Профиль"),
                    InlineKeyboardButton.WithCallbackData(text:"Расписание",callbackData:"Расписание")
                } });
        private static readonly InlineKeyboardMarkup ProfileKeyboardMarkup = new(new[]{
            new[]
            {
                    InlineKeyboardButton.WithCallbackData(text:"Обновить данные",callbackData:"Обновить данные"),
                    InlineKeyboardButton.WithCallbackData(text:"Меню",callbackData:"Меню")
            }
        });
        private static readonly InlineKeyboardMarkup MasterMenuInlineMarkup = new(new[]{
                new[] {
                    InlineKeyboardButton.WithCallbackData(text:"Расписание",callbackData:"Расписание"),
                    InlineKeyboardButton.WithCallbackData(text:"Люди",callbackData:"Люди"),
                    InlineKeyboardButton.WithCallbackData(text:"Отмена записи",callbackData:"Отменить запись")
                } });
        private static readonly InlineKeyboardMarkup WeeksNumbersInlineMarkup = new(new[]{
                new[] {
                    InlineKeyboardButton.WithCallbackData(text:"Прошлая",callbackData:"Прошлая"),
                    InlineKeyboardButton.WithCallbackData(text:"Текущая",callbackData:"Текущая"),
                    InlineKeyboardButton.WithCallbackData(text:"Следующая",callbackData:"Следующая"),
                }});
        private static readonly InlineKeyboardMarkup MasterTimeTableInlineMarkup = new(new[]{
                new[] {
                    InlineKeyboardButton.WithCallbackData(text:"Показать",callbackData:"Показать"),
                    InlineKeyboardButton.WithCallbackData(text:"Изменить",callbackData:"Изменить"),
                    InlineKeyboardButton.WithCallbackData(text:"Составить",callbackData:"Составить")
                } });
        private static readonly InlineKeyboardMarkup ProfileUpdateKeyboardMarkup = new(new[]{
            new[]
            {
                    InlineKeyboardButton.WithCallbackData(text:"Сохранить",callbackData:"Сохранить"),
                    InlineKeyboardButton.WithCallbackData(text:"Отменить",callbackData:"Отменить")
            }
        });
        public static InlineKeyboardMarkup GetKeyBoard(KeyboardEnum keybord, bool includeOptions = false)
        {
            var buttonRows = new List<List<InlineKeyboardButton>>();

            var field = typeof(ConstantInlineMarkups).GetFields(BindingFlags.NonPublic | BindingFlags.Static)
                .FirstOrDefault(x => x.Name == keybord.ToString());
            var markup = (InlineKeyboardMarkup)field.GetValue(typeof(InlineKeyboardMarkup));
            foreach (var buttonRow in markup.InlineKeyboard)
            {
                var listRow = new List<InlineKeyboardButton>();
                foreach (var button in buttonRow)
                    listRow.Add(InlineKeyboardButton.WithCallbackData(text: button.Text, callbackData: button.CallbackData));
                buttonRows.Add(listRow);
            }
            if (includeOptions)
            {
                buttonRows.Add(new List<InlineKeyboardButton>() {
                InlineKeyboardButton.WithCallbackData(text:"Принять",callbackData:"Принять"),
                InlineKeyboardButton.WithCallbackData(text:"Отмена",callbackData:"Отмена")
            });
            }
            else if (keybord != KeyboardEnum.StartKeyboardMarkup)
                buttonRows.Add(new List<InlineKeyboardButton>() { InlineKeyboardButton.WithCallbackData(text: "Вернуться", callbackData: "Вернуться") });
            
            var keyboardMarkup = new InlineKeyboardMarkup(buttonRows);
            return keyboardMarkup;
        }
        public enum KeyboardEnum
        {
            AddOrDelete,
            WeekInlineMarkup,
            MenuInlineMarkup,
            HoursInlineMarkup,
            FilesKeyboardMarkup,
            StartKeyboardMarkup,
            ProfileKeyboardMarkup,
            MasterMenuInlineMarkup,
            WeeksNumbersInlineMarkup,
            MasterTimeTableInlineMarkup,
            ProfileUpdateKeyboardMarkup,
        }


        //public static readonly InlineKeyboardMarkup BossReportKeyboardMarkup = new(new[]{
        //        new [] {
        //                                                    InlineKeyboardButton.WithCallbackData(text:"Зарядка",callbackData:"Зарядка"),
        //            InlineKeyboardButton.WithCallbackData(text:"Загрузить кружок",callbackData:"Загрузить кружок"),
        //            InlineKeyboardButton.WithCallbackData(text:"Кардио",callbackData:"Кардио"),
        //            InlineKeyboardButton.WithCallbackData(text:"Силовые тренировки",callbackData:"Силовые тренировки")

        //            "Отправить профиль","Отправить питание","Отправить тренировку"," " 
        //        } });


    }
}
