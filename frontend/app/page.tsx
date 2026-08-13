import { HeroSection } from "@/components/hero";
import { AboutSection } from "@/components/about";
import { LaboratorySection } from "@/components/laboratory";
import { ServicesSection } from "@/components/services";
import { ContactSection } from "@/components/contact";

export default function Home() {
  return (
    <>
      <HeroSection />
      <AboutSection />
      <LaboratorySection />
      <ServicesSection />
      <ContactSection />
    </>
  );
}
