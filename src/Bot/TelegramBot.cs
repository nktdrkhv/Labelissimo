using EasyBotFramework;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using QuestPDF.Fluent;
using Labelissimo.Templates;
using Labelissimo.Helpers;

namespace Labelissimo.Bot;

public class InnerTelegramBot : EasyBot
{
    public InnerTelegramBot(string token) : base(new()
    {
        BotToken = token,
        TimeSpanBetweenCacheClean = 600_000,
        AllowedIdleTime = 1000_000
    })
    { }

    public override async Task OnPrivateChat(Chat chat, User user, UpdateInfo update)
    {
        try
        {
            if (update.UpdateKind == UpdateKind.NewMessage && update.MsgCategory == MsgCategory.Text)
            {
                var messageSender = new MessageSender(Telegram, chat, user);
                if (update.Message!.Text == "/start")
                    await messageSender.StartCommandAsync();
                if (update.Message!.Text == "/grazie")
                {
                    await messageSender.GrazieCommandAsync();
                    while (await NewMessage(update) is var msgCategory)
                        if (msgCategory is MsgCategory.MediaOrDoc &&
                            update.Message.Type is MessageType.Document)
                            break;

                    var fileHelper = new FileHelper(Telegram);
                    var documentPath = await fileHelper.DownloadFileAsync(update.Message.Document!.FileId);
                    var textSource = new TxtHelper(documentPath);

                    var chooseTemplateMsg = await messageSender.ChooseTemplateAsync();
                    var chosenTemplate = await ButtonClicked(update, chooseTemplateMsg);

                    // todo: get sizes before template setup

                    IDocument? doc;
                    switch (chosenTemplate)
                    {
                        case "mirror":
                            await messageSender.ChooseTemplateAsync(chooseTemplateMsg, "Зеркальный документ");
                            var options = await MirrorTemplateSetup(update, messageSender, textSource);
                            doc = new MirrorDocument(options);
                            break;
                        default:
                            doc = null;
                            break;
                    }
                    if (doc is not null)
                    {
                        var pdf = doc.GeneratePdf();
                        var inputFile = fileHelper.PrepareFile(pdf);
                        _ = await messageSender.UploadDocumentAsync(inputFile);
                    }
                }
            }
        }
        catch
        {
            await Telegram.SendTextMessageAsync(chat, "Произошла ошибка, начните заново");
        }
    }

    public async Task<MirrorDocumentOptions> MirrorTemplateSetup(UpdateInfo update, MessageSender sender, TxtHelper txt)
    {
        var options = new MirrorDocumentOptions();

        /* --------- Page Size Setup ---------*/

        var choosePageSizeMsg = await sender.ChoosePageSizeAsync();
        PageSize? pageSize = null;
        while (await NextEvent(update) is var updateKind)
            if (updateKind is UpdateKind.NewMessage && update.MsgCategory is MsgCategory.Text)
            {
                var textNums = update.Message.Text!.Split(' ', 2);
                var width = float.Parse(textNums[0]);
                var height = float.Parse(textNums[1]);
                pageSize = new PageSize(width, height);
                choosePageSizeMsg = await sender.ChoosePageSizeAsync(choosePageSizeMsg, pageSize);
            }
            else if (updateKind is UpdateKind.CallbackQuery && update.Message.MessageId == choosePageSizeMsg.MessageId)
            {
                var pageSizeText = update.CallbackData;
                pageSize = update.CallbackData switch
                {
                    "A2" => PageSizes.A2,
                    "A3" => PageSizes.A3,
                    "A4" => PageSizes.A4,
                    "A5" => PageSizes.A5,
                    _ => throw new Exception(),
                };

                choosePageSizeMsg = await sender.ChoosePageSizeAsync(choosePageSizeMsg, pageSizeText);
                var pageOrientation = await ButtonClicked(update, choosePageSizeMsg);
                string orientationText;
                switch (pageOrientation)
                {
                    case "vertical":
                        orientationText = "вертикальная";
                        pageSize.Portrait();
                        break;
                    case "horizontal":
                        orientationText = "горизонтальная";
                        pageSize.Landscape();
                        break;
                    default:
                        throw new Exception();
                }
                choosePageSizeMsg = await sender.ChoosePageSizeAsync(choosePageSizeMsg, pageSizeText, orientationText);
            }
        options.PageSize = pageSize!;

        /* --------- Text Setup ---------*/

        //

        return options;
    }
}