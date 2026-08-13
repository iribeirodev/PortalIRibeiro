"use client";

import { useEffect, useRef } from "react";
import { usePathname } from "next/navigation";
import { registrarVisita } from "@/lib/api";

export function Telemetry() {
  const pathname = usePathname();
  const lastFiredRef = useRef<string | null>(null);

  useEffect(() => {
    const pagina = pathname || "/";
    if (lastFiredRef.current === pagina) return;
    lastFiredRef.current = pagina;
    void registrarVisita(pagina);
  }, [pathname]);

  return null;
}
