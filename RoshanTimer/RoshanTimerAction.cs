using System;
using StreamDeckLib;
using StreamDeckLib.Messages;
using System.Threading.Tasks;
using System.Timers;

namespace RoshanTimer
{
    [ActionUuid(Uuid="com.adrian-miasik.roshan-timer.DefaultPluginAction")]
    public class RoshanTimerAction : BaseStreamDeckActionWithSettingsModel<Models.CounterSettingsModel>
    {
        private Timer applicationTimer; // Used for processing input. Cannot be paused.
        private bool isKeyHeld;
        private DateTime pressedKeyTime;
        private bool isApplicationRestarting;

        private Timer roshanTimer; // Used for keeping track of roshan's respawn time. Can be paused.
        private bool isRoshanTimerPaused;

        public override Task OnKeyDown(StreamDeckEventPayload args)
        {
            pressedKeyTime = DateTime.Now;
            isKeyHeld = true;

            return base.OnKeyDown(args);
        }

        public override async Task OnKeyUp(StreamDeckEventPayload args)
        {
            isKeyHeld = false;
            
            // Ignore re-init when the user was holding to reset the roshan timer/app.
            if (isApplicationRestarting)
            {
                isApplicationRestarting = false;
                return;
            }

            // First press - Create application timer to poll for inputs such as long presses
            if (applicationTimer == null)
            {
                applicationTimer = new Timer();
                applicationTimer.Elapsed += (sender, eventArgs) =>
                {
                    ApplicationTimerTick(args);
                };
                applicationTimer.AutoReset = true;
                applicationTimer.Interval = 100; // 10 ticks per second
                applicationTimer.Start();
            }
            
            // First press - Create roshan timer
            if (roshanTimer == null)
            {
                roshanTimer = new Timer();
                roshanTimer.Elapsed += (sender, eventArgs) =>
                {
                    RoshanTimerTick(args);
                };
                roshanTimer.AutoReset = true;
                roshanTimer.Interval = 1000; // Tick one per second
                roshanTimer.Start();
                isRoshanTimerPaused = false;
                await Manager.SetTitleAsync(args.context, GetFormattedString(SettingsModel.Counter));
                await Manager.SetImageAsync(args.context, "images/dead.png");
                
                // Early exit
                return;
            }
            
            // Play / Pause
            if (isRoshanTimerPaused)
            {
                ResumeRoshanTimer(args);
            }
            else
            {
                PauseRoshanTimer(args);
            }

            await base.OnKeyUp(args);
        }

        private void ApplicationTimerTick(StreamDeckEventPayload args)
        {
            if (!isKeyHeld)
            {
                return;
            }
            
            // If user held key for longer than 1 second...
            if ((DateTime.Now - pressedKeyTime).TotalSeconds > 1)
            {
                RestartApplication(args);
            }
        }

        private void RestartApplication(StreamDeckEventPayload args)
        {
            isApplicationRestarting = true;
            SettingsModel.Counter = 0;
            
            // Properly dispose of roshan timer (Will get re-created on button release on next press)
            roshanTimer.Stop();
            roshanTimer.Dispose();
            roshanTimer = null;

            // Reset action image to Roshan
            Manager.SetImageAsync(args.context, "images/actionDefaultImage@2x.png");
            Manager.SetTitleAsync(args.context, string.Empty);
        }

        /// <summary>
        /// An update method that gets invoked every second. Intended for timer incrementing.
        /// </summary>
        /// <param name="args"></param>
        private void RoshanTimerTick(StreamDeckEventPayload args)
        {
            if (roshanTimer == null)
            {
                return;
            }

            SettingsModel.Counter++;
            int totalMinutes = SettingsModel.Counter / 60;

            if (totalMinutes < 8)
            {
                Manager.SetImageAsync(args.context, "images/dead.png");
            }
            else if (totalMinutes < 11)
            {
                Manager.SetImageAsync(args.context, "images/maybe.png");
            }
            else
            {
                Manager.SetImageAsync(args.context, "images/alive.png");
            }

            Manager.SetTitleAsync(args.context, GetFormattedString(SettingsModel.Counter));
        }

        /// <summary>
        /// Resumes our timer so it can keep accumulating seconds. Useful for when the Dota 2 match is about to be
        /// unpaused.
        /// </summary>
        /// <param name="args"></param>
        private async void ResumeRoshanTimer(StreamDeckEventPayload args)
        {
            // Resume timer
            roshanTimer.Start();
            isRoshanTimerPaused = false;
            await Manager.SetTitleAsync(args.context, GetFormattedString(SettingsModel.Counter));
        }

        /// <summary>
        /// Pauses the timer so it can no longer accumulate seconds. Useful for when the Dota 2 match is paused.
        /// </summary>
        /// <param name="args"></param>
        private async void PauseRoshanTimer(StreamDeckEventPayload args)
        {
            roshanTimer.Stop();
            isRoshanTimerPaused = true;
                
            await Manager.SetTitleAsync(args.context, "Paused");
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