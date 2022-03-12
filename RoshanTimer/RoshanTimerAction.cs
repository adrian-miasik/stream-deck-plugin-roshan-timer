using StreamDeckLib;
using StreamDeckLib.Messages;
using System.Threading.Tasks;

namespace RoshanTimer
{
    [ActionUuid(Uuid="com.adrian-miasik.roshan-timer.DefaultPluginAction")]
    public class RoshanTimerAction : BaseStreamDeckActionWithSettingsModel<Models.CounterSettingsModel>
    {
        public override async Task OnKeyDown(StreamDeckEventPayload args)
        {
            SettingsModel.Counter++;
            await Manager.SetImageAsync(args.context, "images/blank.png");
            await Manager.SetTitleAsync(args.context, SettingsModel.Counter.ToString());
        }
    }
}