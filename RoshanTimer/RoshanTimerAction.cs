using System;
using StreamDeckLib;
using StreamDeckLib.Messages;
using System.Threading.Tasks;
using System.Timers;

namespace RoshanTimer
{
    [ActionUuid(Uuid="com.adrian-miasik.roshan-timer.action")]
    public class RoshanTimerAction : BaseStreamDeckActionWithSettingsModel<Models.TotalSecondsSettingsModel>
    {
        private Timer applicationTimer; // Used for processing input. Cannot be paused.
        
        // Hold
        private bool isKeyHeld;
        private DateTime pressedKeyTime;
        private bool ignoreKeyRelease;

        // Double Click
        private int numberOfPresses;
        private DateTime releasedKeyTime;
        private bool hasDoubleClicked;
        
        // Roshan
        private Timer roshanTimer; // Used for keeping track of roshan's respawn time. Can be paused.
        private bool isRoshanTimerPaused;
        private int deathCount;

        public override Task OnKeyDown(StreamDeckEventPayload args)
        {
            if (numberOfPresses > 1)
            {
                // If user last keystroke was less than than 0.4f second ago...
                if ((DateTime.Now - releasedKeyTime).TotalSeconds < 0.4f)
                {
                    // User has double clicked!
                    numberOfPresses = 0;
                    hasDoubleClicked = true;
                    
                    // Debug time between double click keys:
                    // Manager.SetImageAsync(args.context, "images/blank.png");
                    // Manager.SetTitleAsync(args.context, (DateTime.Now - releasedKeyTime).TotalSeconds.ToString("F2"));
                }
            }
            
            pressedKeyTime = DateTime.Now;
            isKeyHeld = true;

            return base.OnKeyDown(args);
        }

        public override Task OnKeyUp(StreamDeckEventPayload args)
        {
            numberOfPresses++;
            releasedKeyTime = DateTime.Now;

            isKeyHeld = false;

            if (hasDoubleClicked)
            {
                hasDoubleClicked = false;
                deathCount++;
                SettingsModel.TotalSeconds = 0; // Reset timer
                ResumeRoshanTimer(args);
                if (deathCount <= 3)
                {
                    Manager.SetImageAsync(args.context, "images/dead" + deathCount + ".png");
                }
                else
                {
                    Manager.SetImageAsync(args.context, "images/dead3.png");
                }
                return Task.CompletedTask;
            }

            // Ignore re-init when the user was holding to reset the roshan timer/app.
            if (ignoreKeyRelease)
            {
                ignoreKeyRelease = false;
                return Task.CompletedTask;
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
                CreateRoshanTimer(args);
                return Task.CompletedTask;
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

            return base.OnKeyUp(args);
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
            ignoreKeyRelease = true;
            SettingsModel.TotalSeconds = 0;
            deathCount = 0;
            numberOfPresses = 0;
            hasDoubleClicked = false;
            
            DeleteRoshanTimer();

            // Reset action image to Roshan
            Manager.SetImageAsync(args.context, "images/actionDefaultImage@2x.png");
            Manager.SetTitleAsync(args.context, string.Empty);
        }

        private void CreateRoshanTimer(StreamDeckEventPayload args)
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
            Manager.SetTitleAsync(args.context, GetFormattedString(SettingsModel.TotalSeconds));
            Manager.SetImageAsync(args.context, "images/dead0.png");
        }

        /// <summary>
        /// Properly dispose of roshan timer (Gets re-created on button release on next press)
        /// </summary>
        private void DeleteRoshanTimer()
        {
            roshanTimer.Stop();
            roshanTimer.Dispose();
            roshanTimer = null;
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

            SettingsModel.TotalSeconds++;
            int totalMinutes = SettingsModel.TotalSeconds / 60;

            if (totalMinutes < 8)
            {
                if (deathCount <= 3)
                {
                    Manager.SetImageAsync(args.context, "images/dead" + deathCount + ".png");
                }
                else
                {
                    Manager.SetImageAsync(args.context, "images/dead3.png");
                }
            }
            else if (totalMinutes < 11)
            {
                if (deathCount <= 3)
                {
                    Manager.SetImageAsync(args.context, "images/maybe" + deathCount + ".png");
                }
                else
                {
                    Manager.SetImageAsync(args.context, "images/maybe3.png");
                }
            }
            else
            {
                if (deathCount <= 3)
                {
                    Manager.SetImageAsync(args.context, "images/alive" + deathCount + ".png");
                }
                else
                {
                    Manager.SetImageAsync(args.context, "images/alive3.png");
                }
            }

            Manager.SetTitleAsync(args.context, GetFormattedString(SettingsModel.TotalSeconds));
        }

        /// <summary>
        /// Resumes our timer so it can keep accumulating seconds. Useful for when the Dota 2 match is about to be
        /// unpaused.
        /// </summary>
        /// <param name="args"></param>
        private void ResumeRoshanTimer(StreamDeckEventPayload args)
        {
            // Resume timer
            roshanTimer.Start();
            isRoshanTimerPaused = false;
            Manager.SetTitleAsync(args.context, GetFormattedString(SettingsModel.TotalSeconds));
        }

        /// <summary>
        /// Pauses the timer so it can no longer accumulate seconds. Useful for when the Dota 2 match is paused.
        /// </summary>
        /// <param name="args"></param>
        private void PauseRoshanTimer(StreamDeckEventPayload args)
        {
            roshanTimer.Stop();
            isRoshanTimerPaused = true;
                
            Manager.SetTitleAsync(args.context, "Paused");
        }

        /// <summary>
        /// Returns a string in the "(0) 0:00" format. First number indicates how many time roshan has died.
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