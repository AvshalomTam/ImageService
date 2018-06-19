package com.example.simchegutfreund.imageserviceapp;

import android.app.Notification;
import android.app.NotificationManager;
import android.app.Service;
import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.content.IntentFilter;
import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.net.ConnectivityManager;
import android.net.NetworkInfo;
import android.net.wifi.WifiManager;
import android.os.Environment;
import android.os.IBinder;
import android.support.annotation.Nullable;
import android.support.v4.app.NotificationCompat;
import android.support.v4.app.NotificationManagerCompat;
import android.util.Log;
import android.widget.Toast;

import java.io.ByteArrayOutputStream;
import java.io.File;
import java.io.FileInputStream;
import java.io.OutputStream;
import java.net.InetAddress;
import java.net.Socket;

public class ImageServiceService extends Service {
    public BroadcastReceiver receiver;

    public ImageServiceService() {
    }

    @Nullable
    @Override
    public IBinder onBind(Intent intent) {
        return null;
    }

    @Override
    public void onCreate() {
        super.onCreate();

        final IntentFilter theFilter = new IntentFilter();
        theFilter.addAction("android.net.wifi.supplicant.CONNECTION_CHANGE");
        theFilter.addAction("android.net.wifi.STATE_CHANGE");
        this.receiver = new BroadcastReceiver() {
            @Override
            public void onReceive(Context context, Intent intent) {
                WifiManager wifiManager = (WifiManager) context.getSystemService(Context.WIFI_SERVICE);
                NetworkInfo networkInfo = intent.getParcelableExtra(WifiManager.EXTRA_NETWORK_INFO);

                if (networkInfo != null) {
                    if (networkInfo.getType() == ConnectivityManager.TYPE_WIFI) {
                        if (networkInfo.getState() == NetworkInfo.State.CONNECTED) {
                            startTransfer();
                        }
                    }
                }
            }
        };
        this.registerReceiver(this.receiver, theFilter);
    }

    private byte[] getBytesFromBitmap(Bitmap bm) {
        ByteArrayOutputStream stream = new ByteArrayOutputStream();
        bm.compress(Bitmap.CompressFormat.PNG, 70, stream);
        return stream.toByteArray();
    }

    private void startTransfer() {
        try {
            InetAddress serverAddr = InetAddress.getByName("10.0.2.2");

            Socket socket = new Socket(serverAddr, 7777);
            OutputStream output = socket.getOutputStream();

            File dcim = Environment.getExternalStoragePublicDirectory(Environment.DIRECTORY_DCIM);
            if (dcim == null) {
                return;
            }

            File[] pics = dcim.listFiles();
            int count = 0;
            if (pics != null) {
                for (File pic : pics) {
                    try {
                        FileInputStream fis = new FileInputStream(pic);
                        Bitmap bm = BitmapFactory.decodeStream(fis);
                        byte[] imgbyte = getBytesFromBitmap(bm);

                        NotificationCompat.Builder builder = new NotificationCompat.Builder(this, "default");
                        int notify_id = 1;
                        NotificationManagerCompat NM = NotificationManagerCompat.from(this);
                        builder.setContentTitle("Picture Transfer").setContentText("Transfer in progress").setPriority(NotificationCompat.PRIORITY_LOW);

                        builder.setContentText("Half way through").setProgress(100, 50, false);
                        NM.notify(notify_id, builder.build());

                        output.write(imgbyte);
                        output.flush();

                        builder.setContentText("Transfer complete").setProgress(0, 0, false);
                        NM.notify(notify_id, builder.build());
                    } catch (Exception e) {
                        Log.e("TCP", "S: Error", e);
                    }
                }
            }
        } catch (Exception e) {
            Log.e("TCP", "C: Error", e);
        }
    }

    public int onStartCommand(Intent intent, int flag, int startId) {
        Toast.makeText(this, "Service starting...", Toast.LENGTH_SHORT).show();
        return START_STICKY;
    }

    public void onDestroy() {
        Toast.makeText(this, "Service ending...", Toast.LENGTH_SHORT).show();
    }
}
