using Telegram.Bot.Types.ReplyMarkups;
using TestGymBot.Domain.Constants;
using TestGymBot.Domain;

namespace TestGymBot
{
    public static class InlineKeyboardUtils
    {
        public static async Task<InlineKeyboardMarkup> AddHourFlag(string data, InlineKeyboardMarkup keyboard)
        {
            foreach (var item in keyboard.InlineKeyboard)
            {
                foreach (var button in item)
                {
                    var number = data;
                    if (button.Text.Contains(number))
                    {
                        button.Text = $"[x]{button.Text}";
                        button.CallbackData = $"[x]{button.Text}";
                        return keyboard;
                    }
                }
            }
            return keyboard;
        }
        public static async Task<InlineKeyboardMarkup> DeleteHourFlag(string data, InlineKeyboardMarkup keyboard)
        {
            foreach (var item in keyboard.InlineKeyboard)
            {
                foreach (var button in item)
                {
                    var number = data;
                    if (number == button.Text)
                    {
                        button.Text = $"{data.Substring(3)}";
                        button.CallbackData = $"{data.Substring(3)}";
                        return keyboard;
                    }
                }
            }
            return keyboard;
        }

        public static async Task<InlineKeyboardMarkup> CreateInlineKeyboard(List<string> text)
        {
            var buttonRows = new List<List<InlineKeyboardButton>>();
            var chunk = 0;
            var size = 6;
            while (chunk < text.Count())
            {
                buttonRows.Add(text.Skip(chunk).Take(size).Select(x => InlineKeyboardButton.WithCallbackData(text: x, callbackData: x)).ToList());
                chunk += size;
            }
            buttonRows.Add(new List<InlineKeyboardButton>{
                    InlineKeyboardButton.WithCallbackData(text:"Принять",callbackData:"Принять"),
                    InlineKeyboardButton.WithCallbackData(text:"Отменить",callbackData:"Отменить") });

            var keyboardMarkup = new InlineKeyboardMarkup(buttonRows);
            return keyboardMarkup;
        }
    }
}
