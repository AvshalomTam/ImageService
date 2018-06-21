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
        bm.compress(Bitmap.CompressFormat.JPEG, 70, stream);
        return stream.toByteArray();
    }

    private void startTransfer(Context context) {
        Toast.makeText(this, "Transfering images...", Toast.LENGTH_SHORT).show();

        File dcim = new File(Environment.getExternalStoragePublicDirectory(Environment.DIRECTORY_DCIM), "Camera");
        if (dcim == null) {
            return;
        }

        final File[] pics = dcim.listFiles();

        final NotificationCompat.Builder builder = new NotificationCompat.Builder(this);
        final int notify_id = 1;
        final NotificationManager NM = (NotificationManager)getSystemService(Context.NOTIFICATION_SERVICE);
        builder.setSmallIcon(R.drawable.ic_launcher_background);
        builder.setContentTitle("Transfer status...");
        builder.setContentText("Transfer in progress...");

        int count = 0;
        if (pics != null) {
            new Thread(new Runnable() {
                @Override
                public void run() {
                    try {
                        InetAddress serverAddr = InetAddress.getByName("10.0.0.9");

                        try {
                            int progress = 0;

                            for (File pic : pics) {
                                builder.setProgress(100, progress, false);
                                NM.notify(notify_id, builder.build());

                                Socket socket = new Socket(serverAddr, 45267);
                                OutputStream outputStream = socket.getOutputStream();
                                InputStream inputStream = socket.getInputStream();
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
                                socket.close();

                                progress += 100 / pics.length;
                                Thread.sleep(2*1000);
                            }

                            builder.setProgress(0,0, false);
                            builder.setContentText("Transfer Complete...");
                            NM.notify(notify_id, builder.build());
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

    public void setProgress(int progress) {

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
