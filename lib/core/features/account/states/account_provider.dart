// ignore_for_file: use_build_context_synchronously

import 'package:ginilog_customer_app/core/components/utils/constants.dart';
import 'package:ginilog_customer_app/core/features/account/services/account_services.dart';

import '../../../components/utils/package_export.dart';
import '../model/user_response_model.dart';

abstract class AccountState {}

class AccountInitial extends AccountState {}

class AccountLoading extends AccountState {}

class AccountSuccess extends AccountState {
  RegisterResponseModel userData;

  AccountSuccess({required this.userData});
}

class AccountFailure extends AccountState {
  final String error;

  AccountFailure(this.error);

  @override
  String toString() {
    return 'AuthFailure: $error';
  }
}

class AccountNotifier extends StateNotifier<AccountState> {
  final AccountService services;
  RegisterResponseModel? userData;

  AccountNotifier({required this.services}) : super(AccountInitial());

  getAccount() async {
    try {
      if (!mounted) {
        state = AccountLoading();
        return;
      }
      RegisterResponseModel response2 = await services.getUserData();
      userData = response2;
      state = AccountSuccess(userData: response2);
    } on Exception catch (e) {
      state = AccountFailure(e.toString());
    }
  }

  updateProfilePics(
      {required String imageFile,
      required String userId,
      required BuildContext context}) async {
    try {
      if (!mounted) {
        state = AccountLoading();
        return;
      }
      var response = await services.updateProfilePics(
        userId: userId,
        imageFile: imageFile,
      );
      if (response) {
        if (!mounted) return;
        state = AccountSuccess(userData: userData!);
        ScaffoldMessenger.of(context).showSnackBar(
            const SnackBar(content: Text("User Updated Successfully")));
      } else {
        state = AccountFailure("User Not Found");
        ScaffoldMessenger.of(context)
            .showSnackBar(const SnackBar(content: Text("User Not Found")));
      }
    } on Exception catch (e) {
      state = AccountFailure(e.toString());
    }
  }

  updateNames(
      {required String userId,
      required String firstName,
      required String lastName,
      required BuildContext context}) async {
    try {
      if (!mounted) {
        state = AccountLoading();
        return;
      }
      var response = await services.updateNames(
        userId: userId,
        firstName: firstName,
        lastName: lastName,
      );
      if (response) {
        if (!mounted) return;
        state = AccountSuccess(userData: userData!);
        ScaffoldMessenger.of(context).showSnackBar(
            const SnackBar(content: Text("User Updated Successfully")));
      } else {
        state = AccountFailure("User Not Found");
        ScaffoldMessenger.of(context)
            .showSnackBar(const SnackBar(content: Text("User Not Found")));
      }
    } on Exception catch (e) {
      state = AccountFailure(e.toString());
    }
  }

  updatePhoneNo(
      {required String userId,
      required String phoneNo,
      required BuildContext context}) async {
    try {
      if (!mounted) {
        state = AccountLoading();
        return;
      }
      var response = await services.updatePhoneNo(
        userId: userId,
        phoneNo: phoneNo,
      );
      if (response) {
        if (!mounted) return;
        state = AccountSuccess(userData: userData!);
        ScaffoldMessenger.of(context).showSnackBar(
            const SnackBar(content: Text("User Updated Successfully")));
      } else {
        state = AccountFailure("User Not Found");
        ScaffoldMessenger.of(context)
            .showSnackBar(const SnackBar(content: Text("User Not Found")));
      }
    } on Exception catch (e) {
      state = AccountFailure(e.toString());
    }
  }

  addNewAddress(
      {required String userId,
      required String address,
      required String addressPostCodes,
      required String houseNo,
      required String city,
      required String userState,
      required double latitude,
      required double longitude,
      required String phoneNo,
      required String userName,
      required BuildContext context}) async {
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
          userName: userName);
      if (response) {
        if (!mounted) return;
        state = AccountSuccess(userData: userData!);
        ScaffoldMessenger.of(context).showSnackBar(
            const SnackBar(content: Text("User Updated Successfully")));
      } else {
        state = AccountFailure("User Not Found");
        ScaffoldMessenger.of(context)
            .showSnackBar(const SnackBar(content: Text("User Not Found")));
      }
    } on Exception catch (e) {
      state = AccountFailure(e.toString());
    }
  }

  updateAddress(
      {required String addressId,
      required String address,
      required String addressPostCodes,
      required String houseNo,
      required String city,
      required double latitude,
      required double longitude,
      required String phoneNo,
      required String userName,
      required BuildContext context}) async {
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
          userName: userName);
      if (response) {
        if (!mounted) return;
        state = AccountSuccess(userData: userData!);
        ScaffoldMessenger.of(context).showSnackBar(
            const SnackBar(content: Text("User Updated Successfully")));
      } else {
        state = AccountFailure("User Not Found");
        ScaffoldMessenger.of(context)
            .showSnackBar(const SnackBar(content: Text("User Not Found")));
      }
    } on Exception catch (e) {
      state = AccountFailure(e.toString());
    }
  }

  deleteAddress(
      {required String addressId, required BuildContext context}) async {
    try {
      if (!mounted) {
        state = AccountLoading();
        return;
      }
      var response = await services.deleteDeliveryAddress(
        addressId: addressId,
      );
      if (response) {
        if (!mounted) {
          state = AccountSuccess(userData: userData!);
          ScaffoldMessenger.of(context).showSnackBar(
              const SnackBar(content: Text("Deleted Successfully")));
          return;
        }
      } else {
        state = AccountFailure("Address Not Found");
        ScaffoldMessenger.of(context)
            .showSnackBar(const SnackBar(content: Text("Address Not Found")));
      }
    } on Exception catch (e) {
      state = AccountFailure(e.toString());
    }
  }

  final GoogleSignIn _googleSignIn = GoogleSignIn(
    scopes: ['email', 'profile'],
  );
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

final streamRepositoryProvider =
    Provider<AccountService>((ref) => AccountService());
final accountProvider =
    StateNotifierProvider<AccountNotifier, AccountState>((ref) {
  final AccountService service = AccountService();
  return AccountNotifier(services: service);
});
