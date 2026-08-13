import { getProjetos } from "@/lib/api";

export async function LaboratorySection() {
  const projetos = await getProjetos({ revalidate: 3600 });

  return (
    <section className="laboratory" id="laboratory">
      <div className="container-fluid px-4 px-lg-5">
        <h2 className="section-title">Laboratório</h2>

        {projetos.length === 0 ? (
          <p className="text-center text-muted">
            Nenhum experimento ativo no laboratório por enquanto.
          </p>
        ) : (
          <div className="laboratory-grid">
            {projetos.map((projeto) => (
              <div
                key={projeto.id}
                className="laboratory-item"
                style={{
                  maxWidth: "380px",
                  width: "100%",
                  margin: "0 auto",
                  display: "flex",
                  flexDirection: "column",
                  position: "relative",
                  overflow: "hidden",
                }}
              >
                <img
                  src={projeto.urlImagem ?? undefined}
                  alt={projeto.titulo}
                  className="laboratory-image"
                  style={{ width: "100%", height: "250px", objectFit: "cover" }}
                />

                <div className="laboratory-overlay" style={{ pointerEvents: "none" }}>
                  <h3>{projeto.titulo}</h3>
                  <p>{projeto.tecnologias.join(", ")}</p>
                </div>

                <div
                  className="laboratory-info"
                  style={{ flexGrow: 1, position: "relative", zIndex: 2 }}
                >
                  <h3>{projeto.titulo}</h3>
                  <p className="mb-3">{projeto.descricao}</p>

                  <div className="laboratory-links" style={{ position: "relative", zIndex: 3 }}>
                    {projeto.urlGithub && (
                      <a
                        href={projeto.urlGithub}
                        target="_blank"
                        rel="noopener noreferrer"
                        className="btn btn-sm btn-outline-dark me-2 d-inline-flex align-items-center gap-2"
                      >
                        <svg viewBox="0 0 24 24" width="16" height="16" fill="currentColor">
                          <path d="M12 2A10 10 0 0 0 2 12c0 4.42 2.87 8.17 6.84 9.5.5.08.66-.23.66-.5v-1.69c-2.77.6-3.36-1.34-3.36-1.34-.46-1.16-1.11-1.47-1.11-1.47-.9-.62.07-.6.07-.6 1 .07 1.53 1.03 1.53 1.03.9 1.52 2.34 1.07 2.91.83.1-.65.35-1.09.63-1.34-2.22-.25-4.55-1.11-4.55-4.94 0-1.1.39-1.99 1.03-2.69-.1-.25-.45-1.27.1-2.64 0 0 .84-.27 2.75 1.02.79-.22 1.65-.33 2.5-.33.85 0 1.71.11 2.5.33 1.91-1.29 2.75-1.02 2.75-1.02.55 1.37.2 2.39.1 2.64.64.7 1.03 1.6 1.03 2.69 0 3.84-2.34 4.68-4.57 4.93.36.31.68.92.68 1.85V21c0 .27.16.59.67.5C19.14 20.16 22 16.42 22 12A10 10 0 0 0 12 2z" />
                        </svg>
                        GitHub
                      </a>
                    )}

                    {projeto.urlDemonstracao && (
                      <a
                        href={projeto.urlDemonstracao}
                        target="_blank"
                        rel="noopener noreferrer"
                        className="btn btn-sm btn-primary"
                      >
                        Ver Demo
                      </a>
                    )}
                  </div>
                </div>
              </div>
            ))}
          </div>
        )}
      </div>
    </section>
  );
}
