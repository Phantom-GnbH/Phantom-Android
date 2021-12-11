using Android;
using Android.Support.V4.Content;
using IXICore.Meta;
using Phantom.Droid.Classes;
using Phantom.Interfaces;
using System;
using Xamarin.Forms;

[assembly: Dependency(typeof(PhantomPermissions))]
namespace Phantom.Droid.Classes
{
    class PhantomPermissions : IPhantomPermissions
    {
        int recordAudioPermissionRequest = 1;

        public void requestAudioRecordingPermissions()
        {
            try
            {
                if (PermissionChecker.CheckSelfPermission(MainActivity.Instance, Manifest.Permission.RecordAudio) != PermissionChecker.PermissionGranted)
                {
                    MainActivity.Instance.RequestPermissions(new string[] { Manifest.Permission.RecordAudio }, recordAudioPermissionRequest);
                }
            }
            catch (Exception e)
            {
                Logging.error("Exception occured while requesting permissions for audio recording: " + e);
            }
        }
    }
}