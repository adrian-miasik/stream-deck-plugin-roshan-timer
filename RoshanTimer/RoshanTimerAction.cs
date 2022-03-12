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
        private bool isTimerPaused;
        
        public override async Task OnKeyUp(StreamDeckEventPayload args)
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
                isTimerPaused = false;
                await Manager.SetTitleAsync(args.context, GetFormattedString(SettingsModel.Counter));
                await Manager.SetImageAsync(args.context, "images/blank.png");
                
                // Early exit
                return;
            }
            
            if (isTimerPaused)
            {
                timer.Start();
                isTimerPaused = false;
                await Manager.SetTitleAsync(args.context, GetFormattedString(SettingsModel.Counter));
            }
            else if (!isTimerPaused)
            {
                // Pause timer
                timer.Stop();
                isTimerPaused = true;
                
                await Manager.SetTitleAsync(args.context, "Paused");
            }

            await base.OnKeyUp(args);
        }

        private async void Tick(StreamDeckEventPayload args)
        {
            SettingsModel.Counter++;
            await Manager.SetTitleAsync(args.context, GetFormattedString(SettingsModel.Counter));
        }

        private string GetFormattedString(int totalSeconds)
        {
            int totalMinutes = totalSeconds / 60;
            if (totalMinutes == 0)
            {
                return totalMinutes + ":" + totalSeconds.ToString("00");
            }
            
            return totalMinutes + ":" + (totalSeconds - totalMinutes * 60).ToString("00");
        }
    }
}