import 'package:ginilog_customer_app/core/components/utils/package_export.dart';
import 'package:ginilog_customer_app/core/components/utils/size_config.dart';
import 'package:ginilog_customer_app/core/components/widgets/back_icon.dart';

class ZoomableImageViewer extends StatelessWidget {
  final String imageUrl;

  const ZoomableImageViewer({super.key, required this.imageUrl});

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: PreferredSize(
        preferredSize: Size.fromHeight(SizeConfig.heightAdjusted(14)),
        child: Padding(
          padding: EdgeInsets.only(top: SizeConfig.heightAdjusted(8)),
          child: const Column(
            children: [GlobalBackButton(backText: '', showBackButton: true)],
          ),
        ),
      ),
      body: SizedBox(
        height: SizeConfig.heightAdjusted(100),
        width: SizeConfig.widthAdjusted(100),
        child: InteractiveViewer(
          panEnabled: true, // Allows panning
          minScale: 0.5, // Minimum zoom scale
          maxScale: 5.0, // Maximum zoom scale
          child: Image.network(imageUrl, fit: BoxFit.fill),
        ),
      ),
    );
  }
}
