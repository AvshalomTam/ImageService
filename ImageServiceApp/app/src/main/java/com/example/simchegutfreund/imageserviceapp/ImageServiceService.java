package com.example.simchegutfreund.imageserviceapp;

import android.app.Notification;
import android.app.NotificationChannel;
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
import android.os.AsyncTask;
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
import java.io.InputStream;
import java.io.OutputStream;
import java.math.BigInteger;
import java.net.InetAddress;
import java.net.Socket;
import java.util.Arrays;
import java.util.Collections;

public class ImageServiceService extends Service {
    public BroadcastReceiver receiver;
    private final IntentFilter theFilter = new IntentFilter();

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

        theFilter.addAction("android.net.wifi.supplicant.CONNECTION_CHANGE");
        theFilter.addAction("android.net.wifi.STATE_CHANGE");
    }

    private byte[] getBytesFromBitmap(Bitmap bm) {
        ByteArrayOutputStream stream = new ByteArrayOutputStream();
        bm.compress(Bitmap.CompressFormat.PNG, 70, stream);
        return stream.toByteArray();
    }

    private void startTransfer(Context context) {
        Toast.makeText(this, "Transfering images...", Toast.LENGTH_SHORT).show();

        File dcim = new File(Environment.getExternalStoragePublicDirectory(Environment.DIRECTORY_DCIM), "Camera");
        if (dcim == null) {
            return;
        }

        final File[] pics = dcim.listFiles();

        /*final int NI = 1;
        NotificationChannel NC = new NotificationChannel("default", "default", NotificationManager.IMPORTANCE_DEFAULT);
        final NotificationCompat.Builder builder = new NotificationCompat.Builder(context, "default")
                .setSmallIcon(R.drawable.ic_launcher_foreground).setContentTitle("BackUp The Pics").setPriority(NotificationCompat.PRIORITY_DEFAULT);
        final NotificationManager notificationManager = (NotificationManager) getSystemService(context.NOTIFICATION_SERVICE);
        notificationManager.createNotificationChannel(NC);
        builder.setProgress(100, 0,false);

        builder.setContentTitle("Picture Transfer").setContentText("Transfer in progress").setPriority(NotificationCompat.PRIORITY_LOW);
        notificationManager.notify(10, builder.build());*/

        int count = 0;
        if (pics != null) {
            new Thread(new Runnable() {
                @Override
                public void run() {
                    try {
                        InetAddress serverAddr = InetAddress.getByName("172.18.28.177");
                        try {
                            int bar = 0;
                            for (File pic : pics) {
                                Socket socket = new Socket(serverAddr, 45267);
                                OutputStream outputStream = socket.getOutputStream();
                                Log.e("TCP", "YES!");
                                Log.e("path", pic.toString());

                                // sending name of pic
                                byte[] name = pic.getName().getBytes();
                                byte[] l_name = padding(BigInteger.valueOf(name.length).toByteArray());
                                outputStream.write(l_name); // sending the size
                                outputStream.flush();
                                outputStream.write(name);
                                outputStream.flush();

                                // sending the pic
                                FileInputStream fis = new FileInputStream(pic);
                                Bitmap bm = BitmapFactory.decodeStream(fis);
                                byte[] img = getBytesFromBitmap(bm);
                                byte[] l_img = padding(BigInteger.valueOf(img.length).toByteArray());
                                outputStream.write(l_img);  // sending the size
                                outputStream.flush();
                                outputStream.write(img);
                                outputStream.flush();

                                bar = bar + 100 / pics.length;
                                // builder.setProgress(100, bar, false);
                                // notificationManager.notify(10, builder.build());
                                socket.close();
                                break;
                            }
                            // builder.setProgress(0,0, false);
                            // builder.setContentText("Transfer complete");
                            // notificationManager.notify(10, builder.build());
                        } catch (Exception e) {
                            Log.e("TCP", "S: Error", e);
                        }
                    } catch (Exception e) {
                        Log.e("TCP", "C: Error", e);
                    }
                }
            }).start();
        }
    }

    public int onStartCommand(Intent intent, int flag, int startId) {
        Toast.makeText(this, "Service starting...", Toast.LENGTH_SHORT).show();

        this.receiver = new BroadcastReceiver() {
            @Override
            public void onReceive(Context context, Intent intent) {
                WifiManager wifiManager = (WifiManager) context.getSystemService(context.WIFI_SERVICE);
                NetworkInfo networkInfo = intent.getParcelableExtra(wifiManager.EXTRA_NETWORK_INFO);

                if (networkInfo != null) {
                    if (networkInfo.getType() == ConnectivityManager.TYPE_WIFI) {
                        if (networkInfo.getState() == NetworkInfo.State.CONNECTED) {
                            startTransfer(context);
                        }
                    }
                }
            }
        };
        this.registerReceiver(this.receiver, theFilter);

        return START_STICKY;
    }

    public void onDestroy() {
        Toast.makeText(this, "Service ending...", Toast.LENGTH_SHORT).show();
    }

    public byte[] padding(byte[] array) {
        if (array.length == 4)
            return array;
        int i = 3;
        int j = array.length-1;
        byte[] padded = new byte[4];

        for ( ; i >= 0 ; i--, j--) {
            if (j < 0)
                padded[i] = 0;
            else
                padded[i] = array[j];
        }
        return padded;
    }
}
