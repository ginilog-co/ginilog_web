import 'dart:convert';
import 'dart:io';

import '../utils/package_export.dart';
import '../utils/constants.dart';

/// Global background handler (must be a top-level function)
Future<void> firebaseMessagingBackgroundHandler(RemoteMessage message) async {
  debugPrint("📩 Background Message received: ${message.messageId}");
  // You could add background data handling here (like saving to Hive/DB)
}

class PushNotificationService {
  final FirebaseMessaging _fcm = FirebaseMessaging.instance;
  final FlutterLocalNotificationsPlugin _flutterLocalNotificationsPlugin =
      FlutterLocalNotificationsPlugin();

  String deviceToken = '';

  Future initialize() async {
    // 🔹 Register background handler
    FirebaseMessaging.onBackgroundMessage(firebaseMessagingBackgroundHandler);

    if (Platform.isIOS) {
      // Request iOS permissions
      NotificationSettings settings = await _fcm.requestPermission(
        alert: true,
        announcement: true,
        badge: true,
        carPlay: true,
        criticalAlert: true,
        provisional: false,
        sound: true,
      );

      if (settings.authorizationStatus != AuthorizationStatus.authorized) {
        debugPrint("🚫 Notification permission denied on iOS");
        return;
      }

      // Wait for APNs token
      String? apnsToken;
      int retry = 0;
      while (apnsToken == null && retry < 10) {
        apnsToken = await _fcm.getAPNSToken();
        if (apnsToken == null) {
          await Future.delayed(const Duration(seconds: 1));
          retry++;
        }
      }

      debugPrint("📲 APNs Token: $apnsToken");
    }

    // Get FCM token
    await _fcm.getToken().then((token) {
      if (token != null) {
        deviceToken = token;
        printData("Token Device", token);
        setToLocalStorage(name: "deviceToken", data: token);
      }
    });

    // Check if app was opened via notification
    RemoteMessage? initialMessage = await _fcm.getInitialMessage();

    void handleMessage(RemoteMessage message) {
      debugPrint("🔔 Notification tapped with data: ${message.data}");
      if (message.data['type'] == 'home') {
        // Navigate to home route
        // AppNavigator.pushNamedReplacement(homeRoute);
      }
    }

    if (initialMessage != null) {
      handleMessage(initialMessage);
    }

    // Listen for interaction when app is in background
    FirebaseMessaging.onMessageOpenedApp.listen(handleMessage);

    // Set foreground presentation options for iOS
    await _fcm.setForegroundNotificationPresentationOptions(
      alert: true,
      badge: true,
      sound: true,
    );

    // Foreground message listener (Android/iOS)
    FirebaseMessaging.onMessage.listen((RemoteMessage message) async {
      debugPrint("📨 Foreground Message received: ${message.messageId}");
      await showNotification(message);
    });

    // Init local notifications
    const androidSettings = AndroidInitializationSettings(
      '@mipmap/ic_launcher',
    );
    const iOSSettings = DarwinInitializationSettings();

    final initSettings = InitializationSettings(
      android: androidSettings,
      iOS: iOSSettings,
    );

    _flutterLocalNotificationsPlugin.initialize(
      initSettings,
      onDidReceiveNotificationResponse: (NotificationResponse response) async {
        debugPrint("🔔 Notification clicked: ${response.payload}");
        if (response.payload != null) {
          final data = jsonDecode(response.payload!);
          if (data['type'] == 'home') {
            // AppNavigator.pushNamedReplacement(homeRoute);
          }
        }
      },
    );
  }

  /// Show local notification when a push is received in foreground
  Future showNotification(RemoteMessage message) async {
    const AndroidNotificationChannel channel = AndroidNotificationChannel(
      'fcm_default_channel',
      'High Importance Notifications',
      importance: Importance.high,
    );

    await _flutterLocalNotificationsPlugin
        .resolvePlatformSpecificImplementation<
          AndroidFlutterLocalNotificationsPlugin
        >()
        ?.createNotificationChannel(channel);

    if (message.notification != null) {
      final notification = message.notification;

      final androidDetails = AndroidNotificationDetails(
        channel.id,
        channel.name,
        importance: Importance.max,
        playSound: true,
        priority: Priority.high,
        ongoing: false, // 👈 set false so it’s dismissible
        color: Colors.deepOrangeAccent,
        channelDescription: channel.description,
        styleInformation: const BigTextStyleInformation(''),
      );

      const iOSDetails = DarwinNotificationDetails(
        presentAlert: true,
        presentBadge: true,
        presentSound: true,
        badgeNumber: 1,
      );

      final platformDetails = NotificationDetails(
        android: androidDetails,
        iOS: iOSDetails,
      );

      await _flutterLocalNotificationsPlugin.show(
        notification.hashCode,
        notification!.title,
        notification.body,
        platformDetails,
        payload: jsonEncode(message.data),
      );
    }
  }
}
