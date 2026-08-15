"use client";

import { useEffect, useRef } from "react";
import { usePathname } from "next/navigation";
import { registerVisit } from "@/lib/api";

export function Telemetry() {
  const pathname = usePathname();
  const lastFiredRef = useRef<string | null>(null);

  useEffect(() => {
    const page = pathname || "/";
    if (lastFiredRef.current === page) return;
    lastFiredRef.current = page;
    void registerVisit(page);
  }, [pathname]);

  return null;
}
