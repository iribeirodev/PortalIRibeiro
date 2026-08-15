import type { Project, ChatRequest, ChatResponse } from "./types";

const BASE_URL = process.env.NEXT_PUBLIC_API_BASE_URL ?? "";

function apiUrl(path: string): string {
  const base = BASE_URL.endsWith("/") ? BASE_URL : `${BASE_URL}/`;
  return `${base}${path}`;
}

export async function getProjects(options?: {
  revalidate?: number;
}): Promise<Project[]> {
  try {
    const fetchOptions: RequestInit = options?.revalidate
      ? { next: { revalidate: options.revalidate } }
      : { cache: "no-store" };
    const res = await fetch(apiUrl("api/backoffice/projects"), {
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

export async function chatWithIris(
  sessionId: string,
  text: string
): Promise<ChatResponse | null> {
  try {
    const res = await fetch(apiUrl("api/iris/chat"), {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ sessionId, text } satisfies ChatRequest),
    });

    if (!res.ok) {
      return {
        text: "Desculpe, a Íris encontrou um erro no processamento HTTP.",
        sessionId,
      };
    }

    return (await res.json()) as ChatResponse;
  } catch (err) {
    console.error("Erro ao conversar com a Íris:", err);
    return {
      text: "Não foi possível conectar ao servidor da Íris.",
      sessionId,
    };
  }
}

export async function registerVisit(page = "/"): Promise<void> {
  try {
    await fetch(apiUrl("api/telemetry/visit"), {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ page }),
    });
  } catch (err) {
    console.error("Erro ao registrar telemetria:", err);
  }
}
