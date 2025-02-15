using Microsoft.AspNetCore.Mvc;

using ScriptBot.BLL.Interfaces;
using ScriptBot.BLL.Models.Telegram;

using Telegram.Bot.Types;

namespace ScriptBot.API.Controllers
{
    [ApiController]
    [Route("/")]
    public class TelegramWebhookController : ControllerBase
    {
        private readonly ICommandService _commandService;
        private readonly ITelegramService _telegramService;

        public TelegramWebhookController(
            ICommandService commandService,
            ITelegramService telegramApiService)
        {
            _commandService = commandService;
            _telegramService = telegramApiService;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Update update)
        {
            if (update?.Message?.Text == null || update.Message.Chat == null)
            {
                return Ok();
            }

            var telegramUpdate = new TelegramUpdateModel
            {
                ChatId = update.Message.Chat.Id,
                Username = update.Message.From?.Username,
                FirstName = update.Message.From?.FirstName,
                LastName = update.Message.From?.LastName,
                MessageText = update.Message.Text,
            };

            var result = await _commandService.HandleCommandAsync(telegramUpdate);

            // Перевірка на помилки
            if (result.IsError)
            {
                foreach (var error in result.Errors)
                {
                    await _telegramService.SendMessageAsync(telegramUpdate.ChatId, error.Description);
                }

                return Ok();
            }

            // Відправка всіх повідомлень отриманих від бізнес-логіки
            if (result.Value is List<TelegramMessageModel> messages)
            {
                foreach (var message in messages)
                {
                    await _telegramService.SendMessageAsync(message.ChatId, message.Text);
                }
            }

            return Ok();
        }
    }
}