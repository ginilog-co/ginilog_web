import '../utils/package_export.dart';

class PushNotificationService {
  final FlutterLocalNotificationsPlugin _flutterLocalNotificationsPlugin =
      FlutterLocalNotificationsPlugin();

  Future showNotification(String? message) async {
    // We create an Android Notification Channel that overrides the default FCM channel to enable heads up notifications.
    AndroidNotificationChannel channel = const AndroidNotificationChannel(
      'fcm_default_channel',
      'High Importance Notifications',
      importance: Importance.high,
    );

    // This creates the channel on the device and if a channel with an id already exists, it will be updated
    await _flutterLocalNotificationsPlugin
        .resolvePlatformSpecificImplementation<
            AndroidFlutterLocalNotificationsPlugin>()
        ?.createNotificationChannel(channel);

    //This is used to display the foreground notification
    if (message != null) {
      // AndroidNotificationDetails androidPlatformChannelSpecifics =
      //     AndroidNotificationDetails(
      //   channel.id,
      //   channel.name,
      //   importance: Importance.max,
      //   playSound: true,
      //   channelDescription: channel.description,
      //   priority: Priority.high,
      //   ongoing: true,
      //   color: Colors.deepOrangeAccent,
      //   styleInformation: const BigTextStyleInformation(''),
      // );

      // var iOSChannelSpecifics = const DarwinNotificationDetails(
      //   presentAlert: true,
      //   presentSound: true,
      //   presentBadge: true,
      // );

      // var platformChannelSpecifics = NotificationDetails(
      //     android: androidPlatformChannelSpecifics, iOS: iOSChannelSpecifics);

      // await _flutterLocalNotificationsPlugin.show(
      // notification.hashCode,
      // notification?.title,
      // notification?.body,
      //  platformChannelSpecifics,
      //  payload: jsonEncode(message.data),
      // );
    }
  }
}
