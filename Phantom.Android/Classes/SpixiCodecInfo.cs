using System.Collections.Generic;
using System.Linq;
using Android.Media;
using Phantom.Droid.Classes;
using Phantom.VoIP;
using Xamarin.Forms;

[assembly: Dependency(typeof(PhantomCodecInfo))]
namespace Phantom.Droid.Classes
{
    class PhantomCodecInfo : IPhantomCodecInfo
    {
        string[] codecMap = new string[] { "amrwb", "amrnb" };
        List<string> cachedCodecs = null;

        public List<string> getSupportedAudioCodecs()
        {
            if(cachedCodecs != null)
            {
                return cachedCodecs;
            }

            var mcl = new MediaCodecList(MediaCodecListKind.AllCodecs);
            var codec_infos = mcl.GetCodecInfos().ToList();


            var cl = new List<string>();
            cl.Add("opus");

            foreach (var codec_in_map in codecMap)
            {
                var codec = codec_infos.Find(x => x.IsEncoder && x.Name.Contains(codec_in_map, System.StringComparison.OrdinalIgnoreCase));
                if (codec != null)
                {
                    cl.Add(codec_in_map);
                }
            }
            cachedCodecs = cl;
            return cl;
        }

        public bool isCodecSupported(string codec_name)
        {
            if (codec_name == "opus")
            {
                return true;
            }

            var mcl = new MediaCodecList(MediaCodecListKind.AllCodecs);
            var codec_infos = mcl.GetCodecInfos().ToList();
            var codec = codec_infos.Find(x => x.IsEncoder && x.Name.Contains(codec_name, System.StringComparison.OrdinalIgnoreCase));
            if (codec != null)
            {
                return true;
            }
            return false;
        }
    }
}