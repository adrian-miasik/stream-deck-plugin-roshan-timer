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
            // First press
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
            
            // Play / Pause
            if (isTimerPaused)
            {
                ResumeTimer(args);
            }
            else
            {
                PauseTimer(args);
            }

            await base.OnKeyUp(args);
        }

        /// <summary>
        /// Resumes our timer so it can keep accumulating seconds. Useful for when the Dota 2 match is about to be
        /// unpaused.
        /// </summary>
        /// <param name="args"></param>
        private async void ResumeTimer(StreamDeckEventPayload args)
        {
            // Resume timer
            timer.Start();
            isTimerPaused = false;
            await Manager.SetTitleAsync(args.context, GetFormattedString(SettingsModel.Counter));
        }

        /// <summary>
        /// Pauses the timer so it can no longer accumulate seconds. Useful for when the Dota 2 match is paused.
        /// </summary>
        /// <param name="args"></param>
        private async void PauseTimer(StreamDeckEventPayload args)
        {
            timer.Stop();
            isTimerPaused = true;
                
            await Manager.SetTitleAsync(args.context, "Paused");
        }

        /// <summary>
        /// An update method that gets invoked every second. Intended for timer incrementing.
        /// </summary>
        /// <param name="args"></param>
        private async void Tick(StreamDeckEventPayload args)
        {
            SettingsModel.Counter++;
            await Manager.SetTitleAsync(args.context, GetFormattedString(SettingsModel.Counter));
        }

        /// <summary>
        /// Returns a string in the "0:00" format.
        /// </summary>
        /// <param name="totalSeconds"></param>
        /// <returns></returns>
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