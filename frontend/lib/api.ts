import type { Projeto, RequisicaoChat, RespostaChat } from "./types";

const BASE_URL = process.env.NEXT_PUBLIC_API_BASE_URL ?? "";

function apiUrl(path: string): string {
  const base = BASE_URL.endsWith("/") ? BASE_URL : `${BASE_URL}/`;
  return `${base}${path}`;
}

export async function getProjetos(options?: {
  revalidate?: number;
}): Promise<Projeto[]> {
  try {
    const fetchOptions: RequestInit = options?.revalidate
      ? { next: { revalidate: options.revalidate } }
      : { cache: "no-store" };
    const res = await fetch(apiUrl("api/backoffice/projetos"), {
      ...fetchOptions,
      signal: AbortSignal.timeout(15000),
    });
    if (!res.ok) return [];
    const data = (await res.json()) as unknown;
    return Array.isArray(data) ? data : [];
  } catch {
    return [];
  }
}

export async function conversarComIris(
  sessaoId: string,
  texto: string
): Promise<RespostaChat | null> {
  try {
    const res = await fetch(apiUrl("api/iris/chat"), {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ sessaoId, texto } satisfies RequisicaoChat),
    });

    if (!res.ok) {
      return {
        texto: "Desculpe, a Íris encontrou um erro no processamento HTTP.",
        sessaoId,
      };
    }

    return (await res.json()) as RespostaChat;
  } catch (err) {
    console.error("Erro ao conversar com a Íris:", err);
    return {
      texto: "Não foi possível conectar ao servidor da Íris.",
      sessaoId,
    };
  }
}

export async function registrarVisita(pagina = "/"): Promise<void> {
  try {
    await fetch(apiUrl("api/telemetria/visita"), {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ pagina }),
    });
  } catch (err) {
    console.error("Erro ao registrar telemetria:", err);
  }
}
