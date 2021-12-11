﻿using System.Collections.Generic;
using Android.Content;
using Android.OS;
using Phantom.Interfaces;
using Xamarin.Forms;
using Android.Net.Wifi;

[assembly: Dependency(typeof(PowerManager_Android))]


public class PowerManager_Android : IPowerManager
{
    Dictionary<string, object> wakeLocks = new Dictionary<string, object>();


    public bool AquireLock(string lock_type = "screenDim")
    {
        lock (wakeLocks)
        {
            if (wakeLocks.ContainsKey(lock_type))
            {
                return false;
            }
            switch (lock_type)
            {
                case "screenDim":
                    PowerManager pm = (PowerManager)Android.App.Application.Context.GetSystemService(Context.PowerService);
                    var pm_lock = pm.NewWakeLock(WakeLockFlags.ScreenDim, "Phantom");
                    pm_lock.Acquire();
                    wakeLocks.Add(lock_type, pm_lock);
                    return true;
                case "partial":
                    pm = (PowerManager)Android.App.Application.Context.GetSystemService(Context.PowerService);
                    pm_lock = pm.NewWakeLock(WakeLockFlags.Partial, "Phantom");
                    pm_lock.Acquire();
                    wakeLocks.Add(lock_type, pm_lock);
                    return true;
                case "proximityScreenOff":
                    if (Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.Lollipop)
                    {
                        pm = (PowerManager)Android.App.Application.Context.GetSystemService(Context.PowerService);
                        if (pm.IsWakeLockLevelSupported((int)WakeLockFlags.ProximityScreenOff))
                        {
                            pm_lock = pm.NewWakeLock(WakeLockFlags.ProximityScreenOff, "Phantom");
                            pm_lock.Acquire();
                            wakeLocks.Add(lock_type, pm_lock);
                            return true;
                        }
                    }
                    break;
                case "wifi":
                    WifiManager wm = (WifiManager)Android.App.Application.Context.GetSystemService(Context.WifiService);
                    var wm_lock = wm.CreateWifiLock(Android.Net.WifiMode.FullHighPerf, "Phantom");
                    wm_lock.Acquire();
                    wakeLocks.Add(lock_type, wm_lock);
                    return true;
            }
            return false;
        }
    }

    public bool ReleaseLock(string lock_type = "screenDim")
    {
        lock (wakeLocks)
        {
            if (!wakeLocks.ContainsKey(lock_type))
            {
                return false;
            }

            switch (lock_type)
            {
                case "screenDim":
                case "partial":
                case "proximityScreenOff":
                    PowerManager.WakeLock pm_lock = (PowerManager.WakeLock)wakeLocks[lock_type];
                    pm_lock.Release();
                    break;
                case "wifi":
                    WifiManager.WifiLock wm_lock = (WifiManager.WifiLock)wakeLocks[lock_type];
                    wm_lock.Release();
                    break;
            }
            wakeLocks.Remove(lock_type);
            return true;
        }
    }
}