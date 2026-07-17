// hooks/useAppDetection.ts
import { useState, useEffect } from 'react';

export function useAppDetection() {
  const [isApp, setIsApp] = useState(false);
  const [isMobile, setIsMobile] = useState(false);

  useEffect(() => {
    // Check if running in app
    const checkApp = () => {
      const ua = navigator.userAgent;
      const isInApp = ua.includes("GINILOGApp") || 
                     document.referrer.includes("ginilog://");
      setIsApp(isInApp);
    };

    // Check if mobile device
    const checkMobile = () => {
      setIsMobile(window.innerWidth < 768);
    };

    checkApp();
    checkMobile();
    window.addEventListener("resize", checkMobile);

    return () => window.removeEventListener("resize", checkMobile);
  }, []);

  return { isApp, isMobile };
}