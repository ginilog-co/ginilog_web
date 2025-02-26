import 'dart:async';

import 'package:ginilog_customer_app/core/components/utils/package_export.dart';

extension RefExt<T> on Ref<T> {
  void refreshIn(Duration duration) {
    Timer(duration, () => invalidateSelf());
  }
}
