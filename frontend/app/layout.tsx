import type { Metadata } from "next";
import type { ReactNode } from "react";
import "bootstrap/dist/css/bootstrap.min.css";
import { Navbar } from "@/components/navbar";
import { Telemetry } from "@/components/telemetry";
import { ResumeAssistant } from "@/components/resume-assistant";
import "./globals.css";

export const metadata: Metadata = {
  title: "Itamar Ribeiro - Engenharia de Software & Portfólio",
  description:
    "Portal profissional de Itamar Ribeiro, desenvolvedor Full Stack Sênior.",
  icons: {
    icon: "/favicon.png",
  },
};

export default function RootLayout({ children }: { children: ReactNode }) {
  return (
    <html lang="pt-BR">
      <body>
        <div id="page">
          <Navbar />

          <main>
            <article>{children}</article>
          </main>

          <footer className="bg-dark text-white py-5">
            <div className="container-fluid px-4 px-lg-5">
              <div className="text-center">
                <p className="mb-0">
                  &copy; 2026 Itamar Ribeiro. All rights reserved.
                </p>
              </div>
            </div>
          </footer>

          <ResumeAssistant />
        </div>

        <Telemetry />
      </body>
    </html>
  );
}
