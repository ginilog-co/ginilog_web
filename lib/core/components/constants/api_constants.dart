import 'dart:convert';
import 'dart:io';

import '../utils/package_export.dart';
import '../utils/constants.dart';

/// Background message handler (must be top-level)
Future<void> firebaseMessagingBackgroundHandler(RemoteMessage message) async {
  debugPrint("📩 Background Message received: ${message.messageId}");
  // Optionally handle background data storage here
}

class PushNotificationService {
  final FirebaseMessaging _fcm = FirebaseMessaging.instance;
  final FlutterLocalNotificationsPlugin _localNotifications =
      FlutterLocalNotificationsPlugin();

  String deviceToken = '';

  /// Initialize FCM and local notifications
  Future<void> initialize() async {
    // 1️⃣ Set background handler
    FirebaseMessaging.onBackgroundMessage(firebaseMessagingBackgroundHandler);

    // 2️⃣ iOS permission and APNs setup
    if (Platform.isIOS) {
      await _requestIOSPermissions();
      // Wait briefly for APNs registration
      await Future.delayed(const Duration(seconds: 3));
    }

    // 3️⃣ Fetch FCM token
    await _fetchFCMToken();

    // 4️⃣ Handle launch from terminated state
    await _handleInitialMessage();

    // 5️⃣ Foreground and background message listeners
    FirebaseMessaging.onMessageOpenedApp.listen(_handleMessage);
    FirebaseMessaging.onMessage.listen((message) => showNotification(message));

    // 6️⃣ Foreground notification presentation options (iOS)
    await _fcm.setForegroundNotificationPresentationOptions(
      alert: true,
      badge: true,
      sound: true,
    );

    // 7️⃣ Initialize local notifications
    await _initializeLocalNotifications();

    // 8️⃣ Listen for FCM token refresh
    FirebaseMessaging.instance.onTokenRefresh.listen((token) {
      debugPrint("🔄 FCM Token refreshed: $token");
      deviceToken = token;
    });
  }

  /// Request iOS notification permissions and fetch APNs token
  Future<void> _requestIOSPermissions() async {
    final settings = await _fcm.requestPermission(
      alert: true,
      badge: true,
      sound: true,
    );

    if (settings.authorizationStatus == AuthorizationStatus.authorized) {
      debugPrint("✅ iOS notifications authorized");

      // Wait a bit before trying to get APNs token
      await Future.delayed(const Duration(seconds: 2));

      final apnsToken = await _fcm.getAPNSToken();
      if (apnsToken != null) {
        debugPrint("📲 APNs Token: $apnsToken");
      } else {
        debugPrint("❌ APNs Token not yet available");
      }
    } else {
      debugPrint("🚫 iOS notification permission denied");
    }
  }

  /// Fetch FCM token and store locally
  Future<void> _fetchFCMToken() async {
    try {
      final token = await _fcm.getToken();
      if (token != null) {
        deviceToken = token;
        printData("Token Device", token);
        setToLocalStorage(name: "deviceToken", data: token);
        // Optionally save it locally or send to your backend
      } else {
        debugPrint("⚠️ FCM token is null, retrying...");
        await Future.delayed(const Duration(seconds: 3));
        final retryToken = await _fcm.getToken();
        if (retryToken != null) {
          deviceToken = retryToken;
          printData("✅ Retried and got FCM token:", retryToken);
          setToLocalStorage(name: "deviceToken", data: retryToken);
          debugPrint(" $retryToken");
        } else {
          debugPrint("❌ Still failed to get FCM token");
        }
      }
    } catch (e) {
      debugPrint("❌ Error fetching FCM token: $e");
    }
  }

  /// Handle app launch from terminated state
  Future<void> _handleInitialMessage() async {
    final initialMessage = await _fcm.getInitialMessage();
    if (initialMessage != null) {
      debugPrint("🚀 App opened from terminated state with message");
      _handleMessage(initialMessage);
    }
  }

  /// Handle notification taps (navigation or logic)
  void _handleMessage(RemoteMessage message) {
    debugPrint("🔔 Notification tapped: ${message.data}");
    final type = message.data['type'];
    final id = message.data['id']?.toString();

    if (type == 'home') {
      debugPrint("🏠 Navigate to Home");
      // AppNavigator.pushNamedReplacement(homeRoute);
    } else if (type == 'order' && id != null) {
      debugPrint("📦 Navigate to Order page with id: $id");
      // navPush(context, OrderDetailsRoute(orderId: id));
    }
  }

  /// Initialize local notifications
  Future<void> _initializeLocalNotifications() async {
    const androidSettings = AndroidInitializationSettings(
      '@mipmap/ic_launcher',
    );
    const iOSSettings = DarwinInitializationSettings();

    await _localNotifications.initialize(
      const InitializationSettings(android: androidSettings, iOS: iOSSettings),
      onDidReceiveNotificationResponse: (response) {
        if (response.payload != null) {
          final data = jsonDecode(response.payload!);
          final type = data['type'];
          final id = data['id']?.toString();
          if (type == 'home') {
            debugPrint("🏠 Local notif tapped: go to home");
            // AppNavigator.pushNamedReplacement(homeRoute);
          } else if (type == 'order' && id != null) {
            debugPrint("📦 Local notif tapped: go to order $id");
            // navPush(context, OrderDetailsRoute(orderId: id));
          }
        }
      },
    );

    // Create Android channel
    const channel = AndroidNotificationChannel(
      'fcm_default_channel',
      'High Importance Notifications',
      importance: Importance.high,
    );

    await _localNotifications
        .resolvePlatformSpecificImplementation<
          AndroidFlutterLocalNotificationsPlugin
        >()
        ?.createNotificationChannel(channel);
  }

  /// Display foreground notifications
  Future<void> showNotification(RemoteMessage message) async {
    if (message.notification == null) return;

    final notification = message.notification!;
    const androidDetails = AndroidNotificationDetails(
      'fcm_default_channel',
      'High Importance Notifications',
      importance: Importance.max,
      priority: Priority.high,
      playSound: true,
      ongoing: false,
      color: Colors.deepOrangeAccent,
      styleInformation: BigTextStyleInformation(''),
    );

    const iOSDetails = DarwinNotificationDetails(
      presentAlert: true,
      presentBadge: true,
      presentSound: true,
      badgeNumber: 1,
    );

    final details = NotificationDetails(
      android: androidDetails,
      iOS: iOSDetails,
    );

    await _localNotifications.show(
      notification.hashCode,
      notification.title,
      notification.body,
      details,
      payload: jsonEncode(message.data),
    );
  }
}
