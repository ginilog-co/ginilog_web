// ignore_for_file: library_private_types_in_public_api

import 'package:flutter/material.dart';

showCustomSnackbar2(BuildContext context,
    {required String title,
    required String content,
    required SnackbarType type}) {
  final snackBar = SnackBar(
    backgroundColor: Colors.transparent,
    elevation: 0,
    duration: const Duration(seconds: 3),
    content: Builder(
      builder: (BuildContext context) {
        return CustomSnackbarContent(
            title: title, content: content, type: type);
      },
    ),
  );
  ScaffoldMessenger.of(context).showSnackBar(snackBar);
}

showCustomSnackbar(BuildContext context,
    {required String title,
    required String content,
    required SnackbarType type,
    bool? isTopPosition}) {
  if (isTopPosition == true) {
    final overlay = Overlay.of(context);
    final overlayEntry = OverlayEntry(
      builder: (context) {
        return CustomSnackbarContent(
          title: title,
          content: content,
          type: type,
          isTopPosition: isTopPosition!,
        );
      },
    );

    overlay.insert(overlayEntry);

    // Remove the Snackbar after 3 seconds
    Future.delayed(const Duration(seconds: 3), () {
      overlayEntry.remove();
    });
  } else {
    final snackBar = SnackBar(
      backgroundColor: Colors.transparent,
      elevation: 0,
      duration: const Duration(seconds: 3),
      content: Builder(
        builder: (BuildContext context) {
          return CustomSnackbarContent(
              title: title, content: content, type: type);
        },
      ),
    );
    ScaffoldMessenger.of(context).showSnackBar(snackBar);
  }
}

enum SnackbarType { success, error }

class CustomSnackbarContent extends StatefulWidget {
  final String title;
  final String content;
  final SnackbarType type;
  final bool isTopPosition;

  const CustomSnackbarContent(
      {super.key,
      required this.title,
      required this.content,
      required this.type,
      this.isTopPosition = false});

  @override
  _CustomSnackbarContentState createState() => _CustomSnackbarContentState();
}

class _CustomSnackbarContentState extends State<CustomSnackbarContent>
    with SingleTickerProviderStateMixin {
  late AnimationController _animationController;
  late Animation<Offset> _offsetAnimation;

  Color get backgroundColor {
    switch (widget.type) {
      case SnackbarType.success:
        return Colors.green;
      case SnackbarType.error:
        return Colors.red;
    }
  }

  IconData get icon {
    switch (widget.type) {
      case SnackbarType.success:
        return Icons.check_circle;
      case SnackbarType.error:
        return Icons.error;
    }
  }

  @override
  void initState() {
    super.initState();
    _animationController = AnimationController(
      duration: const Duration(milliseconds: 500),
      vsync: this,
    );
    _offsetAnimation = Tween<Offset>(
      begin: widget.isTopPosition == true
          ? const Offset(0.0, -1.0)
          : const Offset(0.0, 1.0), // From the top
      end: Offset.zero,
    ).animate(CurvedAnimation(
      parent: _animationController,
      curve: Curves.easeOut,
    ));
    _animationController.forward();
  }

  @override
  void dispose() {
    _animationController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return widget.isTopPosition == true
        ? Positioned(
            top:
                MediaQuery.of(context).viewInsets.top + 50, // Show near the top
            left: 20,
            right: 20,
            child: SlideTransition(
              position: _offsetAnimation,
              child: Align(
                alignment: Alignment.topCenter,
                child: ConstrainedBox(
                  constraints: const BoxConstraints(
                    maxWidth: 600, // Maximum width of the Snackbar
                  ),
                  child: Material(
                    elevation: 8,
                    borderRadius: BorderRadius.circular(12),
                    child: Container(
                      padding: const EdgeInsets.all(16),
                      decoration: BoxDecoration(
                        color: backgroundColor,
                        borderRadius: BorderRadius.circular(12),
                      ),
                      child: Row(
                        crossAxisAlignment: CrossAxisAlignment.start,
                        children: [
                          Icon(icon, color: Colors.white),
                          const SizedBox(width: 12),
                          Expanded(
                            child: Column(
                              crossAxisAlignment: CrossAxisAlignment.start,
                              mainAxisSize: MainAxisSize.min,
                              children: [
                                Text(
                                  widget.title,
                                  style: const TextStyle(
                                    color: Colors.white,
                                    fontWeight: FontWeight.bold,
                                    fontSize: 16,
                                  ),
                                ),
                                const SizedBox(height: 4),
                                Text(
                                  widget.content,
                                  style: const TextStyle(
                                      color: Colors.white, fontSize: 14),
                                ),
                              ],
                            ),
                          ),
                        ],
                      ),
                    ),
                  ),
                ),
              ),
            ),
          )
        : SlideTransition(
            position: _offsetAnimation,
            child: Container(
              padding: const EdgeInsets.all(6),
              margin: const EdgeInsets.symmetric(horizontal: 5),
              decoration: BoxDecoration(
                color: backgroundColor,
                borderRadius: BorderRadius.circular(12),
              ),
              child: Row(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Icon(icon, color: Colors.white),
                  const SizedBox(width: 12),
                  Expanded(
                    child: Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      mainAxisSize: MainAxisSize.min,
                      children: [
                        Text(
                          widget.title,
                          style: const TextStyle(
                            color: Colors.white,
                            fontWeight: FontWeight.bold,
                            fontSize: 16,
                          ),
                        ),
                        const SizedBox(height: 4),
                        Text(
                          widget.content,
                          style: const TextStyle(
                              color: Colors.white, fontSize: 14),
                        ),
                      ],
                    ),
                  ),
                ],
              ),
            ),
          );
  }
}
