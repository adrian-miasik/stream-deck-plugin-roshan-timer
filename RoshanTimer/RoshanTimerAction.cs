using StreamDeckLib;
using StreamDeckLib.Messages;
using System.Threading.Tasks;
using System.Timers;

namespace RoshanTimer
{
    [ActionUuid(Uuid="com.adrian-miasik.roshan-timer.DefaultPluginAction")]
    public class RoshanTimerAction : BaseStreamDeckActionWithSettingsModel<Models.CounterSettingsModel>
    {
        private Timer timer;
        
        public override Task OnKeyDown(StreamDeckEventPayload args)
        {
            if (timer == null)
            {
                timer = new Timer();
                timer.Elapsed += (sender, eventArgs) =>
                {
                    Tick(args);
                };
                timer.AutoReset = true;
                timer.Interval = 1000; // Tick one per second
                timer.Start();
            }

            return Task.CompletedTask;
        }

        private async void Tick(StreamDeckEventPayload args)
        {
            SettingsModel.Counter++;
            await Manager.SetImageAsync(args.context, "images/blank.png");
            await Manager.SetTitleAsync(args.context, SettingsModel.Counter.ToString());
        }
    }
}