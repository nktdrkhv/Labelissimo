using QuestPDF.Helpers;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;

namespace Labelissimo.Bot;

public class MessageSender
{
    private readonly ITelegramBotClient _bot;
    private readonly Chat _chat;
    private readonly User _user;

    public MessageSender(ITelegramBotClient bot, Chat chat, User user)
    {
        _bot = bot;
        _chat = chat;
        _user = user;
    }

    public async Task StartCommandAsync() => await _bot.SendTextMessageAsync(_chat, "Приветсвую");
    public async Task GrazieCommandAsync() => await _bot.SendTextMessageAsync(_chat, "Отправьте документ с исходными данными");
    public async Task<Message> ChooseTemplateAsync()
    {
        var templates = new List<InlineKeyboardButton>()
        {
            InlineKeyboardButton.WithCallbackData("Зеркальный документ", "mirror")
        };
        var templatesButtons = new InlineKeyboardMarkup(templates);
        return await _bot.SendTextMessageAsync(_chat, "Выберите шаблон", replyMarkup: templatesButtons);
    }
    public async Task<int> ChooseTemplateAsync(Message msg, string templateName)
    {
        await _bot.EditMessageTextAsync(_chat, msg.MessageId, $"Выбранный шаблон: **{templateName}**");
        return msg.MessageId;
    }

    public async Task<Message> UploadDocumentAsync(InputOnlineFile doc) => await _bot.SendDocumentAsync(_chat, doc);

    public async Task<Message> ChoosePageSizeAsync()
    {
        var sizes = new List<InlineKeyboardButton>()
        {
            InlineKeyboardButton.WithCallbackData("A2", "A2"),
            InlineKeyboardButton.WithCallbackData("A3", "A3"),
            InlineKeyboardButton.WithCallbackData("A4", "A4"),
            InlineKeyboardButton.WithCallbackData("A5", "A5")
        };
        var sizesButtons = new InlineKeyboardMarkup(sizes);
        return await _bot.SendTextMessageAsync(_chat, "Выберите один из заготовленных форматов или отправьте соотношение сторон через пробел: __640 480__ (ширина и высота)", replyMarkup: sizesButtons);
    }
    public async Task<Message> ChoosePageSizeAsync(Message msg, string pageSize)
    {
        var orientation = new List<InlineKeyboardButton>()
        {
            InlineKeyboardButton.WithCallbackData("Вертикально", "vertical"),
            InlineKeyboardButton.WithCallbackData("Горизонтально", "horizontal"),
        };
        var orientationButtons = new InlineKeyboardMarkup(orientation);
        return await _bot.EditMessageTextAsync(_chat, msg.MessageId, $"Выбранный формат: **{pageSize}**\nКак расположить страницу?", replyMarkup: orientationButtons);
    }
    public async Task<Message> ChoosePageSizeAsync(Message msg, string pageSize, string pageOrientation) => await _bot.EditMessageTextAsync(_chat, msg.MessageId, $"Выбранный формат: **{pageSize}**\nВыбранная ориентация страницы: **{pageOrientation}**");
    public async Task<Message> ChoosePageSizeAsync(Message msg, PageSize pageSize) => await _bot.EditMessageTextAsync(_chat, msg.MessageId, $"Выбранный формат: __ширина__ **{pageSize.Width}** : __высота__ **{pageSize.Height}**");
}