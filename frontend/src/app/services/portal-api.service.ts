import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { map, Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import type {
  ChatRequest,
  ChatResponse,
  Project,
  ProjectJson,
} from '../models/portal.models';

@Injectable({ providedIn: 'root' })
export class PortalApiService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = environment.apiUrl;

  /**
   * Busca os projetos ativos para a vitrine do portfólio.
   * @returns Observable com a lista de projetos (vazia em caso de falha/resposta inválida).
   */
  getProjects(): Observable<Project[]> {
    return this.http.get<ProjectJson[]>(`${this.baseUrl}/projects`).pipe(
      map((items) => (Array.isArray(items) ? items : []).map(toProject)),
    );
  }

  /**
   * Registra uma visita de telemetria na página informada.
   * @param page Caminho da página visitada (padrão `/`).
   */
  registerVisit(page = '/'): Observable<unknown> {
    return this.http.post(`${this.baseUrl}/telemetry/visit`, { page });
  }

  /**
   * Envia uma mensagem ao chat da Íris e recebe a resposta gerada.
   * @param sessionId Identificador da sessão de conversa.
   * @param text Texto da pergunta do usuário.
   */
  sendChat(sessionId: string, text: string): Observable<ChatResponse> {
    return this.http.post<ChatResponse>(
      `${this.baseUrl}/iris/chat`,
      { sessionId, text } satisfies ChatRequest,
    );
  }
}

/**
 * Converte o payload bruto da API no modelo tipado de projeto do frontend.
 * @param raw Projeto como retornado pelo backend (camelCase).
 */
function toProject(raw: ProjectJson): Project {
  return {
    id: raw.id,
    title: raw.title,
    description: raw.description,
    technologies: raw.technologies,
    imageUrl: raw.imageUrl,
    githubUrl: raw.gitHubUrl,
    demoUrl: raw.demoUrl,
    createdAt: raw.createdAt,
    isActive: raw.isActive,
  };
}