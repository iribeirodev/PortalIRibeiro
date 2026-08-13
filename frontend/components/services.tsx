const services = [
  {
    title: "Desenvolvimento",
    description: "Aplicações web robustas com as melhores tecnologias",
    icon: "💻",
  },
  {
    title: "Otimização",
    description: "Performance e SEO para melhor visibilidade",
    icon: "⚡",
  },
];

export function ServicesSection() {
  return (
    <section className="services" id="services">
      <div className="container-fluid px-4 px-lg-5">
        <h2 className="section-title">Meus Serviços</h2>
        <div className="services-grid">
          {services.map((service) => (
            <div className="service-card" key={service.title}>
              <div className="service-icon">{service.icon}</div>
              <h3>{service.title}</h3>
              <p>{service.description}</p>
            </div>
          ))}
        </div>
      </div>
    </section>
  );
}
