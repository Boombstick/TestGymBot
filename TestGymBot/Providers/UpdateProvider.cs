using Telegram.Bot;
using TestGymBot.Decors;
using Telegram.Bot.Types;
using TestGymBot.Controllers;
using TestGymBot.Domain.Abstractions.Services;
using Microsoft.Extensions.Caching.Memory;

namespace TestGymBot.Providers
{
    public class UpdateProvider : IDisposable
    {
        private readonly IPersonsService _personsService;
        private readonly ITimesService _timesService;
        private readonly CacheDecor _cache;

        public UpdateProvider(IPersonsService personsService, ITimesService timesService, CacheDecor cache)
        {
            _cache = cache;
            _personsService = personsService;
            _timesService = timesService;
        }

        public async Task PostUpdateAsync(ITelegramBotClient client, Update update)
        {
            if (update.Message is not null)
            {

                var key = update.Message.From.Username + "mess";
                if (_cache.TryGetValue(key, out MessageController? _controllerMessage))
                {
                    await _controllerMessage.InMessage(client, update);
                    _cache.UpdateEntry(key, _controllerMessage, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(30)));
                }
                else
                {
                    var controller = new MessageController(_personsService, _timesService, _cache);
                    await controller.InMessage(client, update);
                    _cache.Set(key, controller, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(30)));
                }
            }

            else
            {
                var key = update.CallbackQuery.From.Username + "call";
                if (_cache.TryGetValue(key, out CallbackQueryController? _controllerCallBack))
                {
                    await _controllerCallBack.InCallbackQuery(client, update);
                    _cache.UpdateEntry(key, _controllerCallBack, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(30)));
                }
                else
                {
                    var controller = new CallbackQueryController(_personsService, _timesService, _cache);
                    await controller.InCallbackQuery(client, update); ;
                    _cache.Set(key, controller, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(30)));
                }
            }
        }



        public void Dispose() => GC.SuppressFinalize(this);
    }
}
