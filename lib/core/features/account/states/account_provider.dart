// ignore_for_file: use_build_context_synchronously

import 'dart:convert';

import 'package:ginilog_customer_app/core/components/utils/constants.dart';
import 'package:ginilog_customer_app/core/components/widgets/custom_snackbar.dart';
import 'package:ginilog_customer_app/core/features/account/services/account_services.dart';

import '../../../components/utils/package_export.dart';
import '../model/user_response_model.dart';

abstract class AccountState {
  final bool hasLoadedInitially;
  final int visitCount;
  const AccountState({this.hasLoadedInitially = false, this.visitCount = 0});
}

class AccountInitial extends AccountState {
  const AccountInitial() : super();
}

class AccountLoading extends AccountState {
  const AccountLoading({super.hasLoadedInitially}) : super();
}

class AccountRefreshing extends AccountState {
  const AccountRefreshing() : super(hasLoadedInitially: true);
}

class AccountSuccess<T> extends AccountState {
  final String message;
  final T? data;
  final List<T>? listData;

  AccountSuccess({
    required this.message,
    this.data,
    this.listData,
    super.visitCount,
  }) : super(hasLoadedInitially: true);
}

class AccountUpdate extends AccountState {
  final String message;
  const AccountUpdate({required this.message})
    : super(hasLoadedInitially: true);
}

class AccountFailure extends AccountState {
  final String error;
  const AccountFailure({required this.error}) : super();
}

class AccountNotifier extends StateNotifier<AccountState> {
  final AccountService services;
  RegisterResponseModel? userData;
  bool _hasFetchedAccount = false;
  int _visitCounter = 0;
  AccountNotifier({required this.services}) : super(AccountInitial());

  Future<void> getAccount({bool forceRefresh = false}) async {
    _visitCounter++;
    printData("Account Visit Count:", "$_visitCounter");
    try {
      if (_visitCounter == 1) {
        if (!_hasFetchedAccount || forceRefresh) {
          if (!_hasFetchedAccount) {
            state = const AccountLoading();
          } else {
            state = const AccountRefreshing();
          }

          final response = await services.getUserData();
          userData = response;
          _hasFetchedAccount = true;
          state = AccountSuccess<RegisterResponseModel>(
            message: "Account Loaded",
            data: userData,
            visitCount: _visitCounter,
          );
        }
      } else {
        if (!mounted) {
          state = AccountLoading();
          return;
        }
        final response = await services.getUserData();
        userData = response;
        _hasFetchedAccount = true;
        state = AccountSuccess<RegisterResponseModel>(
          message: "Account Loaded",
          data: response,
          visitCount: _visitCounter,
        );
      }
    } catch (e) {
      state = AccountFailure(error: e.toString());
    }
  }

  updateProfile({
    String? firstName,
    String? lastName,
    String? imageFile,
    String? phoneNo,
    bool? availability,
  }) async {
    try {
      state = AccountLoading();

      var response = await services.updateProfile(
        imageFile: imageFile,
        firstName: firstName,
        lastName: lastName,
        phoneNo: phoneNo,
        availability: availability,
      );

      if (response.statusCode == 200 || response.statusCode == 201) {
        var dataModel = RegisterResponseModel.fromJson(
          jsonDecode(response.body),
        );
        userData = dataModel;
        state = AccountSuccess<RegisterResponseModel>(
          message: "User Updated",
          data: userData,
        );
      } else {
        state = AccountFailure(error: response.body);
      }
    } catch (e) {
      state = AccountFailure(error: e.toString());
    }
  }

  updateProfile2({
    String? firstName,
    String? lastName,
    String? imageFile,
    String? phoneNo,
    bool? availability,
    required BuildContext context,
  }) async {
    try {
      if (!mounted) {
        state = AccountLoading();
        return;
      }
      var response = await services.updateProfile(
        imageFile: imageFile,
        firstName: firstName,
        lastName: lastName,
        phoneNo: phoneNo,
        availability: availability,
      );
      if (response.statusCode == 200 || response.statusCode == 201) {
        var dataModel = RegisterResponseModel.fromJson(
          jsonDecode(response.body),
        );
        userData = dataModel;
        state = AccountSuccess<RegisterResponseModel>(
          message: "User Updated",
          data: userData,
        );
        showCustomSnackbar(
          context,
          title: "User Success",
          content: "User Created Successfully",
          type: SnackbarType.success,
          isTopPosition: false,
        );
      } else {
        state = AccountFailure(error: response.body);
        showCustomSnackbar(
          context,
          title: "User Update Error",
          content: response.body,
          type: SnackbarType.error,
          isTopPosition: false,
        );
      }
    } on Exception catch (e) {
      state = AccountFailure(error: e.toString());
    }
  }

  addNewAddress({
    required String userId,
    required String address,
    required String addressPostCodes,
    required String houseNo,
    required String city,
    required String userState,
    required double latitude,
    required double longitude,
    required String phoneNo,
    required String userName,
    required BuildContext context,
  }) async {
    try {
      if (!mounted) {
        state = AccountLoading();
        return;
      }
      var response = await services.addNewAddress(
        userId: userId,
        address: address,
        addressPostCodes: addressPostCodes,
        houseNo: houseNo,
        city: city,
        state: userState,
        latitude: latitude,
        longitude: longitude,
        phoneNo: phoneNo,
        userName: userName,
      );
      if (response.statusCode == 200 || response.statusCode == 201) {
        var dataModel = RegisterResponseModel.fromJson(
          jsonDecode(response.body),
        );
        userData = dataModel;
        state = AccountSuccess<RegisterResponseModel>(
          message: "User Updated",
          data: userData,
        );
        showCustomSnackbar(
          context,
          title: "User Success",
          content: "User Created Successfully",
          type: SnackbarType.success,
          isTopPosition: false,
        );
      } else {
        state = AccountFailure(error: response.body);
        showCustomSnackbar(
          context,
          title: "User Update Error",
          content: response.body,
          type: SnackbarType.error,
          isTopPosition: false,
        );
      }
    } on Exception catch (e) {
      state = AccountFailure(error: e.toString());
    }
  }

  updateAddress({
    required String addressId,
    required String address,
    required String addressPostCodes,
    required String houseNo,
    required String city,
    required double latitude,
    required double longitude,
    required String phoneNo,
    required String userName,
    required BuildContext context,
  }) async {
    try {
      if (!mounted) {
        state = AccountLoading();
        return;
      }
      var response = await services.updateAddress(
        addressId: addressId,
        address: address,
        addressPostCodes: addressPostCodes,
        houseNo: houseNo,
        city: city,
        latitude: latitude,
        longitude: longitude,
        phoneNo: phoneNo,
        userName: userName,
      );
      if (response.statusCode == 200 || response.statusCode == 201) {
        var dataModel = RegisterResponseModel.fromJson(
          jsonDecode(response.body),
        );
        userData = dataModel;
        state = AccountSuccess<RegisterResponseModel>(
          message: "User Updated",
          data: userData,
        );
        showCustomSnackbar(
          context,
          title: "User Success",
          content: "User Created Successfully",
          type: SnackbarType.success,
          isTopPosition: false,
        );
      } else {
        state = AccountFailure(error: response.body);
        showCustomSnackbar(
          context,
          title: "User Update Error",
          content: response.body,
          type: SnackbarType.error,
          isTopPosition: false,
        );
      }
    } on Exception catch (e) {
      state = AccountFailure(error: e.toString());
    }
  }

  deleteAddress({
    required String addressId,
    required BuildContext context,
  }) async {
    try {
      if (!mounted) {
        state = AccountLoading();
        return;
      }
      var response = await services.deleteDeliveryAddress(addressId: addressId);
      if (response.statusCode == 200 || response.statusCode == 201) {
        var dataModel = RegisterResponseModel.fromJson(
          jsonDecode(response.body),
        );
        userData = dataModel;
        state = AccountSuccess<RegisterResponseModel>(
          message: "User Updated",
          data: userData,
        );
        showCustomSnackbar(
          context,
          title: "User Success",
          content: "User Created Successfully",
          type: SnackbarType.success,
          isTopPosition: false,
        );
      } else {
        state = AccountFailure(error: response.body);
        showCustomSnackbar(
          context,
          title: "User Update Error",
          content: response.body,
          type: SnackbarType.error,
          isTopPosition: false,
        );
      }
    } on Exception catch (e) {
      state = AccountFailure(error: e.toString());
    }
  }

  final GoogleSignIn _googleSignIn = GoogleSignIn(scopes: ['email', 'profile']);
  Future<void> handleSignOut() async {
    removeFromLocalStorage(name: "token");
    removeFromLocalStorage(name: "userEmail");
    removeFromLocalStorage(name: "userId");
    removeFromLocalStorage(name: "userPassword");
    removeFromLocalStorage(name: "deviceToken");
    removeFromLocalStorage(name: "state");
    removeFromLocalStorage(name: "city");
    removeFromLocalStorage(name: "address");
    removeFromLocalStorage(name: "latitude");
    removeFromLocalStorage(name: "longitude");
    await _googleSignIn.signOut();
  }
}

final streamRepositoryProvider = Provider<AccountService>(
  (ref) => AccountService(),
);
final accountProvider = StateNotifierProvider<AccountNotifier, AccountState>((
  ref,
) {
  final service = ref.read(streamRepositoryProvider);
  return AccountNotifier(services: service);
});
