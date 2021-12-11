﻿using Android.Content;
using Phantom.Interfaces;
using Xamarin.Forms;
using System.Threading.Tasks;
using Android.Support.V4.Content;
using Phantom.Droid;
using Java.IO;
using Android.Webkit;
using IXICore.Meta;
using System;

[assembly: Dependency(typeof(FileOperations_Android))]

public class FileOperations_Android : IFileOperations
{
    private readonly Context _context;
    public FileOperations_Android()
    {
        _context = Android.App.Application.Context;
    }

    public Task share(string filepath, string title)
    {
        File file = new File(filepath);
        Intent shareIntent = new Intent();
        shareIntent.SetAction(Intent.ActionSend);
        shareIntent.SetType("application/octet-stream");
        Android.Net.Uri uriShare = FileProvider.GetUriForFile(_context, "com.ixian.provider", file);
        shareIntent.PutExtra(Intent.ExtraStream, uriShare);

        var chooserIntent = Intent.CreateChooser(shareIntent, title ?? string.Empty);
        chooserIntent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.NewTask);
        _context.StartActivity(chooserIntent);

        return Task.FromResult(true);
    }

    public string getMimeType(Android.Net.Uri uri)
    {
        string mime_type = null;
        if (uri.Scheme.Equals(ContentResolver.SchemeContent))
        {
            ContentResolver cr = MainActivity.Instance.ContentResolver;
            mime_type = cr.GetType(uri);
        }
        else
        {
            string ext = MimeTypeMap.GetFileExtensionFromUrl(uri.ToString());
            mime_type = MimeTypeMap.Singleton.GetMimeTypeFromExtension(ext.ToLower());
        }
        if (mime_type == null)
        {
            mime_type = "*/*";
        }
        return mime_type;
    }

    public void open(string file_path)
    {
        var context = MainActivity.Instance;
        File f;
        if(file_path.StartsWith(context.FilesDir.AbsolutePath))
        {
            f = new File(context.FilesDir, file_path.Substring(context.FilesDir.AbsolutePath.Length));
        }else
        {
            f = new File(file_path);
        }

        if(f == null || !f.Exists())
        {
            return;
        }

        try
        {
            Android.Net.Uri file_uri = FileProvider.GetUriForFile(context, "com.ixian.provider", f);

            string mime_type = getMimeType(file_uri);

            Intent intent = new Intent(Intent.ActionView);
            intent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.ClearWhenTaskReset | ActivityFlags.NewTask | ActivityFlags.GrantReadUriPermission);
            intent.SetDataAndType(file_uri, mime_type);

            context.StartActivity(Intent.CreateChooser(intent, "Open file with"));
        }catch(Exception e)
        {
            Logging.error("Exception occured while trying to open file " + e);
        }
    }


}