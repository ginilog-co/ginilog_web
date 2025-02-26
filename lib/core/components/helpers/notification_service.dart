import 'package:flutter/foundation.dart';
import '../utils/package_export.dart';

class NotificationService {
  final FlutterLocalNotificationsPlugin _flutterLocalNotificationsPlugin =
      FlutterLocalNotificationsPlugin();

  Future<void> initializeNotifications() async {
    try {
      // Request notification permissions
      final permissionStatus = await Permission.notification.request();

      if (permissionStatus.isGranted) {
        const androidSettings =
            AndroidInitializationSettings('@mipmap/ic_launcher');
        const iOSSettings = DarwinInitializationSettings(
          requestSoundPermission: true,
          requestAlertPermission: true,
          requestBadgePermission: true,
        );

        const initializationSettings = InitializationSettings(
          android: androidSettings,
          iOS: iOSSettings,
        );

        await _flutterLocalNotificationsPlugin.initialize(
          initializationSettings,
          onDidReceiveNotificationResponse: _onSelectNotification,
        );

        if (kDebugMode) {
          print("Notifications initialized successfully");
        }
      } else {
        if (kDebugMode) {
          print("Notification permission denied");
        }
      }
    } catch (e) {
      if (kDebugMode) {
        print("Error initializing notifications: $e");
      }
    }
  }

  Future<void> showNotification(
      int id, String title, String body, String payload) async {
    const androidPlatformChannelSpecifics = AndroidNotificationDetails(
      'default_channel_id',
      'Default Channel',
      channelDescription: 'This is the default channel for notifications.',
      importance: Importance.max,
      priority: Priority.high,
      icon: '@mipmap/ic_launcher',
    );

    const iOSPlatformChannelSpecifics = DarwinNotificationDetails();

    const platformChannelSpecifics = NotificationDetails(
      android: androidPlatformChannelSpecifics,
      iOS: iOSPlatformChannelSpecifics,
    );

    await _flutterLocalNotificationsPlugin.show(
      id,
      title,
      body,
      platformChannelSpecifics,
      payload: payload,
    );
  }

  Future<void> _onSelectNotification(NotificationResponse response) async {
    if (kDebugMode) {
      print("Notification tapped: ${response.payload}");
    }
    // Handle navigation or actions based on the notification payload
  }
}
